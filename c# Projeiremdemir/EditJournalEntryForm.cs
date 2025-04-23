using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using TerapiTakipUygulamasi.Models;

namespace TerapiTakipUygulamasi
{
    public class EditJournalEntryForm : Form
    {
        private readonly JournalEntry _entry;
        private readonly DatabaseHelper _dbHelper;
        private ComboBox? _cmbMood;
        private RichTextBox? _txtContent;
        private Button? _btnSave;
        private Label? _lblError;

        public EditJournalEntryForm(JournalEntry entry)
        {
            _entry = entry;
            _dbHelper = new DatabaseHelper();
            InitializeUI();
            LoadEntryData();
        }

        private void InitializeUI()
        {
            this.Text = "Günlük Girişini Düzenle";
            this.Size = new Size(600, 500);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(255, 245, 250);
            this.StartPosition = FormStartPosition.CenterScreen;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(255, 245, 250)
            };

            var lblTitle = new Label
            {
                Text = "Günlük Girişini Düzenle ✏️",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(95, 99, 104)
            };

            _lblError = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(235, 87, 87)
            };

            var moodPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 60
            };

            var lblMood = new Label
            {
                Text = "Duygu Durumu:",
                Font = new Font("Segoe UI", 11),
                Location = new Point(0, 5),
                AutoSize = true,
                ForeColor = Color.FromArgb(95, 99, 104)
            };

            _cmbMood = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(120, 2),
                Width = 200,
                BackColor = Color.FromArgb(250, 250, 255),
                ForeColor = Color.FromArgb(95, 99, 104)
            };
            _cmbMood.Items.AddRange(new string[] { "Çok İyi", "İyi", "Normal", "Kötü", "Çok Kötü" });

            moodPanel.Controls.Add(lblMood);
            moodPanel.Controls.Add(_cmbMood);

            _txtContent = new RichTextBox
            {
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(250, 250, 255),
                ForeColor = Color.FromArgb(95, 99, 104),
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };

            _btnSave = new Button
            {
                Text = "Değişiklikleri Kaydet 💾",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                BackColor = Color.FromArgb(213, 240, 247),
                ForeColor = Color.FromArgb(95, 99, 104),
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Bottom,
                Height = 40,
                Cursor = Cursors.Hand
            };
            _btnSave.FlatAppearance.BorderSize = 0;
            _btnSave.Click += BtnSave_Click;

            mainPanel.Controls.Add(lblTitle, 0, 0);
            mainPanel.Controls.Add(_lblError, 0, 1);
            mainPanel.Controls.Add(moodPanel, 0, 2);
            mainPanel.Controls.Add(_txtContent, 0, 3);

            this.Controls.Add(mainPanel);
            this.Controls.Add(_btnSave);
        }

        private void LoadEntryData()
        {
            if (_cmbMood == null || _txtContent == null) return;

            _cmbMood.Text = _entry.Mood;
            _txtContent.Text = _entry.Content;
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
                _entry.Mood = _cmbMood.Text;
                _entry.Content = _txtContent.Text;
                _entry.Analysis = AnalyzeSentiment(_txtContent.Text);
                _entry.Suggestions = GenerateSuggestions(_cmbMood.Text, _entry.Analysis);

                _dbHelper.UpdateJournalEntry(_entry);
                
                MessageBox.Show("Günlük girişi başarıyla güncellendi. 🌟", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                _lblError.Text = $"Günlük girişi güncellenirken bir hata oluştu: {ex.Message}";
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
    }
} 