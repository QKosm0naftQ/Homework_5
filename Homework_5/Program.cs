using Homework_5.Data.Entities;
using Homework_5.Data;
using System.Text;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Homework_5.Models;
using Newtonsoft.Json.Serialization;
namespace Homework_5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            ///
            NovaPoshtaService NovaPoshtaService = new NovaPoshtaService();
            NovaPoshtaService.seedAreas();
            NovaPoshtaService.SeedCities();
            NovaPoshtaService.SeedDepartments();
            //var list = NovaPoshtaService.GetListAreas();
            //foreach (var area in list)
            //{
            //    Console.WriteLine(area.Description);
            //}

            //------------



        }
    }
}
