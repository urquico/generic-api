using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Utils.SwaggerSummary
{
    public class UsersSummary
    {
        public const string GET_ALL_USERS = "Get all users with pagination and filtering options.";
        public const string GET_SINGLE_USERS = "Get user by ID";
        public const string CREATE_USER = "Create a new user";
        public const string UPDATE_USER = "Update an existing user by ID";
        public const string RESET_PASSWORD = "Reset a user's password by ID";
        public const string SOFT_DELETE_USER = "Soft delete a user by ID";
        public const string FORCE_DELETE_USER = "Force delete a user by ID";
        public const string RESTORE_USER = "Restore a soft-deleted user by ID";
        public const string SELF_INFO = "Retrieve the authenticated user's information";
        public const string SELF_UPDATE = "Update the authenticated user's information.";
        public const string SELF_CHANGE_PASSWORD = "Change the authenticated user's password.";
    }
}
