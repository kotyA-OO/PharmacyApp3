using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PharmacyApp3
{
    public partial class Form1 : Form
    {
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Button btnLogin;

        string connStr = "server=localhost;port=3306;user=root;password=1303;database=pharmacy_db;";

        public Form1()
        {
            this.Text = "Вход в систему";
            this.Size = new Size(350, 250);
            this.StartPosition = FormStartPosition.CenterScreen;

            txtLogin = new TextBox();
            txtLogin.Location = new Point(50, 50);
            txtLogin.Width = 200;
            this.Controls.Add(txtLogin);

            txtPassword = new TextBox();
            txtPassword.Location = new Point(50, 90);
            txtPassword.Width = 200;
            txtPassword.PasswordChar = '*';
            this.Controls.Add(txtPassword);

            btnLogin = new Button();
            btnLogin.Text = "Войти";
            btnLogin.Location = new Point(100, 140);
            btnLogin.Width = 100;
            btnLogin.Click += new EventHandler(btnLogin_Click);
            this.Controls.Add(btnLogin);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    // Ищем ТОЛЬКО по логину (пароль проверим через хеш)
                    string sql = "SELECT role, password, salt FROM users WHERE username=@u";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@u", txtLogin.Text);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string dbHash = reader["password"].ToString();
                            string dbSalt = reader["salt"].ToString();
                            string role = reader["role"].ToString();

                            // Хешируем введенный пароль с солью из базы
                            string inputHash = PasswordHasher.HashPassword(txtPassword.Text, dbSalt);

                            if (inputHash == dbHash)
                            {
                                MessageBox.Show("Добро пожаловать! Ваша роль: " + role);

                                if (role == "client")
                                {
                                    ClientForm client = new ClientForm();
                                    client.Show();
                                }
                                else
                                {
                                    MainForm main = new MainForm(role);
                                    main.Show();
                                }
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Неверный логин или пароль.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Пользователь не найден.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения: " + ex.Message);
                }
            }
        }
    }
}