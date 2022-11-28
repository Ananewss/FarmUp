using FarmUp.Dtos.Seller;
using FarmUp.Dtos.Seller.Todolist;
using FarmUp.Services.Seller;
using FarmUp.Services.Seller.Todolist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FarmUp.Controllers
{
    public class SellerController : Controller
    {
        private readonly ILogger<SellerController> _logger;

        private readonly TodoListService _todolist;

        private readonly SellerActivityService _SellerActService;

        private readonly WeatherForecastService _weatherForecastService;

        private readonly TodayPriceService _todayPriceService;
        public SellerController(ILogger<SellerController> logger,TodoListService todoList, WeatherForecastService weatherForecastService, TodayPriceService todayPriceService, SellerActivityService SellerActService)
        {
            _logger = logger;
            _todolist = todoList;
            _weatherForecastService = weatherForecastService;
            _todayPriceService = todayPriceService;
            _SellerActService = SellerActService;
        }

        public async Task<ActionResult> TodoList()
        {
            //TodoListDto renderList = new TodoListDto();
            SellerActivityDto sellerActivityDto = new SellerActivityDto();
            var getValue = await _todolist.SellerToDoListProblemList();
            ViewData["selectTodoListValue"]= getValue.DoList;
            ViewData["selectProblemListValue"] = getValue.ProblemList;
            TodoListFormDto todoListFormDto = new TodoListFormDto();
            return View(sellerActivityDto);
        }

        //[HttpPost]
        //public async Task<ActionResult> TodoList(TodoListFormDto todoListFormDtoParam,List<IFormFile> imgFile)
        //{
        //    TodoListDto renderList = new TodoListDto();
        //    var getValue = await _todolist.SellerToDoListProblemList();
        //    ViewData["selectTodoListValue"] = getValue.DoList;
        //    ViewData["selectProblemListValue"] = getValue.ProblemList;
        //    TodoListFormDto todoListFormDto = new TodoListFormDto();
        //    return View(todoListFormDto);
        //}


        //For Activity selectList
        //Pon Created at 26-11-65
        [HttpGet]
        public async Task<ActionResult> GetAllActivity()
        {
            var readActivitySelectList = await _todolist.ReadActivitylistSelectListObj();

            var transToArray = readActivitySelectList.activitylistSelectObjs.Select(md=>md.ActDesc).ToArray();
            return Ok(transToArray);
        }


        //Use This For Save Activity
        //Pon Created at 27-11-65
        [HttpPost]
        public async Task<ActionResult> TodoList(SellerActivityDto sellerActivityDto, List<IFormFile> imgFile)
        {
            if (ModelState.IsValid)
            {
                var SaveDataResponse = await _todolist.AddActivityAndProblem(sellerActivityDto, imgFile);
                return RedirectToAction("SaveActivitySuccess");
            }

            else
            {
                return View();
            }
            //SellerActivityDto sellerActivityDtoObj = new SellerActivityDto();
            //var AddDataResponse = await _todolist.AddActivityAndProblem(sellerActivityDto, imgFile);
            ////var addDataResp 
            //return View(sellerActivityDtoObj);
            //var SaveTodoListResponse = _SellerActService

        }
        //[HttpPost]
        //public async Task<ActionResult> TodoList(string[] ActDesc, string[] ActTopic, List<IFormFile> imgFile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        return RedirectToAction("SaveActivitySuccess");
        //    }

        //    else
        //    {
        //        return View();
        //    }
        //    //SellerActivityDto sellerActivityDtoObj = new SellerActivityDto();
        //    //var AddDataResponse = await _todolist.AddActivityAndProblem(sellerActivityDto, imgFile);
        //    ////var addDataResp 
        //    //return View(sellerActivityDtoObj);
        //    //var SaveTodoListResponse = _SellerActService

        //}
        public ActionResult SaveActivitySuccess()
        {
            return View();
        }

        public async Task<ActionResult> WeatherForecast()
        {
            var lineUerId = HttpContext.Session.GetString("lineUserId");
            var readWeatherData = await _weatherForecastService.GetWeatherForecastByLocation(lineUerId);
            return View(readWeatherData);
        }

        public ActionResult PlantMedic()
        {
            return View();
        }

        public async Task<ActionResult> TodayPriceAsync()
        {
            var getValue = await _todayPriceService.GetTodayPrice();
            return View(getValue);
        }

        public ActionResult TestJQWidget()
        {
            return View();
        }

    }
}
