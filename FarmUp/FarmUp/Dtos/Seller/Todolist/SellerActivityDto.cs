namespace FarmUp.Dtos.Seller.Todolist
{
    public class SellerActivityDto
    {
        public string UserId { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;

        //public List<SellerActivityTrObj> sellerActivityTrObjs { get; set; } = new List<SellerActivityTrObj>();
        public string[]? ActDesc { get; set; }
        public string[]? ActTopic { get; set; }

    }

    public class SellerActivityTrObj
    {
        public string ActDesc { get; set; } = string.Empty;
        public string ActTopic { get; set; } = string.Empty;
    }
}
