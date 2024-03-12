using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class AnswerType
    {
        public AnswerType()
        {
            Questions = new HashSet<Question>();
        }

        public int AnswerTypeId { get; set; }
        public string? TypeName { get; set; }
        public string? AnswerType1 { get; set; }
        public int? FrontEndControlType { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}
