namespace WeatherApi.Models
{
    public class WeatherForecast
    {
        protected WeatherForecast() { }

        public WeatherForecast(string jsonApiResult)
        {
            JsonApiResult = jsonApiResult;
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; init; }

        public DateTime CreatedAt { get; init; }

        public string JsonApiResult { get; init; } = default!;
    }
}
