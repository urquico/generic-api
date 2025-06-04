using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Utils.Users
{
    public class SoftDeletedMessages
    {
        public const string USER_NOT_FOUND = "User not found or has been soft-deleted.";
        public const string ACTIVITY = "{0} has been soft-deleted.";
        public const string SUCCESS = "User has been successfully soft-deleted.";
    }
}
