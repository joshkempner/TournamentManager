using System;

namespace TournamentManager.Helpers
{
    public static class DateTimeExtensions
    {
        public static double YearsAgo(this DateTime date)
        {
            return (DateTime.Now - date).TotalDays / 365.25;
        }
    }
}
