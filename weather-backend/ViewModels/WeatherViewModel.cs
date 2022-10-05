namespace weather_backend.ViewModels
{
    public class WeatherViewModel
    {
        // Name of location
        public string? Name { get; set; }
        // Temperature, humidity and pressure data
        public MainViewModel? Main { get; set; }

    }
}
