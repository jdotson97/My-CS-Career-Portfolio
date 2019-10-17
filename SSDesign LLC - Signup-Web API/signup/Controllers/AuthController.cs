using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web.Mvc;
using signup.Models;
using Newtonsoft.Json;
using SSDesign.Admin;

namespace signup.Controllers
{
    public class AuthController : Controller
    {
        private SSDCustomer _user;

        public AuthController()
        {
            _user = new SSDCustomer();
        }

        //=============================================================================================================================

        [HttpGet]
        public ActionResult Index(string username, string password)
        {
            //Obfuscates the URLs a little because if someone requests Auth with no parameters itll return 404 as if it doesnt exist
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password)) { return new HttpStatusCodeResult(404); }

            using (SSDDBConnection ssdbconnect = new SSDDBConnection())
            {
                try
                {
                    ssdbconnect.Open();

                    var result = ssdbconnect.Query<SSDCustomer>($"SELECT * FROM Customers WHERE CONVERT(VARCHAR, username) = '{username}';");

                    //Check the username...
                    if (!result.Any())
                    {
                        return new HttpStatusCodeResult(400);
                    }

                    //Check the password...
                    using (SHA1Managed sha1 = new SHA1Managed())
                    {
                        //Compute the hash of the entered password...
                        var shaHash = sha1.ComputeHash(sha1.ComputeHash(Encoding.UTF8.GetBytes(password + result[0].Salt)));
                        var computedHash = Convert.ToBase64String(shaHash);

                        //And compare that hash to the one stored in the database
                        if (computedHash == result[0].Hash)
                        {
                            //Login SUCCESS! - Return 200 and Json for user
                            return Content(JsonConvert.SerializeObject(result[0])); //new HttpStatusCodeResult(200);
                        }
                        else
                        {
                            //Login FAILURE - Return 400 Password mismatch!
                            return new HttpStatusCodeResult(400);
                        }
                    }
                }
                catch
                {
                    return new HttpStatusCodeResult(500);
                }               
            }
        }

        //=============================================================================================================================

        [HttpGet]
        public ActionResult Version(string productCode)
        {
            using (SSDDBConnection ssdbconnect = new SSDDBConnection())
            {
                try
                {
                    ssdbconnect.Open();

                    switch (productCode)
                    {
                        case "ezcmm":
                            {
                                var result = ssdbconnect.Query<SSDProduct>("SELECT * FROM Products WHERE PID = 1");

                                return Content(JsonConvert.SerializeObject(new SSDProductVersion(result[0].Version, result[0].Mandatory)));
                            }
                        default:
                            {
                                return new HttpStatusCodeResult(400);
                            }
                    }
                }
                catch
                {
                    return new HttpStatusCodeResult(500);
                }                            
            }             
        }
        [HttpPost]
        public ActionResult Version(string username, string password, string productCode, string newVersion, bool mandatory, string key)
        {
            //If the elevated api key is not entered, the server returns 404 to obfuscate the request endpoint...
            if (key != ConfigurationManager.AppSettings["adminKey"])
            {
                return new HttpStatusCodeResult(404);
            }
            
            //Open database client to check the recieved credentials...
            using (SSDDBConnection ssdbconnect = new SSDDBConnection(true))
            {
                ssdbconnect.Open();

                if (ssdbconnect.IsOpen)
                {
                    var result = ssdbconnect.Query<SSDCustomer>($"SELECT * FROM Customers WHERE CONVERT(VARCHAR, username) = '{username}';");

                    #region ** Check Username and Password **
                    //Check the recieved username...
                    if (!result.Any())
                    {
                        return new HttpStatusCodeResult(400);
                    }

                    //Check the recieved password...
                    using (SHA1Managed sha1 = new SHA1Managed())
                    {
                        //Compute the hash of the entered password for admin access...
                        var shaHash = sha1.ComputeHash(sha1.ComputeHash(Encoding.UTF8.GetBytes(password + result[0].Salt + "SSDAdmin")));
                        var computedHash = Convert.ToBase64String(shaHash);

                        if (computedHash != result[0].Hash)
                        {
                            //Bad Password or Not Authorized!
                            return new HttpStatusCodeResult(400);
                        }
                    }
                    #endregion

                    switch (productCode)
                    {
                        case "ezcmm":
                            {
                                ssdbconnect.Query(
                                    $"UPDATE Products SET Version = '{newVersion}' WHERE PID = 1; " +
                                    $"UPDATE Products SET Mandatory = {((mandatory) ? '1' : '0')} WHERE PID = 1;"
                                    );

                                return new HttpStatusCodeResult(200);
                            }
                        default:
                            {
                                return new HttpStatusCodeResult(400);
                            }
                    }
                }

                return new HttpStatusCodeResult(500);
            }                    
        }
    }
}