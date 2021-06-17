using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryManagementProject
{
    public partial class PurchaseSaleReport : Form
    {
        string value = MainMenu.reportType;
        private int transactionID= 0;
        public PurchaseSaleReport()
        {
            InitializeComponent();
        }
        private void PurchaseSaleReport_Load(object sender, EventArgs e)
        {
            this.Text = MainMenu.reportType;
            this.Refresh();
            btnDelete.Enabled = false;
            btnGenerate.Enabled = false;
            lblInstructions.Text = value == "Purchase Report" ? "Purchase Transactions" : "Sales Transactions";
            lblCustSupplierAddress.Hide();
            lblCustSupplierName.Hide();
            lblTransactionDate.Hide();
            GetTransactionDetaildvg();
        }
        private void GetTransactionDetaildvg()
        {
            if (value== "Purchase Report")
            {
                dgvTransactionDetails.DataSource = 
                    DBConnection.GetTableByQuery("SELECT tm.transactionMasterID, " +
                    "cs.name as supplierName, " +
                    "tm.transactionDate, " +
                    "tm.grandTotal  FROM TransactionMaster " +
                    "tm INNER JOIN CustSupplier " +
                    "cs ON tm.custSupplierID= cs.custSupplierID " +
                    "WHERE tm.type='Purchase';");
            }
            else if (value == "Sales Report")
            {
                dgvTransactionDetails.DataSource = 
                    DBConnection.GetTableByQuery("SELECT tm.transactionMasterID, " +
                    "cs.name as supplierName, " +
                    "tm.transactionDate, " +
                    "tm.grandTotal  FROM TransactionMaster " +
                    "tm INNER JOIN CustSupplier " +
                    "cs ON tm.custSupplierID= cs.custSupplierID " +
                    "WHERE tm.type='Sales';");
            }
        }
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (transactionID == 0)
            {
                MessageBox.Show("Select a Transaction Detail from the table.");
            }
            else
            {
                DataTable d1 = new DataTable();
                d1 = DBConnection.GetTableByQuery("SELECT cs.name, cs.address, " +
                    "tm.transactionDate FROM TransactionMaster tm " +
                    "INNER JOIN CustSupplier cs ON tm.custSupplierID= cs.custSupplierId " +
                    "WHERE tm.transactionMasterID= " + transactionID + ";");
                lblCustSupplierName.Text = "Name: " + d1.Rows[0][0];
                lblCustSupplierAddress.Text = "Address: " + d1.Rows[0][1];
                lblTransactionDate.Text = "Date: " + d1.Rows[0][2].ToString().Substring(0, 10);

                lblCustSupplierName.Show();
                lblCustSupplierAddress.Show();
                lblTransactionDate.Show();

                DataTable d2 = new DataTable();
                d2 = DBConnection.GetTableByQuery
                    ("SELECT vat, tax, discount, grandtotal FROM TransactionMaster " +
                    "WHERE transactionMasterID= " + transactionID + ";");
                DataTable d3 = new DataTable();
                d3.Columns.Add("Name");
                d3.Columns.Add("Quantity");
                d3.Rows.Add("VAT", d2.Rows[0][0] + "%");
                d3.Rows.Add("Tax", d2.Rows[0][1] + "%");
                d3.Rows.Add("Discount", d2.Rows[0][2] + "%");
                d3.Rows.Add("Total", d2.Rows[0][3]);
                d3.Rows.Add("Total (in words)", NumberToWords(double.Parse(d2.Rows[0][3].ToString())));
                dgvTotal.DataSource = d3;
                dgvTotal.Columns[0].Width = 150;
                dgvTotal.Columns[1].Width = 700;
                dgvProducts.DataSource = DBConnection.GetTableByQuery("SELECT p.productName, " +
                    "td.quantity, " +
                    "td.rate, " +
                    "td.total FROM TransactionDetails td INNER JOIN " +
                    "Product p ON td.productID = p.productID " +
                    "WHERE td.transactionMasterID = " + transactionID + ";");
                dgvProducts.Columns[0].Width = 315;
                dgvProducts.Columns[3].Width = 150;
            }
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string transactionType = value == "Purchase Report" ? "Purchase" : "Sales"; ;
            dgvTransactionDetails.DataSource = DBConnection.GetTableByQuery("SELECT tm.transactionMasterID, " +
                    "cs.name as supplierName, " +
                    "tm.transactionDate, " +
                    "tm.grandTotal  FROM TransactionMaster " +
                    "tm INNER JOIN CustSupplier " +
                    "cs ON tm.custSupplierID = cs.custSupplierID " +
                    "WHERE tm.type = '" + transactionType + "' AND " +
                    "(tm.transactionMasterID LIKE '" + txtSearch.Text + "%' OR " +
                    "cs.name LIKE '" + txtSearch.Text + "%' OR " +
                    "tm.transactionDate LIKE '" + txtSearch.Text + "%' OR " +
                    "tm.grandTotal LIKE '" + txtSearch.Text + "%'); ");
        }
        private void HeaderClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnGenerate.Enabled = true;
            btnDelete.Enabled = true;
            transactionID = int.Parse(dgvTransactionDetails.Rows[e.RowIndex].Cells[0].Value.ToString());
            btnGenerate.Focus();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this field.",
                "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                DBConnection.ExecutingNonQuery("DELETE FROM TransactionDetails WHERE transactionMasterID= "
                    + transactionID + ";");
                DBConnection.ExecutingNonQuery("DELETE FROM TransactionMaster WHERE transactionMasterID= "
                    + transactionID + ";");
            }
            else
            {
                MessageBox.Show("OK!");
            }
        }
        /* Code taken from Source: 
        https://stackoverflow.com/questions/43334034/convert-number-with-decimals-in-currency-to-words */
        public static string NumberToWords(double doubleNumber)
        {
            var beforeFloatingPoint = (int)Math.Floor(doubleNumber);
            var beforeFloatingPointWord = $"{NumberToWords(beforeFloatingPoint)} rupees";
            var afterFloatingPointWord =
                $"{SmallNumberToWord((int)((doubleNumber - beforeFloatingPoint) * 100), "")} paisa";
            return $"{beforeFloatingPointWord} and {afterFloatingPointWord}";
        }
        private static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            var words = "";

            if (number / 1000000000 > 0)
            {
                words += NumberToWords(number / 1000000000) + " billion ";
                number %= 1000000000;
            }

            if (number / 1000000 > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if (number / 1000 > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            words = SmallNumberToWord(number, words);

            return words;
        }
        private static string SmallNumberToWord(int number, string words)
        {
            if (number <= 0) return words;
            if (words != "")
                words += " ";

            var unitsMap = new[] { "zero", "one", "two", "three", "four", 
                "five", "six", "seven", "eight", "nine", "ten", "eleven", 
                "twelve", "thirteen", "fourteen", "fifteen", "sixteen", 
                "seventeen", "eighteen", "nineteen" };
            var tensMap = new[] { "zero", "ten", "twenty", "thirty", 
                "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += "-" + unitsMap[number % 10];
            }
            return words;
        }
    }
}
