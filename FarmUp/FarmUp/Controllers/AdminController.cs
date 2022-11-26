using FruitProject.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace FruitProject.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult User()
        {
            return View();
        }
    }
}
