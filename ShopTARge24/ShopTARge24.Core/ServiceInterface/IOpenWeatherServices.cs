namespace ShopTARge24.Core.ServiceInterface
{
    public interface IOpenWeatherServices
    {
        Task<string> GetWeatherAsync(string city);
    }
}

