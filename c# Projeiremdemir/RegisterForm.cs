using System;
using System.Drawing;
using System.Windows.Forms;

namespace TerapiTakipUygulamasi
{
    public partial class RegisterForm : Form
    {
        private readonly DatabaseHelper _dbHelper;
        private TextBox? _txtUsername;
        private TextBox? _txtPassword;
        private TextBox? _txtConfirmPassword;
        private Button? _btnRegister;
        private Button? _btnBack;

        public RegisterForm()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Kayıt Ol";
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(255, 242, 245);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Ana panel
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(255, 242, 245)
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            // Başlık
            var lblTitle = new Label
            {
                Text = "Yeni Hesap Oluştur",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            // Kullanıcı adı
            var usernamePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(5)
            };

            var lblUsername = new Label
            {
                Text = "Kullanıcı Adı:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 10),
                AutoSize = true
            };

            _txtUsername = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(120, 7),
                Size = new Size(200, 30)
            };

            usernamePanel.Controls.AddRange(new Control[] { lblUsername, _txtUsername });

            // Şifre
            var passwordPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(5)
            };

            var lblPassword = new Label
            {
                Text = "Şifre:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 10),
                AutoSize = true
            };

            _txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(120, 7),
                Size = new Size(200, 30),
                PasswordChar = '*'
            };

            passwordPanel.Controls.AddRange(new Control[] { lblPassword, _txtPassword });

            // Şifre tekrar
            var confirmPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(5)
            };

            var lblConfirm = new Label
            {
                Text = "Şifre Tekrar:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 10),
                AutoSize = true
            };

            _txtConfirmPassword = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(120, 7),
                Size = new Size(200, 30),
                PasswordChar = '*'
            };

            confirmPanel.Controls.AddRange(new Control[] { lblConfirm, _txtConfirmPassword });

            // Butonlar
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(255, 242, 245)
            };

            _btnRegister = new Button
            {
                Text = "Kayıt Ol",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(10, 10),
                Size = new Size(150, 30)
            };
            _btnRegister.Click += BtnRegister_Click;

            _btnBack = new Button
            {
                Text = "Geri",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(170, 10),
                Size = new Size(150, 30)
            };
            _btnBack.Click += BtnBack_Click;

            buttonPanel.Controls.AddRange(new Control[] { _btnRegister, _btnBack });

            // Ana panel düzeni
            mainPanel.Controls.Add(lblTitle, 0, 0);
            mainPanel.Controls.Add(usernamePanel, 0, 1);
            mainPanel.Controls.Add(passwordPanel, 0, 2);
            mainPanel.Controls.Add(confirmPanel, 0, 3);
            mainPanel.Controls.Add(buttonPanel, 0, 4);

            this.Controls.Add(mainPanel);
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            if (_txtUsername == null || _txtPassword == null || _txtConfirmPassword == null) return;

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

            if (_txtPassword.Text != _txtConfirmPassword.Text)
            {
                MessageBox.Show("Şifreler eşleşmiyor.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Şifre en az 6 karakter olmalıdır.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _dbHelper.RegisterUser(_txtUsername.Text, _txtPassword.Text);
                MessageBox.Show("Kayıt başarılı! Giriş yapabilirsiniz.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt sırasında bir hata oluştu: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBack_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
} 