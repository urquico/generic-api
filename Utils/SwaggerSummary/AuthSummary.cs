using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Utils.SwaggerSummary
{
    public class AuthSummary
    {
        public const string SIGNUP = "Signup a new user";
        public const string LOGIN = "Login an existing user";
        public const string LOGOUT = "Logout the current user";
        public const string REFRESH = "Refresh the access token for the current user";
    }
}
