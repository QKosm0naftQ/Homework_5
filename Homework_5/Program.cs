using Homework_5.Data.Entities;
using Homework_5.Data;
using System.Text;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Homework_5.Models;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
namespace Homework_5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            NovaPoshtaService NovaPoshtaService = new NovaPoshtaService();
            NovaPoshtaService.seedAreas();
            NovaPoshtaService.SeedCities();
            NovaPoshtaService.SeedDepartments();

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            Console.WriteLine("RunTime " + elapsedTime);



        }
    }
}
