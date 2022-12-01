using FarmUp.Dtos.Admin;
using FarmUp.Services.Admin;
using FruitProject.Controllers;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Net;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace FarmUp.Controllers
{
    public class BlankController : Controller
    {

        private IConfiguration _config;
        private readonly ILogger<BlankController> _logger;
        public BlankController(IConfiguration config, ILogger<BlankController> logger)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public String GetLIFF(int? id)
        {
            var content = Request.Form.FirstOrDefault().Key;
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                var lineUserId = dict["lineUserId"];
                var refer = (dict.ContainsKey("ref"))? dict["ref"] : "";

                HttpContext.Session.SetString("lineUserId", lineUserId);


                string strConn = _config.GetConnectionString($"onedurian");

                MySqlConnection mSqlConn = new MySqlConnection(strConn);
                try
                {
                    var sql = @"SELECT * 
                        FROM ma_user
                        WHERE usr_line_id = @usr_line_id";
                    mSqlConn.Open();

                    MySqlCommand mCmd = new MySqlCommand(sql, mSqlConn);
                    mCmd.Parameters.AddWithValue("@usr_line_id", lineUserId);

                    var readData = mCmd.ExecuteReader();
                    if (!readData.Read())
                    {
                        refer = "-Account-Register";
                    }
                    readData.Close();

                }
                catch (Exception ex)
                {

                }
                mSqlConn.Close();

                //HttpContext.Session.GetString("lineUserId")

                //if (refer != null)
                //{
                //    if (refer.Equals("WeatherForecast"))
                //        //RedirectToAction("WeatherForecast","Seller");
                //        Redirect("/Seller/WeatherForecast");
                //}

                return refer.Replace("-","/");
            }
        }
    }
}
