using System;
using System.Collections.Generic;

namespace AHI.Infrastructure.UserContext.Abstraction
{
    public interface IUserContext
    {
        IUserContext SetId(Guid id);
        Guid Id { get; }
        IUserContext SetUpn(string upn);
        string Upn { get; }
        IUserContext SetName(string firstName, string middleName, string lastName);
        string FirstName { get; }
        string MiddleName { get; }
        string LastName { get; }
        IUserContext SetAvatar(string avatar);
        string Avatar { get; }
        IUserContext SetDateTimeFormat(string dateTimeFormat);
        string DateTimeFormat { get; }
        IUserContext SetTimezone(Models.TimeZone timeZone);
        Models.TimeZone Timezone { get; }
        IUserContext SetRightShorts(IEnumerable<string> rightShorts);
        IUserContext SetRightHashes(IEnumerable<string> rights);
        IEnumerable<string> RightShorts { get; }
        // IEnumerable<string> RightHashes { get; }
        IUserContext SetObjectRightShorts(IEnumerable<string> objectRightShorts);
        IUserContext SetObjectRightHashes(IEnumerable<string> objectRightHashes);
        IEnumerable<string> ObjectRightShorts { get; }

        public string ApplicationId { get; }
        IUserContext SetApplicationId(string applicationId);
    }
}
