using Microsoft.Extensions.Options;
using WeatherApi.Configuration;

namespace WeatherApi.Services
{
    public class WeatherClientService 
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<WeatherServiceOptions> _options;

        private string ClientName => WeatherServiceOptions.ClientKey;

        public WeatherClientService(
            IHttpClientFactory httpClientFactory,
            IOptions<WeatherServiceOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options;
        }

        public async Task<string?> GetWeatherAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(ClientName);

                var response = await client.GetAsync(_options.Value.GetWeatherEndpoint, cancellationToken);
                response.EnsureSuccessStatusCode();

                var apiResult = await response.Content.ReadAsStringAsync(cancellationToken);
                return apiResult;  
            }
            catch
            {
                return null;
            }
        }
    }
}
