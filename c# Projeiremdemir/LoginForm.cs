using System;
using System.Drawing;
using System.Windows.Forms;

namespace TerapiTakipUygulamasi
{
    public partial class LoginForm : Form
    {
        private readonly DatabaseHelper _dbHelper;
        private TextBox? _txtUsername;
        private TextBox? _txtPassword;
        private Button? _btnLogin;
        private Button? _btnRegister;

        public LoginForm()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Giriş Yap";
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(255, 245, 250);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Ana panel
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(255, 245, 250)
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            // Başlık
            var lblTitle = new Label
            {
                Text = "Terapi Takip Uygulaması",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(95, 99, 104)
            };

            // Kullanıcı adı
            var usernamePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 250, 255),
                Padding = new Padding(5)
            };

            var lblUsername = new Label
            {
                Text = "Kullanıcı Adı:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 10),
                AutoSize = true,
                ForeColor = Color.FromArgb(95, 99, 104)
            };

            _txtUsername = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(120, 7),
                Size = new Size(200, 30),
                BackColor = Color.FromArgb(250, 250, 255),
                ForeColor = Color.FromArgb(95, 99, 104)
            };

            usernamePanel.Controls.AddRange(new Control[] { lblUsername, _txtUsername });

            // Şifre
            var passwordPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 250, 255),
                Padding = new Padding(5)
            };

            var lblPassword = new Label
            {
                Text = "Şifre:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 10),
                AutoSize = true,
                ForeColor = Color.FromArgb(95, 99, 104)
            };

            _txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(120, 7),
                Size = new Size(200, 30),
                PasswordChar = '*',
                BackColor = Color.FromArgb(250, 250, 255),
                ForeColor = Color.FromArgb(95, 99, 104)
            };

            passwordPanel.Controls.AddRange(new Control[] { lblPassword, _txtPassword });

            // Butonlar
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(255, 245, 250)
            };

            _btnLogin = new Button
            {
                Text = "Giriş Yap",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(213, 240, 247), // Pastel mavi
                ForeColor = Color.FromArgb(95, 99, 104),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(10, 10),
                Size = new Size(170, 35)
            };
            _btnLogin.Click += BtnLogin_Click;

            _btnRegister = new Button
            {
                Text = "Kayıt Ol",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 237, 200), // Pastel yeşil
                ForeColor = Color.FromArgb(95, 99, 104),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(190, 10),
                Size = new Size(170, 35)
            };
            _btnRegister.Click += BtnRegister_Click;

            buttonPanel.Controls.AddRange(new Control[] { _btnLogin, _btnRegister });

            // Ana panel düzeni
            mainPanel.Controls.Add(lblTitle, 0, 0);
            mainPanel.Controls.Add(usernamePanel, 0, 1);
            mainPanel.Controls.Add(passwordPanel, 0, 2);
            mainPanel.Controls.Add(buttonPanel, 0, 3);

            this.Controls.Add(mainPanel);
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            if (_txtUsername == null || _txtPassword == null) return;

            if (string.IsNullOrWhiteSpace(_txtUsername.Text))
            {
                MessageBox.Show("Lütfen kullanıcı adını girin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_txtPassword.Text))
            {
                MessageBox.Show("Lütfen şifreyi girin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_dbHelper.ValidateUser(_txtUsername.Text, _txtPassword.Text))
                {
                    var userId = _dbHelper.GetUserId(_txtUsername.Text);
                    if (userId != -1)
                    {
                        this.Hide();
                        var mainForm = new MainForm(userId);
                        mainForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı ID'si alınamadı.", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı veya şifre hatalı.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Giriş sırasında bir hata oluştu: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            var registerForm = new RegisterForm();
            registerForm.ShowDialog();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }
    }
} 