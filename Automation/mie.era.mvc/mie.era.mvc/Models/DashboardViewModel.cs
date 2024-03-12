namespace mie.era.mvc.Models
{
    public class DashboardViewModel
    {
        public int CompletedReferencesCount { get; set; }
        public int TotalReferencesCount { get; set; }
        public string AverageCompletionTime { get; set; }
        public string AverageCandidateScore { get; set; }
    }
}
