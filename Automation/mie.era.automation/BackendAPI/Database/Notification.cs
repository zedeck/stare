using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class Notification
    {
        public int? RefereeId { get; set; }
        public string? NotificationType { get; set; }
        public string? NotificationStatus { get; set; }
        public DateTime? NotificationDateTime { get; set; }
        public string? Message { get; set; }
        public int NotificationId { get; set; }

        public virtual Referee? Referee { get; set; }
    }
}
