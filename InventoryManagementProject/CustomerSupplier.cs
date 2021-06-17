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
    public partial class CustomerSupplier : Form
    {
        private string value = MainMenu.CustomerOrSupplier;
        private int supplierID = 0;
        public CustomerSupplier()
        {
            InitializeComponent();
        }
        private void CustomerSupplier_Load(object sender, EventArgs e)
        {
            LoadButtonSettings();
            this.Text = value;
            this.Refresh();

            if (value == "Customer")
            {
                lblName.Text = "Customer Name";
            }
            else if (value == "Supplier")
            {
                lblName.Text = "Supplier Name";
            }
            GetTableFromDvg();
        }
        private void LoadButtonSettings()
        {
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }
        private bool AreFieldsEmpty()
        {
            if (txtName.Text == String.Empty || txtEmail.Text == String.Empty
              || txtPhoneNumber.Text == String.Empty || txtAddress.Text == String.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (AreFieldsEmpty()==true)
            {
                MessageBox.Show("Fields cannot be empty.", "Empty Fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (value == "Customer")
                {
                    DBConnection.ExecutingNonQuery("INSERT INTO CustSupplier VALUES " +
                    "('" + txtName.Text + "', 'Customer', '" + txtEmail.Text +
                    "', '" + txtPhoneNumber.Text + "', '" + txtMobile.Text +
                    "', '" + txtAddress.Text + "');");
                    MessageBox.Show("Field is saved.", "Successful");
                }
                else if (value == "Supplier")
                {
                    DBConnection.ExecutingNonQuery("INSERT INTO CustSupplier VALUES " +
                    "('" + txtName.Text + "', 'Supplier', '" + txtEmail.Text +
                    "', '" + txtPhoneNumber.Text + "', '" + txtMobile.Text +
                    "', '" + txtAddress.Text + "');");
                    MessageBox.Show("Field is saved.", "Successful");
                }
            }
            GetTableFromDvg();
            Clear();
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (AreFieldsEmpty() == true)
            {
                MessageBox.Show("Fields cannot be empty.", "Empty Fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var result = MessageBox.Show("Are you sure you want to delete this field?",
                        "Warning",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    DBConnection.ExecutingNonQuery("UPDATE CustSupplier SET " +
                       "Name = '" + txtName.Text + "', " +
                       "email = '" + txtEmail.Text + "', " +
                       "phone = '" + txtPhoneNumber.Text + "', " +
                       "mobile = '" + txtMobile.Text + "', " +
                       "address = '" + txtAddress.Text + "' " +
                       "WHERE custSupplierID =" + supplierID);
                    MessageBox.Show("Field is updated.", "Successful");
                }
                else
                {
                    MessageBox.Show("OK!");
                }
                GetTableFromDvg();
                Clear();
                LoadButtonSettings();
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to edit this field?",
                "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                DBConnection.ExecutingNonQuery("DELETE FROM CustSupplier WHERE custSupplierId= "
                    + supplierID);
                GetTableFromDvg();
            }
            else
            {
                MessageBox.Show("OK!");
            }
            Clear();
            LoadButtonSettings();
        }
        public void GetTableFromDvg()
        {

                var dt = DBConnection.GetTableByQuery("SELECT custSupplierId As "+ value + "ID, " +
                    "Name, email, phone, mobile, address  FROM CustSupplier WHERE type='" + value + "'");
                dvgCustSupplier.DataSource = dt;
        }
        private void Clear()
        {
            txtName.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            txtMobile.Clear();
            txtPhoneNumber.Clear();
            txtSearch.Clear();
            txtName.Focus();
        }
        private void dvgCustSupplier_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnSave.Enabled = false;
            btnDelete.Enabled = true;
            btnUpdate.Enabled = true;
            supplierID = int.Parse(dvgCustSupplier.Rows[e.RowIndex].Cells[0].Value.ToString());
            txtName.Text = dvgCustSupplier.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtEmail.Text = dvgCustSupplier.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtPhoneNumber.Text = dvgCustSupplier.Rows[e.RowIndex].Cells[3].Value.ToString();
            txtMobile.Text = dvgCustSupplier.Rows[e.RowIndex].Cells[4].Value.ToString();
            txtAddress.Text = dvgCustSupplier.Rows[e.RowIndex].Cells[5].Value.ToString();
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var dt = DBConnection.GetTableByQuery("SELECT custSupplierId As " + value + "ID, " +
                    "Name, email, phone, mobile, address  FROM CustSupplier WHERE type='" + value + "' AND " +
           "(Name LIKE '" + txtSearch.Text + "%' OR " +
           "email LIKE '" + txtSearch.Text + "%' OR " +
           "phone LIKE '" + txtSearch.Text + "%' OR " +
           "mobile LIKE '" + txtSearch.Text + "%' OR " +
           "address LIKE '" + txtSearch.Text + "%');");
            dvgCustSupplier.DataSource = dt;
        }
        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            string textBoxSender = ((TextBox)sender).Name;

            if (textBoxSender.Equals("txtPhoneNumber") || textBoxSender.Equals("txtMobile"))
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
            if (e.KeyChar == 13)
            {
                switch (textBoxSender)
                {
                    case "txtName":
                        txtEmail.Focus();
                        break;
                    case "txtEmail":
                        txtPhoneNumber.Focus();
                        break;
                    case "txtPhoneNumber":
                        txtMobile.Focus();
                        break;
                    case "txtMobile":
                        txtAddress.Focus();
                        break;
                    case "txtAddress":
                        if (btnSave.Enabled == true)
                        {
                            btnSave.PerformClick();
                        }
                        else
                        {
                            btnUpdate.Focus();
                        }
                        break;
                }
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            LoadButtonSettings();
        }
    }
}
