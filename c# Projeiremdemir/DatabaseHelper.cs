using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using TerapiTakipUygulamasi.Models;

namespace TerapiTakipUygulamasi
{
    public class DatabaseHelper : IDisposable
    {
        private readonly string _dbPath;
        private readonly string _connectionString;
        private SQLiteConnection? _connection;
        private bool _disposed = false;

        public DatabaseHelper()
        {
            _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "terapi.db");
            _connectionString = $"Data Source={_dbPath};Version=3;";
            InitializeConnection();
        }

        private void InitializeConnection()
        {
            try
            {
                if (_connection == null || _connection.State != ConnectionState.Open)
                {
                    _connection = new SQLiteConnection(_connectionString);
                    _connection.Open();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Veritabanı bağlantısı başlatılırken hata oluştu: {ex.Message}");
            }
        }

        public void InitializeDatabase()
        {
            try
            {
                if (!File.Exists(_dbPath))
                {
                    SQLiteConnection.CreateFile(_dbPath);
                }

                // Users tablosu
                using var command = new SQLiteCommand(
                    @"CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        Password TEXT NOT NULL
                    )", _connection);
                command.ExecuteNonQuery();

                // Journal tablosu
                command.CommandText = @"CREATE TABLE IF NOT EXISTS Journal (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Date DATETIME NOT NULL,
                    Mood TEXT NOT NULL,
                    Content TEXT NOT NULL,
                    Sentiment REAL,
                    Analysis TEXT,
                    Suggestions TEXT,
                    FOREIGN KEY(UserId) REFERENCES Users(Id)
                )";
                command.ExecuteNonQuery();

                // Admin kullanıcısını kontrol et ve yoksa ekle
                command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
                var adminExists = Convert.ToInt32(command.ExecuteScalar()) > 0;

                if (!adminExists)
                {
                    command.CommandText = "INSERT INTO Users (Username, Password) VALUES ('admin', 'admin123')";
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Veritabanı tabloları oluşturulurken hata oluştu: {ex.Message}");
            }
        }

        public bool ValidateUser(string username, string password)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                using var cmd = new SQLiteCommand(
                    "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password",
                    connection);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcı doğrulama hatası: {ex.Message}");
            }
        }

        public void RegisterUser(string username, string password)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                using var cmd = new SQLiteCommand(
                    "INSERT INTO Users (Username, Password) VALUES (@username, @password)",
                    connection);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex) when (ex.ResultCode == SQLiteErrorCode.Constraint)
            {
                throw new Exception("Bu kullanıcı adı zaten kullanılıyor.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcı kaydı hatası: {ex.Message}");
            }
        }

        public int GetUserId(string username)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                using var cmd = new SQLiteCommand(
                    "SELECT Id FROM Users WHERE Username = @username",
                    connection);
                cmd.Parameters.AddWithValue("@username", username);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcı ID'si alınırken hata oluştu: {ex.Message}");
            }
        }

        public void AddJournalEntry(int userId, string mood, string content, string analysis, string suggestions)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                using var command = new SQLiteCommand(
                    @"INSERT INTO Journal (UserId, Date, Mood, Content, Sentiment, Analysis, Suggestions) 
                      VALUES (@UserId, @Date, @Mood, @Content, @Sentiment, @Analysis, @Suggestions)", connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Date", DateTime.Now);
                command.Parameters.AddWithValue("@Mood", mood);
                command.Parameters.AddWithValue("@Content", content);
                command.Parameters.AddWithValue("@Sentiment", CalculateSentiment(content));
                command.Parameters.AddWithValue("@Analysis", analysis);
                command.Parameters.AddWithValue("@Suggestions", suggestions);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Günlük girişi eklenirken bir hata oluştu: " + ex.Message);
            }
        }

        private double CalculateSentiment(string content)
        {
            var positiveWords = new[] { "mutlu", "iyi", "güzel", "harika", "mükemmel", "sevindim", "başarılı" };
            var negativeWords = new[] { "üzgün", "kötü", "kızgın", "sinir", "stres", "endişe", "kaygı" };

            content = content.ToLower();
            var positiveCount = positiveWords.Count(word => content.Contains(word));
            var negativeCount = negativeWords.Count(word => content.Contains(word));

            if (positiveCount == 0 && negativeCount == 0) return 0;
            return (positiveCount - negativeCount) / (double)(positiveCount + negativeCount);
        }

        public List<JournalEntry> GetJournalEntries(int userId)
        {
            var entries = new List<JournalEntry>();

            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                using var command = new SQLiteCommand(
                    @"SELECT Id, UserId, Date, Mood, Content, Sentiment, Analysis, Suggestions 
                      FROM Journal 
                      WHERE UserId = @UserId 
                      ORDER BY Date DESC", connection);

                command.Parameters.AddWithValue("@UserId", userId);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    entries.Add(new JournalEntry
                    {
                        Id = reader.GetInt32(0),
                        UserId = reader.GetInt32(1),
                        Date = reader.GetDateTime(2),
                        Mood = reader.GetString(3),
                        Content = reader.GetString(4),
                        Sentiment = reader.GetDouble(5),
                        Analysis = reader.GetString(6),
                        Suggestions = reader.GetString(7)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Günlük girişleri alınırken bir hata oluştu: " + ex.Message);
            }

            return entries;
        }

        public List<MoodStatistic> GetMoodStatistics(int userId)
        {
            var statistics = new List<MoodStatistic>();
            try
            {
                if (_connection == null || _connection.State != ConnectionState.Open)
                {
                    InitializeConnection();
                }

                string query = @"
                    SELECT 
                        Mood,
                        COUNT(*) as Count,
                        (COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Journal WHERE UserId = @UserId AND Date >= date('now', '-30 days'))) as Percentage
                    FROM Journal
                    WHERE UserId = @UserId AND Date >= date('now', '-30 days')
                    GROUP BY Mood";

                using (var cmd = new SQLiteCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            statistics.Add(new MoodStatistic
                            {
                                Mood = reader["Mood"].ToString() ?? string.Empty,
                                Count = Convert.ToInt32(reader["Count"]),
                                Percentage = Convert.ToDouble(reader["Percentage"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Duygu istatistikleri alınırken hata oluştu: {ex.Message}", ex);
            }
            return statistics;
        }

        public List<WeeklyTrend> GetWeeklyMoodTrend(int userId)
        {
            var trends = new List<WeeklyTrend>();
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                using var command = new SQLiteCommand(
                    @"SELECT 
                        date(Date) as EntryDate,
                        AVG(Sentiment) as AvgSentiment,
                        CASE 
                            WHEN AVG(Sentiment) >= 4.5 THEN 'Çok İyi'
                            WHEN AVG(Sentiment) >= 3.5 THEN 'İyi'
                            WHEN AVG(Sentiment) >= 2.5 THEN 'Normal'
                            WHEN AVG(Sentiment) >= 1.5 THEN 'Kötü'
                            ELSE 'Çok Kötü'
                        END as Mood
                    FROM Journal
                    WHERE UserId = @UserId AND Date >= date('now', '-30 days')
                    GROUP BY date(Date)
                    ORDER BY EntryDate DESC", connection);

                command.Parameters.AddWithValue("@UserId", userId);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    trends.Add(new WeeklyTrend
                    {
                        Date = reader.GetDateTime(0),
                        AverageSentiment = reader.GetDouble(1),
                        Mood = reader.GetString(2)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Haftalık duygu trendi alınırken bir hata oluştu: " + ex.Message);
            }
            return trends;
        }

        public EntryStatistics GetEntryStatistics(int userId)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(
                        "SELECT COUNT(*) as Total, " +
                        "CAST(COUNT(*) AS REAL) / 30.0 as Average " +
                        "FROM Journal " +
                        "WHERE UserId = @UserId AND " +
                        "Date >= date('now', '-30 days')", connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new EntryStatistics
                                {
                                    TotalEntries = reader.GetInt32(0),
                                    AveragePerDay = reader.GetDouble(1)
                                };
                            }
                        }
                    }
                }
                return new EntryStatistics { TotalEntries = 0, AveragePerDay = 0 };
            }
            catch (Exception ex)
            {
                throw new Exception("İstatistikler alınırken bir hata oluştu: " + ex.Message);
            }
        }

        public Dictionary<string, int> GetTodayGlobalMood()
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                using var cmd = new SQLiteCommand(@"
                    SELECT Mood, COUNT(*) as Count
                    FROM Journal
                    WHERE Date = date('now')
                    GROUP BY Mood
                    ORDER BY Count DESC",
                    connection);

                var moodCounts = new Dictionary<string, int>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    moodCounts.Add(reader.GetString(0), reader.GetInt32(1));
                }

                return moodCounts;
            }
            catch (Exception ex)
            {
                throw new Exception("Günlük duygu durumu alınırken bir hata oluştu: " + ex.Message);
            }
        }

        public void DeleteJournalEntry(int entryId)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                using var command = new SQLiteCommand(
                    "DELETE FROM Journal WHERE Id = @Id",
                    connection);
                command.Parameters.AddWithValue("@Id", entryId);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Günlük girişi silinirken hata oluştu: {ex.Message}");
            }
        }

        public void UpdateJournalEntry(JournalEntry entry)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                using var command = new SQLiteCommand(
                    @"UPDATE Journal 
                      SET Mood = @Mood, 
                          Content = @Content, 
                          Sentiment = @Sentiment,
                          Analysis = @Analysis,
                          Suggestions = @Suggestions
                      WHERE Id = @Id",
                    connection);

                command.Parameters.AddWithValue("@Id", entry.Id);
                command.Parameters.AddWithValue("@Mood", entry.Mood);
                command.Parameters.AddWithValue("@Content", entry.Content);
                command.Parameters.AddWithValue("@Sentiment", CalculateSentiment(entry.Content));
                command.Parameters.AddWithValue("@Analysis", entry.Analysis);
                command.Parameters.AddWithValue("@Suggestions", entry.Suggestions);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Günlük girişi güncellenirken hata oluştu: {ex.Message}");
            }
        }

        public bool CreateJournalEntry(JournalEntry entry)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                string insertSql = @"
                    INSERT INTO Journal (UserId, Date, Mood, Content, Sentiment, Analysis, Suggestions)
                    VALUES (@UserId, @Date, @Mood, @Content, @Sentiment, @Analysis, @Suggestions)";

                using var command = new SQLiteCommand(insertSql, connection);
                command.Parameters.AddWithValue("@UserId", entry.UserId);
                command.Parameters.AddWithValue("@Date", entry.Date);
                command.Parameters.AddWithValue("@Mood", entry.Mood);
                command.Parameters.AddWithValue("@Content", entry.Content);
                command.Parameters.AddWithValue("@Sentiment", CalculateSentiment(entry.Content));
                command.Parameters.AddWithValue("@Analysis", entry.Analysis);
                command.Parameters.AddWithValue("@Suggestions", entry.Suggestions);

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _connection?.Close();
                    _connection?.Dispose();
                }
                _disposed = true;
            }
        }

        ~DatabaseHelper()
        {
            Dispose(false);
        }

        public static void CreateCleanDatabase(string outputPath)
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            SQLiteConnection.CreateFile(outputPath);

            using var connection = new SQLiteConnection($"Data Source={outputPath};Version=3;");
            connection.Open();

            // Users tablosu
            using var command = new SQLiteCommand(
                @"CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL
                )", connection);
            command.ExecuteNonQuery();

            // Journal tablosu
            command.CommandText = @"CREATE TABLE IF NOT EXISTS Journal (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Date DATETIME NOT NULL,
                Mood TEXT NOT NULL,
                Content TEXT NOT NULL,
                Sentiment REAL,
                Analysis TEXT,
                Suggestions TEXT,
                FOREIGN KEY(UserId) REFERENCES Users(Id)
            )";
            command.ExecuteNonQuery();

            // Örnek admin kullanıcısı
            command.CommandText = "INSERT INTO Users (Username, Password) VALUES ('admin', 'admin123')";
            command.ExecuteNonQuery();
        }
    }

    public class JournalEntry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string Mood { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public double Sentiment { get; set; }
        public string Analysis { get; set; } = string.Empty;
        public string Suggestions { get; set; } = string.Empty;
    }

    public class MoodStatistic
    {
        public string Mood { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class WeeklyTrend
    {
        public DateTime Date { get; set; }
        public double AverageSentiment { get; set; }
        public string Mood { get; set; } = string.Empty;
    }

    public class EntryStatistics
    {
        public int TotalEntries { get; set; }
        public double AveragePerDay { get; set; }
    }
} 