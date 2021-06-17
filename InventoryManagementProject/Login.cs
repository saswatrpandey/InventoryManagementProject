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
    public partial class Login : Form
    {
        public static string securityQuestion;
        public static string securityAnswer;
        public static bool isAdmin;
        int count = 1;
        public Login()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM Users WHERE username= '" + 
                txtUsername.Text + "' and password= HASHBYTES('SHA2_512','" + txtPassword.Text +"');";
            var data = DBConnection.GetTableByQuery(query);
            bool validUser = false;
            if (data.Rows.Count > 0)
            {
               validUser = true;
            }
            else
            {
                count++;
                MessageBox.Show("Invalid Username or Password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();           
            }
            if (validUser == true)
            {
                securityAnswer = data.Rows[0]["securityAnswer"].ToString();
                securityQuestion = data.Rows[0]["securityQuestion"].ToString();
                isAdmin = bool.Parse(data.Rows[0]["isAdmin"].ToString());
                if (count > 3 && count <= 5)
                {
                    this.Hide();
                    SecurityQuestion sqObj = new SecurityQuestion();
                    sqObj.Show();
                }
                else
                {
                    this.Hide();
                    MainMenu mmObj = new MainMenu();
                    mmObj.Show();                  
                }
            }
            if(count > 5)
            {
                string query2= "DELETE FROM Users WHERE username='" + txtUsername.Text + "';";
                DBConnection.ExecutingNonQuery(query2);
                MessageBox.Show("Too mant attemts!!! \n" +
                    "Your account has been permanently disabled." +
                    "\nContact Admin to create a new account." +
                    "\nProgram Shutting Down.", "LOGIN ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            string textBoxSender = ((TextBox)sender).Name;
            
            if(e.KeyChar == 13)
            {
                switch(textBoxSender)
                {
                    case "txtUsername":
                        txtPassword.Focus();
                        break;
                    case "txtPassword":
                        btnLogin.PerformClick();
                        break;
                }
            }
        }
    }
}
