namespace FarmUp.Dtos.Seller
{
    public class SellItemGradeDto
    {
        public string pdg_id { get; set; } = string.Empty;
        public string pdg_description { get; set; } = string.Empty;

    }

    public class SellItem
    {
        public DateTime prd_datetime { get; set; }
        public string prd_pdt_desc { get; set; } = string.Empty;
        public int prd_pdg_id { get; set; }
        public string prd_pdg_desc { get; set; } = string.Empty;
        public decimal prd_amount { get; set; }
        public decimal prd_price_per_unit { get; set; }
        public DateTime prd_harvest_time { get; set; }

    }

    public class SellItemDtoList
    {
        public List<SellItemGradeDto> sellItemGradeDtoList { get; set; } = new List<SellItemGradeDto>();
        public List<SellItem> sellItemList { get; set; } = new List<SellItem>();
    }
}

