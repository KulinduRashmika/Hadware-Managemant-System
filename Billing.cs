using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.SqlClient;

namespace GUI_CW_2
{
    public partial class Billing : Form
    {
        private SqlConnection connection = new SqlConnection("Data Source=LAPTOP-672HOPO3;Initial Catalog=HardwareDb;Integrated Security=True");

        public Billing()
        {
            InitializeComponent();
            LoadItems();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            RefreshClientBillTable();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (Print reportViewer = new Print())
            {
                reportViewer.WindowState = FormWindowState.Maximized;
                reportViewer.ShowDialog();
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Close();
            new Login().Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            new Dasboard().Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
            new Catergories().Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
            new Customers().Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            new Billing().Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
            new Dasboard().Show();
        }

        private void LoadItems()
        {
            try
            {
                connection.Open();
                string query = "SELECT ItCode AS ItemID, ItName AS ItemName, Price FROM ItemTbl";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable itemTable = new DataTable();
                adapter.Fill(itemTable);
                itemsDataGridView.DataSource = itemTable; // itemsDataGridView refers to Items List grid
                itemsDataGridView.ReadOnly = true;

                itemsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                itemsDataGridView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        

       

        private void ResetFields()
        {
            amountTextBox.Clear();
            totalPriceTextBox.Clear();
        }

        

       
        private void reportDocument1_InitReport(object sender, EventArgs e)
        {
           
        }

        private void addToButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Retrieve values from input fields
                string itemNo = textBox1.Text;
                string amount = amountTextBox.Text;
                string customer = textBox3.Text;
                string paymentMethod = GetPaymentMethod();
                string billDate = dateTimePicker1.Value.ToString("yyyy-MM-dd");

                // Validate inputs
                if (string.IsNullOrWhiteSpace(itemNo) || string.IsNullOrWhiteSpace(amount) ||
                    string.IsNullOrWhiteSpace(customer) || string.IsNullOrWhiteSpace(paymentMethod))
                {
                    MessageBox.Show("Please fill all fields before adding to the bill.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Save details to the database
                string query = "INSERT INTO BillingTbl (BDate, Customer, PaymentMethod, Amount, ItemNo) VALUES (@BDate, @Customer, @PaymentMethod, @Amount, @ItemNo)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BDate", billDate);
                    command.Parameters.AddWithValue("@Customer", customer);
                    command.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@ItemNo", itemNo);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                // Refresh DataGridView to include the new row
                RefreshClientBillTable();

                // Clear input fields
                textBox1.Clear();
                amountTextBox.Clear();
                textBox3.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            
        }
        private void RefreshClientBillTable()
        {
            try
            {
                string query = "SELECT * FROM BillingTbl";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                clientBillTable.DataSource = dataTable; // Assuming clientBillTable is your DataGridView

                clientBillTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                clientBillTable.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing the data grid: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetPaymentMethod()
        {
            if (radioButton1.Checked) return "Cash";
            if (radioButton3.Checked) return "Card";
            if (radioButton2.Checked) return "Mobile";
            return null;
        }


        private void resetButton_Click_1(object sender, EventArgs e)
        {
            ResetFields();
            // Logic to clear bill data (if applicable)
        }

        private void amountTextBox_TextChanged_1(object sender, EventArgs e)
        {
            if (itemsDataGridView.SelectedRows.Count > 0 && int.TryParse(amountTextBox.Text, out int amount))
            {
                decimal price = Convert.ToDecimal(itemsDataGridView.SelectedRows[0].Cells["Price"].Value);
                totalPriceTextBox.Text = (price * amount).ToString();
            }
            else
            {
                totalPriceTextBox.Text = "";
            }
        }

        private void Billing_Load(object sender, EventArgs e)
        {

        }
    }
}
