namespace FarmUp.Dtos.Seller.Todolist
{
    public class SellerTodolistActivityDataMasterDto
    {
        public string ActDesc { get; set; } = string.Empty;
        public string Created_by { get; set; } = string.Empty;
        public string updated_by { get; set; } = string.Empty;
    }

    public class SellerTodolistActivityDataMasterResponseDto
    {
        public ResponseMSG ResponseMSG { get; set; } = new ResponseMSG();
    }
}
