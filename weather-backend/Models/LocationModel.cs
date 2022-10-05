namespace weather_backend.Models
{
    /// <summary>
    /// A location, with latitude, longitude and other data
    /// </summary>
    public class LocationModel
    {
        public string? Name { get; set; }
        public string? Country { get; set; }
        public float? Lat { get; set; }
        public float? Lon { get; set; }
    }
}
