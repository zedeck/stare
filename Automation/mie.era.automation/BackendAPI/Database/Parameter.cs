using System;
using System.Collections.Generic;

namespace BackendAPI.Database
{
    public partial class Parameter
    {
        public int ParameterId { get; set; }
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
