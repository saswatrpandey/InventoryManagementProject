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
    public partial class Category : Form
    {
        private int supplierID = 0;
        public Category()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtCategoryName.Text==String.Empty)
            {
                MessageBox.Show("Fields cannot be empty.", "Empty Fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DBConnection.ExecutingNonQuery("INSERT INTO Category VALUES" +
                "('" + txtCategoryName.Text + "', '" + txtRemarks.Text + "');");
                MessageBox.Show("Field is saved.", "Successful");
                GetTableFromDvg();
            }
            Clear();
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtCategoryName.Text != String.Empty)
            {
                var result= MessageBox.Show("Are you sure you want to update this field?",
                "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    DBConnection.ExecutingNonQuery("UPDATE Category SET " +
                        "categoryName = '" + txtCategoryName.Text +"', " +
                        "remarks = '" + txtRemarks.Text + "' " +
                        "WHERE categoryID =" + supplierID);
                    MessageBox.Show("Field is updated.", "Successful");
                    GetTableFromDvg();
                }
                else
                {
                    MessageBox.Show("OK!");
                }
            }
            else
            {
                MessageBox.Show("Fields cannot be empty.", "Empty Fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Clear();
            LoadButtonSettings();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this field.", 
                "Warning", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                DBConnection.ExecutingNonQuery("DELETE FROM Category WHERE CategoryID= "
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
        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            LoadButtonSettings();
        }
        private void Category_Load(object sender, EventArgs e)
        {
            LoadButtonSettings();
            GetTableFromDvg();
        }
        public void GetTableFromDvg()
        {
            var dt = DBConnection.GetTableByQuery("SELECT * FROM Category;");
            dgvCategory.DataSource = dt;
        }
        private void LoadButtonSettings()
        {
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }
        private void dgvCategory_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnSave.Enabled = false;
            btnDelete.Enabled = true;
            btnUpdate.Enabled = true;
            supplierID = int.Parse(dgvCategory.Rows[e.RowIndex].Cells[0].Value.ToString());
            txtCategoryName.Text = dgvCategory.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtRemarks.Text = dgvCategory.Rows[e.RowIndex].Cells[2].Value.ToString();
        }
        private void Clear()
        {
            txtCategoryName.Clear();
            txtRemarks.Clear();
            txtSearch.Clear();
            txtCategoryName.Focus();
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var dt = DBConnection.GetTableByQuery("SELECT * FROM Category WHERE " +
                "categoryName LIKE '"+ txtSearch.Text +"%' OR " +
                "remarks LIKE '" + txtSearch.Text + "%';");
            dgvCategory.DataSource = dt;
        }
        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            string textBoxSender = ((TextBox)sender).Name;
            if (e.KeyChar == 13)
            {
                switch (textBoxSender)
                {
                    case "txtCategoryName":
                        txtRemarks.Focus();
                        break;
                    case "txtRemarks":
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
    }
}
