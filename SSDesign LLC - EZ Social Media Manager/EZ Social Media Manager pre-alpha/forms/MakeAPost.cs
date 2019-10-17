using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using SSDesign.MediaAPI;

/// <summary>
/// There is a bug where the user highlights all the text in the richtextbox and backspaces (deleting it), it does not register as text changing
/// and therefore does not adjust the character limit unless done manually. This really needs fixed before real postingb
/// 
/// make the auto schedule time feaure actually do something non-retarded
/// </summary>

namespace EZ_Social_Media_Manager_pre_alpha
{
    public partial class MakeAPost : Form
    {
        public int CharacterLimit { get; set; }
        public string AttachmentFilePath { get; set; }

        public MakeAPost()
        {
            InitializeComponent();
            CharacterLimit = 63203; //facebook character limit
        }
        
        #region ** Backend Methods **
        private void AdjustCharacterLimit(int newLimit)
        {
            CharacterLimit = newLimit;
            CharacterLimitLabel.Text = $"Characters Remaining: {CharacterLimit}";

            PostButton.Enabled = !(PostEntry.Text.Length > CharacterLimit);
        }
        public void AdjustForBestTimeToPost()
        {
            PostSchedular.Value = new DateTime
                (PostSchedular.Value.Year,
                PostSchedular.Value.Month,
                PostSchedular.Value.Day,
                15, 00, 00);
        }

        private void DarkTheme()
        {
            BackColor = Color.DimGray;

            foreach (Control control in Controls)
            {
                if (control is Panel)
                {
                    control.BackColor = (Color)new ColorConverter().ConvertFromString("#565656");

                    foreach (Control panelControl in control.Controls)
                    {
                        if (panelControl is Button)
                        {
                            panelControl.BackColor = Color.DimGray;
                            panelControl.ForeColor = Color.LightGray;
                        }
                        else if (panelControl is CheckBox || panelControl is Label)
                        {
                            panelControl.ForeColor = Color.LightGray;
                        }
                    }
                }
                else if (control is Button)
                {
                    control.BackColor = Color.DimGray;
                    control.ForeColor = Color.LightGray;
                }
                else if (control is CheckBox || control is Label)
                {
                    control.ForeColor = Color.LightGray;
                }
            }
        }

        public string ParseScheduledTimeToUnix(DateTime rawDate)
        {
            DateTime dateTime = PostSchedular.Value;
            var dateTimeOffset = new DateTimeOffset(dateTime);
            var unixDateTime = dateTimeOffset.ToUnixTimeSeconds();

            return unixDateTime.ToString();
        }
        public bool ScheduledTimeIsValid(DateTime time)
        {
            return ((time >= DateTime.Now.AddMinutes(10)) && (time <= DateTime.Now.AddMonths(6)));
        }
        
        public void UpdateSentNotafier(string message, Color? color = null)
        {
            SentLabel.ForeColor = color ?? Color.Green;
            SentLabel.Text = message;
        }
        public bool VerifyPostingRequirements()
        {
            if (!FBCheckbox.Checked && !Twitter_Checkbox.Checked && !Instagram_Checkbox.Checked)
            {
                UpdateSentNotafier("Please select a media to post to!", Color.Red);
                return false;
            }
            if (Schedule_Post_Checkbox.Checked && (!ScheduledTimeIsValid(PostSchedular.Value)))
            {
                UpdateSentNotafier("Facebook: Scheduled time must be at least\n10 minutes from now, and no more than\n6 months in the future", Color.Red);
                return false;
            }
            if (Instagram_Checkbox.Checked && (AttachmentFilePath == null))
            {
                UpdateSentNotafier("Instagram: Post must contain at least one picture!", Color.Red);
                return false;
            }

            return true;
        }
        #endregion

        #region ** Event Handlers **
        private void TwitterCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            AdjustCharacterLimit(((Twitter_Checkbox.Checked) ? 140 : ((Instagram_Checkbox.Checked) ? 2200 : 63203)) - PostEntry.Text.Length);
        }
        private void InstagramCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!Twitter_Checkbox.Checked)
            {
                AdjustCharacterLimit(((Instagram_Checkbox.Checked) ? 2200 : 63203) - PostEntry.Text.Length);
            }
        }
        private void SchedulePostCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            PostSchedular.Enabled = Schedule_Post_Checkbox.Checked;
        }

        private void MakeAPost_Load(object sender, EventArgs e)
        {
            SentLabel.Text = "";
            CharacterLimitLabel.Text = $"Characters Remaining: {CharacterLimit}";
            PostSchedular.Enabled = false;
            PostButton.Enabled = false;
            AttachmentFilePath = null;

            if (Properties.Settings.Default.DarkTheme)
            {
                DarkTheme();
            }

            if (FacebookAPIInfo.FBAccount != null)
            {
                AccountStatusLabel.Text = string.Format("Facebook: {0}\nInstagram: {1}\n", FacebookAPIInfo.FBAccount.name, "Not Connected");

                for (int i = 0; i < FacebookAPIInfo.FBPages.Count; ++i)
                {
                    FBPagesCombo.Items.Add(FacebookAPIInfo.FBPages[i].name);
                }

                if (FBPagesCombo.Items.Count > 0)
                {
                    FBPagesCombo.SelectedIndex = 0;
                    FBCheckbox.Checked = false;
                }
            }
        }

        private void Message_Textbox_Text_Changed(object sender, EventArgs e)
        {
            UpdateSentNotafier("");
            CharacterLimitLabel.Text = $"Characters Remaining: {CharacterLimit - PostEntry.Text.Length}";

            PostButton.Enabled = ((!(PostEntry.Text.Length > CharacterLimit)) && (PostEntry.Text.Length != 0));
        }

        private void PagesDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            FBCheckbox.Checked = true;
        }      

        private void PostButton_Click(object sender, EventArgs e)
        {
            if (!VerifyPostingRequirements())
            {
                return;
            }

            UpdateSentNotafier("Sending post...");

            if (FBCheckbox.Checked)
            {
                FacebookService fbService = new FacebookService(new FacebookClient());
                Task PostToPageTask;
                FacebookPage page = FacebookAPIInfo.FBPages[FBPagesCombo.SelectedIndex];

                if (Schedule_Post_Checkbox.Checked)
                {
                    if (ScheduledTimeIsValid(PostSchedular.Value)) //IN THE FUTURE THIS WILL BE DONE LAST, AS YOU NEED TO CHECK TIMES FOR ALL MEDIAS FIRST
                    {
                        PostToPageTask = fbService.CreateFacebookObject(
                            page.access_token, 
                            page.id,
                            ((AttachmentFilePath != null) ? "photos" : "feed"), 
                            PostEntry.Text, 
                            AttachmentFilePath,
                            ParseScheduledTimeToUnix(PostSchedular.Value));

                        //PostToPageTask = fbService.PostToPageAsync(FacebookAPIInfo.Pages[Pages_Drop_Down.SelectedIndex], richTextBox1.Text, attachedFilePath,
                        //    ParseScheduledTimeToUnix(Post_Schedular.Value));                       
                    }
                    else
                    {
                        SentLabel.ForeColor = Color.Red;
                        SentLabel.Text = "Facebook: Scheduled time must be at least\n10 minutes from now, and no more than\n6 months in the future";
                        return;
                    }
                }
                else if (Auto_Pick_Time.Checked)
                {
                    PostToPageTask = fbService.CreateFacebookObject(
                           page.access_token,
                           page.id,
                           ((AttachmentFilePath != null) ? "photos" : "feed"),
                           PostEntry.Text,
                           AttachmentFilePath,
                           "some time"
                           );                          
                }
                else
                {
                    PostToPageTask = fbService.CreateFacebookObject(
                           page.access_token,
                           page.id,
                           ((AttachmentFilePath != null) ? "photos" : "feed"),
                           PostEntry.Text,
                           AttachmentFilePath);

                    //PostToPageTask = fbService.PostToPageAsync(FacebookAPIInfo.Pages[Pages_Drop_Down.SelectedIndex], richTextBox1.Text, attachedFilePath);
                }

                Task.WaitAll(PostToPageTask);
            }

            /*if (Instagram_Checkbox.Checked)
            {
                var page = FacebookAPIInfo.FBPages[FBPagesCombo.SelectedIndex];

                var PostToPageTask = FacebookAPIInfo.facebookService.CreateFacebookObject
                (
                        page.access_token,
                        page.instagram_business_account.id,
                        "media",
                        PostEntry.Text,
                        AttachmentFilePath,
                        null,
                        $"caption={PostEntry.Text}"
                );
            }*/

            PostEntry.Text = "";
            UpdateSentNotafier("Post sent!");
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddPictureButton_Click(object sender, EventArgs e)
        {
            AttachmentFilePath = null;

            OpenFileDialog fileExplorer = new OpenFileDialog();
            fileExplorer.Filter = "Image Files |*.jpg;*.gif;*.png;";
            fileExplorer.FilterIndex = 1;

            if (fileExplorer.ShowDialog() == DialogResult.OK)
            {
                AttachmentFilePath = fileExplorer.FileName;
                FileNameLabel.Text = fileExplorer.SafeFileName;
            }
        }

        private void BoldButton_Toggle_Click(object sender, EventArgs e)
        {
            if (PostEntry.SelectionFont.Bold)
            {
                PostEntry.SelectionFont = new Font(PostEntry.Font, FontStyle.Regular);
            }
            else
            {
                PostEntry.SelectionFont = new Font(PostEntry.Font, FontStyle.Bold);
            }
        }
        private void ItalicsButton_Click(object sender, EventArgs e)
        {
            if (PostEntry.SelectionFont.Italic)
            {
                PostEntry.SelectionFont = new Font(PostEntry.Font, FontStyle.Regular);
            }
            else
            {
                PostEntry.SelectionFont = new Font(PostEntry.Font, FontStyle.Italic);
            }
        }
        private void UnderlineButton_Click(object sender, EventArgs e)
        {
            if (PostEntry.SelectionFont.Underline)
            {
                PostEntry.SelectionFont = new Font(PostEntry.Font, FontStyle.Regular);
            }
            else
            {
                PostEntry.SelectionFont = new Font(PostEntry.Font, FontStyle.Underline);
            }
        }
        #endregion
    }
}
