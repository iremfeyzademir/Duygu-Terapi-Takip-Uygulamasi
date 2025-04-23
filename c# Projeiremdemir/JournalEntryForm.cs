using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using TerapiTakipUygulamasi.Models;

namespace TerapiTakipUygulamasi
{
    public partial class JournalEntryForm : Form
    {
        private readonly int _userId;
        private readonly DatabaseHelper _dbHelper;
        private ComboBox? _cmbMood;
        private RichTextBox? _txtContent;
        private RichTextBox? _txtSuggestions;
        private Button? _btnSave;
        private Label? _lblTitle;
        private Label? _lblError;
        private Panel? _mainPanel;

        public JournalEntryForm(int userId)
        {
            _userId = userId;
            _dbHelper = new DatabaseHelper();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Yeni Günlük Girişi";
            this.Size = new Size(800, 700);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(255, 245, 250);
            this.StartPosition = FormStartPosition.CenterScreen;

            _mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(255, 245, 250)
            };

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(255, 245, 250)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80)); // Başlık
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // Hata mesajı
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70)); // Duygu seçimi
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50)); // İçerik
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30)); // Öneriler
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Kaydet butonu

            // Başlık
            _lblTitle = new Label
            {
                Text = "Bugün Nasıl Hissediyorsun? 💭",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(95, 99, 104)
            };

            // Hata mesajı
            _lblError = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(235, 87, 87)
            };

            // Duygu seçimi
            var moodPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(255, 245, 250)
            };

            var lblMood = new Label
            {
                Text = "Duygu Durumu:",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Location = new Point(0, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(95, 99, 104)
            };

            _cmbMood = new ComboBox
            {
                Font = new Font("Segoe UI", 12),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(150, 17),
                Width = 250,
                Height = 35,
                BackColor = Color.FromArgb(250, 250, 255),
                ForeColor = Color.FromArgb(95, 99, 104)
            };
            _cmbMood.Items.AddRange(new string[] { "Çok İyi", "İyi", "Normal", "Kötü", "Çok Kötü" });
            _cmbMood.SelectedIndex = 2;
            _cmbMood.SelectedIndexChanged += (s, e) => UpdateSuggestions();

            moodPanel.Controls.Add(lblMood);
            moodPanel.Controls.Add(_cmbMood);

            // İçerik
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 10),
                BackColor = Color.FromArgb(255, 245, 250)
            };

            var contentLabel = new Label
            {
                Text = "Günlük İçeriği:",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(95, 99, 104),
                Dock = DockStyle.Top,
                Height = 30
            };

            _txtContent = new RichTextBox
            {
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(250, 250, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                ForeColor = Color.FromArgb(95, 99, 104)
            };
            _txtContent.TextChanged += (s, e) => UpdateSuggestions();

            contentPanel.Controls.Add(_txtContent);
            contentPanel.Controls.Add(contentLabel);

            // Öneriler
            var suggestionsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 10),
                BackColor = Color.FromArgb(255, 245, 250)
            };

            _txtSuggestions = new RichTextBox
            {
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(250, 250, 255),
                ForeColor = Color.FromArgb(95, 99, 104),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill
            };

            suggestionsPanel.Controls.Add(_txtSuggestions);

            // Kaydet Butonu
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0),
                BackColor = Color.FromArgb(255, 245, 250)
            };

            _btnSave = new Button
            {
                Text = "Günlüğü Kaydet 💝",
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                BackColor = Color.FromArgb(213, 240, 247),
                ForeColor = Color.FromArgb(95, 99, 104),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 50),
                Location = new Point((buttonPanel.Width - 200) / 2, 0),
                Cursor = Cursors.Hand
            };
            _btnSave.FlatAppearance.BorderSize = 0;
            _btnSave.Click += BtnSave_Click;

            buttonPanel.Controls.Add(_btnSave);

            // Kontrolleri mainLayout'a ekle
            mainLayout.Controls.Add(_lblTitle, 0, 0);
            mainLayout.Controls.Add(_lblError, 0, 1);
            mainLayout.Controls.Add(moodPanel, 0, 2);
            mainLayout.Controls.Add(contentPanel, 0, 3);
            mainLayout.Controls.Add(suggestionsPanel, 0, 4);
            mainLayout.Controls.Add(buttonPanel, 0, 5);

            _mainPanel.Controls.Add(mainLayout);
            this.Controls.Add(_mainPanel);

            UpdateSuggestions();
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (_cmbMood == null || _txtContent == null || _lblError == null) return;

            _lblError.Text = "";

            if (string.IsNullOrWhiteSpace(_txtContent.Text))
            {
                _lblError.Text = "Lütfen günlük içeriğini giriniz.";
                return;
            }

            try
            {
                var entry = new JournalEntry
                {
                    UserId = _userId,
                    Date = DateTime.Now,
                    Mood = _cmbMood.Text,
                    Content = _txtContent.Text,
                    Analysis = AnalyzeSentiment(_txtContent.Text),
                    Suggestions = GenerateSuggestions(_cmbMood.Text, AnalyzeSentiment(_txtContent.Text))
                };

                if (_dbHelper.CreateJournalEntry(entry))
                {
                    MessageBox.Show("Günlük girişiniz başarıyla kaydedildi. 🌟", "Başarılı",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    _lblError.Text = "Günlük kaydedilirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                _lblError.Text = $"Günlük kaydedilirken bir hata oluştu: {ex.Message}";
            }
        }

        private string AnalyzeSentiment(string content)
        {
            var positiveWords = new[] { "mutlu", "iyi", "güzel", "harika", "mükemmel", "sevindim", "başarılı" };
            var negativeWords = new[] { "üzgün", "kötü", "kızgın", "sinir", "stres", "endişe", "kaygı" };

            var positiveCount = positiveWords.Count(word => content.ToLower().Contains(word));
            var negativeCount = negativeWords.Count(word => content.ToLower().Contains(word));

            if (positiveCount > negativeCount)
                return "Genel olarak olumlu bir duygu durumu tespit edildi.";
            else if (negativeCount > positiveCount)
                return "Genel olarak olumsuz bir duygu durumu tespit edildi.";
            else
                return "Nötr bir duygu durumu tespit edildi.";
        }

        private string GenerateSuggestions(string mood, string analysis)
        {
            var suggestions = new StringBuilder();

            switch (mood)
            {
                case "Çok İyi":
                    suggestions.AppendLine("Harika bir gün geçirmişsiniz! 🌟");
                    suggestions.AppendLine("Bu pozitif enerjinizi korumak için:");
                    suggestions.AppendLine("- Günlük rutininizi sürdürün");
                    suggestions.AppendLine("- Başkalarıyla paylaşın");
                    suggestions.AppendLine("- Anıları kaydedin");
                    break;

                case "İyi":
                    suggestions.AppendLine("Güzel bir gün geçirmişsiniz. 😊");
                    suggestions.AppendLine("Bu durumu sürdürmek için:");
                    suggestions.AppendLine("- Pozitif aktiviteler yapın");
                    suggestions.AppendLine("- Sosyal bağlantılarınızı güçlendirin");
                    suggestions.AppendLine("- Kendinize zaman ayırın");
                    break;

                case "Normal":
                    suggestions.AppendLine("Sıradan bir gün geçirmişsiniz. 😐");
                    suggestions.AppendLine("Daha iyi hissetmek için:");
                    suggestions.AppendLine("- Yeni aktiviteler deneyin");
                    suggestions.AppendLine("- Rutininizi değiştirin");
                    suggestions.AppendLine("- Hedefler belirleyin");
                    break;

                case "Kötü":
                    suggestions.AppendLine("Zor bir gün geçirmişsiniz. 😔");
                    suggestions.AppendLine("Kendinizi daha iyi hissetmek için:");
                    suggestions.AppendLine("- Dinlenin ve rahatlayın");
                    suggestions.AppendLine("- Sevdiğiniz şeyleri yapın");
                    suggestions.AppendLine("- Desteğe ihtiyaç duyuyorsanız yardım isteyin");
                    break;

                case "Çok Kötü":
                    suggestions.AppendLine("Çok zor bir gün geçirmişsiniz. 😢");
                    suggestions.AppendLine("Yardım almanız önerilir:");
                    suggestions.AppendLine("- Profesyonel destek alın");
                    suggestions.AppendLine("- Güvendiğiniz kişilerle konuşun");
                    suggestions.AppendLine("- Acil durumda yardım hatlarını arayın");
                    break;
            }

            if (analysis.Contains("olumlu"))
            {
                suggestions.AppendLine("\nOlumlu duygularınızı sürdürmek için bu önerileri değerlendirin. 🌈");
            }
            else if (analysis.Contains("olumsuz"))
            {
                suggestions.AppendLine("\nOlumsuz duygularınızla başa çıkmak için bu önerileri değerlendirin. 💪");
            }

            return suggestions.ToString();
        }

        private void UpdateSuggestions()
        {
            if (_txtContent == null || _txtSuggestions == null || _cmbMood == null) return;

            string content = _txtContent.Text;
            string mood = _cmbMood.Text;

            var analysis = AnalyzeSentiment(content);
            var suggestions = GenerateSuggestions(mood, analysis);

            _txtSuggestions.Text = $"Analiz:\n{analysis}\n\nÖneriler:\n{suggestions}";
        }
    }
} 