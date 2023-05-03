using Microsoft.EntityFrameworkCore;
using Neox.KnowHowTransfer.Models;
using System.Data.Common;

namespace Neox.KnowHowTransfer.DatabaseContext
{
    public class NeoxDBContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        public NeoxDBContext(DbContextOptions<NeoxDBContext> options) : base(options)
        {
        }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    builder.Entity<Employee>();
        //}
        
        public static NeoxDBContext CreatDBContext(DbConnection dbConnection)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NeoxDBContext>();
            optionsBuilder.UseSqlServer(dbConnection);
            var dbContext = new NeoxDBContext(optionsBuilder.Options);
            return dbContext;
        }

    }
}
