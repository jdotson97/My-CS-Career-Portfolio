namespace EZ_Social_Media_Manager_pre_alpha
{
    partial class MessagePopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessagePopup));
            this.Accept_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.Message_Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Accept_Button
            // 
            this.Accept_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Accept_Button.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Accept_Button.Location = new System.Drawing.Point(146, 52);
            this.Accept_Button.Name = "Accept_Button";
            this.Accept_Button.Size = new System.Drawing.Size(116, 30);
            this.Accept_Button.TabIndex = 10;
            this.Accept_Button.Text = "Delete";
            this.Accept_Button.UseVisualStyleBackColor = true;
            this.Accept_Button.Click += new System.EventHandler(this.AcceptButton_Click);
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Button.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cancel_Button.Location = new System.Drawing.Point(12, 52);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(126, 30);
            this.Cancel_Button.TabIndex = 11;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // Message_Label
            // 
            this.Message_Label.AutoSize = true;
            this.Message_Label.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Message_Label.Location = new System.Drawing.Point(12, 9);
            this.Message_Label.Name = "Message_Label";
            this.Message_Label.Size = new System.Drawing.Size(251, 32);
            this.Message_Label.TabIndex = 12;
            this.Message_Label.Text = "Are you sure you want to permanently \r\ndelete this post?\r\n";
            this.Message_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MessagePopup
            // 
            this.AcceptButton = this.Cancel_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_Button;
            this.ClientSize = new System.Drawing.Size(274, 89);
            this.Controls.Add(this.Message_Label);
            this.Controls.Add(this.Cancel_Button);
            this.Controls.Add(this.Accept_Button);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MessagePopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Are You Sure?";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Accept_Button;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.Label Message_Label;
    }
}