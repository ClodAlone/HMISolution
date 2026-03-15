using WeatherForecast = UnoApptest.Client.Models.WeatherForecast;

namespace UnoApptest.Services.Caching;

public interface IWeatherCache
{
    ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token);
}
