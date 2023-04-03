using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace Skills
{
    public class EmployeeDb : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Skill> Skills { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=LAPTOP-AI5QJL80\\SQLEXPRESS;Initial Catalog=NeoxDatenbank;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Employee>().HasKey(e => e.Employee_Id);
            modelBuilder.Entity<Skill>().HasKey(s => s.Skill_Id); // Hinzufügen der primären Schlüsselspalte für Skill
            //modelBuilder.Entity<Skill>().HasNoKey(); // Angeben, dass Skill keine Schlüsselspalte hat
        }
    }
}

