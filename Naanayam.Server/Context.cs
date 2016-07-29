using System;

namespace Naanayam.Server
{
    public class Context
    {
        public string User { get; private set; }

        public uint Account { get; private set; }

        public Context(string user)
        {
            if (string.IsNullOrEmpty(user))
                throw new NullReferenceException(ErrorMessage.EMPTY_USER_CONTEXT);

            User = user;
        }

        public void Impersonate(string user)
        {
            User = user;
        }

        public void ChangeAccount(uint account)
        {
            Account = account;
        }

        public void Clear()
        {
            User = string.Empty;
        }
    }
}