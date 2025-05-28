using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Utils.Auth
{
    public class LoginMessage
    {
        public const string EMAIL_NOT_FOUND =
            "Email not found. Please check your email or sign up if you don't have an account.";

        public const string INVALID_PASSWORD =
            "Invalid password. Please check your password and try again.";

        public const string SUCCESS_LOGIN = "Login successful.";

        public const string LOGIN_ACTIVITY_LOG = "User logged in with email: {0}";
    }
}
