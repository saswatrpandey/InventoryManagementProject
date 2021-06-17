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
    public partial class Unit : Form
    {
        private int supplierID;
        public Unit()
        {
            InitializeComponent();
        }
        private void Unit_Load(object sender, EventArgs e)
        {
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            GetTableFromDvg();
        }
        public void GetTableFromDvg()
        {
            var dt = DBConnection.GetTableByQuery("SELECT * FROM Unit;");
            dgvUnit.DataSource = dt;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtUnitName.Text == String.Empty || txtAcronym.Text == String.Empty)
            {
                MessageBox.Show("Fields cannot be empty.", "Empty Fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DBConnection.ExecutingNonQuery("INSERT INTO Unit VALUES" +
                "('" + txtUnitName.Text + "', '" + txtAcronym.Text + "');");;
                MessageBox.Show("Field is saved.", "Successful");
                GetTableFromDvg();
                Clear();
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtUnitName.Text != String.Empty || txtAcronym.Text != String.Empty)
            { 
                var result = MessageBox.Show("Are you sure you want to edit this field.",
                "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    DBConnection.ExecutingNonQuery("UPDATE Unit SET " +
                        "unitName = '" + txtUnitName.Text + "', " +
                        "acronym = '" + txtAcronym.Text + "' " +
                        "WHERE unitID =" + supplierID);
                    MessageBox.Show("Field is updated.", "Successful");
                    GetTableFromDvg();
                }
                else
                {
                    MessageBox.Show("OK!");
                }
                Clear();
            }
            else
            {
                MessageBox.Show("Fields cannot be empty.", "Empty Fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this field.",
                "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                DBConnection.ExecutingNonQuery("DELETE FROM Unit WHERE UnitID= "
                    + supplierID);
                MessageBox.Show("Field is deleted.", "Successful");
                GetTableFromDvg();
            }
            else
            {
                MessageBox.Show("OK!");
            }
            Clear();
        }
        private void dgvUnit_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnSave.Enabled = false;
            btnDelete.Enabled = true;
            btnUpdate.Enabled = true;
            supplierID = int.Parse(dgvUnit.Rows[e.RowIndex].Cells[0].Value.ToString());
            txtUnitName.Text = dgvUnit.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtAcronym.Text = dgvUnit.Rows[e.RowIndex].Cells[2].Value.ToString();
        }
        private void Clear()
        {
            txtUnitName.Clear();
            txtAcronym.Clear();
            txtSearch.Clear();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            txtUnitName.Focus();
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var dt = DBConnection.GetTableByQuery("SELECT * FROM Unit WHERE " +
                "unitName LIKE '" + txtSearch.Text + "%' OR " +
                "acronym LIKE '" + txtSearch.Text + "%';");
            dgvUnit.DataSource = dt;
        }
        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            string textBoxSender = ((TextBox)sender).Name;
            if (e.KeyChar == 13)
            {
                switch (textBoxSender)
                {
                    case "txtUnitName":
                        txtAcronym.Focus();
                        break;
                    case "txtAcronym.Text":
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
        }
    }
}
