using Naanayam.Enum;
using System;
using System.Collections.Generic;

namespace Naanayam.Server
{
    public partial interface IServer
    {
        #region Properties

        string Version { get; }

        Context Context { get; }

        #endregion

        #region Account

        List<Account> GetAccounts(string username = null);

        bool CreateAccount(string name, string description, string currency);

        bool UpdateAccount(uint id, string name, string description, string currency);

        bool DeleteAccount(uint id);

        #endregion

        #region Transaction

        List<Transaction> GetTransactions(uint accountId, DateTime? transactionDateFrom = null, DateTime? transactionDateTo = null);

        bool CreateTransaction(uint account, DateTime timestamp, TransactionType type, string category, string description, double amount);

        bool UpdateTransaction(uint id, DateTime timestamp, TransactionType type, string category, string description, double amount);

        bool DeleteTransaction(uint id);

        #region Category

        List<string> GetTransactionCategories(string transactionType);

        bool AddTransactionCategory(string transactionType, string transactionCategory);

        bool RemoveTransactionCategory(string transactionType, string transactionCategory);

        List<string> GetTransactionCategories(string transactionType, string transactionCategory);

        bool AddTransactionCategory(string transactionType, string transactionCategory, string transactionSubCategory);

        bool RemoveTransactionCategory(string transactionType, string transactionCategory, string transactionSubCategory);

        #endregion

        #region Type

        List<string> GetTransactionTypes();

        bool AddTransactionType(string transactionType);

        bool RemoveTransactionType(string transactionType);

        #endregion

        #endregion

        #region User

        long GetUserCount();

        List<User> GetUsers(int skip = 0, int count = 1);

        bool IsUserInRole(Role role, string username = "");

        bool IsUserInRole(string role, string username = "");

        bool IsUserExists(string username = "");

        bool IsUserAdmin(string username = "");

        User GetUserByUsername(string username);

        bool CreateUser(string username, string password, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null);

        bool UpdateUser(string username, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null);

        bool AddUserToRole(string username, string role);

        bool RemoveUserFromRole(string username, string role);

        List<KeyValuePair<string, string>> GetAllUserSettings(string username);

        string GetUserSettings(string username, string key);

        bool IsUserSettingsExists(string key, string username = null);

        bool CreateUserSettings(string username, string key, string value);

        bool UpdateUserSettings(string username, string key, string value);

        bool DeleteUserSettings(string username, string key);

        bool ResetUserPasword(string username, string password);

        bool EnableUser(string username);

        bool DisableUser(string username);

        bool DeleteUser(string username);

        #endregion

        #region Reports

        List<Report.CategoryValue> GetMonthlyExpensesByCategoryReport(uint accountId, DateTime? transactionDateFrom, DateTime? transactionDateTo);

        #endregion

        #region Settings

        List<KeyValuePair<string, string>> GetSettings();

        string GetSettings(string key);

        bool CreateSettings(string key, string value);

        bool UpdateSettings(string key, string value);

        bool DeleteSettings(string key);

        #endregion

        #region Enum

        List<string> GetRoles();

        List<string> GetCurrencies();

        #endregion

        #region Install

        bool Install();

        bool Uninstall();

        #endregion

        #region Misc

        string HashPassword(string password);

        string HashPassword(byte[] password);

        string Hash(string data);

        string Hash(byte[] data);

        #endregion
    }
}