using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class InquiryManagerRequest
    {
        public int InquiryId { get; set; }
        public int? RequestId { get; set; }
        public string? InquiryStatus { get; set; }
        public string? InquiryType { get; set; }
        public string? InquiryDetails { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? ReviewNotes { get; set; }
        public int? PcvNumber { get; set; }
        public string? Credentials { get; set; }

        public virtual Request? Request { get; set; }
    }
}
