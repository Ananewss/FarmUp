using FarmUp.Models.Response.Product;

namespace FarmUp.Models.Response
{
    public class BuyItemResponse
    {
        public List<ProductTypeResp> ProductTypeResp { get; set; } //Master ProductType
        public List<ProductGradeResp> ProductGradeResp { get; set; } //Master ProductGrade
        public List<TodayPriceResp> TodayPriceResp { get; set; } //view TodayPriceResp
         
    }
     
}
