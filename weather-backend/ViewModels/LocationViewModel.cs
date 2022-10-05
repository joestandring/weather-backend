namespace weather_backend.ViewModels
{
    /// <summary>
    /// A location, with latitude, longitude and other data
    /// </summary>
    public class LocationViewModel
    {
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? Lat { get; set; }
        public string? Long { get; set; }
    }
}
