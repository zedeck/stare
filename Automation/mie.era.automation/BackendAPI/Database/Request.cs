using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class Request
    {
        public Request()
        {
            InquiryManagerRequests = new HashSet<InquiryManagerRequest>();
            InternalNotifications = new HashSet<InternalNotification>();
            RefereeResponses = new HashSet<RefereeResponse>();
            Responses = new HashSet<Response>();
        }

        public int? UserId { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Status { get; set; }
        public int? QuestionSetId { get; set; }
        public string? RemoteKey { get; set; }
        public int? RefereeId { get; set; }
        public int RequestId { get; set; }

        public virtual Referee? Referee { get; set; }
        public virtual ICollection<InquiryManagerRequest> InquiryManagerRequests { get; set; }
        public virtual ICollection<InternalNotification> InternalNotifications { get; set; }
        public virtual ICollection<RefereeResponse> RefereeResponses { get; set; }
        public virtual ICollection<Response> Responses { get; set; }
    }
}
