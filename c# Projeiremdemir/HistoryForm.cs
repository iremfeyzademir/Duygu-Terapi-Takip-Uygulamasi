using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using TerapiTakipUygulamasi.Models;

namespace TerapiTakipUygulamasi
{
    public partial class HistoryForm : Form
    {
        private readonly int _userId;
        private readonly DatabaseHelper _dbHelper;
        private readonly Panel _mainPanel;
        private readonly FlowLayoutPanel _entriesPanel;

        public HistoryForm(int userId)
        {
            _userId = userId;
            _dbHelper = new DatabaseHelper();

            this.Text = "Geçmiş Girişler";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(255, 245, 250);

            // Ana panel
            _mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(255, 245, 250)
            };

            // Başlık
            var titleLabel = new Label
            {
                Text = "Günlük Girişleriniz 📝",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(95, 99, 104),
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Girişler paneli
            _entriesPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(255, 245, 250)
            };

            _mainPanel.Controls.Add(_entriesPanel);
            _mainPanel.Controls.Add(titleLabel);
            this.Controls.Add(_mainPanel);

            LoadEntries();
        }

        private void LoadEntries()
        {
            try
            {
                var entries = _dbHelper.GetJournalEntries(_userId);
                DisplayEntries(entries);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Günlük girişleri yüklenirken bir hata oluştu: {ex.Message}",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void DisplayEntries(List<JournalEntry> entries)
        {
            _entriesPanel.Controls.Clear();

            if (entries.Count == 0)
            {
                var noEntriesLabel = new Label
                {
                    Text = "Henüz hiç günlük girişiniz bulunmuyor.\nYeni bir giriş eklemek için ana menüye dönün.",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.FromArgb(95, 99, 104),
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Padding = new Padding(10),
                    Margin = new Padding(0, 50, 0, 0)
                };
                _entriesPanel.Controls.Add(noEntriesLabel);
                return;
            }

            foreach (var entry in entries)
            {
                var entryPanel = CreateEntryPanel(entry);
                _entriesPanel.Controls.Add(entryPanel);
            }
        }

        private Panel CreateEntryPanel(JournalEntry entry)
        {
            var panel = new Panel
            {
                Width = _entriesPanel.Width - 40,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15),
                Padding = new Padding(15),
                BackColor = Color.FromArgb(250, 250, 255)
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(5),
                BackColor = Color.Transparent
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));

            // Tarih başlığı
            var dateHeader = new Label
            {
                Text = entry.Date.ToString("dd MMMM yyyy, dddd"),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(95, 99, 104),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(dateHeader, 0, 0);
            layout.SetColumnSpan(dateHeader, 2);

            // Saat
            AddDetailRow(layout, "Saat:", entry.Date.ToString("HH:mm"), 1);
            AddDetailRow(layout, "Duygu:", entry.Mood, 2);
            AddDetailRow(layout, "İçerik:", entry.Content, 3);
            AddDetailRow(layout, "Analiz:", entry.Analysis, 4);

            // Butonlar
            var buttonPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 10, 0, 0)
            };

            var editButton = new Button
            {
                Text = "Düzenle ✏️",
                BackColor = Color.FromArgb(213, 240, 247),
                ForeColor = Color.FromArgb(95, 99, 104),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0)
            };
            editButton.FlatAppearance.BorderSize = 0;
            editButton.Click += (s, e) => EditEntry(entry);

            var deleteButton = new Button
            {
                Text = "Sil 🗑️",
                BackColor = Color.FromArgb(255, 200, 200),
                ForeColor = Color.FromArgb(95, 99, 104),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            deleteButton.FlatAppearance.BorderSize = 0;
            deleteButton.Click += (s, e) => DeleteEntry(entry);

            buttonPanel.Controls.Add(editButton);
            buttonPanel.Controls.Add(deleteButton);
            layout.Controls.Add(buttonPanel, 1, 5);

            panel.Controls.Add(layout);
            return panel;
        }

        private void AddDetailRow(TableLayoutPanel layout, string label, string value, int row)
        {
            var lblTitle = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(95, 99, 104),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(95, 99, 104),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true
            };

            layout.Controls.Add(lblTitle, 0, row);
            layout.Controls.Add(lblValue, 1, row);
        }

        private void EditEntry(JournalEntry entry)
        {
            var editForm = new EditJournalEntryForm(entry);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadEntries();
            }
        }

        private void DeleteEntry(JournalEntry entry)
        {
            var result = MessageBox.Show(
                "Bu günlük girişini silmek istediğinizden emin misiniz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    _dbHelper.DeleteJournalEntry(entry.Id);
                    LoadEntries();
                    MessageBox.Show(
                        "Günlük girişi başarıyla silindi.",
                        "Başarılı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Günlük girişi silinirken bir hata oluştu: {ex.Message}",
                        "Hata",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
    }
} 