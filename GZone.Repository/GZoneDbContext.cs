using GZone.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GZone.Repository
{
    public class GZoneDbContext : DbContext
    {
        //Constructor
        public GZoneDbContext() { }
        public GZoneDbContext(DbContextOptions<GZoneDbContext> options) : base(options) { }

        //Binding Models
        public DbSet<Account> Account { get; set; }

        //Config Connection
        private static string GetConnectionString()
        {
            string root = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ?? "";
            string apiDirectory = Path.Combine(root, "GZone.API");
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(apiDirectory)
                .AddJsonFile("appsettings.json", true, true).Build();
            return configuration["ConnectionStrings:DefaultConnection"]!;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(GetConnectionString());
            }
        }

        //Config Model
    }
}
