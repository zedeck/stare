namespace mie.era.mvc.Models
{
    public class FELCandidateListViewModel
    {
        public int? RequestKey { get; set; }
        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
        public string? UIMobileNumber { get; set; }
        public string? Score { get; set; }
        public string? TotalReferences { get; set; }
        public string? ReferenceStatus { get; set; }
        public string? AssignedTo { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
