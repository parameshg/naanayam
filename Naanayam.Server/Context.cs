using System;

namespace Naanayam.Server
{
    public class Context
    {
        private string _username;

        public string Username { get { return _username ?? _username.ToLower(); } private set { _username = value; } }

        public uint Account { get; private set; }

        public Context(string user)
        {
            if (string.IsNullOrEmpty(user))
                throw new NullReferenceException(ErrorMessage.EMPTY_USER_CONTEXT);

            Username = user;
        }

        public void Impersonate(string user)
        {
            Username = user;
        }

        public void ChangeAccount(uint account)
        {
            Account = account;
        }

        public void Clear()
        {
            Username = string.Empty;
        }
    }
}