using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class Question
    {
        public Question()
        {
            RefereeResponses = new HashSet<RefereeResponse>();
        }

        public int QuestionId { get; set; }
        public int? QuestionSetId { get; set; }
        public string? QuestionText { get; set; }
        public bool? IsActive { get; set; }
        public int? AnswerTypeId { get; set; }
        public int? AnswerGroupId { get; set; }
        public int? LeadingQuestion { get; set; }
        public string? LeadingQuestionAnswer { get; set; }

        public virtual AnswerOptionGroup? AnswerGroup { get; set; }
        public virtual AnswerType? AnswerType { get; set; }
        public virtual QuestionSet? QuestionSet { get; set; }
        public virtual ICollection<RefereeResponse> RefereeResponses { get; set; }
    }
}
