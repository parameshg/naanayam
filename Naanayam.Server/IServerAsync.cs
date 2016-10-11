using Naanayam.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naanayam.Server
{
    public partial interface IServer
    {
        #region Account

        Task<List<Account>> GetAccountsAsync(string username = null);

        Task<bool> CreateAccountAsync(string name, string description, string currency);

        Task<bool> UpdateAccountAsync(uint id, string name, string description, string currency);

        Task<bool> DeleteAccountAsync(uint id);

        #endregion

        #region Transaction

        Task<List<Transaction>> GetTransactionsAsync(uint accountId, DateTime? transactionDateFrom = null, DateTime? transactionDateTo = null);

        Task<bool> CreateTransactionAsync(uint account, DateTime timestamp, TransactionType type, string category, string description, double amount);

        Task<bool> UpdateTransactionAsync(uint id, DateTime timestamp, TransactionType type, string category, string description, double amount);

        Task<bool> DeleteTransactionAsync(uint id);

        #region Category

        Task<List<string>> GetTransactionCategoriesAsync(string transactionType);

        Task<bool> AddTransactionCategoryAsync(string transactionType, string transactionCategory);

        Task<bool> RemoveTransactionCategoryAsync(string transactionType, string transactionCategory);

        Task<List<string>> GetTransactionCategoriesAsync(string transactionType, string transactionCategory);

        Task<bool> AddTransactionCategoryAsync(string transactionType, string transactionCategory, string transactionSubCategory);

        Task<bool> RemoveTransactionCategoryAsync(string transactionType, string transactionCategory, string transactionSubCategory);

        #endregion

        #region Type

        Task<List<string>> GetTransactionTypesAsync();
        
        Task<bool> AddTransactionTypeAsync(string transactionType);
        
        Task<bool> RemoveTransactionTypeAsync(string transactionType);

        #endregion

        #endregion

        #region Users

        Task<bool> AuthenticateAsync(string username, string password);

        Task<long> GetUserCountAsync();

        Task<List<User>> GetUsersAsync(int skip = 0, int count = 1);

        Task<bool> IsUserInRoleAsync(Role role, string username = "");

        Task<bool> IsUserInRoleAsync(string role, string username = "");

        Task<bool> IsUserExistsAsync(string username = "");

        Task<bool> IsUserAdminAsync(string username = "");

        Task<User> GetUserByUsernameAsync(string username);

        Task<bool> CreateUserAsync(string username, string password, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null);

        Task<bool> UpdateUserAsync(string username, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null);

        Task<bool> AddUserToRoleAsync(string username, string role);

        Task<bool> RemoveUserFromRoleAsync(string username, string role);

        Task<List<KeyValuePair<string, string>>> GetAllUserSettingsAsync(string username);

        Task<string> GetUserSettingsAsync(string username, string key);

        Task<bool> IsUserSettingsExistsAsync(string key, string username = null);

        Task<bool> CreateUserSettingsAsync(string username, string key, string value);

        Task<bool> UpdateUserSettingsAsync(string username, string key, string value);

        Task<bool> DeleteUserSettingsAsync(string username, string key);

        Task<bool> ResetUserPaswordAsync(string username, string password);

        Task<bool> EnableUserAsync(string username);

        Task<bool> DisableUserAsync(string username);

        Task<bool> DeleteUserAsync(string username);

        #endregion

        #region Reports

        Task<List<Report.CategoryValue>> GetMonthlyExpensesByCategoryReportAsync(uint accountId, DateTime? transactionDateFrom, DateTime? transactionDateTo);

        #endregion

        #region Settings

        Task<List<KeyValuePair<string, string>>> GetSettingsAsync();

        Task<string> GetSettingsAsync(string key);

        Task<bool> CreateSettingsAsync(string key, string value);

        Task<bool> UpdateSettingsAsync(string key, string value);

        Task<bool> DeleteSettingsAsync(string key);

        #endregion

        #region Enum

        Task<List<string>> GetRolesAsync();

        Task<List<string>> GetCurrenciesAsync();

        #endregion

        #region Misc

        Task<bool> InstallAsync();

        Task<bool> UninstallAsync();

        #endregion
    }
}