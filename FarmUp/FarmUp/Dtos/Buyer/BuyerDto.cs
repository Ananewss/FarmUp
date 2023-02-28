namespace FarmUp.Dtos.Buyer
{ 
    public class BuyerDto
    { 
        public int buy_id { get; set; }
        public int buy_usr_id { get; set; }
        public string buy_name { get; set; } = string.Empty;
        public string buy_size { get; set; } = string.Empty;
        public string buy_location { get; set; } = string.Empty;
        public string buy_district { get; set; } = string.Empty;
        public string buy_province { get; set; } = string.Empty;
        public string buy_country { get; set; } = string.Empty;
        public string buy_district_th { get; set; } = string.Empty;
        public string buy_country_th { get; set; } = string.Empty;
        public string buy_GAP_id { get; set; } = string.Empty;
        public string buy_GAP_file { get; set; } = string.Empty;
        public string buy_dt_weather_noti { get; set; } = string.Empty;
        public string created_at { get; set; } = string.Empty;
        public string update_at { get; set; } = string.Empty;
        public string deleted_at { get; set; } = string.Empty;
        public string update_by { get; set; } = string.Empty;

    }
}
