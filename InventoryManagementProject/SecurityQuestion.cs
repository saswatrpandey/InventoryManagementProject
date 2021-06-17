using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryManagementProject
{
    public partial class SecurityQuestion : Form
    {
        public SecurityQuestion()
        {
            InitializeComponent();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (txtSecurityQuestion.Text== Login.securityAnswer)
            {
                MessageBox.Show("Successful.");
                MainMenu mmObj = new MainMenu();
                mmObj.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show( "Login Unsuccesful", "Incorrect answers.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

        }

        private void txtSecurityQuestion_KeyPress(object sender, KeyPressEventArgs e)
        {
            string textBoxSender = ((TextBox)sender).Name;
            if (e.KeyChar == 13)
            {
                switch (textBoxSender)
                {
                    case "txtSecurityQuestion":
                        btnNext.PerformClick();
                        break;
                }
            }
        }

        private void SecurityQuestion_Load(object sender, EventArgs e)
        {
            lblSecurityQuestion.Text = Login.securityQuestion;
        }
    }
}
