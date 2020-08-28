using System;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace EZ_Social_Media_Manager_pre_alpha
{
    public partial class Login : Form
    {
        public static bool UserIsAuthorized = false;
        public static SSDProductVersion version { get; set; }

        public Login()
        {
            InitializeComponent();
            Notification.Text = String.Empty;

            version = new SSDProductVersion(Assembly.GetExecutingAssembly().GetName().Version.ToString(), false);
            VersionLabel.Text = $"Version: {version.ProductVersionNumber}";      
    
            if (Properties.Settings.Default.DarkTheme)
            {
                BackColor = Color.DimGray;
                Remember_Credentials.ForeColor = Color.LightGray;

                foreach (Control control in Controls)
                {
                    if (control is Label)
                    {
                        control.ForeColor = Color.LightGray;
                    }
                    if (control is TextBox)
                    {
                        control.BackColor = (Color)new ColorConverter().ConvertFromString("#565656");
                        control.ForeColor = Color.LightGray;
                    }
                    if (control is Button)
                    {
                        control.BackColor = Color.DimGray;
                        control.ForeColor = Color.LightGray;
                    }
                }
            }

            UsernameInput.Text = EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.Username;
            PasswordInput.Text = EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.Password;

            Remember_Credentials.Checked = true;
            PasswordInput.UseSystemPasswordChar = true;
        }
        private void Login_Load(object sender, EventArgs e)
        {
            using (HttpClient httpClient = new HttpClient
            {
                //Release
                BaseAddress = new Uri("https://ssdesignsignup.azurewebsites.net/")
                //Debug
                //BaseAddress = new Uri("http://localhost:50091/")
            })
            {
                try
                {
                    var response = Task.Run(() => httpClient.GetAsync($"Auth/Version?productCode=ezcmm")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = Task.Run(() => response.Content.ReadAsStringAsync());

                        SSDProductVersion serverVersion = new SSDProductVersion(JsonConvert.DeserializeObject<SSDProductVersion>(result.Result));

                        if ((!(serverVersion.ProductVersionNumber == version.ProductVersionNumber)))
                        {
                            if ((Convert.ToInt32(serverVersion.Date) > Convert.ToInt32(version.Date)) ||
                                ((Convert.ToInt32(serverVersion.Date) == Convert.ToInt32(version.Date)) &&
                                (Convert.ToInt32(serverVersion.Time) > Convert.ToInt32(version.Time))))
                            {
                                UpdateButton.Visible = true;

                                if (serverVersion.IsMandatory)
                                {
                                    LoginButton.Enabled = false;
                                    Notification.ForeColor = Color.Green;
                                    Notification.Text = "Update Required! Please click the update button.";
                                }
                            }
                        }
                    }
                }
                catch
                {
                    Notification.ForeColor = Color.Red;
                    Notification.Text = "Unable to connect to API! Please check connection...";
                }             
            }
        }


        private void SaveCredentials()
        {
            EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.Username = UsernameInput.Text;
            EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.Password = PasswordInput.Text;
            EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.Save();
        }
        private void ClearCredentials()
        {
            EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.Username = null;
            EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.Password = null;
            EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.Save();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            using (Authentication auth = new Authentication(UsernameInput.Text, PasswordInput.Text))
            {
                try
                {
                    Notification.ForeColor = Color.Black;
                    Notification.Text = "Checking your credentials...";

                    if (auth.AuthenticateUser())
                    {
                        //Notification.Text = "Success! Logging in...";

                        if (Remember_Credentials.Checked)
                        {
                            SaveCredentials();
                        }
                        else
                        {
                            ClearCredentials();
                        }

                        //this is to tell the app that the user is allowed to access the main menu
                        UserIsAuthorized = true;

                        Close();
                    }
                    else
                    {
                        Notification.ForeColor = Color.Red;
                        Notification.Text = "Sorry, the credentials you entered appear to be incorrect!";
                        PasswordInput.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Notification.ForeColor = Color.Red;
                    Notification.Text = $"Error: {ex.Message}";
                }
            }          
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void SignUpButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://ssdesignsignup.azurewebsites.net/");
        }
        private void ForgotButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://ssdesignsignup.azurewebsites.net/");
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            UpdateButton.Text = "Updating";           
            UpdateButton.BackColor = Color.LightSeaGreen;

            LoginButton.Enabled = false;
            ExitButton.Enabled = false;
            UpdateButton.Enabled = false;

            ProgressBar.Visible = true;

            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += client_UpdateProgess;
                client.DownloadFileCompleted += client_Completed;

                client.DownloadFileAsync(
                    //Directory of the release installer on server
                    new System.Uri(@"http://107.211.69.236:443/ezcmm/EZ-Installer.msi"),
                    //Directory of install on client machine
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\EZ-Installer.msi"
                );
            }          
        }
        private void client_UpdateProgess(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }
        private void client_Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            System.Diagnostics.Process.Start($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\EZ-Installer.msi");

            Application.Exit();
        }
    }
}
