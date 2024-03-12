using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class Referee
    {
        public Referee()
        {
            Notifications = new HashSet<Notification>();
            Requests = new HashSet<Request>();
        }

        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? IsActive { get; set; }
        public string? Relationship { get; set; }
        public int Refereeid { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }
}
