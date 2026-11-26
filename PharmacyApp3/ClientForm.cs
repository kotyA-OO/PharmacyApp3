using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PharmacyApp3
{
    public class ClientForm : Form
    {
        private DataGridView dataGridView;
        string connStr = "server=localhost;port=3306;user=root;password=1303;database=pharmacy_db;";

        public ClientForm()
        {
            this.Text = "Каталог лекарств (Клиент)";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Таблица только для чтения
            dataGridView = new DataGridView();
            dataGridView.Location = new Point(20, 20);
            dataGridView.Size = new Size(540, 300);
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.ReadOnly = true; // Клиент не может редактировать
            dataGridView.AllowUserToDeleteRows = false;
            this.Controls.Add(dataGridView);

            LoadCatalog();
        }

        private void LoadCatalog()
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    // Клиент видит только название, цену и наличие
                    string sql = "SELECT name AS 'Название', price AS 'Цена', quantity AS 'В наличии' FROM medicines";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки: " + ex.Message);
                }
            }
        }
    }
}