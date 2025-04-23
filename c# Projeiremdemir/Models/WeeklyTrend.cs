using System;

namespace TerapiTakipUygulamasi.Models
{
    public class WeeklyTrend
    {
        public DateTime Date { get; set; }
        public double AverageSentiment { get; set; }
        public string Mood { get; set; } = string.Empty;
    }
} 