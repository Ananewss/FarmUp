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
            var salt = RandomString(5);
            var completePass = "!" + loginModel.Password + salt;

            var hash = EncodeSHA1(completePass);

            string connString = "datasource=localhost;port=3306;database=orchardmatching;username=root;password=";
            MySqlConnection conn = new MySqlConnection(connString);

            string sqlCommand = @"INSERT INTO ma_user(usr_phone,usr_password,usr_salt,usr_is_active)
                VALUES(@usr_phone,@usr_password,@usr_salt,@usr_is_active);";

            conn.Open();
            MySqlCommand comm = conn.CreateCommand();
            comm.CommandText = sqlCommand;
            comm.Parameters.AddWithValue("@usr_phone", loginModel.Phone);
            comm.Parameters.AddWithValue("@usr_password", hash);
            comm.Parameters.AddWithValue("@usr_salt", salt);
            comm.Parameters.AddWithValue("@usr_is_active", "F");
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
