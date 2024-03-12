using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class User
    {
        public int UserId { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool? IsActive { get; set; }
        public int? InternalPersonId { get; set; }
    }
}
