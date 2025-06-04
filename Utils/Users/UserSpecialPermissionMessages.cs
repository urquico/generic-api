using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Utils.Users
{
    public class UserSpecialPermissionMessages
    {
        public const string USER_NOT_FOUND = "User not found";
        public const string PERMISSION_NOT_FOUND = "Permission not found";
        public const string PERMISSION_ALREADY_EXISTS = "User already has the {0} permission.";
        public const string ACTIVITY_GRANTED = "{0} has been granted the {1} permission.";
        public const string ACTIVITY_REVOKED = "{0} has been revoked the {1} permission.";
        public const string SUCCESS = "User's special permission has been successfully updated.";
    }
}
