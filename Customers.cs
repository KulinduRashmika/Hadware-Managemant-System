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

namespace GUI_CW_2
{
    public partial class Customers : Form
    {
        private string connectionString = @"Data Source=LAPTOP-672HOPO3;Initial Catalog=HardwareDb;Integrated Security=True";
        public Customers()
        {
            InitializeComponent();
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

        private void Customers_Load(object sender, EventArgs e)
        {
            // Load customers when the form loads
            LoadCustomers();
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            // Add a new customer
            string customerName = txtCustomerName.Text.Trim();
            string gender = cmbGender.SelectedItem?.ToString();
            string phone = txtContactNo.Text.Trim();

            if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(gender) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Please fill all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Insert into CustomerTbl
                    string query = "INSERT INTO CustomerTbl (CustName, Gender, Phone) VALUES (@CustName, @Gender, @ContactNo)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CustName", customerName);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@ContactNo", phone);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Customer added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadCustomers();
                            ClearInputs();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add customer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnEditCustomer_Click(object sender, EventArgs e)
        {
            // Edit selected customer
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                int custCode = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["Customer ID"].Value);
                string customerName = dgvCustomers.SelectedRows[0].Cells["Customer Name"].Value.ToString();
                string gender = dgvCustomers.SelectedRows[0].Cells["Gender"].Value.ToString();
                string phone = dgvCustomers.SelectedRows[0].Cells["Contact No"].Value.ToString();

                if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(gender) || string.IsNullOrEmpty(phone))
                {
                    MessageBox.Show("Please fill all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        // Update CustomerTbl
                        string query = "UPDATE CustomerTbl SET CustName = @CustName, Gender = @Gender, Phone = @ContactNo WHERE CustCode = @CustCode";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@CustCode", custCode);
                            cmd.Parameters.AddWithValue("@CustName", customerName);
                            cmd.Parameters.AddWithValue("@Gender", gender);
                            cmd.Parameters.AddWithValue("@ContactNo", phone);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadCustomers();
                                ClearInputs();
                            }
                            else
                            {
                                MessageBox.Show("Failed to update customer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to edit.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Delete selected customer
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                int custCode = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["Customer ID"].Value);

                var confirmResult = MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmResult == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            string query = "DELETE FROM CustomerTbl WHERE CustCode = @CustCode";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@CustCode", custCode);
                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadCustomers();
                                }
                                else
                                {
                                    MessageBox.Show("Failed to delete customer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void LoadCustomers()
        {
            // Load customers from the database into the DataGridView
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT CustCode AS [Customer ID], CustName AS [Customer Name], Gender, Phone AS [Contact No] FROM CustomerTbl";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvCustomers.DataSource = dt;  // Bind DataTable to DataGridView
                    }

                    dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvCustomers.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while loading customers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void ClearInputs()
        {
            // Clear input fields
            txtCustomerName.Clear();
            cmbGender.SelectedIndex = -1;
            txtContactNo.Clear();
            txtCustomerName.Focus();
        }

    }
}
