namespace FarmUp.Dtos.Seller
{
    public class WeatherForecastDto
    {
        public string Wea_Id { get; set; } = string.Empty;
        public DateTime DT { get; set; } = DateTime.MinValue;
        public string Temp { get; set; } = string.Empty;
        public string Wind { get; set; } = string.Empty;
        public string Humidity { get; set; } = string.Empty;
        public string UV { get; set; } = string.Empty;
        public string CloudCover { get; set; } = string.Empty;
        public string RainAmt { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string CloudyMsg { get; set; } = string.Empty;
        public string SubDistrict { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

    }

    public class WeatherForecastDtoList
    {
        public string SubDistrict { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public List<WeatherForecastDto> weatherForecastDtosList { get; set; } = new List<WeatherForecastDto>();
    }
}
