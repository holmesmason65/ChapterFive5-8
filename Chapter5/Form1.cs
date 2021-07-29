/* Mason Holmes 
 * Chapter 5 : Example 5-5
 * 7/28/2021
 * This program provides a GUI for retrieving data from and provides state control for editing the database.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace Chapter5
{
    public partial class Form1 : Form
    {
        SqlConnection booksConnection;
        SqlCommand authorsCommand;
        SqlDataAdapter authorsAdapter;
        DataTable authorsTable;
        CurrencyManager authorsManager;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string path = Path.GetFullPath("SQLBooksDB.mdf");

            booksConnection = new SqlConnection($@"Data Source=.\SQLEXPRESS; AttachDbFilename={path};
                                                    Integrated Security=True; Connect Timeout=30; User Instance=True");
            booksConnection.Open();

            // establish command object 
            authorsCommand = new SqlCommand("Select * from authors ORDER BY Author", booksConnection);

            // establish data adpater 
            authorsAdapter = new SqlDataAdapter();
            authorsAdapter.SelectCommand = authorsCommand;
            authorsTable = new DataTable();
            authorsAdapter.Fill(authorsTable);

            // bind controls to data table 
            txtAuthorID.DataBindings.Add("Text", authorsTable, "Au_ID");
            txtAuthorName.DataBindings.Add("Text", authorsTable, "Author");
            txtYearBorn.DataBindings.Add("Text", authorsTable, "Year_Born");


            // establish currency manager
            authorsManager = (CurrencyManager)this.BindingContext[authorsTable];

            // page 5-40
            this.Show(); // what does this do???
            SetState("View");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // close the connection 
            booksConnection.Close();
            // dispose of the objects 
            booksConnection.Dispose();
            authorsCommand.Dispose();
            authorsAdapter.Dispose();
            authorsTable.Dispose();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (authorsManager.Position == 0)
            {
                Console.Beep();
            }
            authorsManager.Position--;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (authorsManager.Position == authorsManager.Count - 1)
            {
                Console.Beep();
            }
            authorsManager.Position++;
        }

        private void btnExit_Click(object sender, EventArgs e) // should be called btnEdit not Exit
        {
            SetState("Edit");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Record saved.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SetState("View");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetState("View");
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            SetState("Add");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult response;
            response = MessageBox.Show("Are you sure you want to delete this record", "Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (response == DialogResult.No)
            {
                return;
            }
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            this.Close(); // ?
        }

        private void SetState(string appState)
        {
            switch (appState)
            {
                case "View":
                    txtAuthorID.BackColor = Color.White;
                    txtAuthorID.ForeColor = Color.Black;
                    txtAuthorName.ReadOnly = true;
                    txtYearBorn.ReadOnly = true;
                    btnPrevious.Enabled = true;
                    btnAddNew.Enabled = true; 
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                    btnEdit.Enabled = true;
                    btnDelete.Enabled = true;
                    btnDone.Enabled = true;
                    txtAuthorName.Focus();
                    break;
                default: // Add or Edit if not in view 
                    txtAuthorID.BackColor = Color.Red;
                    txtAuthorID.ForeColor = Color.White;
                    txtAuthorName.ReadOnly = false;
                    txtYearBorn.ReadOnly = false;
                    btnPrevious.Enabled = false;
                    btnAddNew.Enabled = false; 
                    btnSave.Enabled = true;
                    btnCancel.Enabled = true;
                    btnEdit.Enabled = false;
                    btnDelete.Enabled = false;
                    btnDone.Enabled = false;
                    txtAuthorName.Focus();
                    break;
            }

        }

        private void txtYearBorn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (int)e.KeyChar == 8)
            {
                // Acceptable Keystrokes
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
                Console.Beep();
            }
        }
    }
}
