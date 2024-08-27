using System;
using System.Collections.Generic;
using System.Linq;
using AHI.Infrastructure.UserContext.Abstraction;

namespace AHI.Infrastructure.UserContext.Internal
{
    public class UserContext : IUserContext
    {
        private Guid _id;
        private string _upn;
        private string _firstName;
        private string _middleName;
        private string _lastName;
        private string _avatar;
        private string _dateTimeFormat;
        private string _applicationId;
        private Models.TimeZone _timezone;
        private List<string> _rightShorts = new List<string>();
        private List<string> _rightHashes = new List<string>();
        private List<string> _objectRightShorts = new List<string>();
        private List<string> _objectRightHashes = new List<string>();
        public Guid Id => _id;
        public string Upn => _upn;
        public string FirstName => _firstName;
        public string MiddleName => _middleName;
        public string LastName => _lastName;
        public string Avatar => _avatar;
        public string DateTimeFormat => _dateTimeFormat;
        public string ApplicationId => _applicationId;
        public Models.TimeZone Timezone => _timezone;
        public IEnumerable<string> RightShorts => _rightShorts;
        public IEnumerable<string> RightHashes => _rightHashes;
        public IEnumerable<string> ObjectRightShorts => _objectRightShorts;
        public IEnumerable<string> ObjectRightHashes => _objectRightHashes;

        public IUserContext SetId(Guid id)
        {
            _id = id;
            return this;
        }
        public IUserContext SetUpn(string upn)
        {
            _upn = upn;
            return this;
        }
        public IUserContext SetName(string firstName, string middleName, string lastName)
        {
            _firstName = firstName;
            _middleName = middleName;
            _lastName = lastName;
            return this;
        }
        public IUserContext SetAvatar(string avatar)
        {
            _avatar = avatar;
            return this;
        }
        public IUserContext SetDateTimeFormat(string dateTimeFormat)
        {
            _dateTimeFormat = dateTimeFormat;
            return this;
        }
        public IUserContext SetTimezone(Models.TimeZone timeZone)
        {
            _timezone = timeZone;
            return this;
        }
        public IUserContext SetRightHashes(IEnumerable<string> rights)
        {
            _rightHashes.Clear();
            _rightHashes.AddRange(rights);
            return this;
        }
        public IUserContext SetRightShorts(IEnumerable<string> rights)
        {
            if (rights != null && rights.Any())
            {
                _rightShorts.Clear();
                _rightShorts.AddRange(rights);
            }
            return this;
        }
        public IUserContext SetObjectRightHashes(IEnumerable<string> rights)
        {
            _objectRightHashes.Clear();
            _objectRightHashes.AddRange(rights);
            return this;
        }
        public IUserContext SetObjectRightShorts(IEnumerable<string> rights)
        {
            if (rights != null && rights.Any())
            {
                _objectRightShorts.Clear();
                _objectRightShorts.AddRange(rights);
            }
            return this;
        }
        public IUserContext SetApplicationId(string applicationId)
        {
            _applicationId = applicationId;
            return this;
        }
    }
}