using System;

namespace Naanayam.Errors
{
    public class InsufficientPrevilegeException : UnauthorizedAccessException
    {
        public string Username { get; set; }

        public InsufficientPrevilegeException()
        {
        }

        public InsufficientPrevilegeException(string username)
        {
            Username = username;
        }

        public InsufficientPrevilegeException(string message, string username)
            : base(string.Format(message, username))
        {
            Username = username;
        }
    }
}