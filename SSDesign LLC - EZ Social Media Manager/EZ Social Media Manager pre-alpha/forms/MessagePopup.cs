using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EZ_Social_Media_Manager_pre_alpha
{
    public partial class MessagePopup : Form
    {
        public MessagePopup()
        {
            InitializeComponent();
        }
        public MessagePopup(string message)
        {
            //Message_Label.Text = message;
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void AcceptButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

    /*public class MessagePopup<T> : MessagePopup
    {
        T Action;

        public MessagePopup<T>() : base()
        {
            
        }
    };*/
}
