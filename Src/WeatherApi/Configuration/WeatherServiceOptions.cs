using System.ComponentModel.DataAnnotations;

namespace WeatherApi.Configuration;

public class WeatherServiceOptions
{
    public const string ClientKey = "WeatherServiceClient";
    public const string OptionKey = "WeatherService";

    [Required(ErrorMessage = "Invalid Base Url")]
    public required string BaseUrl { get; set; }

    [Required(ErrorMessage = "Weather Endpoint is required")]
    public required string GetWeatherEndpoint { get; set; }

    [Range(1, 30, ErrorMessage = "Response TimeOut must be between 1 and 30 seconds")]
    public uint ResponseTimeOutSeconds { get; set; }
}
