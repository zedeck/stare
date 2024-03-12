using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class InternalNotification
    {
        public long InternalNotificationId { get; set; }
        public int? RequestId { get; set; }
        public string? SigEmail { get; set; }
        public string? SigUser { get; set; }
        public DateTime? NotifyDate { get; set; }

        public virtual Request? Request { get; set; }
    }
}
