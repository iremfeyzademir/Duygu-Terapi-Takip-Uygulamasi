using System;

namespace TerapiTakipUygulamasi.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string Mood { get; set; } = "";
        public string Content { get; set; } = "";
        public double Sentiment { get; set; }
        public string Analysis { get; set; } = "";
        public string Suggestions { get; set; } = "";
    }
} 