using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naanayam.Data
{
    public partial interface IDatabase
    {
        #region Account

        Task<List<Account>> GetAccountsAsync(string username);

        Task<bool> CreateAccountAsync(uint id, string username, string name, string description, string currency);

        Task<bool> UpdateAccountAsync(uint id, string name, string description, string currency);

        Task<bool> DeleteAccountAsync(uint id);

        #endregion

        #region Transaction

        Task<List<Transaction>> GetTransactionsAsync(uint accountId, string username, DateTime? dateFrom = null, DateTime? dateTo = null);

        Task<bool> CreateTransactionAsync(uint id, string username, uint accountId, DateTime date, int type, string category, string description, double amount);

        Task<bool> UpdateTransactionAsync(uint id, DateTime timestamp, int type, string category, string description, double amount);

        Task<bool> DeleteTransactionAsync(uint id);

        #endregion

        #region User

        Task<long> GetUserCountAsync(bool showAllUsers = false);

        Task<List<User>> GetUsersAsync(int skip = 0, int count = 1, bool showAll = false);

        Task<User> GetUserByUsernameAsync(string username);

        Task<bool> CreateUserAsync(bool enabled, string username, string password, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null);

        Task<bool> UpdateUserAsync(bool enabled, string username, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null);

        Task<bool> AddUserToRoleAsync(string username, string role);

        Task<bool> RemoveUserFromRoleAsync(string username, string role);

        Task<List<KeyValuePair<string, string>>> GetAllUserSettingsAsync(string username);

        Task<string> GetUserSettingsAsync(string username, string key);

        Task<bool> IsUserSettingsExistsAsync(string username, string key);

        Task<bool> CreateUserSettingsAsync(string username, string key, string value);

        Task<bool> UpdateUserSettingsAsync(string username, string key, string value);

        Task<bool> DeleteUserSettingsAsync(string username, string key);

        Task<bool> ResetUserPaswordAsync(string username, string password);

        Task<bool> EnableUserAsync(string username, bool enabled);

        Task<bool> SetInternalUserAsync(string username);

        Task<bool> DeleteUserAsync(string username);

        #endregion

        #region Settings

        Task<List<KeyValuePair<string, string>>> GetSettingsAsync();

        Task<string> GetSettingsAsync(string key);

        Task<bool> CreateSettingsAsync(string key, string value);

        Task<bool> UpdateSettingsAsync(string key, string value);

        Task<bool> DeleteSettingsAsync(string key);

        #endregion

        #region Enum

        Task<List<string>> GetEnumValuesAsync(string name);

        Task<bool> AddEnumValueAsync(string name, string value);

        Task<bool> RemoveEnumValueAsync(string name, string value);

        Task<bool> RemoveEnumAsync(string name);

        #endregion

        #region Misc

        Task<uint> GetNextIdAsync(string name);

        Task<bool> CreateIndexesAsync();

        Task<bool> PurgeAsync();

        #endregion
    }
}