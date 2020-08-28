using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EZ_Social_Media_Manager_pre_alpha
{
    public class SSDUser : IDisposable
    {
        public string username;
        public string Firstname;
        public string Lastname;
        public string Email;

        public SSDUser()
        {
            username = "test";
            Firstname = "Testy";
            Lastname = "McTesterson";
            Email = "testing@ssdesign.com";
        }
        public SSDUser(string username, string firstname, string lastname, string email)
        {
            this.username = username;
            this.Firstname = firstname;
            this.Lastname = lastname;
            this.Email = email;
        }
        public SSDUser(SSDUser copy) : this(copy.username, copy.Firstname, copy.Lastname, copy.Email) {}

        public bool isNull()
        {
            return (string.IsNullOrEmpty(username));
        }

        void IDisposable.Dispose()
        {

        }
    }

    public class SSDProductVersion
    {
        public string ProductVersionNumber { get; set; }
        public bool IsMandatory { get; set; }

        public string MajorRelease { get; set; }
        public string MinorRelease { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

        public  SSDProductVersion()
        {
            ProductVersionNumber = "0.0.0.0";
            IsMandatory = false;
        }
        public SSDProductVersion(string version, bool mandatory)
        {
            ProductVersionNumber = version;
            IsMandatory = mandatory;

            string[] pieces = ProductVersionNumber.Split('.');

            MajorRelease = pieces[0];
            MinorRelease = pieces[1];
            Date = pieces[2];
            Time = pieces[3];
        }
        public SSDProductVersion(SSDProductVersion copy) : this(copy.ProductVersionNumber, copy.IsMandatory) { }
    }

    public class Authentication : IDisposable
    {
        public static SSDUser user { get; set; }

        public string username { get; set; }
        public string password { get; set; }

        public Authentication()
        {
            username = "NULL";
            password = "NULL";

            user = new SSDUser();

            user.Firstname = "SSDesign";
            user.Lastname = "User";
        }
        public Authentication(string username, string password) : this()
        {
            this.username = username;
            this.password = password;
        }

        public bool AuthenticateUser()
        {
            using (HttpClient httpClient = new HttpClient
            {
                //Release
                BaseAddress = new Uri("https://ssdesignsignup.azurewebsites.net//")
                //Debug
                //BaseAddress = new Uri("http://localhost:50091/")
            })
            {
                try
                {
                    var response = Task.Run(() => httpClient.GetAsync($"Auth?username={username}&password={password}")).Result;

                    if (!response.IsSuccessStatusCode)
                        return false;

                    var result = Task.Run(() => response.Content.ReadAsStringAsync());

                    user = new SSDUser(JsonConvert.DeserializeObject<SSDUser>(result.Result));

                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    throw ex;
                    //return false;
                }
            }             
        }

        void IDisposable.Dispose()
        {

        }
    }
}