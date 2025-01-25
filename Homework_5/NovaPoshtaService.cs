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

namespace Homework_5
{
    public class NovaPoshtaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly BombaDbContext _context;

        public NovaPoshtaService()
        {
            _httpClient = new HttpClient();
            _url = "https://api.novaposhta.ua/v2.0/json/";
            _context = new BombaDbContext();
            _context.Database.Migrate();
        }

        public void seedAreas()
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
                        foreach (var item in result.Data)
                        {
                            var entity = new AreaEntity
                            {
                                Ref = item.Ref,
                                AreasCenter = item.AreasCenter,
                                Description = item.Description
                            };
                            _context.Areas.Add(entity);
                            _context.SaveChanges();
                        }
                    }
                }
            }
        }

        public void SeedCities()
        {
            if (!_context.Cities.Any())
            {
                var listAreas = GetListAreas();
                foreach (var area in listAreas)
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
                            foreach (var city in result.Data)
                            {
                                var cityEntity = new CityEntity
                                {
                                    Ref = city.Ref,
                                    Description = city.Description,
                                    TypeDescription = city.SettlementTypeDescription,
                                    AreaRef = city.Area,
                                    AreaId = area.Id,
                                };
                                _context.Cities.Add(cityEntity);
                            }
                            _context.SaveChanges();
                        }
                    }
                }
            }
        }

        public void SeedDepartments()
        {
            if (!_context.Departments.Any()) 
            {
                var listCities = _context.Cities.ToList();

                foreach (var city in listCities)
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
                            foreach (var dep in result.Data)
                            {
                                var departmentEntity = new DepartmentEntity
                                {
                                    Ref = dep.Ref,
                                    Description = dep.Description,
                                    Address = dep.ShortAddress,
                                    Phone = dep.Phone,
                                    CityRef = dep.CityRef,
                                    CityId = city.Id
                                };
                                _context.Departments.Add(departmentEntity);
                            }
                            _context.SaveChanges();
                        }
                    }
                }
            }
        }

        public List<AreaEntity> GetListAreas()
        {
            return _context.Areas.ToList();
        }
    }
}
