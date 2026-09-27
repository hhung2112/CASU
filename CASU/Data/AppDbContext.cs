using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using CASU.Models;

namespace CASU.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ProductionLogModel> ProductionLogs { get; set; }
        public DbSet<ProductionModel> ProductionHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(local);Database=CasuminaProductionDb;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
    }

    // Hỗ trợ Design-Time Tools (dotnet ef migrations)
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=(local);Database=CasuminaProductionDb;Trusted_Connection=True;TrustServerCertificate=True;");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}