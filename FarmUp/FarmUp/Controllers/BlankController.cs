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
        public async Task<HttpResponseMessage> GetLIFF(int? id)
        {
            var content = Request.Form.FirstOrDefault().Key;
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                var message = dict["messages"];
                var data1 = dict["data1"];

                return null;
            }
        }
    }
}
