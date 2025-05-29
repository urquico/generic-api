using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Utils.Auth
{
    public class LogoutMessages
    {
        public const string MISSING_TOKEN = "Token is missing.";
        public const string ACTIVITY_LOG = "User logged out successfully.";
        public const string SUCCESS = "Logout successful. You have been logged out.";
    }
}
