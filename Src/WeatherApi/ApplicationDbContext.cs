using Microsoft.EntityFrameworkCore;
using WeatherApi.Models;

namespace WeatherService.Configurations
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WeatherForecast>()
                .ToTable("WeatherData")
                .HasIndex(w => w.Id);

            modelBuilder.Entity<WeatherForecast>()
                .Property(w => w.JsonApiResult)
                .IsRequired();
        }

        public DbSet<WeatherForecast> Weathers => Set<WeatherForecast>();
    }
}
