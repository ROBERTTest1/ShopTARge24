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
                string weather = json["weather"]?[0]?["description"]?.ToString();

                return $"City: {cityName} | Temperature: {temp:F1}°C | Weather: {weather}";
            }
        }
    }
}

