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
    public partial class RegisterForm : Form
    {
        private string connectionString = @"Data Source=LAPTOP-672HOPO3;Initial Catalog=HardwareDb;Integrated Security=True";
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get input data from form
            string userName = txtUserName.Text.Trim();
            string password = txtPassword.Text.Trim();
            string contactNumber = txtContactNumber.Text.Trim();

            // Input validation
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(contactNumber))
            {
                MessageBox.Show("Please fill all the fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Insert data into the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO [User] (UserName, Password, Contact) VALUES (@Username, @password, @contactNumber)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", userName);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@contactNumber", contactNumber);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("User registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                Login login = new Login();
                login.Show();    // Close the registration form after saving
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Login login = new Login();
            login.Show();
        }
    }
}
    

