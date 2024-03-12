namespace BackendAPI.Models
{
    public class LRequestModel
    {

        public int questionSetId { get; set; }
        public string remoteKey { get; set; } = string.Empty;
        public string refereeName { get; set; } = string.Empty;
        public string refereeEmail { get; set; } = string.Empty;
        public string refereePhoneNumber { get; set; } = string.Empty;
        public string relationsShip { get; set; } = string.Empty;
        public int refereeId { get; set; }
     
    }
}
