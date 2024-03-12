using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class QuestionSet
    {
        public QuestionSet()
        {
            Questions = new HashSet<Question>();
        }

        public int QuestionSetId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}
