using System;

namespace Naanayam.Errors
{
    public class UserNotFoundException : UnauthorizedAccessException
    {
        public string Username { get; set; }

        public UserNotFoundException()
        {
        }

        public UserNotFoundException(string username)
        {
            Username = username;
        }

        public UserNotFoundException(string message , string username)
            : base(string.Format(message, username))
        {
            Username = username;
        }
    }
}