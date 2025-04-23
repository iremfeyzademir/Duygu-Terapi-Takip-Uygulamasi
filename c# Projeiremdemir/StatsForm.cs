using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;
using TerapiTakipUygulamasi.Models;

namespace TerapiTakipUygulamasi
{
    public class StatsForm : Form
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly int _userId;
        private Chart moodChart;
        private Chart weeklyTrendChart;
        private Label summaryLabel;
        private Panel summaryPanel;

        public StatsForm(int userId)
        {
            _userId = userId;
            _dbHelper = new DatabaseHelper();
            InitializeUI();
            this.Load += StatsForm_Load;
        }

        private void InitializeUI()
        {
            this.Text = "İstatistikler";
            this.Size = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Özet paneli
            summaryPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(255, 242, 245),
                Padding = new Padding(10)
            };

            summaryLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "İstatistikler yükleniyor..."
            };

            summaryPanel.Controls.Add(summaryLabel);

            // Duygu dağılımı grafiği
            moodChart = new Chart
            {
                Dock = DockStyle.Top,
                Height = 250,
                BackColor = Color.White
            };

            var moodChartArea = new ChartArea();
            moodChart.ChartAreas.Add(moodChartArea);
            moodChartArea.BackColor = Color.White;

            var moodLegend = new Legend();
            moodChart.Legends.Add(moodLegend);

            var moodSeries = new Series("Duygular")
            {
                ChartType = SeriesChartType.Pie,
                BorderWidth = 1,
                BorderColor = Color.White
            };
            moodChart.Series.Add(moodSeries);

            var moodTitle = new Title("Son 30 Gün Duygu Dağılımı")
            {
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            moodChart.Titles.Add(moodTitle);

            // Haftalık trend grafiği
            weeklyTrendChart = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            var trendChartArea = new ChartArea();
            weeklyTrendChart.ChartAreas.Add(trendChartArea);
            trendChartArea.BackColor = Color.White;
            trendChartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            trendChartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            trendChartArea.AxisY.Minimum = 1;
            trendChartArea.AxisY.Maximum = 5;

            var trendLegend = new Legend();
            weeklyTrendChart.Legends.Add(trendLegend);

            var trendSeries = new Series("Haftalık Trend")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2
            };
            weeklyTrendChart.Series.Add(trendSeries);

            var trendTitle = new Title("Haftalık Duygu Trendi")
            {
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            weeklyTrendChart.Titles.Add(trendTitle);

            // Kontrolleri forma ekle
            this.Controls.Add(weeklyTrendChart);
            this.Controls.Add(moodChart);
            this.Controls.Add(summaryPanel);
        }

        private void StatsForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadMoodStatistics();
                LoadWeeklyTrend();
                UpdateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show("İstatistikler yüklenirken bir hata oluştu: " + ex.Message, 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadMoodStatistics()
        {
            try
            {
                var moodStats = _dbHelper.GetMoodStatistics(_userId);
                moodChart.Series["Duygular"].Points.Clear();

                if (moodStats.Count == 0)
                {
                    var point = moodChart.Series["Duygular"].Points.Add(100);
                    point.LegendText = "Veri Yok";
                    point.Color = Color.LightGray;
                    return;
                }

                foreach (var stat in moodStats)
                {
                    var point = moodChart.Series["Duygular"].Points.Add(stat.Percentage);
                    point.LegendText = stat.Mood;
                    point.Color = GetMoodColor(stat.Mood);
                    point.Label = $"{stat.Percentage:0.0}%";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Duygu istatistikleri yüklenirken bir hata oluştu: {ex.Message}", 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadWeeklyTrend()
        {
            try
            {
                var weeklyTrend = _dbHelper.GetWeeklyMoodTrend(_userId);
                weeklyTrendChart.Series["Haftalık Trend"].Points.Clear();

                if (weeklyTrend.Count == 0)
                {
                    var point = weeklyTrendChart.Series["Haftalık Trend"].Points.Add(3);
                    point.AxisLabel = "Veri Yok";
                    return;
                }

                foreach (var trend in weeklyTrend)
                {
                    var point = weeklyTrendChart.Series["Haftalık Trend"].Points.Add(trend.AverageSentiment);
                    point.AxisLabel = trend.Date.ToString("dd/MM");
                    point.Color = GetMoodColor(trend.Mood);
                    point.ToolTip = trend.Mood;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Haftalık trend yüklenirken bir hata oluştu: {ex.Message}", 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSummary()
        {
            try
            {
                var stats = _dbHelper.GetEntryStatistics(_userId);
                summaryLabel.Text = $"Son 30 günde toplam {stats.TotalEntries} günlük girişi yapıldı.\n" +
                                   $"Günlük ortalama {stats.AveragePerDay:0.0} giriş yapıldı.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Özet bilgiler yüklenirken bir hata oluştu: {ex.Message}", 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Color GetMoodColor(string mood)
        {
            return mood.ToLower() switch
            {
                "çok iyi" => Color.FromArgb(92, 184, 92),    // Yeşil
                "iyi" => Color.FromArgb(91, 192, 222),       // Açık Mavi
                "normal" => Color.FromArgb(240, 173, 78),    // Turuncu
                "kötü" => Color.FromArgb(217, 83, 79),       // Kırmızı
                "çok kötü" => Color.FromArgb(153, 84, 187),  // Mor
                _ => Color.LightGray
            };
        }
    }
} 