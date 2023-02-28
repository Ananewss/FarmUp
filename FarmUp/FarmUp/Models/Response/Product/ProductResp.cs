namespace FarmUp.Models.Response.Product
{
    public class ProductTypeResp
    {
        public int pdt_id { get; set; }
        public string pdt_description { get; set; }
    }

    public class ProductGradeResp
    {
        public int pdg_id { get; set; }
        public string pdg_description { get; set; } 
    }

    public class TodayPriceResp
    {
        public Guid tdp_id { get; set; }
        public int tdp_buy_usr_id { get; set; }
        public string buy_name { get; set; }
        public string buy_location { get; set; }
        public DateTime? tdp_date { get; set; }
        public int pdt_id { get; set; }
        public string pdt_description { get; set; }
        public int pdg_id { get; set; }
        public string pdg_description { get; set; }
        public decimal tdp_price { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public string updated_by { get; set; }
    }

    public class TrProductResp
    {
        public Guid prd_id { get; set; }
        public int prd_usr_id { get; set; }
        public string slr_farm_name { get; set; }
        public string slr_farm_location { get; set; }
        public string slr_district_th { get; set; }
        public DateTime? prd_datetime { get; set; }
        public DateTime? prd_harvest_time { get; set; }
        public int pdt_id { get; set; }
        public string pdt_description { get; set; }
        public int pdg_id { get; set; }
        public string pdg_description { get; set; }
        public decimal prd_amount { get; set; }
        public decimal prd_price_per_unit { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public string updated_by { get; set; }
    }
}
