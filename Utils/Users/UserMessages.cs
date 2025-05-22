namespace GenericApi.Utils.Users
{
    public static class UserMessages
    {
        public const string SAME_AS_OLD_PASSWORD =
            "New password must be different from the current password.";
        public const string PASSWORD_MISMATCH = "Passwords do not match.";
        public const string PASSWORD_CHANGE_FAILED =
            "Password change failed. Please try again later.";

        public const string PASSWORD_CHANGED_SUCCESS = "Password has been changed.";
        public const string CONTAINS_USER_INFO =
            "New password cannot contain your username or email.";

        public const string PASSWORD_MUST_NOT_BE_EMPTY = "Password must not be empty.";
    }
}
