using FarmUp.Controllers;
using FarmUp.Dtos.Admin;
using FarmUp.Services.Admin;
using FarmUp.Services.Seller;
using FruitProject.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FruitProject.Controllers
{
    public class AdminController : Controller
    {
        private IConfiguration _config;
        private readonly ILogger<AdminController> _logger;
        private readonly BoardcastService _boardcastService;
        private readonly AdminTodayPriceService _adminTodayPriceService;
        public AdminController(IConfiguration config, ILogger<AdminController> logger, BoardcastService boardcastService, AdminTodayPriceService adminTodayPriceService)
        {
            _logger = logger;
            _boardcastService = boardcastService;
            _adminTodayPriceService = adminTodayPriceService;
            _config = config;
        }

        public ActionResult User()
        {
            return View();
        }

        public async Task<ActionResult> BroadcastAsync()
        {
            var getValue = await _boardcastService.GetBoardcastUser();
            return View(getValue);
        }

        public async Task<ActionResult> TodayPrice()
        {
            var getValue = await _adminTodayPriceService.GetProductList();
            return View(getValue);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Test(int? id)
        {
            var content = Request.Form.FirstOrDefault().Key;
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                var message = dict["messages"];
                var line = dict["line"];

                if (line.Equals("All"))
                {

                    // To create messages
                    var message1 = new Message("text", message);
                    var root = new Root();
                    root.addMessage(message1);

                    // To serialize
                    var json = JsonConvert.SerializeObject(root);
                    var rs = await SendLineMessageAsync("https://api.line.me/v2/bot/message/broadcast", json);
                    return rs;
                }
                else
                {
                    var splited = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    HttpResponseMessage rs = null;
                    HttpResponseMessage error = null;
                    foreach (var lineUserId in splited)
                    {
                        var message1 = new Message("text", message);
                        var root = new Root();
                        root.to = lineUserId.Trim();
                        root.addMessage(message1);

                        // To serialize
                        var json = JsonConvert.SerializeObject(root);
                        rs = await SendLineMessageAsync("https://api.line.me/v2/bot/message/push", json);
                        if (rs.StatusCode != HttpStatusCode.OK)
                            error = rs;
                    }
                    if (error != null) return error;
                    else return rs;
                }
            }
        }

        private async Task<HttpResponseMessage> SendLineMessageAsync(String url, String json)
        {
            var accessToken = _config["lineBearer"];
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", "Bearer " + accessToken);

            request.Content = new StringContent(
                json.ToString(),
                Encoding.UTF8,
                "application/json"
            );

            return await client.SendAsync(request);
        }

        [HttpPost]
        public async Task<bool> AddTodayPriceAsync(int? id)
        {
            var content = Request.Form.FirstOrDefault().Key;
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

                var rs = await _adminTodayPriceService.InsertTodayPriceAsync(dict);

                return rs;
            }
            return true;
        }
    }
}
