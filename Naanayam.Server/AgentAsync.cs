using Naanayam.Enum;
using Naanayam.Errors;
using Naanayam.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Naanayam.Server
{
    public partial class Agent : IServer
    {
        #region Account

        public async Task<List<Account>> GetAccountsAsync(string username = null)
        {
            List<Account> result = new List<Account>();

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                username = string.IsNullOrEmpty(username) ? Context.Username : username;

                result.AddRange(await Database.GetAccountsAsync(username));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> CreateAccountAsync(string name, string description, string currency)
        {
            bool result = false;

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                uint id = await GetNextIdAsync(ID.ACCOUNT);

                result = await Database.CreateAccountAsync(id, Context.Username, name, description, currency);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> UpdateAccountAsync(uint id, string name, string description, string currency)
        {
            bool result = false;

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result = await Database.UpdateAccountAsync(id, name, description, currency);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> DeleteAccountAsync(uint id)
        {
            bool result = false;

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result = await Database.DeleteAccountAsync(id);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region Transaction

        public async Task<List<Transaction>> GetTransactionsAsync(DateTime? transactionDateFrom = null, DateTime? transactionDateTo = null)
        {
            List<Transaction> result = new List<Transaction>();

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result.AddRange(await Database.GetTransactionsAsync(Context.Account, Context.Username, transactionDateFrom, transactionDateTo));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> CreateTransactionAsync(DateTime timestamp, TransactionType type, string category, string description, double amount)
        {
            bool result = false;

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                uint id = await GetNextIdAsync(ID.TRANSACTION);

                result = await Database.CreateTransactionAsync(id, Context.Username, Context.Account, timestamp, (int)type, category, description, amount);
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> UpdateTransactionAsync(uint id, DateTime timestamp, TransactionType type, string category, string description, double amount)
        {
            bool result = false;

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result = await Database.UpdateTransactionAsync(id, timestamp, (int)type, category, description, amount);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> DeleteTransactionAsync(uint id)
        {
            bool result = false;

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result = await Database.DeleteTransactionAsync(id);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #region Category

        public async Task<List<string>> GetTransactionCategoriesAsync(string transactionType)
        {
            List<string> result = new List<string>();

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                string json = await GetUserSettingsAsync(GetCategoryKey(transactionType));

                result.AddRange(await Serializer<List<string>>.Current.DeserializeFromJsonAsync(json));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> AddTransactionCategoryAsync(string transactionType, string transactionCategory)
        {
            bool result = false;

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                List<string> o = new List<string>();

                o.AddRange(await GetTransactionCategoriesAsync(transactionType));

                o.Add(transactionCategory);

                string json = await Serializer<List<string>>.Current.SerializeToJsonAsync(o);

                if (await IsUserSettingsExistsAsync(GetCategoryKey(transactionType)))
                    await UpdateUserSettingsAsync(GetCategoryKey(transactionType), json);
                else
                    await CreateUserSettingsAsync(GetCategoryKey(transactionType), json);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> RemoveTransactionCategoryAsync(string transactionType, string transactionCategory)
        {
            bool result = false;

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                // remove category

                List<string> o = new List<string>();

                o.AddRange(await GetTransactionCategoriesAsync(transactionType));

                o.Remove(transactionCategory);

                string json = await Serializer<List<string>>.Current.SerializeToJsonAsync(o);

                await UpdateUserSettingsAsync(GetCategoryKey(transactionType), json);

                // remove its corresponding sub categories

                await DeleteUserSettingsAsync(GetCategoryKey(transactionType, transactionCategory));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<List<string>> GetTransactionCategoriesAsync(string transactionType, string transactionCategory)
        {
            List<string> result = new List<string>();

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                string json = await GetUserSettingsAsync(GetCategoryKey(transactionType + "." + transactionCategory));

                result.AddRange(await Serializer<List<string>>.Current.DeserializeFromJsonAsync(json));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> AddTransactionCategoryAsync(string transactionType, string transactionCategory, string transactionSubCategory)
        {
            bool result = false;

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                List<string> o = new List<string>();

                o.AddRange(await GetTransactionCategoriesAsync(transactionType, transactionCategory));

                o.Add(transactionSubCategory);

                string json = await Serializer<List<string>>.Current.SerializeToJsonAsync(o);

                if (await IsUserSettingsExistsAsync(GetCategoryKey(transactionType, transactionCategory)))
                    await UpdateUserSettingsAsync(GetCategoryKey(transactionType, transactionCategory), json);
                else
                    await CreateUserSettingsAsync(GetCategoryKey(transactionType, transactionCategory), json);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> RemoveTransactionCategoryAsync(string transactionType, string transactionCategory, string transactionSubCategory)
        {
            bool result = false;

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                List<string> o = new List<string>();

                o.AddRange(await GetTransactionCategoriesAsync(transactionType, transactionCategory));

                o.Remove(transactionSubCategory);

                string json = await Serializer<List<string>>.Current.SerializeToJsonAsync(o);

                if (await IsUserSettingsExistsAsync(GetCategoryKey(transactionType, transactionCategory)))
                    await UpdateUserSettingsAsync(GetCategoryKey(transactionType, transactionCategory), json);
                else
                    await CreateUserSettingsAsync(GetCategoryKey(transactionType, transactionCategory), json);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region Type

        public async Task<List<string>> GetTransactionTypesAsync()
        {
            List<string> result = new List<string>();

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result.AddRange(await Database.GetEnumValuesAsync(Enum.Type));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> AddTransactionTypeAsync(string transactionType)
        {
            bool result = false;

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result = await Database.AddEnumValueAsync(Enum.Type, transactionType);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> RemoveTransactionTypeAsync(string transactionType)
        {
            bool result = false;

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result = await Database.RemoveEnumValueAsync(Enum.Type, transactionType);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #endregion

        #region User

        public async Task<long> GetUserCountAsync()
        {
            long result = 0;

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            result = await Database.GetUserCountAsync();

            return result;
        }

        public async Task<List<User>> GetUsersAsync(int page = 1, int count = 10)
        {
            List<User> result = null;

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            result = await Database.GetUsersAsync((page - 1) * count, count);

            return result;
        }

        public async Task<bool> IsUserExistsAsync(string username = null)
        {
            bool result = false;

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            User o = await Database.GetUserByUsernameAsync(username);

            result = o != null;

            return result;
        }

        public async Task<bool> IsUserAdminAsync(string username = null)
        {
            return await IsUserInRoleAsync(Role.Administrator, username) || await IsUserInRoleAsync(Role.System, username);
        }

        public async Task<bool> IsUserInRoleAsync(string role, string username = null)
        {
            bool result = false;

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            User o = await Database.GetUserByUsernameAsync(username);

            if (o != null)
                result = o.Roles.Contains(role);

            return result;
        }

        public async Task<bool> IsUserInRoleAsync(Role role, string username = null)
        {
            return await IsUserInRoleAsync(role.ToString(), username);
        }

        public async Task<User> GetUserByUsernameAsync(string username = null)
        {
            User result = null;

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = await Database.GetUserByUsernameAsync(username);

            result = result != null && result.Enabled ? result : null;

            return result;
        }

        public async Task<bool> CreateUserAsync(string username, string password, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null)
        {
            bool result = false;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            password = await HashPasswordAsync(password);

            result = await Database.CreateUserAsync(false, username, password, firstName, lastName, email, phone, loginProvider, loginProviderKey);

            if (result)
            {
                foreach (string type in await Database.GetEnumValuesAsync(Enum.Type))
                {
                    List<string> categories = new List<string>();

                    categories.AddRange(await Database.GetEnumValuesAsync(GetCategoryKey(type)));

                    string json = await Serializer<List<string>>.Current.SerializeToJsonAsync(categories);

                    await Database.CreateUserSettingsAsync(username, GetCategoryKey(type), json);

                    foreach(string category in categories)
                    {
                        List<string> subCategories = new List<string>();

                        subCategories.AddRange(await Database.GetEnumValuesAsync(GetCategoryKey(type, category)));

                        json = await Serializer<List<string>>.Current.SerializeToJsonAsync(subCategories);

                        await Database.CreateUserSettingsAsync(username, GetCategoryKey(type, category), json);
                    }
                }
            }

            return result;
        }

        public async Task<bool> UpdateUserAsync(string username, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null)
        {
            bool result = false;

            if (!await IsUserAdminAsync())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = await Database.UpdateUserAsync(false, username, firstName, lastName, email, phone, loginProvider, loginProviderKey);

            return result;
        }

        public async Task<bool> AddUserToRoleAsync(string username, string role)
        {
            bool result = false;

            if (!await IsUserAdminAsync())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = await Database.AddUserToRoleAsync(username, role);

            return result;
        }

        public async Task<bool> RemoveUserFromRoleAsync(string username, string role)
        {
            bool result = false;

            if (!await IsUserAdminAsync())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = await Database.RemoveUserFromRoleAsync(username, role);

            return result;
        }

        public async Task<List<KeyValuePair<string, string>>> GetAllUserSettingsAsync(string username)
        {
            List<KeyValuePair<string, string>> result = null;

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = await Database.GetAllUserSettingsAsync(username);

            return result;
        }

        public async Task<string> GetUserSettingsAsync(string key, string username = null)
        {
            string result = string.Empty;

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = await Database.GetUserSettingsAsync(username, key);

            return result;
        }

        public async Task<bool> IsUserSettingsExistsAsync(string key, string username = null)
        {
            bool result = false;

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = await Database.IsUserSettingsExistsAsync(username, key);

            return result;
        }

        public async Task<bool> CreateUserSettingsAsync(string key, string value, string username = null)
        {
            bool result = false;

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = await Database.CreateUserSettingsAsync(username, key, value);

            return result;
        }

        public async Task<bool> UpdateUserSettingsAsync(string key, string value, string username = null)
        {
            bool result = false;

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = await Database.UpdateUserSettingsAsync(username, key, value);

            return result;
        }

        public async Task<bool> DeleteUserSettingsAsync(string value, string username = null)
        {
            bool result = false;

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = await Database.DeleteUserSettingsAsync(username, value);

            return result;
        }

        public async Task<bool> ChangeUserPaswordAsync(string username, string oldPassword, string newPassword)
        {
            bool result = false;

            if (string.IsNullOrEmpty(username))
                throw new InvalidParameterException("username");

            if (string.IsNullOrEmpty(oldPassword))
                throw new InvalidParameterException("oldPassword");

            if (string.IsNullOrEmpty(newPassword))
                throw new InvalidParameterException("newPassword");

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            User o = await Database.GetUserByUsernameAsync(username);

            if (o != null)
            {
                oldPassword = await HashPasswordAsync(oldPassword);

                if (o.Password == oldPassword)
                {
                    newPassword = await HashPasswordAsync(newPassword);

                    result = await Database.ResetUserPaswordAsync(username, newPassword);
                }
            }

            return result;
        }

        public async Task<bool> ResetUserPaswordAsync(string password, string username = null)
        {
            bool result = false;

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            if (string.IsNullOrEmpty(username))
                throw new InvalidParameterException("username");

            if (string.IsNullOrEmpty(password))
                throw new InvalidParameterException("password");

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username.ToLower();

            password = await HashPasswordAsync(password);

            result = await Database.ResetUserPaswordAsync(username, password);

            return result;
        }

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            bool result = false;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            if (string.IsNullOrEmpty(username))
                throw new InvalidParameterException("username");

            if (string.IsNullOrEmpty(username))
                throw new InvalidParameterException("password");

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            User o = await Database.GetUserByUsernameAsync(username);

            if (o != null)
            {
                password = await HashPasswordAsync(password);

                result = o.Password == password;
            }

            return result;
        }

        public async Task<bool> AuthenticateAsync(string username, string loginProvider, string loginProviderKey)
        {
            bool result = false;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            if (string.IsNullOrEmpty(username))
                throw new InvalidParameterException("username");

            if (string.IsNullOrEmpty(loginProvider))
                throw new InvalidParameterException("loginProvider");

            if (string.IsNullOrEmpty(loginProviderKey))
                throw new InvalidParameterException("loginProviderKey");

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            User o = await Database.GetUserByUsernameAsync(username);

            if (o != null)
                result = o.Provider == loginProvider && o.ProviderKey == loginProviderKey;

            return result;
        }

        public async Task<bool> EnableUserAsync(string username = null)
        {
            bool result = false;

            if (!await IsUserAdminAsync())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username.ToLower();

            result = await Database.EnableUserAsync(username, true);

            return result;
        }

        public async Task<bool> DisableUserAsync(string username = null)
        {
            bool result = false;

            if (!await IsUserAdminAsync())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username.ToLower();

            result = await Database.EnableUserAsync(username, false);

            return result;
        }

        public async Task<bool> DeleteUserAsync(string username = null)
        {
            bool result = false;

            if (!await IsUserAdminAsync())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username.ToLower();

            User user = GetUserByUsername(username);

            if (user != null)
            {
                if (user.Enabled)
                    throw new InvalidOperationException(ErrorMessage.USER_ENABLED);

                result = await Database.DeleteUserAsync(username);
            }

            return result;
        }

        #endregion

        #region Enum

        public async Task<List<string>> GetRolesAsync()
        {
            List<string> result = new List<string>();

            result.AddRange(await Database.GetEnumValuesAsync(Enum.Role));

            return result;
        }

        public async Task<List<string>> GetCurrenciesAsync()
        {
            List<string> result = new List<string>();

            result.AddRange(await Database.GetEnumValuesAsync(Enum.Currency));

            return result;
        }

        #endregion

        #region Reports

        public async Task<List<Report.CategoryValue>> GetMonthlyExpensesByCategoryReportAsync(uint accountId, DateTime? from, DateTime? to)
        {
            List<Report.CategoryValue> result = new List<Report.CategoryValue>();

            try
            {
                if (!await IsUserExistsAsync())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                List<string> categories = await GetTransactionCategoriesAsync(TransactionType.Expense.ToString());

                List<Transaction> transactions = await GetTransactionsAsync(from, to);

                string category = string.Empty;

                Report.CategoryValue item = null;

                foreach(Transaction i in transactions)
                {
                    if (i.Type == TransactionType.Expense)
                    {
                        category = i.Category.Split(new char[] { '.' })[0];

                        foreach (var o in result)
                        {
                            if (o.Category.Equals(category))
                            {
                                item = o;
                                break;
                            }
                        }

                        if (item != null)
                        {
                            item.Value += i.Amount;
                        }
                        else
                        {
                            item = new Report.CategoryValue() { Category = category, Value = i.Amount };

                            result.Add(item);
                        }

                        item = null;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region Settings

        public async Task<List<KeyValuePair<string, string>>> GetSettingsAsync()
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string,string>>();

            try
            {
                if (await IsUserExistsAsync(Context.Username))
                {

                }
                else
                    throw new UserNotFoundException(Context.Username);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<string> GetSettingsAsync(string key)
        {
            string result = string.Empty;

            try
            {
                if (await IsUserExistsAsync(Context.Username))
                {

                }
                else
                    throw new UserNotFoundException(Context.Username);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> CreateSettingsAsync(string key, string value)
        {
            bool result = false;

            try
            {
                if (await IsUserExistsAsync(Context.Username))
                {

                }
                else
                    throw new UserNotFoundException(Context.Username);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> UpdateSettingsAsync(string key, string value)
        {
            bool result = false;

            try
            {
                if (await IsUserExistsAsync(Context.Username))
                {

                }
                else
                    throw new UserNotFoundException(Context.Username);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> DeleteSettingsAsync(string key)
        {
            bool result = false;

            try
            {
                if (await IsUserExistsAsync(Context.Username))
                {

                }
                else
                    throw new UserNotFoundException(Context.Username);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region Misc

        public async Task<string> HashPasswordAsync(string password)
        {
            return await HashPasswordAsync(Encoding.ASCII.GetBytes(password));
        }

        public async Task<string> HashPasswordAsync(byte[] password)
        {
            string result = string.Empty;

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            if (PasswordHashIterations > 0)
            {
                result = Hasher.Default.Execute(password, HashType);

                for (int i = 1; i < PasswordHashIterations; i++)
                    result = Hasher.Default.Execute(result, HashType);

                result = result.ToUpper();
            }

            return result;
        }

        public async Task<string> HashAsync(string data)
        {
            return await HashAsyc(Encoding.ASCII.GetBytes(data));
        }

        public async Task<string> HashAsyc(byte[] data)
        {
            string result = string.Empty;

            if (!await IsUserExistsAsync())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            result = Hasher.Default.Execute(data, HashType);

            result = result.ToUpper();

            return result;
        }

        #endregion

        #region Install

        public async Task<bool> InstallAsync()
        {
            bool result = false;

            try
            {
                if (!string.IsNullOrEmpty(Version))
                    throw new InvalidOperationException(ErrorMessage.DATABASE_ALREADY_INSTALLED);

                string defaultSystemUsername = Constants.User.SYSTEM;
                string defaultSystemPassword = GetRandomPassword(64);
                Role defaultSystemRole = Role.System;

                string defaultPublicUsername = Constants.User.ANONYMOUS;
                string defaultPublicPassword = GetRandomPassword(64);
                Role defaultPublicRole = Role.User;

                string defaultAdminUsername = Constants.User.ADMINISTRATOR;
                string defaultAdminPassword = Constants.User.PASSWORD;
                Role defaultAdminRole = Role.Administrator;

                #region Settings

                // Create System User

                await Database.CreateUserAsync(true, defaultSystemUsername, defaultSystemPassword, null, null, null, null);

                await Database.SetInternalUserAsync(defaultSystemUsername);

                await Database.AddUserToRoleAsync(defaultSystemUsername, defaultSystemRole.ToString());

                // Create System Settings

                await Database.CreateSettingsAsync("system.version", Assembly.GetExecutingAssembly().GetName().Version.ToString());

                await Database.CreateSettingsAsync("system.name", "Param");

                await Database.CreateSettingsAsync("system.shortname", "Param");

                await Database.CreateSettingsAsync("system.copyright", "Copyright © Paramesh Gunasekaran, 2015 - 2016.");

                // Create Mail Settings

                await Database.CreateSettingsAsync("mail.enabled", "False");

                await Database.CreateSettingsAsync("mail.server", "localhost");

                await Database.CreateSettingsAsync("mail.port", "25");

                await Database.CreateSettingsAsync("mail.from", "noreply@localhost");

                await Database.CreateSettingsAsync("mail.username", "noreply");

                await Database.CreateSettingsAsync("mail.password", "password");

                // Create Enum Values

                foreach (string i in GetEnumValues(typeof(Role)))
                    await Database.AddEnumValueAsync(Enum.Role, i);

                foreach (string i in GetEnumValues(typeof(Currency)))
                    await Database.AddEnumValueAsync(Enum.Currency, i);

                await Database.AddEnumValueAsync(Enum.Type, "Income");
                await Database.AddEnumValueAsync(Enum.Type, "Expense");
                await Database.AddEnumValueAsync(Enum.Type, "Transfer");

                await Database.AddEnumValueAsync(GetCategoryKey("Income"), "Salary");
                await Database.AddEnumValueAsync(GetCategoryKey("Income", "Salary"), "Salary");

                await Database.AddEnumValueAsync(GetCategoryKey("Expense"), "Travel");
                await Database.AddEnumValueAsync(GetCategoryKey("Expense", "Travel"), "Bus");
                await Database.AddEnumValueAsync(GetCategoryKey("Expense", "Travel"), "Train");
                await Database.AddEnumValueAsync(GetCategoryKey("Expense", "Travel"), "Car");
                await Database.AddEnumValueAsync(GetCategoryKey("Expense", "Travel"), "Air");

                await Database.AddEnumValueAsync(GetCategoryKey("Expense"), "Utility");
                await Database.AddEnumValueAsync(GetCategoryKey("Expense", "Utility"), "Power");
                await Database.AddEnumValueAsync(GetCategoryKey("Expense", "Utility"), "Gas");
                await Database.AddEnumValueAsync(GetCategoryKey("Expense", "Utility"), "Water");
                await Database.AddEnumValueAsync(GetCategoryKey("Expense", "Utility"), "Internet");
                await Database.AddEnumValueAsync(GetCategoryKey("Expense", "Utility"), "Mobile");

                await Database.AddEnumValueAsync(GetCategoryKey("Expense"), "Tax");
                await Database.AddEnumValueAsync(GetCategoryKey("Expense", "Tax"), "IncomeTax");

                #region Create Users

                // Create Anonymous Internal User and Assign Role
                {
                    await Database.CreateUserAsync(true, defaultPublicUsername, defaultPublicPassword, "Anonymous", "User", null, null);

                    await Database.SetInternalUserAsync(defaultPublicUsername);

                    await Database.AddUserToRoleAsync(defaultPublicUsername, defaultPublicRole.ToString());
                }

                // Create Admin User, Assign Role and Add Categories, Sub-Categories to User Settings
                {
                    defaultAdminUsername = string.IsNullOrEmpty(defaultAdminUsername) ? null : defaultAdminUsername.ToLower();

                    defaultAdminPassword = await HashPasswordAsync(defaultAdminPassword);

                    Database.CreateUser(false, defaultAdminUsername, defaultAdminPassword, "System", "Administrator", "root@localhost", "1234567890");

                    await Database.EnableUserAsync(defaultAdminUsername, true);

                    foreach (string type in await Database.GetEnumValuesAsync(Enum.Type))
                    {
                        List<string> categories = new List<string>();

                        categories.AddRange(await Database.GetEnumValuesAsync(GetCategoryKey(type)));

                        string json = await Serializer<List<string>>.Current.SerializeToJsonAsync(categories);

                        await Database.CreateUserSettingsAsync(defaultAdminUsername, GetCategoryKey(type), json);

                        foreach (string category in categories)
                        {
                            List<string> subCategories = new List<string>();

                            subCategories.AddRange(await Database.GetEnumValuesAsync(GetCategoryKey(type, category)));

                            json = await Serializer<List<string>>.Current.SerializeToJsonAsync(subCategories);

                            await Database.CreateUserSettingsAsync(defaultAdminUsername, GetCategoryKey(type, category), json);
                        }
                    }

                    await Database.AddUserToRoleAsync(defaultAdminUsername, defaultAdminRole.ToString());

                    await Database.CreateAccountAsync(await GetNextIdAsync(ID.ACCOUNT), defaultAdminUsername, "Bank", "Default Account", "USD");
                }

                #endregion

                #endregion
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> UninstallAsync()
        {
            bool result = false;

            try
            {
                if (!await IsUserAdminAsync())
                    throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

                result = await Database.PurgeAsync();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region Helpers

        private async Task<uint> GetNextIdAsync(string name)
        {
            uint result = 0;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            result = await Database.GetNextIdAsync(name);

            return result;
        }

        private async Task<List<string>> GetEnumValuesAsync(Type o)
        {
            List<string> result = new List<string>();

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            foreach (var i in o.GetEnumValues())
                result.Add(i.ToString());

            return result;
        }

        private async Task<string> GetRandomPasswordAsync(int n)
        {
            StringBuilder result = new StringBuilder();

            Random rnd = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < n; i++)
            {
                result.Append(((char)rnd.Next(33, 126)).ToString());

                Thread.Sleep(10);
            }

            return result.ToString();
        }

        #endregion
    }
}