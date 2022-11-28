using FarmUp.Dtos.Admin;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace FarmUp.Controllers
{
    public class BlankController : Controller
    {
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
