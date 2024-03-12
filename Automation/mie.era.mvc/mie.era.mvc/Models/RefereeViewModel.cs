namespace mie.era.mvc.Models
{
    public class RefereeViewModel
    {
        public int? UserID { get; set; }

        public DateTime? RequestDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? Status { get; set; }

        public int QuestionSetID { get; set; }

        public string? RemoteKey { get; set; }

        public int RequestID { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool? IsActive { get; }

        public string? Relationship { get; set; }

        public string? CandidateName { get; set; }

        public string? CandidateSurname { get; set; }

        public DateTime? CandidateDOB { get; set; }

        public string? CandidateCell { get; set; }

        public string? CandidateEmail { get; set; }

        public string? CandidateId { get; set; }

        public string? CandidatePassport { get; set; }

        public string? SigEmail { get; set; }

        public string? SigName { get; set; }
    }
}
