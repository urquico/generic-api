using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Utils.Auth
{
    public class RefreshMessages
    {
        public const string MISSING_TOKEN = "Token is missing.";
        public const string INVALID_TOKEN = "Token is invalid or expired.";
        public const string REVOKED_TOKEN = "Token has been revoked.";
        public const string USER_NOT_FOUND = "User not found.";
        public const string SUCCESS = "Token refreshed successfully.";
    }
}
