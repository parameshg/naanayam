using System.Collections.Generic;

namespace Naanayam
{
    public class User
    {
        public bool Enabled { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public bool Internal { get; set; }

        public string Provider { get; set; }

        public string ProviderKey { get; set; }

        public List<string> Roles { get; set; }

        public List<KeyValuePair<string, string>> Settings { get; set; }

        public User()
        {
            Roles = new List<string>();

            Settings = new List<KeyValuePair<string, string>>();
        }
    }
}