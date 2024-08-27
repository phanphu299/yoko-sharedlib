using System;
using System.Collections.Generic;
namespace AHI.Infrastructure.UserContext.Models
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string Upn { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<string> RightShorts { get; set; }
        public IEnumerable<string> RightHashes { get; set; }
        public IEnumerable<string> ObjectRightShorts { get; set; }
        public IEnumerable<string> ObjectRightHashes { get; set; }
        public string DateTimeFormat { get; set; }
        public Models.TimeZone TimezoneDto { get; set; }
        public string Avatar { get; set; }
        public UserInfo()
        {
            RightShorts = new List<string>();
            RightHashes = new List<string>();
            ObjectRightShorts = new List<string>();
            ObjectRightHashes = new List<string>();
        }
    }
}
