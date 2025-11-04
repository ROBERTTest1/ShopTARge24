using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ShopTARge24.Core.ServiceInterface;

namespace ShopTARge24.ApplicationServices.Services
{
    public class OpenWeatherServices : IOpenWeatherServices
    {
        private readonly string _apiKey = "98c8dc6efd52d35086752e86a1cf6748";
        private readonly string _baseUrl = "https://api.openweathermap.org/data/2.5/weather";
        

        public async Task<string> GetWeatherAsync(string city)
        {
            using (var httpClient = new HttpClient())
            {
                var url = $"{_baseUrl}?q={city}&appid={_apiKey}&units=metric";
                var response = await httpClient.GetStringAsync(url);

                JObject json = JObject.Parse(response);
                string cityName = json["name"]?.ToString();
                double temp = json["main"]?["temp"]?.ToObject<double>() ?? 0;
                double feelsLike = json["main"]?["feels_like"]?.ToObject<double>() ?? 0;
                int humidity = json["main"]?["humidity"]?.ToObject<int>() ?? 0;
                int pressure = json["main"]?["pressure"]?.ToObject<int>() ?? 0;
                double windSpeed = json["wind"]?["speed"]?.ToObject<double>() ?? 0;
                string weather = json["weather"]?[0]?["description"]?.ToString();

                return $"City: {cityName} | Temperature: {temp:F2}°C | Temp Feels like: {feelsLike:F2}°C | Humidity: {humidity}% | Pressure: {pressure} Hpa | Wind Speed: {windSpeed:F2} Km/h | Weather Condition: {weather}";
            }
        }
    }
}

