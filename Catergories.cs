using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace GUI_CW_2
{
    public partial class Catergories : Form
    {
        // Database connection string (replace with your actual details)
        private string connectionString = @"Data Source=LAPTOP-672HOPO3;Initial Catalog=HardwareDb;Integrated Security=True";

        public Catergories()
        {
            InitializeComponent();
        }

        private void Catergories_Load(object sender, EventArgs e)
        {
            // Load categories when the form loads
            LoadCategories();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Refresh categories list
            LoadCategories();
            MessageBox.Show("Categories refreshed successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Navigate to Dashboard
            this.Close();
            new Dasboard().Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Navigate to Customers
            this.Close();
            new Customers().Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Navigate to Billing
            this.Close();
            new Billing().Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Navigate to Dashboard
            this.Close();
            new Dasboard().Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Navigate to Login
            this.Close();
            new Login().Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Add a new category
            string catName = txtCategoryName.Text.Trim();

            if (string.IsNullOrEmpty(catName))
            {
                MessageBox.Show("Please enter the Category Name.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Use the SQL INSERT statement without specifying CatCode, as it will auto-increment
                    string query = "INSERT INTO CategoryTbl (CatName) VALUES (@CatName)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CatName", catName);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Category added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadCategories();
                            ClearInputs();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add category.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadCategories()
        {
            // Load categories from the database into the DataGridView
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT CatCode AS [Category ID], CatName AS [Category Name] FROM CategoryTbl";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvCategories.DataSource = dt;  // Bind DataTable to DataGridView
                    }

                    // Ensure the DataGridView is refreshed
                    dgvCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvCategories.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearInputs()
        {
            // Clear input fields
            txtCategoryName.Clear();
            txtCategoryName.Focus();
        }

        private void delete_btn_Click_1(object sender, EventArgs e)
        {
            // Check if a row is selected in the DataGridView
            if (dgvCategories.SelectedRows.Count > 0)
            {
                // Get the Category ID (CatCode) from the selected row
                int catCode = Convert.ToInt32(dgvCategories.SelectedRows[0].Cells["Category ID"].Value);

                // Confirm deletion with the user
                var confirmResult = MessageBox.Show("Are you sure you want to delete this category?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmResult == DialogResult.Yes)
                {
                    // Perform the deletion from the database
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            string query = "DELETE FROM CategoryTbl WHERE CatCode = @CatCode";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@CatCode", catCode);
                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    // Successfully deleted from the database, refresh the DataGridView
                                    MessageBox.Show("Category deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadCategories();
                                }
                                else
                                {
                                    MessageBox.Show("Failed to delete category.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // No row selected in the DataGridView
                MessageBox.Show("Please select a category to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void edit_btn_Click(object sender, EventArgs e)
        {
            // Check if a row is selected in the DataGridView
            if (dgvCategories.SelectedRows.Count > 0)
            {
                // Get the Category ID (CatCode) from the selected row
                int catCode = Convert.ToInt32(dgvCategories.SelectedRows[0].Cells["Category ID"].Value);
                string catName = dgvCategories.SelectedRows[0].Cells["Category Name"].Value.ToString();

                // Populate the input fields with the selected category's data
                txtCategoryName.Text = catName;
                txtCategoryName.Focus();

                // Button to update data on the database
                // Save changes to the database when the user edits
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        // Prepare the UPDATE query for the selected category
                        string query = "UPDATE CategoryTbl SET CatName = @CatName WHERE CatCode = @CatCode";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@CatCode", catCode);
                            cmd.Parameters.AddWithValue("@CatName", txtCategoryName.Text.Trim());

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Category updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadCategories();  // Refresh the list of categories
                                ClearInputs();
                            }
                            else
                            {
                                MessageBox.Show("Failed to update category.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // No row selected in the DataGridView
                MessageBox.Show("Please select a category to edit.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}


