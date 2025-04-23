using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace TerapiTakipUygulamasi
{
    public partial class MainForm : Form
    {
        private readonly int _userId;
        private readonly DatabaseHelper _dbHelper;
        private Button? _btnNewEntry;
        private Button? _btnHistory;
        private Button? _btnStats;
        private Button? _btnLogout;
        private Label? _lblWelcome;
        private Panel? _mainPanel;

        public MainForm(int userId)
        {   
            InitializeComponent();
            _userId = userId;
            _dbHelper = new DatabaseHelper();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            this._btnNewEntry = new Button();
            this._btnHistory = new Button();
            this._btnStats = new Button();
            this._btnLogout = new Button();
            this._lblWelcome = new Label();
            this._mainPanel = new Panel();

            // Form
            this.Text = "Terapi Takip Uygulaması";
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(255, 245, 250);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Main Panel
            this._mainPanel.Dock = DockStyle.Fill;
            this._mainPanel.Padding = new Padding(20);
            this._mainPanel.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);

            // Welcome Label
            this._lblWelcome.Text = "Hoş Geldiniz";
            this._lblWelcome.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this._lblWelcome.AutoSize = true;
            this._lblWelcome.Location = new System.Drawing.Point(100, 50);
            this._lblWelcome.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);

            // New Entry Button
            this._btnNewEntry.Text = "Yeni Günlük Girişi";
            this._btnNewEntry.Size = new System.Drawing.Size(200, 50);
            this._btnNewEntry.Location = new System.Drawing.Point(100, 150);
            this._btnNewEntry.Font = new System.Drawing.Font("Segoe UI", 12F);
            this._btnNewEntry.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnNewEntry.ForeColor = System.Drawing.Color.White;
            this._btnNewEntry.FlatStyle = FlatStyle.Flat;
            this._btnNewEntry.Click += new EventHandler(BtnNewEntry_Click);

            // History Button
            this._btnHistory.Text = "Geçmiş Girişler";
            this._btnHistory.Size = new System.Drawing.Size(200, 50);
            this._btnHistory.Location = new System.Drawing.Point(100, 220);
            this._btnHistory.Font = new System.Drawing.Font("Segoe UI", 12F);
            this._btnHistory.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnHistory.ForeColor = System.Drawing.Color.White;
            this._btnHistory.FlatStyle = FlatStyle.Flat;
            this._btnHistory.Click += new EventHandler(BtnHistory_Click);

            // Stats Button
            this._btnStats.Text = "İstatistikler";
            this._btnStats.Size = new System.Drawing.Size(200, 50);
            this._btnStats.Location = new System.Drawing.Point(100, 290);
            this._btnStats.Font = new System.Drawing.Font("Segoe UI", 12F);
            this._btnStats.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnStats.ForeColor = System.Drawing.Color.White;
            this._btnStats.FlatStyle = FlatStyle.Flat;
            this._btnStats.Click += new EventHandler(BtnStats_Click);

            // Logout Button
            this._btnLogout.Text = "Çıkış Yap";
            this._btnLogout.Size = new System.Drawing.Size(200, 50);
            this._btnLogout.Location = new System.Drawing.Point(100, 360);
            this._btnLogout.Font = new System.Drawing.Font("Segoe UI", 12F);
            this._btnLogout.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this._btnLogout.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this._btnLogout.FlatStyle = FlatStyle.Flat;
            this._btnLogout.Click += new EventHandler(BtnLogout_Click);

            // Add controls to panel
            this._mainPanel.Controls.Add(this._lblWelcome);
            this._mainPanel.Controls.Add(this._btnNewEntry);
            this._mainPanel.Controls.Add(this._btnHistory);
            this._mainPanel.Controls.Add(this._btnStats);
            this._mainPanel.Controls.Add(this._btnLogout);

            // Add panel to form
            this.Controls.Add(this._mainPanel);
        }

        private void InitializeUI()
        {
            this.Text = "Terapi Takip Uygulaması";
            this.Size = new Size(400, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(255, 245, 250);
            this.StartPosition = FormStartPosition.CenterScreen;

            _mainPanel = new Panel
            {
                Dock = DockStyle.None,
                Size = new Size(350, 450),
                Padding = new Padding(10),
                BackColor = Color.FromArgb(255, 245, 250),
                Location = new Point(10, 50)
            };

            // Hoşgeldin mesajı
            _lblWelcome = new Label
            {
                Text = "Hoş Geldiniz!",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60,
                ForeColor = Color.FromArgb(95, 99, 104)
            };

            // Yeni Giriş Butonu
            _btnNewEntry = new Button
            {
                Text = "Yeni Günlük Girişi 📝",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                BackColor = Color.FromArgb(213, 240, 247),
                ForeColor = Color.FromArgb(95, 99, 104),
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Top,
                Height = 45,
                Margin = new Padding(0, 20, 0, 10),
                Cursor = Cursors.Hand
            };
            _btnNewEntry.FlatAppearance.BorderSize = 0;
            _btnNewEntry.Click += BtnNewEntry_Click;

            // Geçmiş Butonu
            _btnHistory = new Button
            {
                Text = "Geçmiş Girişler 📚",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                BackColor = Color.FromArgb(253, 235, 208),
                ForeColor = Color.FromArgb(95, 99, 104),
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Top,
                Height = 45,
                Margin = new Padding(0, 10, 0, 10),
                Cursor = Cursors.Hand
            };
            _btnHistory.FlatAppearance.BorderSize = 0;
            _btnHistory.Click += BtnHistory_Click;

            // İstatistik Butonu
            _btnStats = new Button
            {
                Text = "İstatistikler 📊",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                BackColor = Color.FromArgb(220, 237, 200),
                ForeColor = Color.FromArgb(95, 99, 104),
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Top,
                Height = 45,
                Margin = new Padding(0, 10, 0, 10),
                Cursor = Cursors.Hand
            };
            _btnStats.FlatAppearance.BorderSize = 0;
            _btnStats.Click += BtnStats_Click;

            // Çıkış Butonu
            _btnLogout = new Button
            {
                Text = "Çıkış Yap 👋",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                BackColor = Color.FromArgb(255, 223, 235),
                ForeColor = Color.FromArgb(95, 99, 104),
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Bottom,
                Height = 35,
                Margin = new Padding(0, 20, 0, 0),
                Cursor = Cursors.Hand
            };
            _btnLogout.FlatAppearance.BorderSize = 0;
            _btnLogout.Click += BtnLogout_Click;

            // Kontrolleri panele ekle
            _mainPanel.Controls.Add(_lblWelcome);
            _mainPanel.Controls.Add(_btnNewEntry);
            _mainPanel.Controls.Add(_btnHistory);
            _mainPanel.Controls.Add(_btnStats);
            _mainPanel.Controls.Add(_btnLogout);

            this.Controls.Add(_mainPanel);
        }

        private void BtnNewEntry_Click(object? sender, EventArgs e)
        {
            try
            {
                var journalEntryForm = new JournalEntryForm(_userId);
                journalEntryForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Yeni giriş formu açılırken bir hata oluştu: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnHistory_Click(object? sender, EventArgs e)
        {
            try
            {
                var historyForm = new HistoryForm(_userId);
                historyForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Geçmiş formu açılırken bir hata oluştu: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnStats_Click(object? sender, EventArgs e)
        {
            try
            {
                var statsForm = new StatsForm(_userId);
                statsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İstatistik formu açılırken bir hata oluştu: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Çıkış yapılırken bir hata oluştu: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var loginForm = new LoginForm();
                loginForm.Show();
            }
        }
    }
}