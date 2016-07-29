using System;

namespace Naanayam.Errors
{
    public class AccessDeniedException : UnauthorizedAccessException
    {
        public string Username { get; set; }

        public AccessDeniedException()
            : base("Invalid username or password")
        {
        }

        public AccessDeniedException(string username)
            : base("Invalid username or password")
        {
            Username = username;
        }
    }
}