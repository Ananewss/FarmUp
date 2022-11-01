using Microsoft.AspNetCore.Mvc.Rendering;

namespace FarmUp.Dtos.Seller
{
    public class TodoListDto
    {
        public List<SelectListItem> DoList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ProblemList { get; set; } = new List<SelectListItem>();

       
    }

    public class TodoListFormDto
    {
        public string[]? todoList { get; set; }
        public string[]? problemList { get; set; }
        public List<IFormFile> imgProblem = new List<IFormFile>();
    }
}
