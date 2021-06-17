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
    public partial class MainMenu : Form
    {
        public static string transactionType;
        public static string reportType;
        public static string CustomerOrSupplier;
        public MainMenu()
        {
            InitializeComponent();
        }
        private void registerNewUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Login.isAdmin == true)
            {
                Registration regObj = new Registration();
                regObj.Show();
            }
            else
            {
                MessageBox.Show("Access Denied", "You are not a admin.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void purchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transactionType = "Purchase";
            PurchaseSale pObj = new PurchaseSale();
            pObj.Show();
        }
        private void salesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transactionType = "Sales";
            PurchaseSale sObj = new PurchaseSale();
            sObj.Show();
        }
        private void customersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerOrSupplier = "Customer";
            CustomerSupplier cObj = new CustomerSupplier();
            cObj.Show();
        }
        private void supplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerOrSupplier = "Supplier";
            CustomerSupplier sObj = new CustomerSupplier();
            sObj.Show();
        }
        private void productToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Product productObj = new Product();
            productObj.Show();
        }
        private void catogoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Category categoryObj = new Category();
            categoryObj.Show();
        }
        private void purchaseReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reportType = "Purchase Report";
            PurchaseSaleReport purchaseReportObj = new PurchaseSaleReport();
            purchaseReportObj.Show();
        }
        private void salesReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reportType = "Sales Report";
            PurchaseSaleReport saleReportObj = new PurchaseSaleReport();
            saleReportObj.Show();
        }
        private void unitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Unit uObj = new Unit();
            uObj.Show();
        }
        private void deleteUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Login.isAdmin == true)
            {
                DeleteUsers du = new DeleteUsers();
                du.Show();
            }
            else
            {
                MessageBox.Show("Access Denied", "You are not a admin.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
