using Dapper.Contrib.Extensions;

namespace FarmUp.Models.Request.Product
{
    public class UpdateBuyItemReq
    {
        public Guid tdp_id { get; set; }
        public DateTime? dtpicker { get; set; }
        public decimal? price { get; set; }
        public int? productype { get; set; }
        public int? productgrade { get; set; }
    }

    public class BuyerMarketSearchReq
    {
        public Guid prd_id { get; set; }
        public DateTime? dtpicker { get; set; }
        public decimal? price { get; set; }
        public int? productype { get; set; }
        public int? productgrade { get; set; }
    }
}
