
namespace Naanayam.Server
{
    public static class ErrorMessage
    {
        public static readonly string USER_NOT_FOUND = "The user '{0}' does not exists in the database";

        public static readonly string INSUFFICIENT_USER_PREVILEGE = "The user '{0}' does not have required previlege to perform this operation";

        public static readonly string EMPTY_DATABASE_CONTEXT = "The database context is null";

        public static readonly string EMPTY_USER_CONTEXT = "The user context is null";

        public static readonly string DATABASE_ALREADY_INSTALLED = "The database is already installed, setup and configured";

        public static readonly string USER_ENABLED = "The user is currently enabled and active";
    }
}