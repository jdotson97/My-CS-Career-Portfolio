using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using FastMember;
using Square.Connect.Api;
using Square.Connect.Client;
using Square.Connect.Model;

namespace SquareBillingWorker
{
    public class SSDDBConnection : IDisposable
    {
        private SqlConnection _sqlConnect;

        public bool IsOpen { get; set; }
        public bool IsElevated { get; }

        public bool Failed { get; set; }
        public string ErrorMessage { get; set; }

        public SSDDBConnection()
        {
            _sqlConnect = new SqlConnection();

            IsOpen = false;
            IsElevated = false;

            Failed = false;
            ErrorMessage = string.Empty;
        }
        public SSDDBConnection(bool admin) : this()
        {
            IsElevated = admin;
        }

        public void Query(string queryString)
        {
            try
            {
                var _command = _sqlConnect.CreateCommand();
                _command.CommandText = queryString;
                var _reader = _command.ExecuteReader();

                _reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<T> Query<T>(string queryString) where T : new()
        {
            List<T> result = new List<T>();

            try
            {
                var _command = _sqlConnect.CreateCommand();
                _command.CommandText = queryString;
                var _reader = _command.ExecuteReader();

                while (_reader.Read())
                {
                    Type type = typeof(T);
                    var accessor = TypeAccessor.Create(type);
                    var members = accessor.GetMembers();
                    var t = new T();

                    for (int i = 0; i < _reader.FieldCount; i++)
                    {
                        if (!_reader.IsDBNull(i))
                        {
                            string fieldName = _reader.GetName(i);

                            if (members.Any(m => string.Equals(m.Name, fieldName, StringComparison.OrdinalIgnoreCase)))
                            {
                                accessor[t, fieldName] = _reader.GetValue(i);
                            }
                        }
                    }

                    result.Add(t);
                }

                _reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public void Open()
        {
            if (IsElevated)
            {
                _sqlConnect = new SqlConnection(Properties.Settings.Default.connectionStringServer);
            }
            else
            {
                _sqlConnect = new SqlConnection(Properties.Settings.Default.connectionStringClient);
            }

            try
            {
                _sqlConnect.Open();
                IsOpen = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
                IsOpen = false;

                throw ex;
            }
        }
        public void Close()
        {
            _sqlConnect.Close();
            _sqlConnect.Dispose();
        }
        public void Dispose()
        {
            _sqlConnect.Dispose();
        }
    }
    public class SSDCustomer : IDisposable
    {
        public string Salt { get; set; }
        public string Hash { get; set; }

        public int CID { get; set; }

        public string username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public bool Enabled { get; set; }
        public string sqid { get; set; }
        public string sqcid { get; set; }
        public string CreatedTime { get; set; }

        public SSDCustomer()
        {
            Salt = "";
            Hash = "";

            CID = -1;

            username = "NULL";
            Firstname = "NULL";
            Lastname = "NULL";
            Email = "NULL";
            Enabled = false;
        }
        public SSDCustomer(string username, string firstname, string lastname, string email, bool Enabled) : this()
        {
            this.username = this.username;
            this.Firstname = firstname;
            this.Lastname = lastname;
            this.Email = email;
            this.Enabled = Enabled;
        }

        public bool isNull()
        {
            return (string.IsNullOrEmpty(username));
        }

        void IDisposable.Dispose()
        {

        }
    }
    public class SSDTransaction
    {
        public int OrderNumber;
        public int CID;
        public double AmountUSD;
        public string DateTime;
        public string Note;

        public SSDTransaction()
        {
            OrderNumber = -1;
            CID = -1;
            AmountUSD = -1;
            DateTime = "NULL";
            Note = "";
        }
        public SSDTransaction(int order, int cid, double amountUsd, string dateTime, string note = null)
        {
            OrderNumber = order;
            CID = cid;
            AmountUSD = amountUsd;
            DateTime = dateTime;
            Note = note ?? "";
        }
        public SSDTransaction(SSDTransaction copy) : this(copy.OrderNumber, copy.CID, copy.AmountUSD, copy.DateTime, copy.Note) { }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (SSDDBConnection ssdbconnect = new SSDDBConnection(true))
            {
                var successAccounts = new List<SSDCustomer>();
                Dictionary<SSDCustomer, List<Error>> failureAccounts = new Dictionary<SSDCustomer, List<Error>>();
                var systemErrorMessage = "NO ERRORS";

                try
                {
                    ssdbconnect.Open();

                    #region ** Find All Due Accounts **
                    // Get all the accounts with a created date day later than that of today's date day
                    var day = (Convert.ToInt32(DateTime.Now.Day.ToString()) > 9) ? (DateTime.Now.Day.ToString()) : ($"0{DateTime.Now.Day.ToString()}");
                    var accounts = ssdbconnect.Query<SSDCustomer>($"SELECT * FROM Customers WHERE SUBSTRING(CreatedTime, 4, 5) >= '{day}'");
                    
                    // List of all accounts that require billing
                    var dueAccounts = new List<SSDCustomer>();
                    var todaysDate = DateTime.Now;
                    var numberDaysInThisMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    
                    // Initialize due accounts
                    if (todaysDate.Day == numberDaysInThisMonth)
                    {
                        // If the date they signed up was later but todays date is the last day of the month
                        dueAccounts = accounts;
                    }
                    else
                    {
                        // Find all in the list that matches todays date day using lambda!
                        dueAccounts = accounts.FindAll((a) => (a.CreatedTime.Substring(3, 2) == todaysDate.Day.ToString()));
                    }
                    #endregion

                    #region ** Charge and Confirm All Due Accounts **                  
                    foreach (var account in dueAccounts)
                    {
                        Guid seed = Guid.NewGuid();
                        string GuidString = Convert.ToBase64String(seed.ToByteArray());
                        GuidString = GuidString.Replace("=", "");
                        GuidString = GuidString.Replace("+", "");
                        var idempotencyKey =  GuidString;

                        // Generate an new order number for the coming transaction
                        var orderNumber = (++(ssdbconnect.Query<SSDTransaction>("SELECT * FROM Transactions WHERE OrderNumber = (SELECT MAX(OrderNumber) FROM Transactions)"))[0].OrderNumber).ToString();

                        // Set the charge amount to the price of the SSDProduct
                        var chargeAmount = new Money
                        (
                            Amount: (int)Math.Round(2.99 * 1.0575 * 100),
                            Currency: "USD"
                        );

                        // Create a new Charge request object
                        var chargeBody = new ChargeRequest
                        (
                            CustomerId: account.sqid,
                            CustomerCardId: account.sqcid,

                            IdempotencyKey: idempotencyKey,
                            AmountMoney: chargeAmount,
                            DelayCapture: false,
                            ReferenceId: orderNumber,
                            Note: "For SSD Product Subscription"                    
                        );

                        var defaultApiConfig = new Configuration();
                        defaultApiConfig.AccessToken = (Properties.Settings.Default.SquareATTest);

                        var transactionsApi = new TransactionsApi(defaultApiConfig);
                        var transApiResponse = new ChargeResponse();
                        var transID = "";

                        // Try to send 3 times if not successful the first
                        for (int i = 0; (string.IsNullOrEmpty(transID)) && (i < 3); ++i)
                        {
                            try
                            {
                                // This avoid double charges because the idempotency key does not change
                                transApiResponse = transactionsApi.Charge(Properties.Settings.Default.SquareLocationIDTest, chargeBody);
                                transID = transApiResponse.Transaction.Id;
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        // Add this charge request to the report
                        if (string.IsNullOrEmpty(transApiResponse.Transaction.Id))
                        {
                            failureAccounts.Add(account, transApiResponse.Errors);
                            continue;
                        }
                        else
                        {
                            successAccounts.Add(account);
                        }

                        // Put this successful transaction into the database
                        ssdbconnect.Query($"INSERT INTO Transactions (OrderNumber, CID, AmountUSD, DateTime, sqtid, sqid, sqcid) " +
                            $"VALUES (" +
                            $"{orderNumber}, " +
                            $"{account.CID}, " +
                            $"{(double)chargeAmount.Amount / 100}, " +
                            $"'{DateTime.Now.ToString("MM/dd/yyyy h:mm tt")}', " +
                            $"'{transID}', " +
                            $"'{account.sqid}', " +
                            $"'{account.sqcid}'" +
                            $");");       
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    // Send us an email that says that this didnt ass work
                    systemErrorMessage = ex.Message;
                }
                finally
                {
                    // Put the email summary in here
                    SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587);
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(Properties.Settings.Default.SMTPUser, Properties.Settings.Default.SMTPPass);
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("support@simplicitysoftwaredesign.com");

                    mail.To.Add("support@simplicitysoftwaredesign.com");
                    mail.To.Add("joshuadotson1997@yahoo.com");
                    mail.To.Add("mattpope97@yahoo.com");

                    mail.Subject = $"Billing Summary for {DateTime.Now.ToLongDateString()}";
                    mail.Body = $"System Error Message: {systemErrorMessage}";

                    mail.Body +=
                        $"Here is a summary of today's billing:" +
                        $"\n\nCompleted:\n";

                    // Add the successful accounts into the email
                    foreach (var account in successAccounts)
                    {
                        mail.Body += $"{account.username}\n";
                    }

                    mail.Body +=
                        $"\n\nFailed:\n";

                    // Add the not so successful accounts into the email
                    foreach (var entry in failureAccounts)
                    {
                        mail.Body += $"{entry.Key.username}\n\tError:{entry.Value.ToString()}\n";
                    }

                    try
                    {
                        smtpClient.Send(mail);
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}
