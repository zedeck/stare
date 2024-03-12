namespace mie.era.mvc.Models
{
    public class FELCandidateViewModel
    {
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateSurname { get; set; } = string.Empty;
        public string? CandidateID { get; set; }
        public string CandidateEmail { get; set; } = string.Empty;
        public string? CandidateCell { get; set; } = string.Empty;
        public DateTime CandidateDOB { get; set; }
        public string CandidatePassport { get; set; } = string.Empty;
        public string? SigEmail { get; set; } = string.Empty;
        public string? SigName { get; set; } = string.Empty;
    }
}
