namespace FarmUp.Dtos.Seller
{
    public class TodayPriceDto
    {
        public string tdp_id { get; set; } = string.Empty;
        public DateTime tdp_date { get; set; } = DateTime.MinValue;
        public string tdp_price { get; set; } = string.Empty;
        public string tdp_price_min { get; set; } = string.Empty;
        public string tdp_min_seller_name { get; set; } = string.Empty;
        public string tdp_price_max { get; set; } = string.Empty;
        public string tdp_max_seller_name { get; set; } = string.Empty;
        public int tdp_pdg_id { get; set; }
        public string pdg_description { get; set; } = string.Empty;
        public int tdp_pdt_id { get; set; }
        public string pdt_description { get; set; } = string.Empty;
        public string additionalCss { get; set; } = string.Empty;

    }

    public class TodayPriceDtoList
    {
        public List<TodayPriceDto> todayPriceDtosList { get; set; } = new List<TodayPriceDto>();
    }
}

