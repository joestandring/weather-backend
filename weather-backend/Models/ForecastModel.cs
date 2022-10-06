namespace weather_backend.Models
{
    public class MainModel
    {
        public float? Temp { get; set; }
        public float? Temp_Max { get; set; }
        public float? Temp_Min { get; set; }
        public int? Pressure { get; set; }
        public int? Humidity { get; set; }
    }

    public class SysModel
    {
        public string? Country { get; set; }
        public int? Sunrise { get; set; }
        public int? Sunset { get; set; }
    }

    public class WeatherModel
    {
        public int? ID { get; set; }
        public string? Main { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
    }

    // A weather forecast overview
    public class ForecastModel
    {
        public string? Name { get; set; }
        public IEnumerable<WeatherModel>? Weather { get; set; }
        public MainModel? Main { get; set; }
        public SysModel? Sys { get; set; }
    }
}
