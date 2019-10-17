using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SSDesign.MediaAPI;

namespace EZ_Social_Media_Manager_pre_alpha
{
    public partial class IGFollowerSummary : Form
    {
        FacebookPage _currentPage { get; set; }

        public IGFollowerSummary()
        {
            InitializeComponent();

            if (Properties.Settings.Default.DarkTheme)
            {
                ToggleDarkTheme();
            }
        }
        public IGFollowerSummary(FacebookPage page) : this()
        {
            _currentPage = page;
            Text = $"{_currentPage.instagram_business_account.name} Follower Summary";
        }

        private void Followers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ToggleDarkTheme()
        {
            BackColor = Color.DimGray;
            UnfollowersLabel.ForeColor = Color.LightGray;
            FollowersLabel.ForeColor = Color.LightGray;
            InstructionLabel.ForeColor = Color.LightGray;
        }
    }
}
