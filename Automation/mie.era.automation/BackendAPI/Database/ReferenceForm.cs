using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class ReferenceForm
    {
        public int FormId { get; set; }
        public string Link { get; set; } = null!;
        public byte[] FormData { get; set; } = null!;
        public int RequestId { get; set; }
        public Guid UniqueId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsExpired { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsActive { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; } = null!;
    }
}
