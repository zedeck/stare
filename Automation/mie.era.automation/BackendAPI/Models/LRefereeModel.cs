namespace BackendAPI.Models
{
    public class LRefereeModel
    {
        public int RefereeID { get; set; }
        public string RefereeName { get; set; } = string.Empty;
        public string RefereeFirstName { get; set; } = string.Empty;
        public string RefereeLastName { get; set; } = string.Empty;
        public string RefereeEmail { get; set; } = string.Empty;
        public string RefereeMobileNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string RefereeCountry { get; set; } = string.Empty;
        public string RefereeCompany { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string RefereeSpeciality { get; set; } = string.Empty;
        public string RefereeJobTitle { get; set; } = string.Empty;

    }
}
