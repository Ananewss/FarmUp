using FruitProject.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;



namespace FruitProject.Controllers
{
    public class AccountController : Controller
    {
        private IConfiguration Configuration;

        public AccountController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        public ActionResult SendWelcomeMesage()
        {
            //send Line message

            //close and return to Line [Or] Welcome message [Or] Profile Page

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                RegisterInformation(model);
            }
          
            return View("RegisterSuccess");
        }

        static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string EncodeSHA1(string source)
        {
            using (SHA1 sha1Hash = SHA1.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                return hash;
            }
        }
        public ActionResult RegisterInformation(LoginModel loginModel)
        {
            var lineUerId = HttpContext.Session.GetString("lineUserId");
            //var lineUerId = "test";
            string connString = Configuration.GetConnectionString($"onedurian");
            loginModel.usr_line_id = lineUerId;
            MySqlConnection conn = new MySqlConnection(connString);
            var salt = RandomString(5);
            //var completePass = "!" + loginModel.Password + salt;



            //var hash = EncodeSHA1(completePass);

            //connString = "Server=queenbeautynetwork.com;Port=3306;Database=oneduriantest;User id=onedurian;password=onedurian;";
            //MySqlConnection conn = new MySqlConnection(connString);

            string sqlCommand = @"INSERT INTO ma_user(usr_firstname,usr_lastname,usr_fullname,usr_phone,usr_salt,usr_line_id,usr_latlong,usr_is_active,updated_by)
                VALUES(@usr_firstname,@usr_lastname,@usr_fullname,@usr_phone,@usr_salt,@usr_line_id,@usr_latlong,@usr_is_active,@updated_by);";

            conn.Open();
            MySqlCommand comm = conn.CreateCommand();
            comm.CommandText = sqlCommand;
            comm.Parameters.AddWithValue("@usr_firstname", loginModel.usr_firstname);
            comm.Parameters.AddWithValue("@usr_lastname", loginModel.usr_lastname);
            comm.Parameters.AddWithValue("@usr_fullname", loginModel.usr_firstname + " " + loginModel.usr_lastname);
            comm.Parameters.AddWithValue("@usr_phone", loginModel.usr_phone);
            comm.Parameters.AddWithValue("@usr_salt", salt);
            comm.Parameters.AddWithValue("@usr_line_id", loginModel.usr_line_id);
            comm.Parameters.AddWithValue("@usr_latlong", loginModel.usr_latlong);
            comm.Parameters.AddWithValue("@usr_is_active", "T");
            comm.Parameters.AddWithValue("@updated_by", "System");
            comm.ExecuteNonQuery();
            long usr_Id = comm.LastInsertedId;

            string sqlCommand_seller = @"INSERT INTO ma_seller(slr_usr_id,slr_farm_name,slr_farm_size,slr_farm_location,slr_district,slr_province,slr_country,slr_district_th,slr_province_th,slr_country_th,updated_by)
                VALUES(@slr_usr_id,@slr_farm_name,@slr_farm_size,@slr_farm_location,@slr_district,@slr_province,@slr_country,@slr_district_th,@slr_province_th,@slr_country_th,@updated_by);";

            comm = conn.CreateCommand();
            comm.CommandText = sqlCommand_seller;
            comm.Parameters.AddWithValue("@slr_usr_id", usr_Id);
            comm.Parameters.AddWithValue("@slr_farm_name", loginModel.slr_farm_name);
            comm.Parameters.AddWithValue("@slr_farm_size", loginModel.slr_farm_size);
            comm.Parameters.AddWithValue("@slr_farm_location", loginModel.slr_farm_location);
            comm.Parameters.AddWithValue("@slr_district", loginModel.slr_district);
            comm.Parameters.AddWithValue("@slr_province", loginModel.slr_province);
            comm.Parameters.AddWithValue("@slr_country", loginModel.slr_country);
            comm.Parameters.AddWithValue("@slr_district_th", loginModel.slr_district_th);
            comm.Parameters.AddWithValue("@slr_province_th", loginModel.slr_province_th);
            comm.Parameters.AddWithValue("@slr_country_th", loginModel.slr_country_th);
            comm.Parameters.AddWithValue("@updated_by", "System");
            comm.ExecuteNonQuery();


            conn.Close();

            return View();
        }

        public ActionResult RegisterMap()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
