using FarmUp.Dtos;
using FarmUp.Dtos.Seller;
using FarmUp.Dtos.Seller.Todolist;
using FarmUp.Services.Seller;
using FarmUp.Services.Seller.Todolist;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.IO;
using FarmUp.Models;
using System.Dynamic;
using Ubiety.Dns.Core;

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


        public SellerController(IWebHostEnvironment webHostEnvironment, IConfiguration config, ILogger<SellerController> logger, TodoListService todoListService, WeatherForecastService weatherForecastService, TodayPriceService todayPriceService, SellerActivityService SellerActService)
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

        public ActionResult TakePhoto()
        {
            return View();
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

            var transToArray = readActivitySelectList.activitylistSelectObjs.Select(md => md.ActDesc).ToArray();
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
            //var lineUerId = HttpContext.Session.GetString("lineUserId");
            var userId = HttpContext.Session.GetString("userId");

            var answerList = GetAnswerList(userId);
            //ViewBag.answerList = answerList;
            //ViewData["AnswerList"] = answerList;
            return View("PlantMedic", answerList);
        }

        public AnswerList GetAnswerList(string user_id)
        {
            string strConn = _config.GetConnectionString($"onedurian");
            MySqlConnection mSqlConn = new MySqlConnection(strConn);
            AnswerList answerList = new AnswerList();
            try
            {
                mSqlConn.Open();
                {
                    MySqlCommand usrCmd = new MySqlCommand(@"SELECT
	                                                        tr_question.question_id, 
	                                                        tr_question.q_from_usr_id, 
	                                                        tr_question.question, 
	                                                        tr_question.answer, 
	                                                        tr_question.answer_from
                                                        FROM
	                                                        tr_question
                                                        WHERE tr_question.q_from_usr_id = @user_id
                                                            AND deleted_at IS NULL
                                                        ORDER BY created_at DESC", mSqlConn);
                    usrCmd.Parameters.AddWithValue("@user_id", user_id);

                    var reader = usrCmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            AnswersModel model = new AnswersModel();
                            //model.q_ID = reader.GetInt32(0);
                            model.q_usr_ID = (int)reader["q_from_usr_id"];
                            model.ans_question = reader["question"].ToString();
                            model.ans_answer = (String.IsNullOrWhiteSpace(reader["answer"].ToString())) ? "[[กรุณารอคำตอบจากผู้เชี่ยวชาญ]]" : reader["answer"].ToString();
                            model.ans_from = (String.IsNullOrWhiteSpace(reader["answer_from"].ToString())) ? "" : reader["answer"].ToString();

                            answerList.AnswersModelList.Add(model);

                        }
                        reader.Close();
                    }
                }
                {
                    MySqlCommand usrCmd = new MySqlCommand(@"SELECT
	                                                        tr_question.question_id, 
	                                                        tr_question.q_from_usr_id, 
	                                                        tr_question.question, 
	                                                        tr_question.answer, 
	                                                        tr_question.answer_from
                                                        FROM
	                                                        tr_question
                                                        WHERE question_order IS NOT NULL
                                                            AND deleted_at IS NULL
                                                        ORDER BY question_order", mSqlConn);

                    var reader = usrCmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            AnswersModel model = new AnswersModel();
                            //model.q_ID = reader.GetInt32(0);
                            model.q_usr_ID = (int)reader["q_from_usr_id"];
                            model.ans_question = reader["question"].ToString();
                            model.ans_answer = (String.IsNullOrWhiteSpace(reader["answer"].ToString())) ? "[[กรุณารอคำตอบจากผู้เชี่ยวชาญ]]" : reader["answer"].ToString();
                            model.ans_from = (String.IsNullOrWhiteSpace(reader["answer_from"].ToString())) ? "" : reader["answer"].ToString();

                            answerList.AnswersModelExtraList.Add(model);

                        }
                        reader.Close();
                    }
                }
                {
                    MySqlCommand usrCmd = new MySqlCommand(@"SELECT
	                                                        imgUrl,
                                                            title,
                                                            price
                                                        FROM
	                                                        tr_advertisement
                                                        WHERE deleted_at IS NULL
                                                        ORDER BY ads_order", mSqlConn);

                    var reader = usrCmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            AdsModel model = new AdsModel();
                            //model.q_ID = reader.GetInt32(0);
                            model.imgUrl = reader["imgUrl"].ToString();
                            model.title = reader["title"].ToString();
                            model.price = (Decimal)reader["price"];

                            answerList.AdsModelList.Add(model);

                        }
                        reader.Close();
                    }
                }
                mSqlConn.Close();   

            }
            catch (Exception)
            {
                mSqlConn.Close();
            }
            return answerList;
        }

        [HttpPost]
        public ActionResult sendQuestion(string Question)
        {
            //var lineUerId = HttpContext.Session.GetString("lineUserId");
            var userId = HttpContext.Session.GetString("userId");

            string strConn = _config.GetConnectionString($"onedurian");
            MySqlConnection mSqlConn = new MySqlConnection(strConn);

            MySqlCommand insertCMD = new MySqlCommand(@"INSERT INTO tr_question(question_id,q_from_usr_id,question)
                                                        VALUES (UNHEX(CONCAT(REPLACE(UUID(), '-', ''))),@usr_id,@question);", mSqlConn);
            insertCMD.Parameters.AddWithValue("@usr_id", userId);
            insertCMD.Parameters.AddWithValue("@question", Question);
            mSqlConn.Open();
            insertCMD.ExecuteNonQuery();
            mSqlConn.Close();

            var answer = GetAnswerList(userId);
            return View("PlantMedic",answer);
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

        [HttpPost]
        public ResponseMSG AddSellItem(int? id)
        {

            var userId = HttpContext.Session.GetString("userId");
            
            var productype = Request.Form["productype"].ToString();
            var productgrade = Request.Form["productgrade"].ToString();
            var productgradetitle = Request.Form["productgradetitle"].ToString();
            var price = Request.Form["price"].ToString();
            var volume = Request.Form["volume"].ToString();
            var dtpicker = Request.Form["dtpicker"].ToString();

            string strConn = _config.GetConnectionString($"onedurian");
            MySqlConnection mSqlConn = new MySqlConnection(strConn);

            MySqlCommand insertCMD = new MySqlCommand(@"INSERT INTO tr_product(prd_id,prd_usr_id,prd_pdt_desc,prd_pdg_id,prd_pdg_desc,prd_amount,prd_price_per_unit,prd_harvest_time,updated_by)
                                                        VALUES (UNHEX(CONCAT(REPLACE(UUID(), '-', ''))),@prd_usr_id,@prd_pdt_desc,@prd_pdg_id,@prd_pdg_desc,@prd_amount,@prd_price_per_unit,@prd_harvest_time,@updated_by);", mSqlConn);
            insertCMD.Parameters.AddWithValue("@prd_usr_id", userId);
            insertCMD.Parameters.AddWithValue("@prd_pdt_desc", productype);
            insertCMD.Parameters.AddWithValue("@prd_pdg_id", productgrade);
            insertCMD.Parameters.AddWithValue("@prd_pdg_desc", productgradetitle);
            insertCMD.Parameters.AddWithValue("@prd_amount", volume);
            insertCMD.Parameters.AddWithValue("@prd_price_per_unit", price);
            insertCMD.Parameters.AddWithValue("@prd_harvest_time", dtpicker);
            insertCMD.Parameters.AddWithValue("@updated_by", userId);
            mSqlConn.Open();
            insertCMD.ExecuteNonQuery();
            mSqlConn.Close();

            ResponseMSG responseMSG = new ResponseMSG();
            responseMSG.Status = 200;
            responseMSG.Result = $"Success";

            return responseMSG;
        }

        public ActionResult SellItem()
        {
            string strConn = _config.GetConnectionString($"onedurian");
            MySqlConnection mSqlConn = new MySqlConnection(strConn);
            SellItemDtoList answerList = new SellItemDtoList();
            try
            {
                mSqlConn.Open();
                {
                    MySqlCommand usrCmd = new MySqlCommand(@"SELECT *
                                                        FROM ma_productgrade
                                                        WHERE deleted_at IS NULL ", mSqlConn);

                    var reader = usrCmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            SellItemGradeDto model = new SellItemGradeDto();
                            //model.q_ID = reader.GetInt32(0);
                            model.pdg_id = reader["pdg_id"].ToString();
                            model.pdg_description = reader["pdg_description"].ToString();

                            answerList.sellItemGradeDtoList.Add(model);

                        }
                        reader.Close();
                    }
                }
                {
                    MySqlCommand usrCmd = new MySqlCommand(@"SELECT *
                                                        FROM tr_product
                                                        WHERE deleted_at IS NULL ", mSqlConn);

                    var reader = usrCmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            SellItem model = new SellItem();
                            //model.q_ID = reader.GetInt32(0);
                            model.prd_datetime = (DateTime) reader["prd_datetime"];
                            model.prd_pdt_desc = reader["prd_pdt_desc"].ToString();
                            model.prd_pdg_id = (int)reader["prd_pdg_id"];
                            model.prd_pdg_desc = reader["prd_pdg_desc"].ToString();
                            model.prd_amount = (Decimal)reader["prd_amount"];
                            model.prd_price_per_unit = (Decimal)reader["prd_price_per_unit"];
                            model.prd_harvest_time = (DateTime)reader["prd_harvest_time"];
                            answerList.sellItemList.Add(model);

                        }
                        reader.Close();
                    }
                }
                mSqlConn.Close();

            }
            catch (Exception)
            {
                mSqlConn.Close();
            }
            return View(answerList);
        }

        public ActionResult TestJQWidget()
        {
            return View();
        }

    }
}
