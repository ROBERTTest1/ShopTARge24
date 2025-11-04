using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ShopTARge24.Core.ServiceInterface;

namespace ShopTARge24.Controllers
{
    public class OpenWeatherController : Controller
    {
        private readonly IOpenWeatherServices _openWeatherService;
        
        public OpenWeatherController(IOpenWeatherServices openWeatherService)
        {
            _openWeatherService = openWeatherService;
        }

        //Url päring
        [HttpGet("/OpenWeather")]
        public async Task<IActionResult> Index(string city)
        {
            if (!string.IsNullOrWhiteSpace(city))
            {
                var weatherData = await _openWeatherService.GetWeatherAsync(city);
                ViewData["WeatherData"] = weatherData;
                ViewData["City"] = city;
            }

            return View();
        }
    }
}

