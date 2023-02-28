using FarmUp.Models.Response.Product;

namespace FarmUp.Models.Response
{
    public class BuyerMarketResponse
    {
        public List<ProductTypeResp> ProductTypeResp { get; set; } //Master ProductType
        public List<ProductGradeResp> ProductGradeResp { get; set; } //Master ProductGrade
        public List<TrProductResp> TrProductResp { get; set; } //view TodayPriceResp
        public List<string> DistrictResp { get; set; }

    }
     
}
