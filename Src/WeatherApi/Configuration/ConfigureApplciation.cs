using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WeatherApi.Services;
using WeatherService.Configurations;

namespace WeatherApi.Configuration
{
    public static class ConfigureApplciation
    {
        public static void ConfigureWeatherServices(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Weather"));
            });

            // Options
            services.AddOptions<WeatherServiceOptions>()
                .BindConfiguration(WeatherServiceOptions.OptionKey)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            // HttpClient
            services.AddHttpClient(WeatherServiceOptions.ClientKey, (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<WeatherServiceOptions>>().Value;

                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(options.ResponseTimeOutSeconds);
            });

            // DI
            services.AddScoped<WeatherClientService>();
        }
    }
}
