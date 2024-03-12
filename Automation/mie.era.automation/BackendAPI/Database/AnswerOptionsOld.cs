using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class AnswerOptionsOld
    {
        public int AnswerOptionsId { get; set; }
        public string? Description { get; set; }
        public string? OptionValues { get; set; }
        public DateTime? CreatedDated { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? IsActive { get; set; }
        public int? UserId { get; set; }
    }
}
