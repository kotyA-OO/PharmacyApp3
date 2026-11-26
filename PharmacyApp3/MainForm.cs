using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PharmacyApp3
{
    public class MainForm : Form
    {
        private DataGridView dataGridView;
        private TextBox txtName;
        private TextBox txtPrice;
        private TextBox txtQty;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnRefresh;

        string connStr = "server=localhost;port=3306;user=root;password=1303;database=pharmacy_db;";
        string userRole;

        public MainForm(string role)
        {
            userRole = role;
            this.Text = "Аптека Здоровье+";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Таблица
            dataGridView = new DataGridView();
            dataGridView.Location = new Point(20, 20);
            dataGridView.Size = new Size(640, 250);
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.Controls.Add(dataGridView);

            // Поля
            this.Controls.Add(new Label() { Text = "Название:", Location = new Point(20, 290) });
            txtName = new TextBox() { Location = new Point(100, 290), Width = 150 };
            this.Controls.Add(txtName);

            this.Controls.Add(new Label() { Text = "Цена:", Location = new Point(270, 290) });
            txtPrice = new TextBox() { Location = new Point(320, 290), Width = 80 };
            this.Controls.Add(txtPrice);

            this.Controls.Add(new Label() { Text = "Кол-во:", Location = new Point(420, 290) });
            txtQty = new TextBox() { Location = new Point(480, 290), Width = 80 };
            this.Controls.Add(txtQty);

            // Кнопки
            btnAdd = new Button() { Text = "Добавить", Location = new Point(20, 340), Width = 120 };
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            btnDelete = new Button() { Text = "Удалить выбранное", Location = new Point(160, 340), Width = 150 };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            btnRefresh = new Button() { Text = "Обновить", Location = new Point(330, 340), Width = 120 };
            btnRefresh.Click += BtnRefresh_Click;
            this.Controls.Add(btnRefresh);

            // Настройка прав
            if (userRole != "admin")
            {
                btnAdd.Enabled = false;
                btnDelete.Enabled = false;
                btnRefresh.Text = "Сохранить кол-во";
                // Поля ввода блокируем, чтобы провизор не добавил новое лекарство
                txtName.Enabled = false;
                txtPrice.Enabled = false;
            }

            this.Load += (s, e) => LoadMedicines();
        }

        private void LoadMedicines()
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT id, name AS 'Название', price AS 'Цена', quantity AS 'Количество' FROM medicines";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView.DataSource = table;
                    if (dataGridView.Columns["id"] != null) dataGridView.Columns["id"].Visible = false;
                }
                catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Заполните название и цену!");
                return;
            }
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = "INSERT INTO medicines (name, price, quantity) VALUES (@n, @p, @q)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@n", txtName.Text);
                    cmd.Parameters.AddWithValue("@p", Convert.ToDecimal(txtPrice.Text));
                    cmd.Parameters.AddWithValue("@q", Convert.ToInt32(txtQty.Text));
                    cmd.ExecuteNonQuery();
                    LoadMedicines();
                    txtName.Clear(); txtPrice.Clear(); txtQty.Clear();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка добавления: " + ex.Message); }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null) return;
            int id = Convert.ToInt32(dataGridView.CurrentRow.Cells["id"].Value);
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = "DELETE FROM medicines WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    LoadMedicines();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка удаления: " + ex.Message); }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            if (userRole == "admin")
            {
                LoadMedicines();
            }
            else
            {
                try
                {
                    if (dataGridView.CurrentRow == null) return;
                    int id = Convert.ToInt32(dataGridView.CurrentRow.Cells["id"].Value);
                    int newQty = Convert.ToInt32(dataGridView.CurrentRow.Cells["Количество"].Value);

                    using (MySqlConnection conn = new MySqlConnection(connStr))
                    {
                        conn.Open();
                        string sql = "UPDATE medicines SET quantity=@q WHERE id=@id";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@q", newQty);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Количество сохранено!");
                    LoadMedicines();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
            }
        }
    }
}