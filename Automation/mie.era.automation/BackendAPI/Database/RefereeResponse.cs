using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class RefereeResponse
    {
        public int AnswerId { get; set; }
        public int? RequestId { get; set; }
        public int? UserId { get; set; }
        public string? AnswerText { get; set; }
        public DateTime? AnswerDate { get; set; }
        public int? QuestionId { get; set; }

        public virtual Question? Question { get; set; }
        public virtual Request? Request { get; set; }
    }
}
