using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrystalDecisions.CrystalReports.Engine;
using System.Windows.Forms;

namespace GUI_CW_2
{
    public partial class Dasboard : Form
    {
        private string connectionString = @"Data Source=LAPTOP-672HOPO3;Initial Catalog=HardwareDb;Integrated Security=True";

        public Dasboard()
        {
            InitializeComponent();
        }

        private void label9_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Check if a row is selected in the DataGridView
            if (dgvItems.SelectedRows.Count > 0)
            {
                // Get the item details from the selected row
                int itemCode = Convert.ToInt32(dgvItems.SelectedRows[0].Cells["Item Code"].Value);
                string itemName = dgvItems.SelectedRows[0].Cells["Item Name"].Value.ToString();
                decimal price = Convert.ToDecimal(dgvItems.SelectedRows[0].Cells["Price"].Value);
                int stock = Convert.ToInt32(dgvItems.SelectedRows[0].Cells["Stock"].Value);
                string manufacturer = dgvItems.SelectedRows[0].Cells["ItManuf"].Value.ToString();
                int category = Convert.ToInt32(dgvItems.SelectedRows[0].Cells["Category Code"].Value);

                // Pre-fill the input fields with the selected item's data
                txtItemName.Text = itemName;
                txtPrice.Text = price.ToString();
                txtStock.Text = stock.ToString();
                txtManufacturer.Text = manufacturer;
                cmbCategory.SelectedValue = category;

                // Now proceed to update the item after validation
                if (string.IsNullOrEmpty(txtItemName.Text) || cmbCategory.SelectedValue == null ||
                    string.IsNullOrEmpty(txtPrice.Text) || string.IsNullOrEmpty(txtStock.Text))
                {
                    MessageBox.Show("Please fill out all fields with valid data.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Convert the fields to proper types
                decimal updatedPrice;
                int updatedStock;
                if (!decimal.TryParse(txtPrice.Text, out updatedPrice))
                {
                    MessageBox.Show("Invalid price format.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtStock.Text, out updatedStock))
                {
                    MessageBox.Show("Invalid stock format.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get the selected category value
                int selectedCategory = Convert.ToInt32(cmbCategory.SelectedValue);

                // Perform the update operation
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "UPDATE ItemTbl SET ItName = @ItName, ItCategory = @CategoryCode, Price = @Price, Stock = @Stock, ItManuf = @ItManuf WHERE ItCode = @ItCode";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Add parameters to the query
                            cmd.Parameters.AddWithValue("@ItName", txtItemName.Text);
                            cmd.Parameters.AddWithValue("@CategoryCode", selectedCategory);
                            cmd.Parameters.AddWithValue("@Price", updatedPrice);
                            cmd.Parameters.AddWithValue("@Stock", updatedStock);
                            cmd.Parameters.AddWithValue("@ItManuf", txtManufacturer.Text);
                            cmd.Parameters.AddWithValue("@ItCode", itemCode);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Item updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadItems(); // Refresh the item list
                                ClearInputs(); // Clear input fields
                            }
                            else
                            {
                                MessageBox.Show("Failed to update item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an item to edit.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void label11_Click(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            new Dasboard().Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide(); // Use Hide instead of Close to keep the current form in memory
            Catergories categoriesForm = new Catergories();
            categoriesForm.Show();
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

        private void button9_Click(object sender, EventArgs e)
        {
            this.Close();
            new Login().Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
        }

        private void Dasboard_Load(object sender, EventArgs e)
        {
            // Load categories into ComboBox
            LoadCategories();
            // Load items into DataGridView
            LoadItems();
        }

        private void LoadCategories()
        {
            // Load categories into the ComboBox
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT CatCode, CatName FROM CategoryTbl";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        // Bind to ComboBox
                        cmbCategory.DisplayMember = "CatName"; // Display the category name
                        cmbCategory.ValueMember = "CatCode";   // Use the category code as the value
                        cmbCategory.DataSource = dt;           // Bind the data to the ComboBox

                        // Add "Add New Category" item at the top
                        DataRow addCategoryRow = dt.NewRow();
                        addCategoryRow["CatCode"] = 0;            // Placeholder value
                        addCategoryRow["CatName"] = "Add Category"; // Display text
                        dt.Rows.InsertAt(addCategoryRow, 0);      // Insert it at the first position
                        cmbCategory.SelectedIndex = 0;           // Default to the first item
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadItems()
        {
            // Load items from the database into the DataGridView
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ItCode AS [Item Code], ItName AS [Item Name], " +
                                   "ItCategory AS [Category Code], Price, Stock, ItManuf " +
                                   "FROM ItemTbl";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvItems.DataSource = dt; // Bind the DataTable to the DataGridView
                    }

                    dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;     
                    dgvItems.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading items: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Get values from input fields
            string itemName = txtItemName.Text.Trim();
            decimal price;
            int stock;
            string itmanuf = txtManufacturer.Text.Trim();

            // Validate input
            if (string.IsNullOrEmpty(itemName) || cmbCategory.SelectedValue == null ||
                string.IsNullOrEmpty(txtPrice.Text) || !decimal.TryParse(txtPrice.Text, out price) ||
                !int.TryParse(txtStock.Text, out stock))
            {
                MessageBox.Show("Please fill out all fields with valid data.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the selected category value
            int category = Convert.ToInt32(cmbCategory.SelectedValue);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO ItemTbl (ItName, ItCategory, Price, Stock, ItManuf) VALUES (@ItName, @CategoryCode, @Price, @Stock, @Manuf)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ItName", itemName);
                        cmd.Parameters.AddWithValue("@CategoryCode", category);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Stock", stock);
                        cmd.Parameters.AddWithValue("@Manuf", itmanuf);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Item added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadItems(); // Refresh items display
                            ClearInputs(); // Clear input fields
                        }
                        else
                        {
                            MessageBox.Show("Failed to add item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count > 0)
            {
                int itemCode = Convert.ToInt32(dgvItems.SelectedRows[0].Cells["Item Code"].Value);

                var confirmResult = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmResult == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            string query = "DELETE FROM ItemTbl WHERE ItCode = @ItCode";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ItCode", itemCode);
                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Item deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadItems();
                                }
                                else
                                {
                                    MessageBox.Show("Failed to delete item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error deleting item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearInputs()
        {
            // Do not clear ComboBox items, just reset the selected index.
            cmbCategory.SelectedIndex = 0; // Reset to "Add Category" or the first item
            txtItemName.Clear();
            txtPrice.Clear();
            txtStock.Clear();
            txtManufacturer.Clear();
            txtItemName.Focus();
        }

        private void stockbtn_Click(object sender, EventArgs e)
        {
            using (Stockprint reportViewer = new Stockprint())
            {
                reportViewer.WindowState = FormWindowState.Maximized;
                reportViewer.ShowDialog();
            }
        }
    }
}

