using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryManagementProject
{
    public partial class Registration : Form
    {
        public string selectedItem;
        public Registration()
        {
            InitializeComponent();
        }
        private void btnRegistration_Click(object sender, EventArgs e)
        {
            var con = DBConnection.DBConnect();
            if (txtPassword.Text=="" || txtPassword.Text=="" 
                || txtConfirmPassword.Text== "" || txtSecurityAnswer.Text =="" 
                || chkAcceptTerms.Checked == false)
            {
                MessageBox.Show("All fields need to be filled and Terms to use must be accepted.",
                    "Empty Field", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                if (ValidatePassword(txtPassword.Text) == true)
                {
                    if (txtPassword.Text == txtConfirmPassword.Text)
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "INSERT INTO Users VALUES(@username, " +
                                "HASHBYTES('SHA2_512', '"+ txtPassword.Text + "'), " +
                                "@email, @phone, @securityQuestion, '"+ txtSecurityAnswer.Text + "', @isAdmin)";
                            cmd.Parameters.AddWithValue("username", txtUsername.Text);
                            cmd.Parameters.AddWithValue("email", txtEmail.Text + "@im.com");
                            cmd.Parameters.AddWithValue("phone", txtPhone.Text);
                            cmd.Parameters.AddWithValue("securityQuestion", cmbSecurityQuestion.Text);
                            cmd.Parameters.AddWithValue("isAdmin", chkAdminUser.Checked);
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show(txtUsername.Text + " is now a user.", 
                                    "Successful", 
                                    MessageBoxButtons.OK, MessageBoxIcon.None);
                                Clear();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("There was an exception. " + ex);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Passwords Did Not Match", 
                            "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("The password must be of 8 characters. " +
                        "\nIt should also consist of a symbol, uppercase letter and a number", 
                        "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        private void Clear()
        {
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtSecurityAnswer.Clear();
            txtUsername.Clear();
            chkAdminUser.Checked = false;
            chkAcceptTerms.Checked = false;
        }
        public static bool ValidatePassword(string password)
        {
            int hasSymbol = 0;
            int hasUpperCase = 0;
            int hasDigit = 0;
            int count = 1;
            bool validPassword = false;
            foreach (char c in password)
            {
                count++;
                if (char.IsLetterOrDigit(c) != true)
                {
                    hasSymbol += 1;
                }
                if (char.IsUpper(c) == true)
                {
                    hasUpperCase += 1;
                }

                if (char.IsDigit(c) == true)
                {
                    hasDigit += 1;
                }
            }
            if (hasDigit >= 1 && hasSymbol >= 1 && hasUpperCase >= 1 && password.Length >= 8)
            {
                validPassword = true;
            }
            else
            {
                validPassword = false;
            }
            return validPassword;
        }
        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            string textBoxSender = ((TextBox)sender).Name;
            if (e.KeyChar == 13)
            {
                switch (textBoxSender)
                {
                    case "txtUsername":
                        txtPassword.Focus();
                        break;
                    case "txtPassword":
                        txtConfirmPassword.Focus();
                        break;
                    case "txtConfirmPassword":
                        txtEmail.Focus();
                        break;
                    case "txtEmail":
                        txtPhone.Focus();
                        break;
                    case "txtPhone":
                        btnRegistration.PerformClick();
                        break;
                }
            }
            if (textBoxSender.Equals("txtPhone"))
            {
                if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == 8 || e.KeyChar == 13)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }
        private void btnClearForm_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Registration_Load(object sender, EventArgs e)
        {
            cmbSecurityQuestion.Items.Add("Where was your mother born?");
            cmbSecurityQuestion.Items.Add("What was your first pet's name?");
            cmbSecurityQuestion.Items.Add("What is your favorite food?");
            cmbSecurityQuestion.Items.Add("What is your favorite television show?");

        }
    }
}

    
    

