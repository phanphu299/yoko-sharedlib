using System;
using System.Collections.Generic;

namespace AHI.Infrastructure.UserContext.Models
{
    public class ClientInfo
    {
        public Guid ClientId { get; set; }
        public IEnumerable<string> RightShorts { get; set; }
        public IEnumerable<string> ObjectRightShorts { get; set; }
        public IEnumerable<string> RightHashes { get; set; }
    }
}