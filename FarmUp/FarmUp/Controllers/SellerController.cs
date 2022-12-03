using FarmUp.Dtos;
using FarmUp.Dtos.Seller;
using FarmUp.Dtos.Seller.Todolist;
using FarmUp.Services.Seller;
using FarmUp.Services.Seller.Todolist;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IO;

namespace FarmUp.Controllers
{
    public class SellerController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly IConfiguration _config;

        private readonly ILogger<SellerController> _logger;

        private readonly TodoListService _todolistSrvice;

        private readonly SellerActivityService _SellerActService;

        private readonly WeatherForecastService _weatherForecastService;

        private readonly TodayPriceService _todayPriceService;

        public SellerController(IWebHostEnvironment webHostEnvironment,IConfiguration config, ILogger<SellerController> logger,TodoListService todoListService, WeatherForecastService weatherForecastService, TodayPriceService todayPriceService, SellerActivityService SellerActService)
        {
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _logger = logger;
            _todolistSrvice = todoListService;
            _weatherForecastService = weatherForecastService;
            _todayPriceService = todayPriceService;
            _SellerActService = SellerActService;
        }

        public async Task<ActionResult> TodoList()
        {
            var lineUerId = HttpContext.Session.GetString("lineUserId");
            var userId = HttpContext.Session.GetString("userId");
            var readTodoListData = await _todolistSrvice.GetTodoListByUser(userId);
            return View(readTodoListData);

            //TodoListDto renderList = new TodoListDto();
            //SellerActivityDto sellerActivityDto = new SellerActivityDto();
            //var getValue = await _todolist.SellerToDoListProblemList();
            //ViewData["selectTodoListValue"]= getValue.DoList;
            //ViewData["selectProblemListValue"] = getValue.ProblemList;
            //TodoListFormDto todoListFormDto = new TodoListFormDto();
            //return View();
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
            var readActivitySelectList = await _todolistSrvice.ReadActivitylistSelectListObj();

            var transToArray = readActivitySelectList.activitylistSelectObjs.Select(md=>md.ActDesc).ToArray();
            return Ok(transToArray);
        }


        //Use This For Save Activity
        //Pon Created at 27-11-65
        //[HttpPost]
        //public async Task<ActionResult> TodoList(SellerActivityDto sellerActivityDto, List<IFormFile> imgFile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var SaveDataResponse = await _todolistSrvice.AddActivityAndProblem(sellerActivityDto, imgFile);
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


        [HttpPost]
        public async Task<ResponseMSG> UploadFile(int? id)
        {
            //upload file
            var datatype = Request.Form["datatype"].ToString();
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                var serverPath = _webHostEnvironment.WebRootPath; ;
                var uploadPath = _config["UploadPath"];
                var fileName = String.Format("{0}_{1}", Guid.NewGuid().ToString(), Path.GetFileName(file.FileName));
                string _path = Path.Combine(serverPath,uploadPath, fileName);
                string _dbPath = Path.Combine(uploadPath, fileName);
                FileStream outputFileStream = new FileStream(_path, FileMode.Create);
                file.CopyTo(outputFileStream);
                outputFileStream.Flush();
                outputFileStream.Close();

                //set database 

                var lineUserId = HttpContext.Session.GetString("lineUserId");
                var userId = HttpContext.Session.GetString("userId");

                var resultData = await _todolistSrvice.AddActivityAndProblem(userId, _dbPath, datatype);
                return resultData;
            }

            ResponseMSG responseMSG = new ResponseMSG();
            responseMSG.Status = 400;
            responseMSG.Result = $"Add data fail. = No selected file";

            return responseMSG;
        }


        [HttpPost]
        public async Task<ResponseMSG> UpdateDesc(int? id)
        {
            //upload file
            var title = Request.Form["title"].ToString();
            var desc = Request.Form["desc"].ToString();
            var imgUrl = Request.Form["imgUrl"].ToString();


            var resultData = _todolistSrvice.UpdateActivity(title, desc, imgUrl);
            return resultData;
        }

        public ActionResult TestJQWidget()
        {
            return View();
        }

    }
}
