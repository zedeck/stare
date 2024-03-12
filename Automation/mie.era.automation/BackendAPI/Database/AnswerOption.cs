using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class AnswerOption
    {
        public int AnswerGroupId { get; set; }
        public int AnswerOptionId { get; set; }
        public string Value { get; set; } = null!;
        public int Score { get; set; }
        public int Sequence { get; set; }
        public bool Active { get; set; }

        public virtual AnswerOptionGroup AnswerGroup { get; set; } = null!;
    }
}
