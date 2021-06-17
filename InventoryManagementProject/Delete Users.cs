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
    public partial class DeleteUsers : Form
    {
        public DeleteUsers()
        {
            InitializeComponent();
        }

        private void Users_Load(object sender, EventArgs e)
        {
            var dt = DBConnection.GetTableByQuery("SELECT * FROM Users;");
            cmbUsers.DisplayMember = "username";
            cmbUsers.ValueMember = "accountID";
            cmbUsers.DataSource = dt;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this user?", 
                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                DBConnection.ExecutingNonQuery("DELETE FROM Users WHERE accountID= " 
                    + cmbUsers.SelectedValue + ";");
                MessageBox.Show("User sucessfully deleted.");
            }
            else
            {
                MessageBox.Show("OK!");
            }
        }
    }
}
