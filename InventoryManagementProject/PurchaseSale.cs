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
    public partial class PurchaseSale : Form
    {
        private DataTable dtCart = new DataTable();
        private int productID;
        private int productCartID;
        string value = MainMenu.transactionType;
        
        public PurchaseSale()
        {
            InitializeComponent();
        }
        private void PurchaseSale_Load(object sender, EventArgs e)
        {
            this.Text = value;
            this.Refresh();
            btnRemoveFromCart.Enabled = false;
            if (value == "Purchase")
            {
                lblCustSupplier.Text = "Supplier";
                lblCartInstructions.Text = "Double Click To Select Items.";
                btnBuySell.Text = "BUY";
                var dt = DBConnection.GetTableByQuery("SELECT * FROM CustSupplier WHERE type='Supplier';");
                cmbCustSupplier.DisplayMember = "name";
                cmbCustSupplier.ValueMember = "custSupplierId";
                cmbCustSupplier.DataSource = dt;
            }
            else if (value == "Sales")
            {
                lblCustSupplier.Text = "Customer";
                btnBuySell.Text = "SELL";
                txtWholeSalePrice.Hide();
                lblWholesalePrice.Hide();
                var dt = DBConnection.GetTableByQuery("SELECT * FROM CustSupplier WHERE type='Customer';");
                cmbCustSupplier.DisplayMember = "name";
                cmbCustSupplier.ValueMember = "custSupplierId";
                cmbCustSupplier.DataSource = dt;
            }
            GetdgvProduct();
            GetdvgCart();
        }
        private void btnBuySell_Click(object sender, EventArgs e)
        {
            decimal grandTotal = 0;
            decimal vatAmount = 0;
            decimal discountAmount = 0;
            decimal taxAmount = 0;
            decimal totalPrice = 0;
            if (AreTextValueValid()==true)
            {
                if (dtCart.Rows.Count > 0)
                {
                    for (int i = 0; i < dtCart.Rows.Count; i++)
                    {
                        totalPrice = totalPrice + decimal.Parse(dtCart.Rows[i]["Total"].ToString());
                    }
                    if (cmbCustSupplier.Text == String.Empty || 
                        txtDiscount.Text == String.Empty || 
                        txtTax.Text == String.Empty || 
                        txtVAT.Text == String.Empty)
                    {
                        MessageBox.Show("Fields cannot be empty.");
                    }
                    else
                    {
                        taxAmount = (decimal.Parse(txtTax.Text) / 100) * totalPrice;
                        vatAmount = (decimal.Parse(txtVAT.Text) / 100) * totalPrice;
                        discountAmount = (decimal.Parse(txtDiscount.Text) / 100) * totalPrice;
                        grandTotal = totalPrice + vatAmount + taxAmount - discountAmount;
                        Transaction(grandTotal);
                    }
                }
                else
                {
                    MessageBox.Show("The Cart is Empty", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Input value or values are not valid.", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Transaction(decimal grandTotal)
        {
            var result = MessageBox.Show("The total price is " + Math.Round(grandTotal, 2) + "." +
                "\nAre you sure you want to confirm this " + value + " Transaction?",
                        "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {

                DBConnection.ExecutingNonQuery("INSERT INTO TransactionMaster VALUES " +
                        "('" + dtpTransactionDate.Value.ToString("yyyy-MM-d") +
                        "', '" + value + "', " +
                        int.Parse(cmbCustSupplier.SelectedValue.ToString()) + ", " +
                        Math.Round(decimal.Parse(txtVAT.Text.ToString()), 2) + ", " +
                        Math.Round(decimal.Parse(txtTax.Text.ToString()), 2) + ", " +
                        Math.Round(decimal.Parse(txtDiscount.Text.ToString()), 2) + ", " +
                        Math.Round(grandTotal, 2) + ");"); ;
                DataTable tmdt = new DataTable();
                tmdt = DBConnection.GetTableByQuery("SELECT transactionMasterID FROM TransactionMaster " +
                    "ORDER BY transactionMasterID DESC;");
                int masterID = int.Parse(tmdt.Rows[0][0].ToString());


                for (int i = 0; i < dtCart.Rows.Count; i++)
                {
                    int pID = int.Parse(dtCart.Rows[i]["Product ID"].ToString());
                    decimal pRate = Math.Round(decimal.Parse(dtCart.Rows[i]["Rate"].ToString()), 2);
                    int pQuantity = int.Parse(dtCart.Rows[i]["Quantity"].ToString());
                    decimal pTotal = Math.Round(decimal.Parse(dtCart.Rows[i]["Total"].ToString()), 2);
                    DBConnection.ExecutingNonQuery("INSERT INTO TransactionDetails VALUES (" +
                        "" + masterID + ", " + pID + ", " + pRate + ", " + pQuantity + ", " + pTotal + ");");
                    if (value == "Purchase")
                    {
                        DBConnection.ExecutingNonQuery("UPDATE Product  SET " +
                            "quantity = quantity + " + int.Parse(dtCart.Rows[i]["Quantity"].ToString()) +
                            "WHERE productID = " + int.Parse(dtCart.Rows[i]["Product ID"].ToString()) + ";");
                    }
                    else if (value == "Sales")
                    {
                        DBConnection.ExecutingNonQuery("UPDATE Product  SET " +
                            "quantity = quantity - " + int.Parse(dtCart.Rows[i]["Quantity"].ToString()) +
                            "WHERE productID = " + int.Parse(dtCart.Rows[i]["Product ID"].ToString()) + ";");
                    }
                }
                MessageBox.Show("The " + value + " Transaction is successfull!!");
                ClearEverything();
            }
            else
            {
                ClearEverything();
            }
        }
        private void ClearEverything()
        {
            dtCart.Clear();
            txtDiscount.Clear();
            txtTax.Clear();
            txtVAT.Clear();
            txtWholeSalePrice.Clear();
            dgvCart.DataSource = dtCart;
        }
        private void GetdgvProduct()
        {
            if (value == "Sales")
            {
                var dt = DBConnection.GetTableByQuery("SELECT p.productId, p.productName, " +
                    "c.categoryName AS category, u.acronym AS unit, p.rate, p.quantity FROM Product p" +
                " INNER JOIN Category c " +
                "ON c.categoryId = p.categoryID INNER JOIN Unit u ON u.UnitID = p.unitID;");
                dgvProduct.DataSource = dt;
            }
            else if (value == "Purchase")
            {
                var dt = DBConnection.GetTableByQuery("SELECT p.productId, p.productName, " +
                    "c.categoryName AS category, u.acronym AS unit FROM Product p" +
                " INNER JOIN Category c " +
                "ON c.categoryId = p.categoryID INNER JOIN Unit u ON u.UnitID = p.unitID;");
                dgvProduct.DataSource = dt;
            }
        }
        private void GetdvgCart()
        {
            dtCart.Columns.Add("Product ID");
            dtCart.Columns.Add("Product Name");
            dtCart.Columns.Add("Quantity");
            dtCart.Columns.Add("Rate");
            dtCart.Columns.Add("Total");
            dgvCart.DataSource= dtCart;
        }
        private void dgvProduct_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int id = -1;
            productID = int.Parse(dgvProduct.Rows[e.RowIndex].Cells[0].Value.ToString());
            if (value == "Sales")
            {
                if (int.Parse(dgvProduct.Rows[e.RowIndex].Cells[5].Value.ToString()) == 0)
                {
                    MessageBox.Show("Sorry this product is out of stock.");
                }
                else
                {
                    if (dtCart.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtCart.Rows.Count; i++)
                        {
                            if (productID == int.Parse(dtCart.Rows[i]["Product ID"].ToString()))
                            {
                                id = i;
                            }
                        }
                        if (id >= 0)
                        {
                            dtCart.Rows[id]["Quantity"] = 
                                (int.Parse(dtCart.Rows[id]["Quantity"].ToString()) + 1).ToString();
                            dtCart.Rows[id]["Total"] = 
                                (decimal.Parse(dtCart.Rows[id]["Rate"].ToString()) * 
                                decimal.Parse(dtCart.Rows[id]["Quantity"].ToString())).ToString();
                        }
                        else
                        {
                            dtCart.Rows.Add(productID, 
                                dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString(), 1, 
                                dgvProduct.Rows[e.RowIndex].Cells[4].Value.ToString(), 
                                dgvProduct.Rows[e.RowIndex].Cells[4].Value.ToString());
                        }
                    }
                    else
                    {
                        dtCart.Rows.Add(productID, 
                            dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString(), 1, 
                            dgvProduct.Rows[e.RowIndex].Cells[4].Value.ToString(), 
                            dgvProduct.Rows[e.RowIndex].Cells[4].Value.ToString());
                    }
                }
            }
            else if (value == "Purchase")
            {
                    if (dtCart.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtCart.Rows.Count; i++)
                        {
                            if (productID == int.Parse(dtCart.Rows[i]["Product ID"].ToString()))
                            {
                                id = i;
                            }
                        }
                        if (id >= 0)
                        {
                            if (txtWholeSalePrice.Text != String.Empty)
                            {
                                MessageBox.Show("You cannot input wholesale price. " +
                                    "Product is already in the cart.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                dtCart.Rows[id]["Quantity"] = 
                                (int.Parse(dtCart.Rows[id]["Quantity"].ToString()) + 1).ToString();
                                dtCart.Rows[id]["Total"] = 
                                decimal.Parse(dtCart.Rows[id]["Rate"].ToString()) * 
                                int.Parse(dtCart.Rows[id]["Quantity"].ToString());
                            } 
                        }
                        else
                        {
                            if (txtWholeSalePrice.Text != "")
                            {
                                dtCart.Rows.Add(productID, 
                                    dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString(), 1, 
                                    txtWholeSalePrice.Text, txtWholeSalePrice.Text);
                                txtWholeSalePrice.Clear();
                                txtWholeSalePrice.Focus();
                            }
                            else
                            {
                                MessageBox.Show("Enter the wholesale price first.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        if (txtWholeSalePrice.Text != "")
                        {
                            dtCart.Rows.Add(productID, 
                                dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString(), 1, 
                                txtWholeSalePrice.Text, txtWholeSalePrice.Text);
                            txtWholeSalePrice.Clear();
                            txtWholeSalePrice.Focus();
                        }
                        else
                        {
                            MessageBox.Show("Enter the wholesale price first.", 
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
            }  
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearEverything();
        }
        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            string textBoxSender = ((TextBox)sender).Name;
            if (textBoxSender.Equals("txtTax") || 
                textBoxSender.Equals("txtDiscount") || 
                textBoxSender.Equals("txtVAT") || 
                textBoxSender.Equals("txtWholeSalePrice"))
            {
                if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == '.' || e.KeyChar == 8)
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
                    case "txtVAT":
                        txtTax.Focus();
                        break;
                    case "txtTax":
                        txtDiscount.Focus();
                        break;
                    case "txtDiscount":
                        dtpTransactionDate.Focus();
                        break;
                }
            }
        }
        private bool AreTextValueValid()
        {
            int t = 0, d= 0, v= 0, w= 0;
            foreach (char c in txtDiscount.Text)
            {
                if (c == '.')
                {
                    d++;
                }
            }
            foreach (char c in txtVAT.Text)
            {
                if (c == '.')
                {
                    v++;
                }
            }
            foreach (char c in txtWholeSalePrice.Text)
            {
                if (c == '.')
                {
                    w++;
                }
            }
            foreach (char c in txtTax.Text)
            {
                if (c == '.')
                {
                    t++;
                }
            }
            if (t > 1 || v > 1 || w > 1 || d > 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void btnRemoveFromCart_Click(object sender, EventArgs e)
        {
            for (int i = dtCart.Rows.Count - 1; i >= 0; i--)
            {
                DataRow drCart = dtCart.Rows[i];
                if (int.Parse(drCart["Product ID"].ToString()) == productCartID)
                {
                    drCart.Delete();
                }
            }
            dtCart.AcceptChanges();
            
        }
        private void dgvCart_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnRemoveFromCart.Enabled = true;
            btnRemoveFromCart.Focus();
            productCartID = int.Parse(dgvCart.Rows[e.RowIndex].Cells[0].Value.ToString());
        }
    }
}
