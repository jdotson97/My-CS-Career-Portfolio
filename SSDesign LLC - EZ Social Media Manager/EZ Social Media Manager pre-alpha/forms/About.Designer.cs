namespace EZ_Social_Media_Manager_pre_alpha
{
    partial class About
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Company = new System.Windows.Forms.Label();
            this.Copyright = new System.Windows.Forms.Label();
            this.Product_Version = new System.Windows.Forms.Label();
            this.Product_Name = new System.Windows.Forms.Label();
            this.Policy_Text_Box = new System.Windows.Forms.TextBox();
            this.Dismiss_Button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(207, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 106);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.Company, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.Copyright, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.Product_Version, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Product_Name, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(188, 106);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // Company
            // 
            this.Company.AutoSize = true;
            this.Company.Location = new System.Drawing.Point(4, 79);
            this.Company.Name = "Company";
            this.Company.Size = new System.Drawing.Size(153, 13);
            this.Company.TabIndex = 3;
            this.Company.Text = "Simplicity Software Design LLC";
            // 
            // Copyright
            // 
            this.Copyright.AutoSize = true;
            this.Copyright.Location = new System.Drawing.Point(4, 53);
            this.Copyright.Name = "Copyright";
            this.Copyright.Size = new System.Drawing.Size(90, 13);
            this.Copyright.TabIndex = 2;
            this.Copyright.Text = "Copyright © 2019";
            // 
            // Product_Version
            // 
            this.Product_Version.AutoSize = true;
            this.Product_Version.Location = new System.Drawing.Point(4, 27);
            this.Product_Version.Name = "Product_Version";
            this.Product_Version.Size = new System.Drawing.Size(93, 13);
            this.Product_Version.TabIndex = 1;
            this.Product_Version.Text = "Version: 1.0.0.326";
            // 
            // Product_Name
            // 
            this.Product_Name.AutoSize = true;
            this.Product_Name.Location = new System.Drawing.Point(4, 1);
            this.Product_Name.Name = "Product_Name";
            this.Product_Name.Size = new System.Drawing.Size(130, 13);
            this.Product_Name.TabIndex = 0;
            this.Product_Name.Text = "EZ-Social Media Manager";
            // 
            // Policy_Text_Box
            // 
            this.Policy_Text_Box.BackColor = System.Drawing.Color.LightGray;
            this.Policy_Text_Box.Location = new System.Drawing.Point(13, 124);
            this.Policy_Text_Box.Multiline = true;
            this.Policy_Text_Box.Name = "Policy_Text_Box";
            this.Policy_Text_Box.ReadOnly = true;
            this.Policy_Text_Box.Size = new System.Drawing.Size(394, 120);
            this.Policy_Text_Box.TabIndex = 3;
            this.Policy_Text_Box.Text = resources.GetString("Policy_Text_Box.Text");
            // 
            // Dismiss_Button
            // 
            this.Dismiss_Button.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Dismiss_Button.Location = new System.Drawing.Point(259, 250);
            this.Dismiss_Button.Name = "Dismiss_Button";
            this.Dismiss_Button.Size = new System.Drawing.Size(148, 43);
            this.Dismiss_Button.TabIndex = 4;
            this.Dismiss_Button.Text = "Dismiss";
            this.Dismiss_Button.UseVisualStyleBackColor = true;
            this.Dismiss_Button.Click += new System.EventHandler(this.DismissButton_Click);
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(419, 296);
            this.Controls.Add(this.Dismiss_Button);
            this.Controls.Add(this.Policy_Text_Box);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About - EZ Social Media Manager";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label Company;
        private System.Windows.Forms.Label Copyright;
        private System.Windows.Forms.Label Product_Version;
        private System.Windows.Forms.Label Product_Name;
        private System.Windows.Forms.TextBox Policy_Text_Box;
        private System.Windows.Forms.Button Dismiss_Button;
    }
}
