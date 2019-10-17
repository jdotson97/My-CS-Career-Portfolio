using System;
using System.Text;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace signup.Controllers
{
    public class SignupController : Controller
    {
        MySqlConnection mysqlConnect;
        MySqlCommand command;

        private string connectionString;

        public SignupController()
        {
            connectionString = String.Format(@"Server=12.202.82.3;Database=ssdesign;Uid={0};Pwd={1};", "jdotson97", "disneyWorld98;2.4");
            mysqlConnect = new MySqlConnection(connectionString);

            try
            {
                mysqlConnect.Open();
            }
            catch
            {
                ViewBag.ErrorMessage = "Database Error Code: 0";
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Message = "Sign Up!";

            return View();
        }
        [HttpPost]
        public ActionResult Index(string Firstname, string Lastname, string Email, string Product, string Username, string Password)
        {
            if (!(mysqlConnect.State.ToString() == "Open"))
            {
                return View();
            }

            #region ** Check Username Availability **
            command = mysqlConnect.CreateCommand();
            command.CommandText = $"SELECT * FROM Customers WHERE username='{Username}'";
            MySqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                ViewBag.Message = "Sign Up!";
                ViewBag.ErrorMessage = "Username already in use!";

                return View();
            }

            reader.Close();

            #endregion

            #region ** Find Largest CID in Database **
            command = mysqlConnect.CreateCommand();
            command.CommandText = $"SELECT MAX(CID) FROM Customers";
            reader = command.ExecuteReader();

            int maxID = -1;
            while (reader.Read())
            {
                maxID = reader.GetInt32(reader.GetOrdinal("MAX(CID)"));

                if (maxID == -1)
                {
                    ViewBag.ErrorMessage = "Database Error Code: 1";
                }
            }

            reader.Close();

            #endregion

            #region ** Compute Salt and Hash and Insert **
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                //FIGURE OUT HOW TO RETURN A PARTICULAR VIEW FOR THE BELOW
                Random rand = new Random();
                byte[] saltSeed = new byte[16];

                rand.NextBytes(saltSeed);

                var salt = sha1.ComputeHash(sha1.ComputeHash(saltSeed));
                var hash = sha1.ComputeHash(sha1.ComputeHash(Encoding.UTF8.GetBytes(Password + Convert.ToBase64String(salt))));

                /*command = mysqlConnect.CreateCommand();
                command.CommandText = 
                    $"INSERT INTO Customers (`username`, `Email`, `Firstname`, `Lastname`, `CID`, `Salt`, `Hash`) " +
                    $"VALUES('{Username}', '{Email}', '{Firstname}', '{Lastname}', {maxID + 1}, '{Convert.ToBase64String(salt)}', '{Convert.ToBase64String(hash)}')";
                reader = command.ExecuteReader(); */
            }

            reader.Close();

            #endregion

            return RedirectToAction("Billing");
        }

        //=============================================================================================================================

        [HttpGet]
        public ActionResult Billing()
        {
            ViewBag.Message = "Sign Up!";

            return View();
        }
        [HttpPost]
        public ActionResult Billing(string Firstname, string Lastname, string CardNumber, string Expiration, string SecurityCode, string Address, string ZipCode)
        {
            ViewBag.Message = "Sign Up!";

            return RedirectToAction("Summary");
        }

        [HttpGet]
        public ActionResult Summary()
        {
            ViewBag.Message = "Sign Up!";

            return View();
        }
        [HttpPost]
        public ActionResult Summary(bool agreedTAC)
        {
            ViewBag.Message = "Sign Up!";

            return View("/Views/Signup/ThankYou.cshtml");
        }
    }
}