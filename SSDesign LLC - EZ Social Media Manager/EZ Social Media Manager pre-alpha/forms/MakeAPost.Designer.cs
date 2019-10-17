namespace EZ_Social_Media_Manager_pre_alpha
{
    partial class MakeAPost
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MakeAPost));
            this.PostEntry = new System.Windows.Forms.RichTextBox();
            this.Post_Box_Label = new System.Windows.Forms.Label();
            this.Post_Panel = new System.Windows.Forms.Panel();
            this.CharacterLimitLabel = new System.Windows.Forms.Label();
            this.Make_Text_Underlined_Toggle = new System.Windows.Forms.Button();
            this.Make_Text_Itallics_Toggle = new System.Windows.Forms.Button();
            this.Make_Text_Bold_Toggle = new System.Windows.Forms.Button();
            this.FileNameLabel = new System.Windows.Forms.Label();
            this.Add_Picture_Button = new System.Windows.Forms.Button();
            this.Checkbox_Panel = new System.Windows.Forms.Panel();
            this.FBPagesCombo = new System.Windows.Forms.ComboBox();
            this.Twitter_Checkbox = new System.Windows.Forms.CheckBox();
            this.Where_To_Post_Label = new System.Windows.Forms.Label();
            this.Instagram_Checkbox = new System.Windows.Forms.CheckBox();
            this.FBCheckbox = new System.Windows.Forms.CheckBox();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.AccountStatusLabel = new System.Windows.Forms.Label();
            this.SentLabel = new System.Windows.Forms.Label();
            this.PostButton = new System.Windows.Forms.Button();
            this.PostSchedular = new System.Windows.Forms.DateTimePicker();
            this.Schedule_Post_Checkbox = new System.Windows.Forms.CheckBox();
            this.Scheduling_Panel_Label = new System.Windows.Forms.Label();
            this.Scheduling_Panel = new System.Windows.Forms.Panel();
            this.Auto_Pick_Time = new System.Windows.Forms.CheckBox();
            this.Post_Panel.SuspendLayout();
            this.Checkbox_Panel.SuspendLayout();
            this.Scheduling_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // PostEntry
            // 
            this.PostEntry.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.PostEntry.Location = new System.Drawing.Point(3, 19);
            this.PostEntry.Name = "PostEntry";
            this.PostEntry.Size = new System.Drawing.Size(564, 158);
            this.PostEntry.TabIndex = 0;
            this.PostEntry.Text = "";
            this.PostEntry.TextChanged += new System.EventHandler(this.Message_Textbox_Text_Changed);
            // 
            // Post_Box_Label
            // 
            this.Post_Box_Label.AutoSize = true;
            this.Post_Box_Label.Location = new System.Drawing.Point(3, 0);
            this.Post_Box_Label.Name = "Post_Box_Label";
            this.Post_Box_Label.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.Post_Box_Label.Size = new System.Drawing.Size(147, 16);
            this.Post_Box_Label.TabIndex = 1;
            this.Post_Box_Label.Text = "What would you like to post?:\r\n";
            // 
            // Post_Panel
            // 
            this.Post_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Post_Panel.Controls.Add(this.CharacterLimitLabel);
            this.Post_Panel.Controls.Add(this.Make_Text_Underlined_Toggle);
            this.Post_Panel.Controls.Add(this.Make_Text_Itallics_Toggle);
            this.Post_Panel.Controls.Add(this.Make_Text_Bold_Toggle);
            this.Post_Panel.Controls.Add(this.FileNameLabel);
            this.Post_Panel.Controls.Add(this.Add_Picture_Button);
            this.Post_Panel.Controls.Add(this.Post_Box_Label);
            this.Post_Panel.Controls.Add(this.PostEntry);
            this.Post_Panel.Location = new System.Drawing.Point(12, 12);
            this.Post_Panel.Name = "Post_Panel";
            this.Post_Panel.Size = new System.Drawing.Size(570, 210);
            this.Post_Panel.TabIndex = 2;
            // 
            // CharacterLimitLabel
            // 
            this.CharacterLimitLabel.AutoSize = true;
            this.CharacterLimitLabel.Location = new System.Drawing.Point(410, 3);
            this.CharacterLimitLabel.Name = "CharacterLimitLabel";
            this.CharacterLimitLabel.Size = new System.Drawing.Size(114, 13);
            this.CharacterLimitLabel.TabIndex = 7;
            this.CharacterLimitLabel.Text = "Remaining Characters:";
            // 
            // Make_Text_Underlined_Toggle
            // 
            this.Make_Text_Underlined_Toggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Make_Text_Underlined_Toggle.Location = new System.Drawing.Point(490, 181);
            this.Make_Text_Underlined_Toggle.Name = "Make_Text_Underlined_Toggle";
            this.Make_Text_Underlined_Toggle.Size = new System.Drawing.Size(75, 23);
            this.Make_Text_Underlined_Toggle.TabIndex = 6;
            this.Make_Text_Underlined_Toggle.Text = "Underline";
            this.Make_Text_Underlined_Toggle.UseVisualStyleBackColor = true;
            this.Make_Text_Underlined_Toggle.Click += new System.EventHandler(this.UnderlineButton_Click);
            // 
            // Make_Text_Itallics_Toggle
            // 
            this.Make_Text_Itallics_Toggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Make_Text_Itallics_Toggle.Location = new System.Drawing.Point(409, 181);
            this.Make_Text_Itallics_Toggle.Name = "Make_Text_Itallics_Toggle";
            this.Make_Text_Itallics_Toggle.Size = new System.Drawing.Size(75, 23);
            this.Make_Text_Itallics_Toggle.TabIndex = 5;
            this.Make_Text_Itallics_Toggle.Text = "Italicize";
            this.Make_Text_Itallics_Toggle.UseVisualStyleBackColor = true;
            this.Make_Text_Itallics_Toggle.Click += new System.EventHandler(this.ItalicsButton_Click);
            // 
            // Make_Text_Bold_Toggle
            // 
            this.Make_Text_Bold_Toggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Make_Text_Bold_Toggle.Location = new System.Drawing.Point(328, 181);
            this.Make_Text_Bold_Toggle.Name = "Make_Text_Bold_Toggle";
            this.Make_Text_Bold_Toggle.Size = new System.Drawing.Size(75, 23);
            this.Make_Text_Bold_Toggle.TabIndex = 4;
            this.Make_Text_Bold_Toggle.Text = "Bold";
            this.Make_Text_Bold_Toggle.UseVisualStyleBackColor = true;
            this.Make_Text_Bold_Toggle.Click += new System.EventHandler(this.BoldButton_Toggle_Click);
            // 
            // FileNameLabel
            // 
            this.FileNameLabel.AutoSize = true;
            this.FileNameLabel.Location = new System.Drawing.Point(117, 186);
            this.FileNameLabel.Name = "FileNameLabel";
            this.FileNameLabel.Size = new System.Drawing.Size(79, 13);
            this.FileNameLabel.TabIndex = 3;
            this.FileNameLabel.Text = "No File Chosen";
            // 
            // Add_Picture_Button
            // 
            this.Add_Picture_Button.Location = new System.Drawing.Point(6, 181);
            this.Add_Picture_Button.Name = "Add_Picture_Button";
            this.Add_Picture_Button.Size = new System.Drawing.Size(105, 23);
            this.Add_Picture_Button.TabIndex = 2;
            this.Add_Picture_Button.Text = "Add Picture(s)";
            this.Add_Picture_Button.UseVisualStyleBackColor = true;
            this.Add_Picture_Button.Click += new System.EventHandler(this.AddPictureButton_Click);
            // 
            // Checkbox_Panel
            // 
            this.Checkbox_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Checkbox_Panel.Controls.Add(this.FBPagesCombo);
            this.Checkbox_Panel.Controls.Add(this.Twitter_Checkbox);
            this.Checkbox_Panel.Controls.Add(this.Where_To_Post_Label);
            this.Checkbox_Panel.Controls.Add(this.Instagram_Checkbox);
            this.Checkbox_Panel.Controls.Add(this.FBCheckbox);
            this.Checkbox_Panel.Location = new System.Drawing.Point(12, 228);
            this.Checkbox_Panel.Name = "Checkbox_Panel";
            this.Checkbox_Panel.Size = new System.Drawing.Size(570, 43);
            this.Checkbox_Panel.TabIndex = 3;
            // 
            // Pages_Drop_Down
            // 
            this.FBPagesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FBPagesCombo.FormattingEnabled = true;
            this.FBPagesCombo.Location = new System.Drawing.Point(104, 17);
            this.FBPagesCombo.Name = "Pages_Drop_Down";
            this.FBPagesCombo.Size = new System.Drawing.Size(121, 21);
            this.FBPagesCombo.TabIndex = 4;
            this.FBPagesCombo.SelectedIndexChanged += new System.EventHandler(this.PagesDropDown_SelectedIndexChanged);
            // 
            // Twitter_Checkbox
            // 
            this.Twitter_Checkbox.AutoSize = true;
            this.Twitter_Checkbox.Location = new System.Drawing.Point(309, 19);
            this.Twitter_Checkbox.Name = "Twitter_Checkbox";
            this.Twitter_Checkbox.Size = new System.Drawing.Size(58, 17);
            this.Twitter_Checkbox.TabIndex = 3;
            this.Twitter_Checkbox.Text = "Twitter";
            this.Twitter_Checkbox.UseVisualStyleBackColor = true;
            this.Twitter_Checkbox.CheckedChanged += new System.EventHandler(this.TwitterCheckbox_CheckedChanged);
            // 
            // Where_To_Post_Label
            // 
            this.Where_To_Post_Label.AutoSize = true;
            this.Where_To_Post_Label.Location = new System.Drawing.Point(3, 0);
            this.Where_To_Post_Label.Name = "Where_To_Post_Label";
            this.Where_To_Post_Label.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.Where_To_Post_Label.Size = new System.Drawing.Size(184, 16);
            this.Where_To_Post_Label.TabIndex = 0;
            this.Where_To_Post_Label.Text = "Where would you like to post this to?:\r\n";
            // 
            // Instagram_Checkbox
            // 
            this.Instagram_Checkbox.AutoSize = true;
            this.Instagram_Checkbox.Location = new System.Drawing.Point(231, 19);
            this.Instagram_Checkbox.Name = "Instagram_Checkbox";
            this.Instagram_Checkbox.Size = new System.Drawing.Size(72, 17);
            this.Instagram_Checkbox.TabIndex = 2;
            this.Instagram_Checkbox.Text = "Instagram";
            this.Instagram_Checkbox.UseVisualStyleBackColor = true;
            this.Instagram_Checkbox.CheckedChanged += new System.EventHandler(this.InstagramCheckbox_CheckedChanged);
            // 
            // Facebook_Checkbox
            // 
            this.FBCheckbox.AutoSize = true;
            this.FBCheckbox.Location = new System.Drawing.Point(3, 19);
            this.FBCheckbox.Name = "Facebook_Checkbox";
            this.FBCheckbox.Size = new System.Drawing.Size(105, 17);
            this.FBCheckbox.TabIndex = 1;
            this.FBCheckbox.Text = "Facebook Page:";
            this.FBCheckbox.UseVisualStyleBackColor = true;
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cancel_Button.Location = new System.Drawing.Point(264, 331);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(156, 58);
            this.Cancel_Button.TabIndex = 5;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // Current_Account_Notafier
            // 
            this.AccountStatusLabel.AutoSize = true;
            this.AccountStatusLabel.Location = new System.Drawing.Point(9, 363);
            this.AccountStatusLabel.Name = "Current_Account_Notafier";
            this.AccountStatusLabel.Size = new System.Drawing.Size(133, 26);
            this.AccountStatusLabel.TabIndex = 6;
            this.AccountStatusLabel.Text = "Facebook: Not Connected\r\nInstagram: Not Connected";
            // 
            // SentLabel
            // 
            this.SentLabel.AutoSize = true;
            this.SentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SentLabel.ForeColor = System.Drawing.Color.Green;
            this.SentLabel.Location = new System.Drawing.Point(12, 324);
            this.SentLabel.Name = "SentLabel";
            this.SentLabel.Size = new System.Drawing.Size(64, 13);
            this.SentLabel.TabIndex = 7;
            this.SentLabel.Text = "Notafication";
            // 
            // PostButton
            // 
            this.PostButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PostButton.Location = new System.Drawing.Point(426, 331);
            this.PostButton.Name = "PostButton";
            this.PostButton.Size = new System.Drawing.Size(156, 58);
            this.PostButton.TabIndex = 9;
            this.PostButton.Text = "POST!";
            this.PostButton.UseVisualStyleBackColor = true;
            this.PostButton.Click += new System.EventHandler(this.PostButton_Click);
            // 
            // PostSchedular
            // 
            this.PostSchedular.CalendarMonthBackground = System.Drawing.SystemColors.AppWorkspace;
            this.PostSchedular.CustomFormat = "h:mm tt MM/dd/yy";
            this.PostSchedular.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PostSchedular.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.PostSchedular.Location = new System.Drawing.Point(130, 19);
            this.PostSchedular.Name = "PostSchedular";
            this.PostSchedular.Size = new System.Drawing.Size(136, 20);
            this.PostSchedular.TabIndex = 11;
            // 
            // Schedule_Post_Checkbox
            // 
            this.Schedule_Post_Checkbox.AutoSize = true;
            this.Schedule_Post_Checkbox.Location = new System.Drawing.Point(3, 22);
            this.Schedule_Post_Checkbox.Name = "Schedule_Post_Checkbox";
            this.Schedule_Post_Checkbox.Size = new System.Drawing.Size(131, 17);
            this.Schedule_Post_Checkbox.TabIndex = 13;
            this.Schedule_Post_Checkbox.Text = "Schedule this post for:";
            this.Schedule_Post_Checkbox.UseVisualStyleBackColor = true;
            this.Schedule_Post_Checkbox.CheckedChanged += new System.EventHandler(this.SchedulePostCheckbox_CheckedChanged);
            // 
            // Scheduling_Panel_Label
            // 
            this.Scheduling_Panel_Label.AutoSize = true;
            this.Scheduling_Panel_Label.Location = new System.Drawing.Point(3, 0);
            this.Scheduling_Panel_Label.Name = "Scheduling_Panel_Label";
            this.Scheduling_Panel_Label.Size = new System.Drawing.Size(242, 13);
            this.Scheduling_Panel_Label.TabIndex = 14;
            this.Scheduling_Panel_Label.Text = "When would you like to post this? (Now is default)";
            // 
            // Scheduling_Panel
            // 
            this.Scheduling_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Scheduling_Panel.Controls.Add(this.Auto_Pick_Time);
            this.Scheduling_Panel.Controls.Add(this.Scheduling_Panel_Label);
            this.Scheduling_Panel.Controls.Add(this.PostSchedular);
            this.Scheduling_Panel.Controls.Add(this.Schedule_Post_Checkbox);
            this.Scheduling_Panel.Location = new System.Drawing.Point(12, 277);
            this.Scheduling_Panel.Name = "Scheduling_Panel";
            this.Scheduling_Panel.Size = new System.Drawing.Size(570, 44);
            this.Scheduling_Panel.TabIndex = 15;
            // 
            // Auto_Pick_Time
            // 
            this.Auto_Pick_Time.AutoSize = true;
            this.Auto_Pick_Time.Enabled = false;
            this.Auto_Pick_Time.Location = new System.Drawing.Point(309, 22);
            this.Auto_Pick_Time.Name = "Auto_Pick_Time";
            this.Auto_Pick_Time.Size = new System.Drawing.Size(256, 17);
            this.Auto_Pick_Time.TabIndex = 15;
            this.Auto_Pick_Time.Text = "Let EZ-Social Media Manager pick the best time!\r\n";
            this.Auto_Pick_Time.UseVisualStyleBackColor = true;
            // 
            // MakeAPost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(594, 401);
            this.Controls.Add(this.PostButton);
            this.Controls.Add(this.SentLabel);
            this.Controls.Add(this.AccountStatusLabel);
            this.Controls.Add(this.Cancel_Button);
            this.Controls.Add(this.Checkbox_Panel);
            this.Controls.Add(this.Post_Panel);
            this.Controls.Add(this.Scheduling_Panel);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MakeAPost";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Let\'s Post!";
            this.Load += new System.EventHandler(this.MakeAPost_Load);
            this.Post_Panel.ResumeLayout(false);
            this.Post_Panel.PerformLayout();
            this.Checkbox_Panel.ResumeLayout(false);
            this.Checkbox_Panel.PerformLayout();
            this.Scheduling_Panel.ResumeLayout(false);
            this.Scheduling_Panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox PostEntry;
        private System.Windows.Forms.Label Post_Box_Label;
        private System.Windows.Forms.Panel Post_Panel;
        private System.Windows.Forms.Label FileNameLabel;
        private System.Windows.Forms.Button Add_Picture_Button;
        private System.Windows.Forms.Panel Checkbox_Panel;
        private System.Windows.Forms.Label Where_To_Post_Label;
        private System.Windows.Forms.CheckBox Instagram_Checkbox;
        private System.Windows.Forms.CheckBox FBCheckbox;
        private System.Windows.Forms.CheckBox Twitter_Checkbox;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.Label AccountStatusLabel;
        private System.Windows.Forms.Button Make_Text_Underlined_Toggle;
        private System.Windows.Forms.Button Make_Text_Itallics_Toggle;
        private System.Windows.Forms.Button Make_Text_Bold_Toggle;
        private System.Windows.Forms.ComboBox FBPagesCombo;
        private System.Windows.Forms.Label SentLabel;
        private System.Windows.Forms.Button PostButton;
        private System.Windows.Forms.DateTimePicker PostSchedular;
        private System.Windows.Forms.Label CharacterLimitLabel;
        private System.Windows.Forms.CheckBox Schedule_Post_Checkbox;
        private System.Windows.Forms.Label Scheduling_Panel_Label;
        private System.Windows.Forms.Panel Scheduling_Panel;
        private System.Windows.Forms.CheckBox Auto_Pick_Time;
    }
}