using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class RefereeAnswer
    {
        public int AnswersId { get; set; }
        public int RequestId { get; set; }
        public string? RequestKey { get; set; }
        public string CandidateName { get; set; } = null!;
        public string CandidateFirstName { get; set; } = null!;
        public string CandidateLastName { get; set; } = null!;
        public int RefereeId { get; set; }
        public string RefereeName { get; set; } = null!;
        public string RefereeFirstName { get; set; } = null!;
        public string RefereeLastName { get; set; } = null!;
        public string RefereeEmail { get; set; } = null!;
        public string RefereeCountry { get; set; } = null!;
        public string RefereeMobile { get; set; } = null!;
        public string RefereeCompanyName { get; set; } = null!;
        public string RefereeJobTitle { get; set; } = null!;
        public string RefereeSpeciality { get; set; } = null!;
        public string ReferenceType { get; set; } = null!;
        public int QuestionnaireSetId { get; set; }
        public string QuestionnaireSetName { get; set; } = null!;
        public int QuestionId { get; set; }
        public string Question { get; set; } = null!;
        public string AnswerType { get; set; } = null!;
        public string Answer { get; set; } = null!;
        public int AnswerWeight { get; set; }
        public DateTime QuestionDate { get; set; }
        public DateTime AnswerDate { get; set; }
    }
}
