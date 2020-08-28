using System;
using System.Reflection;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EZ_Social_Media_Manager_pre_alpha
{
    public partial class MainMenu : Form
    {
        //ThreadStart APIConnectRef = new ThreadStart(AttemptAPIAutoConnect)
        private System.Windows.Forms.Timer Timer = new System.Windows.Forms.Timer();

        private Size _originalSize;

        private List<Button> _mainPanelButtons;

        private static readonly string FacebookAPIDialogRequestURI = String.Format("https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri=https://www.facebook.com/connect/login_success.html&response_type=token", FacebookInfo.AppID);

        public MainMenu()
        {
            InitializeComponent();

            VersionNumberLabel.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            Web_Browser.Visible = false;
            SettingsPanel.Visible = false;

            Notification_Label.Text = "";
            Time_Label.Text = DateTime.Now.ToShortTimeString();
            Welcome_Label.Text = $"Welcome, {Authentication.user.Firstname} {Authentication.user.Lastname}!";

            #region ** Theme Implementation **
            PrimaryButton.BackColor = Properties.Settings.Default.Primary;
            SecondaryButton.BackColor = Properties.Settings.Default.Secondary;
            AccentsButton.BackColor = Properties.Settings.Default.Accents;

            if (Properties.Settings.Default.DarkTheme)
            {
                ToggleDarkTheme();
                DarkCheckbox.Checked = true;
            }
            else if (Properties.Settings.Default.CustomTheme)
            {
                ToggleCustomTheme();
                CustomCheckbox.Checked = true;
            }
            else
            {
                ToggleLightTheme();
                LightCheckbox.Checked = true;
            }
            #endregion

            #region ** Clock Implementation **
            Timer.Interval = 1000;
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Start();
            #endregion

            #region ** Button Container Implementation **
            _mainPanelButtons = new List<Button>();

            //_mainPanelButtons.Add(//FacebookButton);
            _mainPanelButtons.Add(button2);
            _mainPanelButtons.Add(button3);
            _mainPanelButtons.Add(button4);
            _mainPanelButtons.Add(button1);
            #endregion

            SetWelcomeMessage();
            LoadSavedSettings();

            AttemptAPIAutoConnect();
        }
        private void MainMenu_Load(object sender, EventArgs e)
        {
            _originalSize = Size;
        }      

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);

            if (_originalSize != Size.Empty)
            {
                MainPanel.Width += (Size.Width - _originalSize.Width);
                MainPanel.Height += (Size.Height - _originalSize.Height);
                NavPanel.Height += (Size.Height - _originalSize.Height);
                ToolbarPanel.Width += (Size.Width - _originalSize.Width);

                SettingsPanel.Width = MainPanel.Width;
                SettingsPanel.Height = MainPanel.Height;
                LinkedAccountsPage.Width = MainPanel.Width;
                LinkedAccountsPage.Height = MainPanel.Height;

                Web_Browser.Width = MainPanel.Width;
                Web_Browser.Height = MainPanel.Height;


                UpdateButtonPositions();

                _originalSize = Size;
            }
        }

        private void UpdateButtonPositions()
        {
            //the 200 represents the width of the facebook button plus the space betweent the buttons (6)
            if ((MainPanel.Width / 200) >= 4)
            {
                if (button4.Location.X != 605)
                {
                    button1.Location = button4.Location;
                    button4.Location = new Point(605, 3);
                }

                if ((MainPanel.Width / 200) >= 5)
                {
                    if (button1.Location.X != 805)
                    {
                        button1.Location = new Point(805, 3);
                    }
                }
                else
                {
                    button1.Location = new Point(5, 204);
                }
            }
            else
            {
                button4.Location = new Point(5, 204);
                button1.Location = new Point(205, 204);
            }
        }

        private void SetWelcomeMessage()
        {
            if (DateTime.Now.Hour > 2 && DateTime.Now.Hour < 12)
            {
                Welcome_Message_Text.Text = $"|  Good Morning, {Authentication.user.Firstname}";
            }
            else if (DateTime.Now.Hour > 12 && DateTime.Now.Hour < 18)
            {
                Welcome_Message_Text.Text = $"|  Good Afternoon, {Authentication.user.Firstname}";
            }
            else if (DateTime.Now.Hour > 18 || DateTime.Now.Hour < 2)
            {
                Welcome_Message_Text.Text = $"|  Good Evening, {Authentication.user.Firstname}";
            }
            else
            {
                Welcome_Message_Text.Text = $"|  Welcome, {Authentication.user.Firstname}";
            }
        }
        private void LoadSavedSettings()
        {
            Refresh_Interval_Trackbar.Value = EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.RefreshInterval;
            Refresh_Message_Interval_Trackbar.Value = EZ_Social_Media_Manager_pre_alpha.Properties.Settings.Default.RefreshMessageInterval;
        }

        public void Timer_Tick(object sender, EventArgs e)
        {
            Time_Label.Text = DateTime.Now.ToShortTimeString();
        }

        public void AttemptAPIAutoConnect()
        {
            AttemptFacebookInstagramAutoConnect();
            //other apis...
        }
        public void AttemptFacebookInstagramAutoConnect()
        {
            WebBrowser browser = new WebBrowser();
            browser.DocumentCompleted += ParseFacebookResponse;

            //attempt to autoconnect to facebook
            browser.Navigate(FacebookAPIDialogRequestURI);
            Notification_Label.Text = "Facebook: Connecting...\nInstagram: Not Connected";
        }

        public async void ParseFacebookResponse(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser browser = sender as WebBrowser;
            Web_Browser.DocumentCompleted += WebBrowser_DocumentCompleted;

            if (browser.Url.AbsoluteUri.Contains("access_token="))
            {
                string SuccessURL = browser.Url.AbsoluteUri;
                Web_Browser.Visible = false;
                FacebookAPIInfo.ParseUserAccessTokenFromURL(ref SuccessURL);
                Notification_Label.Text = "Facebook: Syncing...\nInstagram: Not Connected";

                if ((FacebookAPIInfo.userAccessToken != null) && ((FacebookAPIInfo.AccessTokenValidUntil > System.Environment.TickCount) || (FacebookAPIInfo.AccessTokenValidUntil == 0)))
                {
                    await FacebookAPIInfo.ImportFacebookAccountData();
                    Notification_Label.Text = "Facebook: Connected\nInstagram: Not Connected";
                    return;
                }

                Notification_Label.Text = "Facebook: ERROR";
            }
            else if ((browser.Url.AbsoluteUri.Contains("user_denied") && browser.Url.AbsoluteUri.Contains("login.php")) 
                ||   (browser.Url.AbsoluteUri.Contains("user_denied") && browser.Url.AbsoluteUri.Contains("reauth.php")))
            {
                Notification_Label.Text = "To use Facebook,\nYou must agree to give\nEZ-Social Media Manager\npermission to access your\nFacebook page(s)!\n\nPlease click the Facebook\nbutton!";
                return;
            }
            else
            {
                Notification_Label.Text = "Error: could not automatically\nverify your credentials.\nPlease ensure your are\nconnected to the internet, then\nclick the Facebook button";
            }
        }        
        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {}

        private void Facebook_Or_Instagram_Button_Click(object sender, EventArgs e)
        {
            if (!FacebookAPIInfo.IsUserLoggedIn)
            {
                Web_Browser.DocumentCompleted += ParseFacebookResponse;
                Web_Browser.Navigate(FacebookAPIDialogRequestURI);
                Web_Browser.Visible = true;
            }
            else
            {
                if ((sender as Button) == Facebook_Button)
                {
                    ManageMedia managementWindow = new ManageMedia("Facebook");
                    managementWindow.Show();
                }
                else
                {
                    ManageMedia managementWindow = new ManageMedia("Instagram");
                    managementWindow.Show();
                }
            }
        }

        #region ** Interface Button Functions **
        private void AboutButton_Click(object sender, EventArgs e)
        {
            About aboutPage = new About();
            aboutPage.Show();
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            Web_Browser.Visible = false;
            SettingsPanel.Visible = false;
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void SchedulePostButton_Click(object sender, EventArgs e)
        {
            MakeAPost postingWindow = new MakeAPost();
            postingWindow.Show();
        }
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            SettingsPanel.Visible = true;
        }
        private void ApplyButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.RefreshInterval = Refresh_Interval_Trackbar.Value;
            Properties.Settings.Default.RefreshMessageInterval = Refresh_Message_Interval_Trackbar.Value;
            Properties.Settings.Default.DarkTheme = DarkCheckbox.Checked;
            Properties.Settings.Default.CustomTheme = CustomCheckbox.Checked;
            
            if (CustomCheckbox.Checked)
            {
                Properties.Settings.Default.Primary = PrimaryButton.BackColor;
                Properties.Settings.Default.Secondary = SecondaryButton.BackColor;
                Properties.Settings.Default.Accents = AccentsButton.BackColor;
            }

            Properties.Settings.Default.Save();

            SavedLabel.Visible = true;
        }
        private void ResetButton_Click(object sender, EventArgs e)
        {
            Refresh_Interval_Trackbar.Value = 300;
            Refresh_Message_Interval_Trackbar.Value = 20;
        }
        #endregion

        private void Facebook_Unlink_Button_Click(object sender, EventArgs e)
        {
            // using (var popup = new MessagePopup("Are you sure you want to unlink this?"))
            // {
            //    if (popup.ShowDialog() == DialogResult.OK)
            //    {
                    Task.Run(() => FacebookAPIInfo.facebookService.DeleteFacebookObject(FacebookAPIInfo.userAccessToken, $"{FacebookAPIInfo.FBAccount.id}/permissions"));
                    FacebookAPIInfo.IsUserLoggedIn = false;

                    Web_Browser.Navigate(string.Format("https://www.facebook.com/logout.php?next={0}&access_token={1}", "https://www.facebook.com/connect/login_success.html", FacebookAPIInfo.userAccessToken));
            //    }
            // }
        }
        private void Facebook_Logout_Button_Click(object sender, EventArgs e)
        {
            Web_Browser.Navigate(string.Format("https://www.facebook.com/logout.php?next={0}&access_token={1}", "https://www.facebook.com/connect/login_success.html", FacebookAPIInfo.userAccessToken));
            FacebookAPIInfo.IsUserLoggedIn = false;
        }

        private void Refresh_Interval_Trackbar_ValueChanged(object sender, EventArgs e)
        {
            SavedLabel.Visible = false;
            Refresh_Interval_Textbox.Text = Refresh_Interval_Trackbar.Value.ToString();
        }
        private void Refresh_Message_Interval_Trackbar_ValueChanged(object sender, EventArgs e)
        {
            SavedLabel.Visible = false;
            Refresh_Message_Interval_Textbox.Text = Refresh_Message_Interval_Trackbar.Value.ToString();
        }
        private void Refresh_Interval_Setting_TextBox_TextChanged(object sender, EventArgs e)
        {
            SavedLabel.Visible = false;

            try
            {
                Refresh_Interval_Trackbar.Value = Convert.ToInt32(Refresh_Interval_Textbox.Text);
            }
            catch
            {
            }
        }
        private void Refresh_Message_Interval_Setting_TextBox_TextChanged(object sender, EventArgs e)
        {
            SavedLabel.Visible = false;

            try
            {
                Refresh_Message_Interval_Trackbar.Value = Convert.ToInt32(Refresh_Message_Interval_Textbox.Text);
            }
            catch
            {
            }
        }

        #region ** Theme Controls **
        private void ToggleDarkTheme()
        {
            BackColor = Color.DimGray;

            Color DarkAssGrey = (Color)new ColorConverter().ConvertFromString("#565656");
            foreach (Panel Control in Controls)
            {
                Control.BackColor = DarkAssGrey;
            }

            HomeButton.BackColor = Color.DimGray;
            HomeButton.ForeColor = Color.LightGray;
            SettingsButton.BackColor = Color.DimGray;
            SettingsButton.ForeColor = Color.LightGray;
            ExitButton.BackColor = Color.DimGray;
            ExitButton.ForeColor = Color.LightGray;

            ConnectionsLabel.ForeColor = Color.LightGray;
            Notification_Label.ForeColor = Color.LightGray;
            Welcome_Label.ForeColor = Color.LightGray;
            VersionNumberLabel.ForeColor = Color.LightGray;
            Welcome_Message_Text.ForeColor = Color.LightGray;
            Time_Label.ForeColor = Color.LightGray;
        }
        private void ToggleLightTheme()
        {
            BackColor = Color.Silver;

            foreach (Panel Control in this.Controls)
            {
                Control.BackColor = Color.Silver;
            }

            HomeButton.BackColor = Color.Silver;
            HomeButton.ForeColor = Color.Black;
            SettingsButton.BackColor = Color.Silver;
            SettingsButton.ForeColor = Color.Black;
            ExitButton.BackColor = Color.Silver;
            ExitButton.ForeColor = Color.Black;

            MainPanel.BackColor = Color.Gray;

            ConnectionsLabel.ForeColor = Color.Black;
            Notification_Label.ForeColor = Color.Black;
            Welcome_Label.ForeColor = Color.Black;
            VersionNumberLabel.ForeColor = Color.Black;
            Welcome_Message_Text.ForeColor = Color.Black;
            Time_Label.ForeColor = Color.Black;
        }
        private void ToggleCustomTheme()
        {
            BackColor = PrimaryButton.BackColor;
            foreach (Panel panel in Controls)
            {
                panel.BackColor = SecondaryButton.BackColor;
            }
        }

        private void DarkCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (DarkCheckbox.Checked)
            {
                LightCheckbox.Checked = false;
                CustomCheckbox.Checked = false;
                ToggleDarkTheme();
            }
        }
        private void LightCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (LightCheckbox.Checked)
            {
                DarkCheckbox.Checked = false;
                CustomCheckbox.Checked = false;
                ToggleLightTheme();
            }
        }
        private void CustomCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (CustomCheckbox.Checked)
            {
                DarkCheckbox.Checked = false;
                LightCheckbox.Checked = false;
                ToggleCustomTheme();
            }
        }

        private void Primary_Click(object sender, EventArgs e)
        {
            if (ColorDialog.ShowDialog() == DialogResult.OK)
            {
                PrimaryButton.BackColor = ColorDialog.Color;
            }
        }
        private void Secondary_Click(object sender, EventArgs e)
        {
            if (ColorDialog.ShowDialog() == DialogResult.OK)
            {
                SecondaryButton.BackColor = ColorDialog.Color;
            }
        }
        private void Accents_Click(object sender, EventArgs e)
        {
            if (ColorDialog.ShowDialog() == DialogResult.OK)
            {
                AccentsButton.BackColor = ColorDialog.Color;
            }
        }
        #endregion
    }
}
