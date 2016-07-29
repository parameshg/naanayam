using System;
using System.Collections.Generic;

namespace Naanayam.Data
{
    public partial interface IDatabase
    {
        #region Account

        List<Account> GetAccounts(string username);

        bool CreateAccount(uint id, string username, string name, string description, string currency);

        bool UpdateAccount(uint id, string name, string description, string currency);

        bool DeleteAccount(uint id);

        #endregion

        #region Transaction

        List<Transaction> GetTransactions(uint account, DateTime? transactionDateFrom = null, DateTime? transactionDateTo = null);

        bool CreateTransaction(uint id, string username, uint account, DateTime timestamp, int type, string category, string description, double amount);

        bool UpdateTransaction(uint id, DateTime timestamp, int type, string category, string description, double amount);

        bool DeleteTransaction(uint id);

        #endregion

        #region User

        long GetUserCount(bool showAll = false);

        List<User> GetUsers(int skip = 0, int count = 1, bool showAll = false);

        User GetUserByUsername(string username);

        bool CreateUser(bool userEnabled, string username, string password, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null);

        bool UpdateUser(bool userEnabled, string username, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null);

        bool AddUserToRole(string username, string role);

        bool RemoveUserFromRole(string username, string role);

        List<KeyValuePair<string, string>> GetAllUserSettings(string username);

        string GetUserSettings(string username, string key);

        bool IsUserSettingsExists(string username, string key);

        bool CreateUserSettings(string username, string key, string value);

        bool UpdateUserSettings(string username, string key, string value);

        bool DeleteUserSettings(string username, string key);

        bool ResetUserPasword(string username, string password);

        bool EnableUser(string username, bool userEnabled);

        bool SetInternalUser(string username);

        bool DeleteUser(string username);

        #endregion

        #region Settings

        List<KeyValuePair<string, string>> GetSettings();

        string GetSettings(string key);

        bool CreateSettings(string key, string value);

        bool UpdateSettings(string key, string value);

        bool DeleteSettings(string key);

        #endregion

        #region Enum

        List<string> GetEnumValues(string name);

        bool AddEnumValue(string name, string value);

        bool RemoveEnumValue(string name, string value);

        bool RemoveEnum(string name);

        #endregion

        #region Misc

        uint GetNextId(string name);

        bool CreateIndexes();

        bool Purge();

        #endregion
    }
}