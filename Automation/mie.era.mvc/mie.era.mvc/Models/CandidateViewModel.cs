namespace mie.era.mvc.Models
{
    public class CandidateDetailsViewModel
    {
        public string CandidateName { get; set; }
        public string CandidateSurname { get; set; }
        public DateTimeOffset? CandidateDOB { get; set; } 
        public string CandidateCell { get; set; }
        public string CandidateEmail { get; set; }
        public string CandidateId { get; set; }
        public string CandidatePassport { get; set; }
        public string SigEmail { get; set; }
        public string SigName { get; set; }
    }
}
