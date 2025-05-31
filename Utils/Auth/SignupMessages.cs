using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Utils.Auth
{
    public class SignupMessages
    {
        public const string EMAIL_ALREADY_EXISTS =
            "Email already exists. Please use a different email address.";
        public const string PASSWORD_CONFIRMATION_MISMATCH =
            "Password confirmation does not match the password.";

        public const string SUCCESS_SIGNUP =
            "Signup successful. Please check your email to verify your account.";

        public const string SIGNUP_ACTIVITY_LOG = "User signed up with email: {0}";
        public const string USER_ROLES_EMPTY =
            "User roles cannot be empty. Please select at least one role.";
    }
}
