using System;
using System.Text;
using System.Net;
using System.Linq;
using System.Net.Mail;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Configuration;
using Square.Connect.Api;
using Square.Connect.Client;
using Square.Connect.Model;
using signup.Models;
using SSDesign.Admin;


namespace signup.Controllers
{
    public class SignupController : Controller, IDisposable
    {
        private bool _disableInserts = false;

        public SignupController()
        {

        }

        public static string getNewIdempotencyKey()
        {
            Guid seed = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(seed.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            return GuidString;
        }
        public static string getHTMLFormatedConfirmationEmail(string OrderNumber, string Firstname, string Lastname, string username, string password)
        {
            return

            $"<!DOCTYPE html>" +
            $"<html>" +
                $"<head>" +
                    $"<meta charset = \"UTF-8\">" +
                    $"<meta name = \"viewport\" content = \"width=device-width, initial-scale=1.0\">" +
                    $"<title>" +
                        $"SSDesign - Welcome!" +
                    $"</title>" +
                $"</head>" +
                $"<body style = \"background-color: whitesmoke; font-family: sans-serif;\" >" +
                    $"<div style = \"color: whitesmoke; background-color: darkred; width: 98%; padding: 14px; text-align: center;\">" +
                        $"<h1 style = \"margin: auto;\"> Simplicity Software Design - Welcome {Firstname}!</h1>" +
                    $"</div>" +
                    $"<main>" +
                        $"<section style = \"color:grey;\">" +
                            $"<div style = \"text-align: center;\">" +
                                $"<h2>Order #{OrderNumber} - Placed On {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToUniversalTime().ToShortTimeString()} UTC</h2>" +
                            $"</div>" +
                            $"<p style = \"padding: 14px;\">" +
                                $"<b>Please keep this email as it contains information that may be useful in the future.</b><br><br>" +
                            $"</p>" +
                            $"<p style = \"padding-left: 28px;\">" +
                                $"Dear {Firstname} {Lastname},<br><br>" +

                                $"Thank you so much for choosing Simplicity Software Design! If you ever have trouble with anything, " +
                                $"feel free to email support@simplicitysoftwaredesign.com with any questions, comments, or concerns.<br><br>" +

                                $"You may download EZ-Social Media Manager <a href=\"http://107.211.69.236:443/ezcmm/EZ-Installer.msi\">here</a>.<br><br>" +

                                $"To login to EZ-Social Media Manager, simply use credentials you choose during signup. They are as follows:<br><br>" +
                                
                                $"Username: <b>{username}</b><br>" +
                                $"Password: <b>{password}</b><br>" +              
                                $"<br><br>" +

                                $"Generate HTML Table invoice in the future..." +
                            $"</p>" +
                        $"</section>" +
                    $"</main>" +
                $"</body>" +
            $"</html>";
        }

        //=============================================================================================================================

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string Firstname, string Lastname, string Email, string Product, string Username, string Password)
        {         
            #region ** Check Username Availability **
            using (SSDDBConnection ssdbconnect = new SSDDBConnection())
            {
                try
                {
                    ssdbconnect.Open();

                    var result = ssdbconnect.Query<SSDCustomer>($"SELECT * FROM Customers WHERE CONVERT(VARCHAR, username) = '{Username}'");

                    if (result.Any())
                    {
                        ViewBag.ErrorMessage = "Username already in use!";
                        return View();
                    }
                }
                catch
                {
                    return new HttpStatusCodeResult(500);
                }
            }
            #endregion

            Session["username"] = Username;
            Session["password"] = Password;

            Session["Firstname"] = Firstname;
            Session["Lastname"] = Lastname;
            Session["Email"] = Email;

            // Check to make sure all the required information is there before we move onto billing
            if (string.IsNullOrEmpty(Firstname) || string.IsNullOrEmpty(Lastname) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ViewBag.ErrorMessage = "ERROR! Missing one or more required* fields!";
                return View();
            }

            return RedirectToAction("Billing");
        }

        //=============================================================================================================================

        [HttpGet]
        public ActionResult Billing()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Billing(string Firstname, string Lastname, string Nonce, string Address1, string Address2, string City, string State, string Country, string ZipCode)
        {
            //Name as it appears on card...
            Session["BillingFirstname"] = Firstname;
            Session["BillingLastname"] = Lastname;

            //Card information...
            Session["Nonce"] = Nonce;

            //Billing address...
            Session["Address1"] = Address1;
            Session["Address2"] = Address2;
            Session["City"] = City;
            Session["State"] = State;
            Session["Country"] = Country;
            Session["ZIP"] = ZipCode;

            // Check to make sure all the required information is there before we move onto billing
            if (string.IsNullOrEmpty(Firstname) || string.IsNullOrEmpty(Lastname) || string.IsNullOrEmpty(Address1) || string.IsNullOrEmpty(City) ||
                string.IsNullOrEmpty(State) || string.IsNullOrEmpty(Country) || string.IsNullOrEmpty(ZipCode))
            {
                ViewBag.ErrorMessage = "ERROR! Missing one or more required* fields!";
                return View();
            }

            return RedirectToAction("Summary");
        }

        //=============================================================================================================================

        [HttpGet]
        public ActionResult Summary()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Summary(bool AgreedTAC = false, string PromoCode = null)
        {
            ViewBag.ErrorMessage = "";

            if (!AgreedTAC)
            {
                ViewBag.ErrorMessage = "You must read and agree to the terms and conditions above!";
                return View();
            }

            #region ** Create Customer, Charge Credit Card, and Verify **
            // Get some required info from our database first before we do the charge and insert
            var ssdNewCID = -1;
            var ssdOrderNumber = "-1";
            var ssdOrderProduct = new SSDProduct();
            var ssdOrderPromo = new SSDPromo();
            using (SSDDBConnection ssdbconnect = new SSDDBConnection())
            {
                try
                {
                    ssdbconnect.Open();

                    ssdNewCID = ++(ssdbconnect.Query<SSDCustomer>($"SELECT * FROM Customers WHERE CID = (SELECT MAX(CID) FROM Customers)"))[0].CID;
                    ssdOrderNumber = (++(ssdbconnect.Query<SSDTransaction>("SELECT * FROM Transactions WHERE OrderNumber = (SELECT MAX(OrderNumber) FROM Transactions)"))[0].OrderNumber).ToString();
                    ssdOrderProduct = (ssdbconnect.Query<SSDProduct>("SELECT * FROM Products WHERE PID = 1"))[0];

                    // See if there are any promo codes that match what the user entered. In the future allow them to check a code first
                    var promoResults = (ssdbconnect.Query<SSDPromo>($"SELECT * FROM Promos WHERE Code = CONVERT(VARCHAR, '{PromoCode}')"));
                    ssdOrderPromo = (!promoResults.Any()) ? ssdOrderPromo : promoResults[0];

                    // This if for the confirmation email
                    Session["Order"] = ssdOrderNumber;

                    if (ssdOrderNumber == "-1" || ssdNewCID == -1)
                    {
                        throw new Exception("Database Code 1. Your card has not been charged");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"Error: {ex.Message}. Please contact support@simplicitysoftwaredesign.com";
                    return View();
                }
            }

            // Create an Address object with billing info
            var billingAddress = new Address
            (
                AddressLine1: (string)Session["Address1"],
                AddressLine2: (string)Session["Address2"],
                Locality: (string)Session["City"],
                AdministrativeDistrictLevel1: (string)Session["State"],
                PostalCode: (string)Session["ZIP"],
                Country: Address.CountryEnum.US        
            );
            
            // Create a new customer object for this user to use with square api
            var customer = new CreateCustomerRequest
            (
                IdempotencyKey: getNewIdempotencyKey(),
                GivenName: $"{(string)Session["Firstname"]} {(string)Session["Lastname"]}",
                EmailAddress: (string)Session["Email"],
                Address: billingAddress
            );

            // Create a new card to link with the customer above for recursive billing later
            var customerCard = new CreateCustomerCardRequest
            (
                CardNonce: (string)Session["Nonce"],
                BillingAddress: billingAddress,
                CardholderName: $"{(string)Session["BillingFirstname"]} {(string)Session["BillingLastname"]}"
            );

            // Set the charge amount to the price of the SSDProduct
            var chargeAmount = new Money
            (
                Amount: (int)Math.Round((ssdOrderProduct.Price * 1.0575 * 100) * ssdOrderPromo.PriceAdj), // 1,
                Currency: Money.CurrencyEnum.USD
            );

            // Create a new Charge request object
            var chargeBody = new ChargeRequest
            (
                IdempotencyKey: getNewIdempotencyKey(),
                AmountMoney: chargeAmount,              
                DelayCapture: false,
                ReferenceId: ssdOrderNumber,
                Note: $"For product {ssdOrderProduct.Name}"
            );         

            // Initialize square api configuration
            var defaultApiConfig = new Square.Connect.Client.Configuration();
            defaultApiConfig.AccessToken = (ConfigurationManager.AppSettings["SquareATTest"]);
            //var defaultApiClient = new ApiClient(defaultApiConfig);

            // Initialize api objects
            var customerApi = new CustomersApi(defaultApiConfig);
            var transactionsApi = new TransactionsApi(defaultApiConfig);

            string sqCustomerID = null;
            string sqCustomerCardID = null;
            string sqTransactionID = null;
            try
            {
                // Create the customer through the Square API. Try at most 3 times
                for (int i = 0; (string.IsNullOrEmpty(chargeBody.CustomerId)) && (i < 3); ++i)
                {
                    try
                    {
                        var custApiResponse = customerApi.CreateCustomer(customer);
                        sqCustomerID = custApiResponse.Customer.Id;
                        chargeBody.CustomerId = sqCustomerID;
                    }
                    catch
                    {
                        continue;
                    }
                } 
                
                // Create the customer's card through the Square API. Try at most 3 times
                for (int i = 0; (string.IsNullOrEmpty(chargeBody.CustomerCardId)) && (i < 3); ++i)
                {
                    try
                    {
                        var custcardApiResponse = customerApi.CreateCustomerCard(sqCustomerID, customerCard);
                        sqCustomerCardID = custcardApiResponse.Card.Id;
                        chargeBody.CustomerCardId = sqCustomerCardID;
                    }
                    catch
                    {
                       continue;
                    }
                }  
                
                // Send the transaction using the newly created customer id and customer card id in charge request. Try at most 3 times
                for (int i = 0; (string.IsNullOrEmpty(sqTransactionID)) && (i < 3); ++i)
                {
                    try
                    {
                        // This avoid double charges because the idempotency key does not change
                        var transApiResponse = transactionsApi.Charge(ConfigurationManager.AppSettings["SquareLocationIDTest"], chargeBody);
                        sqTransactionID = transApiResponse.Transaction.Id;
                    }
                    catch
                    {
                        continue;
                    }
                }

                // Check to make sure all api calls were successful
                if (string.IsNullOrEmpty(sqCustomerID) || string.IsNullOrEmpty(sqCustomerCardID) || string.IsNullOrEmpty(sqTransactionID))
                {
                    throw new Exception("Square had trouble processing your request");
                }

                // Put this successful transaction into the database
                using (SSDDBConnection ssdbconnect = new SSDDBConnection(true))
                {
                    ssdbconnect.Open();

                    if (!_disableInserts)
                    {
                        ssdbconnect.Query($"INSERT INTO Transactions (OrderNumber, CID, AmountUSD, DateTime, sqtid, sqid, sqcid, PromoCode) " +
                            $"VALUES (" +
                            $"{ssdOrderNumber}, " +
                            $"{ssdNewCID}, " +
                            $"{(double)chargeAmount.Amount / 100}, " +
                            $"'{DateTime.Now.ToString("MM/dd/yyyy h:mm tt")}', " +
                            $"'{sqTransactionID}', " +
                            $"'{sqCustomerID}', " +
                            $"'{sqCustomerCardID}', " +
                            $"'{ssdOrderPromo.Code}'" +
                            $");"
                        );
                    }                   
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}. Please contact support@simplicitysoftwaredesign.com";
                return View();
            }
            #endregion         

            #region ** Insert New User Into Database **
            using (SSDDBConnection ssdbconnect = new SSDDBConnection(true))
            {
                try
                {
                    ssdbconnect.Open();

                    using (SHA1Managed sha1 = new SHA1Managed())
                    {
                        //FIGURE OUT HOW TO RETURN A PARTICULAR VIEW FOR THE BELOW
                        Random rand = new Random();
                        byte[] saltSeed = new byte[16];
                        rand.NextBytes(saltSeed);

                        // Create the salt and hash values for authentication through the api
                        var salt = sha1.ComputeHash(sha1.ComputeHash(saltSeed));
                        var hash = sha1.ComputeHash(sha1.ComputeHash(Encoding.UTF8.GetBytes(Session["password"] + Convert.ToBase64String(salt))));

                        // Finally, attempt to insert the new user into the database
                        if (!_disableInserts)
                        {
                            ssdbconnect.Query($"INSERT INTO Customers (username, Email, Firstname, Lastname, CID, Salt, Hash, sqid, sqcid, CreatedTime) " +
                                $"VALUES(" +
                                    $"'{Session["username"]}', " +
                                    $"'{Session["Email"]}', " +
                                    $"'{Session["Firstname"]}', " +
                                    $"'{Session["Lastname"]}', " +
                                    $"{ssdNewCID}, " +
                                    $"'{Convert.ToBase64String(salt)}', " +
                                    $"'{Convert.ToBase64String(hash)}', " +
                                    $"'{sqCustomerID}', " +
                                    $"'{sqCustomerCardID}', " +
                                    $"'{DateTime.Now.ToString("MM/dd/yyyy h:mm tt")}'" +
                                    $")"

                                    //This is for when I put in recurring promos or 'discounts'
                                    //((ssdOrderPromo.SingleUse) ? ssdOrderPromo.Code : "")}
                                );
                        }                    
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"Error: {ex.Message}. Please contact support@simplicitysoftwaredesign.com";
                    return View();
                }
            }
            #endregion

            return RedirectToAction("ThankYou");
        }

        //=============================================================================================================================

        [HttpGet]
        public ActionResult ThankYou()
        {
            SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(Properties.Settings.Default.SMTPUser, Properties.Settings.Default.SMTPPass);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("support@simplicitysoftwaredesign.com");
            mail.To.Add(new MailAddress((string)Session["Email"]));
            mail.Subject = $"Welcome, {(string)Session["Firstname"]}!";
            mail.IsBodyHtml = true;
            mail.Body = getHTMLFormatedConfirmationEmail((string)Session["Order"], (string)Session["Firstname"], (string)Session["Lastname"], (string)Session["username"], (string)Session["password"]);

            try
            {
                smtpClient.Send(mail);
            }
            finally
            {
                // Clear all session info from this signup...
                Session.Abandon();             
            }

            return View();
        }

        public new void Dispose()
        {
            //Session.Abandon();
            base.Dispose();
        }
    }
}