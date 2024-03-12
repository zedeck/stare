using BackendAPI.Database;

namespace BackendAPI.Models
{
    public class LCreateFormRequestModel
    {
        public int RequestNumber { get; set; }
        public int QuestionSetID { get; set; }
        public string CandidateID { get; set; } = string.Empty;
        public int RefereeID { get; set; }
    
    }
}
