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
    public partial class Product : Form
    {
        private int productID;
        public Product()
        {
            InitializeComponent();
        }
        private void LoadUnit()
        {
            var dt = DBConnection.GetTableByQuery("SELECT * FROM Unit");
            cmbUnit.DisplayMember = "unitName";
            cmbUnit.ValueMember = "unitID";
            cmbUnit.DataSource = dt;
        }
        private void LoadCategory()
        {
            var dt = DBConnection.GetTableByQuery("SELECT * FROM Category;");
            cmbCategory.DisplayMember = "categoryName";
            cmbCategory.ValueMember = "categoryID";
            cmbCategory.DataSource = dt;
        }
        private bool AreFieldsEmpty()
        {
            if (txtProductName.Text == String.Empty || txtQuantity.Text == String.Empty || txtRate.Text == String.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsRateValid()
        {
            int numOfPointsUsed=0;
            foreach (char c in txtRate.Text)
            {
                if (c == '.')
                {
                    numOfPointsUsed++;
                }
            }
            if (numOfPointsUsed>1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if(AreFieldsEmpty() == true)
            {
                MessageBox.Show("Fields cannot be empty.", "Empty Fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (IsRateValid() == true)
                {
                    DBConnection.ExecutingNonQuery("INSERT INTO Product VALUES " +
                        "('" + txtProductName.Text + "', " + cmbCategory.SelectedValue + "," +
                        cmbUnit.SelectedValue + "," + txtRate.Text + "," + txtQuantity.Text + ");");
                    MessageBox.Show("Field is saved.", "Successful");
                    GetTableFromDvg();
                    Clear();
                }
                else
                {
                    MessageBox.Show("Rate is not valid.");
                    txtRate.Clear();
                    txtRate.Focus();
                }
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this field?",
                "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                DBConnection.ExecutingNonQuery("DELETE FROM Product WHERE productID= "
                    + productID);
                GetTableFromDvg();
            }
            else
            {
                MessageBox.Show("OK!");
            }
            Clear();
            LoadButtonSettings();
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (AreFieldsEmpty() == true)
            {
                MessageBox.Show("Fields cannot be empty.", "Empty Fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (IsRateValid() == true)
                {
                    var result = MessageBox.Show("Are you sure you want to update this field?",
                        "Warning",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        DBConnection.ExecutingNonQuery("UPDATE Product SET productName= '" + txtProductName.Text +
                        "', categoryID= " + cmbCategory.SelectedValue +
                        ", unitID= " + cmbUnit.SelectedValue +
                        ", rate= " + txtRate.Text +
                        ", quantity= " + txtQuantity.Text + " WHERE productID=" + productID + ";");
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
                else
                {
                    MessageBox.Show("Rate is not valid.");
                }
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }
        private void Product_Load(object sender, EventArgs e)
        {
            LoadButtonSettings();
            GetTableFromDvg();
            LoadUnit();
            LoadCategory();
        }
        private void LoadButtonSettings()
        {
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }
        public void GetTableFromDvg()
        {
            var dt = DBConnection.GetTableByQuery("SELECT p.productId, p.productName, c.categoryName, u.acronym, " +
                "p.rate, p.quantity FROM Product p"+ 
                " INNER JOIN Category c ON c.categoryId = p.categoryID INNER JOIN Unit u ON u.UnitID = p.unitID;");
            dgvProduct.DataSource = dt;
        }
        private void Clear()
        {
            txtProductName.Clear();
            txtQuantity.Clear();
            txtRate.Clear();
            txtSearch.Clear();
            //cmbCategory.ResetText();
            //cmbUnit.ResetText();
            txtProductName.Focus();
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            btnSave.Enabled = true;
        }
        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            string textBoxSender = ((TextBox)sender).Name;
                if (textBoxSender.Equals("txtRate"))
                {
                    if ((e.KeyChar >= '0' && e.KeyChar <='9') || e.KeyChar=='.' || e.KeyChar ==8)
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
                if (textBoxSender.Equals("txtQuantity"))
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
            if (textBoxSender.Equals("txtQuantity") && e.KeyChar == 13)
            {
                if (btnSave.Enabled == true)
                {
                    btnSave.PerformClick();
                }
                else
                {
                    btnUpdate.Focus();
                }
            }
            if (e.KeyChar == 13)
            {
                switch (textBoxSender)
                {
                    case "txtProductName":
                        txtRate.Focus();
                        break;
                    case "txtRate":
                        txtQuantity.Focus();
                        break;
                    case "txtQuantity":
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
        private void dgvProduct_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnDelete.Enabled = true;
            btnUpdate.Enabled = true;
            btnSave.Enabled = false;
            productID = int.Parse(dgvProduct.Rows[e.RowIndex].Cells[0].Value.ToString());
            DataTable product = new DataTable();
            product = DBConnection.GetTableByQuery("SELECT u.acronym, " +
                "c.categoryName FROM Product p " +
                "INNER JOIN Unit u ON p.unitID= u.unitID " +
                "INNER JOIN Category c ON p.categoryID= c.categoryID " +
                "WHERE productID = " + productID);
            txtProductName.Text = dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString();
            cmbUnit.Text = product.Rows[0]["acronym"].ToString();
            cmbCategory.Text = product.Rows[0]["categoryName"].ToString();
            txtRate.Text = dgvProduct.Rows[e.RowIndex].Cells[4].Value.ToString();
            txtQuantity.Text = dgvProduct.Rows[e.RowIndex].Cells[5].Value.ToString();
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            dgvProduct.DataSource = DBConnection.GetTableByQuery("SELECT p.productId, p.productName, " +
                "c.categoryName, u.acronym, p.rate, p.quantity " +
                "FROM Product p INNER JOIN Category c ON c.categoryId = p.categoryID INNER JOIN Unit u " +
                "ON u.UnitID = p.unitID WHERE " +
                "(p.productId LIKE '" + txtSearch.Text + "%' " +
                "OR p.productName LIKE '" + txtSearch.Text + "%' " +
                "OR c.categoryName LIKE '" + txtSearch.Text + "%' " +
                "OR u.acronym LIKE '" + txtSearch.Text + "%' " +
                "OR p.rate LIKE '" + txtSearch.Text + "%' " +
                "OR p.quantity LIKE '" + txtSearch.Text + "%');");
        }
    }
}
