using Dapper.Contrib.Extensions;

namespace FarmUp.Dtos.Product
{
    public class ProductTypeDBModel
    {
        public int pdt_id { get; set; }
        public string pdt_description { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public string updated_by { get; set; }
    }

    public class ProductGradeDBModel
    {
        public int pdg_id { get; set; }
        public string pdg_description { get; set; } = string.Empty;
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public string updated_by { get; set; }
    }

    [Table("tr_today_price")]
    public class trTodayPriceDBModel
    {
        [ExplicitKey]
        public byte[] tdp_id { get; set; }
        public int tdp_buy_usr_id { get; set; } 
        public string tdp_buyer_name { get; set; } 
        public DateTime? tdp_date { get; set; } 
        public decimal tdp_price { get; set; }
        public int tdp_pdg_id { get; set; }
        public int tdp_pdt_id { get; set; } 
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public string updated_by { get; set; }
    }

    [Table("vw_tr_product")]
    public class vwTrProductDBModel
    {
        [ExplicitKey]
        public byte[] prd_id { get; set; }
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

     

    [Table("vw_today_price")]
    public class vwTodayPriceDBModel
    {
        [ExplicitKey]
        public byte[] tdp_id { get; set; }
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
}
