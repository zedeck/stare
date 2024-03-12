namespace BackendAPI.Models
{
    public class EditRefereeInfo
    {

        public int RefereeID { get; set; }
        public string RefereeName { get; set; } = string.Empty;
        public string RefereeFirstName { get; set; } = string.Empty;
        public string RefereeLastName { get; set; } = string.Empty;
        public string RefereeEmail { get; set; } = string.Empty;
        public string RefereeCountry { get; set; } = string.Empty;
        public string RefereeMobile { get; set; } = string.Empty;
        public string RefereeCompanyName { get; set; } = string.Empty;
        public string RefereeJobTitle { get; set; } = string.Empty;
        public string RefereeSpeciality { get; set; } = string.Empty;

    }
}
