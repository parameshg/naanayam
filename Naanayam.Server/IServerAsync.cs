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

        Task<Dictionary<string, List<string>>> GetTransactionCategoriesAsync();

        Task<bool> AddTransactionCategoryAsync(string category);

        Task<bool> RemoveTransactionCategoryAsync(string category);

        Task<List<string>> GetTransactionCategoriesAsync(string category);

        Task<bool> AddTransactionCategoryAsync(string category, string subCategory);

        Task<bool> RemoveTransactionCategoryAsync(string category, string subCategory);

        #endregion

        #region Type

        Task<List<string>> GetTransactionTypesAsync();
        
        Task<bool> AddTransactionTypeAsync(string transactionType);
        
        Task<bool> RemoveTransactionTypeAsync(string transactionType);

        #endregion

        #endregion

        #region Users

        Task<bool> IsUserSettingsExistsAsync(string key, string username = null);

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