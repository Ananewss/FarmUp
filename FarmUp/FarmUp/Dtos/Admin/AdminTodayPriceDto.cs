namespace FarmUp.Dtos.Admin
{
    public class ProductTypeDto
    {
        public string pdt_id { get; set; } = string.Empty;
        public string pdt_description { get; set; } = string.Empty;

    }

    public class ProductGradeDto
    {
        public string pdg_id { get; set; } = string.Empty;
        public string pdg_description { get; set; } = string.Empty;
    }

    public class AdminTodayPriceDto
    {
        public string pdt_description { get; set; } = string.Empty;
        public string pdg_description { get; set; } = string.Empty;
        public string price { get; set; } = string.Empty;
        public string buyer_name { get; set; } = string.Empty;
    }

    public class ProductDtoList
    {
        public List<ProductTypeDto> productTypeDtoList { get; set; } = new List<ProductTypeDto>();
        public List<ProductGradeDto> productGradeDtoList { get; set; } = new List<ProductGradeDto>();
        public List<AdminTodayPriceDto> adminTodayPriceDtoList { get; set; } = new List<AdminTodayPriceDto>();
    }
}

