using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using System.Reflection.Metadata.Ecma335;

namespace BackendAPI.Models
{
    public class LRefereeQAModel
    {
        public int Id { get; set; }
        public string RequestKey { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateFirstName { get; set; } = string.Empty;
        public string CandidateLastName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
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
        public string ReferenceType { get; set; } = string.Empty;
        public int QuestionnaireSetId { get; set; }
        public string QuestionnaireSetName { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public int QuestionID { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string AnswerType { get; set; } = string.Empty;
        public int AnswerWeight { get; set; }
        public Int32 RequestID { get; set; }
    }
}
