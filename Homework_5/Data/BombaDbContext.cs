using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homework_5.Constants;
using Homework_5.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Homework_5.Data
{
    public class BombaDbContext : DbContext
    {
        public DbSet<AreaEntity> Areas { get; set; }
        public DbSet<CityEntity> Cities { get; set; }
        public DbSet<DepartmentEntity> Departments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(AppDatabase.ConnectionString);
        }

    }
}
