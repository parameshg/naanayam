using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Naanayam.Data
{
    internal class Entity
    {
        public class Account
        {
            [BsonId]
            public uint ID { get; set; }

            public string Username { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public string Currency { get; set; }
        }

        public class Transaction
        {
            [BsonId]
            public uint ID { get; set; }

            public string Username { get; set; }

            public uint Account { get; set; }

            public DateTime Timestamp { get; set; }

            public int Type { get; set; }
            
            public string Category { get; set; }

            public string Description { get; set; }

            public int Amount { get; set; }
        }

        public class User
        {
            [BsonId]
            public string Username { get; set; }

            public string Password { get; set; }

            public bool Enabled { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Email { get; set; }

            public string Phone { get; set; }

            public bool Internal { get; set; }

            public string Provider { get; set; }

            public string ProviderKey { get; set; }

            public List<string> Roles { get; set; }

            public List<Settings> Settings { get; set; }

            public User()
            {
                Roles = new List<string>();

                Settings = new List<Settings>();
            }
        }

        public class Settings
        {
            [BsonId]
            public string Key { get; set; }

            public string Value { get; set; }
        }

        public class Counter
        {
            [BsonId]
            public string Name { get; set; }

            public uint Value { get; set; }
        }

        public class Enum
        {
            [BsonId]
            public string Name { get; set; }

            public List<string> Values { get; set; }
        }
    }
}