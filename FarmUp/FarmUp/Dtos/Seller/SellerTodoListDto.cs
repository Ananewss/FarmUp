namespace FarmUp.Dtos.Seller
{
    public class SellerTodoListDto
    {
        public string actId { set; get; }
        public string ImageUrl { set; get; }
        public string ActDesc { set; get; }
        public string ActTopic { set; get; }
    }

    public class SellerTodoListDtoList
    {
        public List<String> titleActivity { get; set; } = new List<string>();
        public List<SellerTodoListDto> todayListDtosList { get; set; } = new List<SellerTodoListDto>();
        public List<SellerTodoListDto> todayAlarmDtosList { get; set; } = new List<SellerTodoListDto>();
    }
}
