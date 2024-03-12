using System;
using System.Linq;
using System.Threading.Tasks;
using BackendAPI.Database;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class StatisticsService
    {
        private readonly ERADBContext _context;

        public StatisticsService(ERADBContext context)
        {
            _context = context;
        }

        // References Count
        public async Task<int> GetCompletedReferencesCount()
        {
            return await _context.Requests.CountAsync(r => r.Status == "Completed");
        }

        public int GetTotalReferencesCount()
        {
            return _context.Requests.Count();
        }

        // Average Completion Time for Responses
        public async Task<double> GetAverageCompletionTime()
        {
            var averageCompletionTime = await _context.Responses
                .Where(r => r.CompletionTime.HasValue)
                .AverageAsync(r => r.CompletionTime.Value);

            return double.IsNaN(averageCompletionTime) ? 0 : averageCompletionTime;
        }

        // Average Candidate Score for Responses
        public async Task<double> GetAverageCandidateScore()
        {
            return (double)await _context.Responses.AverageAsync(r => r.Score);
        }

        public async Task<int> GetNotificationCount(int refereeId)
        {
            return await _context.Notifications
                .Where(n => n.RefereeId == refereeId && n.NotificationType == "SMS")
                .CountAsync();
        }

        public async Task<DateTime?> GetMaxNotificationDateTime(int refereeId)
        {
            return await _context.Notifications
                .Where(n => n.RefereeId == refereeId)
                .MaxAsync(n => (DateTime?)n.NotificationDateTime);
        }

    }
}
