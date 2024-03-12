using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class Response
    {
        public int ResponseId { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string? ResponseDetails { get; set; }
        public string? ResponseType { get; set; }
        public decimal? Score { get; set; }
        public string? ResponseStatus { get; set; }
        public int? ResponseByUserId { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? RejectionReason { get; set; }
        public string? Attachments { get; set; }
        public string? VerificationDetails { get; set; }
        public string? Comments { get; set; }
        public int? CompletionTime { get; set; }
        public int? RequestId { get; set; }

        public virtual Request? Request { get; set; }
    }
}
