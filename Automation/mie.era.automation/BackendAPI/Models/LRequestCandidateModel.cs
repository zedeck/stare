using BackendAPI.Database;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BackendAPI.Models
{
    public class LRequestCandidateModel
    {
        public int? UserID { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Status { get; set; }
        public int? QuestionSetID { get; set; }
        public string? RemoteKey { get; set; }
        public int? RefereeID { get; set; }
        public int RequestID { get; set; }
       
    }
}
