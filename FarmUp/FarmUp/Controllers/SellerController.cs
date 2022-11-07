using FarmUp.Dtos.Seller;
using FarmUp.Services.Seller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FarmUp.Controllers
{
    public class SellerController : Controller
    {
        private readonly ILogger<SellerController> _logger;

        private readonly TodoList _todolist;

        private readonly WeatherForecastService _weatherForecastService;
        public SellerController(ILogger<SellerController> logger,TodoList todoList, WeatherForecastService weatherForecastService)
        {
            _logger = logger;
            _todolist = todoList;
            _weatherForecastService = weatherForecastService;
        }

        public async Task<ActionResult> TodoList()
        {
            //TodoListDto renderList = new TodoListDto();
            var getValue = await _todolist.SellerToDoListProblemList();
            ViewData["selectTodoListValue"]= getValue.DoList;
            ViewData["selectProblemListValue"] = getValue.ProblemList;
            TodoListFormDto todoListFormDto = new TodoListFormDto();
            return View(todoListFormDto);
        }

        [HttpPost]
        public async Task<ActionResult> TodoList(TodoListFormDto todoListFormDtoParam,List<IFormFile> imgFile)
        {
            TodoListDto renderList = new TodoListDto();
            var getValue = await _todolist.SellerToDoListProblemList();
            ViewData["selectTodoListValue"] = getValue.DoList;
            ViewData["selectProblemListValue"] = getValue.ProblemList;
            TodoListFormDto todoListFormDto = new TodoListFormDto();
            return View(todoListFormDto);
        }

        public async Task<ActionResult> WeatherForecast()
        {
            var readWeatherData = await _weatherForecastService.GetWeatherForecastByLocation("", "", "");
            return View(readWeatherData);
        }

        public ActionResult PlantMedic()
        {
            return View();
        }

        public ActionResult TestJQWidget()
        {
            return View();
        }
    }
}
