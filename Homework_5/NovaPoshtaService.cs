using Homework_5.Data.Entities;
using Homework_5.Data;
using Homework_5.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Homework_5.Model.Areas;
using Homework_5.Models.Department;
using Homework_5.Models.City;
using AutoMapper;
using Homework_5.Mapping;

namespace Homework_5
{
    public class NovaPoshtaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly BombaDbContext _context;
        private readonly int _procesLimit;
        private readonly IMapper _mapper;


        public NovaPoshtaService()
        {
            _httpClient = new HttpClient();
            _url = "https://api.novaposhta.ua/v2.0/json/";
            _context = new BombaDbContext();
            _context.Database.Migrate();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = config.CreateMapper();
            _procesLimit = Environment.ProcessorCount - 2;
        }

        public async Task seedAreas()
        {
            if (!_context.Areas.Any())
            {
                var modelRequest = new
                {
                    apiKey = "0ee0afeefc706709e3e963263a8acc54",
                    modelName = "Address",
                    calledMethod = "getAreas",
                    methodProperties = new MethodProperties()
                };
                string json = JsonConvert.SerializeObject(modelRequest, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                });

                HttpContent contex = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage responce = _httpClient.PostAsync(_url, contex).Result;

                if (responce.IsSuccessStatusCode)
                {
                    string jsonResp = responce.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<AreaResponse>(jsonResp);
                    if (result != null && result.Success && result.Data != null)
                    {
                        using var semaphore = new SemaphoreSlim(_procesLimit);

                        await Parallel.ForEachAsync(result.Data, async (item, x) =>
                        {
                            try
                            {
                                var entity = _mapper.Map<AreaEntity>(item);
                                using var localContext = new BombaDbContext();
                                await localContext.Areas.AddAsync(entity);
                                await localContext.SaveChangesAsync();
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        });
                    }
                }
            }
        }

        public async Task SeedCities()
        {
            if (!_context.Cities.Any()) 
            {
                var listAreas = GetListAreas();
                using var semaphore = new SemaphoreSlim(_procesLimit);

                await Parallel.ForEachAsync(listAreas, async (area, x) =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var modelRequest = new CityPostModel
                        {
                            ApiKey = "0ee0afeefc706709e3e963263a8acc54",
                            MethodProperties = new MethodCityProperties()
                            {
                                AreaRef = area.Ref
                            }
                        };

                        string json = JsonConvert.SerializeObject(modelRequest, new JsonSerializerSettings
                        {
                            Formatting = Formatting.Indented
                        });
                        HttpContent context = new StringContent(json, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = _httpClient.PostAsync(_url, context).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string jsonResp = response.Content.ReadAsStringAsync().Result;
                            var result = JsonConvert.DeserializeObject<CityResponse>(jsonResp);
                            if (result != null && result.Data != null && result.Success)
                            {

                                var cityEntities = result.Data.Select(city =>
                                {
                                    var entity = _mapper.Map<CityEntity>(city);
                                    entity.AreaId = area.Id;
                                    return entity;
                                });

                                using var localContext = new BombaDbContext();
                                await localContext.Cities.AddRangeAsync(cityEntities);
                                await localContext.SaveChangesAsync();
                            }
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

            }
        }

        public async Task SeedDepartments()
        {
            if (!_context.Departments.Any()) 
            {
                var listCities = _context.Cities.ToList();
                using var semaphore = new SemaphoreSlim(_procesLimit);
                await Parallel.ForEachAsync(listCities, async (city, _) =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var modelRequest = new DepartmentPostModel
                        {
                            ApiKey = "0ee0afeefc706709e3e963263a8acc54",
                            MethodProperties = new MethodDepatmentProperties()
                            {
                                CityRef = city.Ref
                            }
                        };

                        string json = JsonConvert.SerializeObject(modelRequest, new JsonSerializerSettings
                        {
                            Formatting = Formatting.Indented 
                        });
                        HttpContent context = new StringContent(json, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = _httpClient.PostAsync(_url, context).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string jsonResp = response.Content.ReadAsStringAsync().Result; 
                            var result = JsonConvert.DeserializeObject<DepartmentResponse>(jsonResp);
                            if (result != null && result.Data != null && result.Success)
                            {
                                var departmentEntities = result.Data.Select(dep =>
                                {
                                    var entity = _mapper.Map<DepartmentEntity>(dep);
                                    entity.CityId = city.Id;
                                    return entity;
                                });

                                using var localContext = new BombaDbContext();
                                await localContext.Departments.AddRangeAsync(departmentEntities);
                                await localContext.SaveChangesAsync();
                            }
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

            }
        }

        public List<AreaEntity> GetListAreas()
        {
            return _context.Areas.ToList();
        }
    }
}
