using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class AnswerOptionGroup
    {
        public AnswerOptionGroup()
        {
            AnswerOptions = new HashSet<AnswerOption>();
            Questions = new HashSet<Question>();
        }

        public int AnswerGroupId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<AnswerOption> AnswerOptions { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
