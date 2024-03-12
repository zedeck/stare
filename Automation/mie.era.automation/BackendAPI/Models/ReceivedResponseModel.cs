namespace BackendAPI.Models
{
    public class ReceivedResponseModel
    {
        public int Id { get; set; }
        public int RequestID { get; set; }
        public DateTime DateCreated { get; set; }
        public string Agent { get; set; } = string.Empty;
        public string Candidate { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }
}
