namespace BackendAPI.Models
{
    public class UserModel
    {

        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string InternalPersonID { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime LastLogin { get; set; }
        public List<LCandidateModel> Candidates { get; set; } = new List<LCandidateModel>();
    }
}
