namespace EZ_Social_Media_Manager_pre_alpha
{
    partial class IGFollowerSummary
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IGFollowerSummary));
            this.Unfollowers = new System.Windows.Forms.ListBox();
            this.Followers = new System.Windows.Forms.ListBox();
            this.UnfollowAll = new System.Windows.Forms.Button();
            this.FollowAll = new System.Windows.Forms.Button();
            this.UnfollowersLabel = new System.Windows.Forms.Label();
            this.FollowersLabel = new System.Windows.Forms.Label();
            this.InstructionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Unfollowers
            // 
            this.Unfollowers.FormattingEnabled = true;
            this.Unfollowers.Location = new System.Drawing.Point(12, 60);
            this.Unfollowers.Name = "Unfollowers";
            this.Unfollowers.Size = new System.Drawing.Size(177, 147);
            this.Unfollowers.TabIndex = 0;
            // 
            // Followers
            // 
            this.Followers.FormattingEnabled = true;
            this.Followers.Location = new System.Drawing.Point(195, 60);
            this.Followers.Name = "Followers";
            this.Followers.Size = new System.Drawing.Size(176, 147);
            this.Followers.TabIndex = 1;
            this.Followers.SelectedIndexChanged += new System.EventHandler(this.Followers_SelectedIndexChanged);
            // 
            // UnfollowAll
            // 
            this.UnfollowAll.Location = new System.Drawing.Point(13, 213);
            this.UnfollowAll.Name = "UnfollowAll";
            this.UnfollowAll.Size = new System.Drawing.Size(176, 33);
            this.UnfollowAll.TabIndex = 2;
            this.UnfollowAll.Text = "UNFOLLOW ALL";
            this.UnfollowAll.UseVisualStyleBackColor = true;
            // 
            // FollowAll
            // 
            this.FollowAll.Location = new System.Drawing.Point(195, 213);
            this.FollowAll.Name = "FollowAll";
            this.FollowAll.Size = new System.Drawing.Size(176, 33);
            this.FollowAll.TabIndex = 3;
            this.FollowAll.Text = "FOLLOW ALL";
            this.FollowAll.UseVisualStyleBackColor = true;
            // 
            // UnfollowersLabel
            // 
            this.UnfollowersLabel.AutoSize = true;
            this.UnfollowersLabel.Location = new System.Drawing.Point(60, 44);
            this.UnfollowersLabel.Name = "UnfollowersLabel";
            this.UnfollowersLabel.Size = new System.Drawing.Size(78, 13);
            this.UnfollowersLabel.TabIndex = 4;
            this.UnfollowersLabel.Text = "New Unfollows";
            // 
            // FollowersLabel
            // 
            this.FollowersLabel.AutoSize = true;
            this.FollowersLabel.Location = new System.Drawing.Point(245, 44);
            this.FollowersLabel.Name = "FollowersLabel";
            this.FollowersLabel.Size = new System.Drawing.Size(76, 13);
            this.FollowersLabel.TabIndex = 5;
            this.FollowersLabel.Text = "New Followers";
            // 
            // InstructionLabel
            // 
            this.InstructionLabel.AutoSize = true;
            this.InstructionLabel.Location = new System.Drawing.Point(97, 9);
            this.InstructionLabel.Name = "InstructionLabel";
            this.InstructionLabel.Size = new System.Drawing.Size(197, 13);
            this.InstructionLabel.TabIndex = 6;
            this.InstructionLabel.Text = "Left click to follow, right click to unfollow";
            // 
            // IGFollowerSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 258);
            this.Controls.Add(this.InstructionLabel);
            this.Controls.Add(this.FollowersLabel);
            this.Controls.Add(this.UnfollowersLabel);
            this.Controls.Add(this.FollowAll);
            this.Controls.Add(this.UnfollowAll);
            this.Controls.Add(this.Followers);
            this.Controls.Add(this.Unfollowers);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "IGFollowerSummary";
            this.Text = "{Account} Follower Summary";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox Unfollowers;
        private System.Windows.Forms.ListBox Followers;
        private System.Windows.Forms.Button UnfollowAll;
        private System.Windows.Forms.Button FollowAll;
        private System.Windows.Forms.Label UnfollowersLabel;
        private System.Windows.Forms.Label FollowersLabel;
        private System.Windows.Forms.Label InstructionLabel;
    }
}