using Naanayam.Data;
using Naanayam.Enum;
using Naanayam.Errors;
using Naanayam.Tools;
using NLog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Naanayam.Server
{
    public partial class Agent : IServer
    {
        #region Properties

        public string Version
        {
            get
            {
                string result = string.Empty;

                try
                {
                    result = Database.GetSettings("system.version");
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                return result;
            }
        }

        public Context Context { get; private set; }

        private IDatabase Database { get; set; }

        private Logger Log { get; set; }

        private int PasswordHashIterations { get { return 100000; } }

        private HashType PasswordHashType { get { return HashType.SHA256; } }

        private HashType HashType { get { return HashType.MD5; } }

        #endregion

        #region .ctor

        public Agent(IDatabase database, Context context)
        {
            Log = LogManager.GetLogger("server");

            Database = database;

            Context = context;
        }

        #endregion

        #region Account

        public List<Account> GetAccounts(string username = null)
        {
            List<Account> result = new List<Account>();

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                username = string.IsNullOrEmpty(username) ? Context.Username : username;

                result.AddRange(Database.GetAccounts(username));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool CreateAccount(string name, string description, string currency)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                uint id = GetNextId(ID.TRANSACTION);

                result = Database.CreateAccount(id, Context.Username, name, description, currency);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool UpdateAccount(uint id, string name, string description, string currency)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result = Database.UpdateAccount(id, name, description, currency);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool DeleteAccount(uint id)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result = Database.DeleteAccount(id);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region Transaction

        public List<Transaction> GetTransactions(DateTime? from = null, DateTime? to = null)
        {
            List<Transaction> result = new List<Transaction>();

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result.AddRange(Database.GetTransactions(Context.Account, Context.Username, from, to));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool CreateTransaction(DateTime timestamp, TransactionType type, string category, string description, double amount)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                uint id = GetNextId(ID.TRANSACTION);

                result = Database.CreateTransaction(id, Context.Username, Context.Account, timestamp, (int)type, category, description, amount);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool UpdateTransaction(uint id, DateTime timestamp, TransactionType type, string category, string description, double amount)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result = Database.UpdateTransaction(id, timestamp, (int)type, category, description, amount);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool DeleteTransaction(uint id)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result = Database.DeleteTransaction(id);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #region Category

        public List<string> GetTransactionCategories(string type)
        {
            List<string> result = new List<string>();

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                string json = GetUserSettings(GetCategoryKey(type));

                result.AddRange(Serializer<List<string>>.Current.DeserializeFromJson(json));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool AddTransactionCategory(string type, string category)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                List<string> o = new List<string>();

                o.AddRange(GetTransactionCategories(type));

                o.Add(category);

                string json = Serializer<List<string>>.Current.SerializeToJson(o);

                if (IsUserSettingsExists(GetCategoryKey(type)))
                    UpdateUserSettings(GetCategoryKey(type), json);
                else
                    CreateUserSettings(GetCategoryKey(type), json);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool RemoveTransactionCategory(string type, string category)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                // remove category

                List<string> o = new List<string>();

                o.AddRange(GetTransactionCategories(type));

                o.Remove(category);

                string json = Serializer<List<string>>.Current.SerializeToJson(o);

                UpdateUserSettings(GetCategoryKey(type), json);

                // remove its corresponding sub categories

                DeleteUserSettings(GetCategoryKey(type, category));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public List<string> GetTransactionCategories(string type, string category)
        {
            List<string> result = new List<string>();

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result.AddRange(GetTransactionCategories(type, category));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool AddTransactionCategory(string type, string category, string subCategory)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                List<string> o = new List<string>();

                o.AddRange(GetTransactionCategories(type, category));

                string json = Serializer<List<string>>.Current.SerializeToJson(o);

                if (IsUserSettingsExists(GetCategoryKey(type, category)))
                    UpdateUserSettings(GetCategoryKey(type, category), json);
                else
                    CreateUserSettings(GetCategoryKey(type, category), json);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool RemoveTransactionCategory(string type, string category, string subCategory)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                List<string> o = new List<string>();

                o.AddRange(GetTransactionCategories(type, category));

                o.Remove(subCategory);

                string json = Serializer<List<string>>.Current.SerializeToJson(o);

                if (IsUserSettingsExists(GetCategoryKey(type, category)))
                    UpdateUserSettings(GetCategoryKey(type, category), json);
                else
                    CreateUserSettings(GetCategoryKey(type, category), json);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region Type

        public List<string> GetTransactionTypes()
        {
            List<string> result = new List<string>();

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result.AddRange(Database.GetEnumValues(Enum.Type));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool AddTransactionType(string type)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result = Database.AddEnumValue(Enum.Type, type);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool RemoveTransactionType(string type)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                result = Database.RemoveEnumValue(Enum.Type, type);
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

        public long GetUserCount()
        {
            long result = 0;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            result = Database.GetUserCount();

            return result;
        }

        public List<User> GetUsers(int page = 1, int count = 10)
        {
            List<User> result = null;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            result = Database.GetUsers((page - 1) * count, count);

            return result;
        }

        public bool IsUserExists(string username = null)
        {
            bool result = false;

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            User o = Database.GetUserByUsername(username);

            result = o != null;

            return result;
        }

        public bool IsUserAdmin(string username = null)
        {
            return IsUserInRole(Role.Administrator, username) || IsUserInRole(Role.System, username);
        }

        public bool IsUserInRole(string role, string username = null)
        {
            bool result = false;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            User o = Database.GetUserByUsername(username);

            if (o != null)
                result = o.Roles.Contains(role);

            return result;
        }

        public bool IsUserInRole(Role role, string username = null)
        {
            return IsUserInRole(role.ToString(), username);
        }

        public User GetUserByUsername(string username = null)
        {
            User result = null;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.GetUserByUsername(username);

            result = result != null && result.Enabled ? result : null;

            return result;
        }

        public bool CreateUser(string username, string password, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null)
        {
            bool result = false;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            password = HashPassword(password);

            result = Database.CreateUser(false, username, password, firstName, lastName, email, phone, loginProvider, loginProviderKey);

            if (result)
            {
                foreach (string type in Database.GetEnumValues(Enum.Type))
                {
                    List<string> categories = new List<string>();

                    categories.AddRange(Database.GetEnumValues(GetCategoryKey(type)));

                    string json = Serializer<List<string>>.Current.SerializeToJson(categories);

                    Database.CreateUserSettingsAsync(username, GetCategoryKey(type), json);

                    foreach (string category in categories)
                    {
                        List<string> subCategories = new List<string>();

                        subCategories.AddRange(Database.GetEnumValues(GetCategoryKey(type, category)));

                        json = Serializer<List<string>>.Current.SerializeToJson(subCategories);

                        Database.CreateUserSettings(username, GetCategoryKey(type, category), json);
                    }
                }
            }

            return result;
        }

        public bool UpdateUser(string username, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.UpdateUser(false, username, firstName, lastName, email, phone, loginProvider, loginProviderKey);

            return result;
        }

        public bool AddUserToRole(string username, string role)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.AddUserToRole(username, role);

            return result;
        }

        public bool RemoveUserFromRole(string username, string role)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.RemoveUserFromRole(username, role);

            return result;
        }

        public List<KeyValuePair<string, string>> GetAllUserSettings(string username)
        {
            List<KeyValuePair<string, string>> result = null;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.GetAllUserSettings(username);

            return result;
        }

        public string GetUserSettings(string key, string username = null)
        {
            string result = string.Empty;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.GetUserSettings(username, key);

            return result;
        }

        public bool IsUserSettingsExists(string key, string username = null)
        {
            bool result = false;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.IsUserSettingsExists(username, key);

            return result;
        }

        public bool CreateUserSettings(string key, string value, string username = null)
        {
            bool result = false;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.CreateUserSettings(username, key, value);

            return result;
        }

        public bool UpdateUserSettings(string key, string value, string username = null)
        {
            bool result = false;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.UpdateUserSettings(username, key, value);

            return result;
        }

        public bool DeleteUserSettings(string value, string username = null)
        {
            bool result = false;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.DeleteUserSettings(username, value);

            return result;
        }

        public bool ChangeUserPasword(string oldPassword, string newPassword, string username = null)
        {
            bool result = false;

            if (string.IsNullOrEmpty(Context.Username))
                throw new InvalidParameterException("username");

            if (string.IsNullOrEmpty(oldPassword))
                throw new InvalidParameterException("oldPassword");

            if (string.IsNullOrEmpty(newPassword))
                throw new InvalidParameterException("newPassword");

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username.ToLower() : username;

            User o = Database.GetUserByUsername(username);

            if (o != null)
            {
                oldPassword = HashPassword(oldPassword);

                if (o.Password == oldPassword)
                {
                    newPassword = HashPassword(newPassword);

                    result = Database.ResetUserPasword(username, newPassword);
                }
            }

            return result;
        }

        public bool ResetUserPasword(string password, string username = null)
        {
            bool result = false;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            if (string.IsNullOrEmpty(username))
                throw new InvalidParameterException("username");

            if (string.IsNullOrEmpty(password))
                throw new InvalidParameterException("password");

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username.ToLower();

            password = HashPassword(password);

            result = Database.ResetUserPasword(username, password);

            return result;
        }

        public bool Authenticate(string username, string password)
        {
            bool result = false;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            if (string.IsNullOrEmpty(username))
                throw new InvalidParameterException("username");

            if (string.IsNullOrEmpty(username))
                throw new InvalidParameterException("password");

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            User o = Database.GetUserByUsername(username);

            if (o != null)
            {
                password = HashPassword(password);

                result = o.Password == password;
            }

            return result;
        }

        public bool Authenticate(string username, string loginProvider, string loginProviderKey)
        {
            bool result = false;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            if (string.IsNullOrEmpty(username))
                throw new InvalidParameterException("username");

            if (string.IsNullOrEmpty(loginProvider))
                throw new InvalidParameterException("loginProvider");

            if (string.IsNullOrEmpty(loginProviderKey))
                throw new InvalidParameterException("loginProviderKey");

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            User o = Database.GetUserByUsername(username);

            if (o != null)
                result = o.Provider == loginProvider && o.ProviderKey == loginProviderKey;

            return result;
        }

        public bool EnableUser(string username = null)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username.ToLower();

            result = Database.EnableUser(username, true);

            return result;
        }

        public bool DisableUser(string username = null)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username.ToLower();

            result = Database.EnableUser(username, false);

            return result;
        }

        public bool DeleteUser(string username = null)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            username = string.IsNullOrEmpty(username) ? Context.Username : username.ToLower();

            User user = GetUserByUsername(username);

            if (user != null)
            {
                if (user.Enabled)
                    throw new InvalidOperationException(ErrorMessage.USER_ENABLED);

                result = Database.DeleteUser(username);
            }

            return result;
        }

        #endregion

        #region Reports

        public List<Report.CategoryValue> GetMonthlyExpensesByCategoryReport(uint accountId, DateTime? from, DateTime? to)
        {
            List<Report.CategoryValue> result = new List<Report.CategoryValue>();

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

                List<string> categories = GetTransactionCategories(TransactionType.Expense.ToString());

                List<Transaction> transactions = GetTransactions(from, to);

                string category = string.Empty;

                Report.CategoryValue item = null;

                foreach (Transaction i in transactions)
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

        #region Enum

        public List<string> GetRoles()
        {
            List<string> result = new List<string>();

            result.AddRange(Database.GetEnumValues(Enum.Role));

            return result;
        }

        public List<string> GetCurrencies()
        {
            List<string> result = new List<string>();

            result.AddRange(Database.GetEnumValues(Enum.Currency));

            return result;
        }

        #endregion

        #region Settings

        public List<KeyValuePair<string, string>> GetSettings()
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string,string>>();

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            result.AddRange(Database.GetSettings());

            return result;
        }

        public string GetSettings(string key)
        {
            string result = string.Empty;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            result = Database.GetSettings(key);

            return result;
        }

        public bool CreateSettings(string key, string value)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            result = Database.CreateSettings(key, value);

            return result;
        }

        public bool UpdateSettings(string key, string value)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            result = Database.UpdateSettings(key, value);

            return result;
        }

        public bool DeleteSettings(string key)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

            result = Database.DeleteSettings(key);

            return result;
        }

        #endregion

        #region Install

        public bool Install()
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

                Database.CreateUser(true, defaultSystemUsername, defaultSystemPassword, null, null, null, null);

                Database.SetInternalUser(defaultSystemUsername);

                Database.AddUserToRole(defaultSystemUsername, defaultSystemRole.ToString());

                // Create System Settings

                Database.CreateSettings("system.version", Assembly.GetExecutingAssembly().GetName().Version.ToString());

                Database.CreateSettings("system.name", "Param");

                Database.CreateSettings("system.shortname", "Param");

                Database.CreateSettings("system.copyright", "Copyright © Paramesh Gunasekaran, 2015 - 2016.");

                // Create Mail Settings

                Database.CreateSettings("mail.enabled", "False");

                Database.CreateSettings("mail.server", "localhost");

                Database.CreateSettings("mail.port", "25");

                Database.CreateSettings("mail.from", "noreply@localhost");

                Database.CreateSettings("mail.username", "noreply");

                Database.CreateSettings("mail.password", "password");

                // Create Enum Values

                foreach (string i in GetEnumValues(typeof(Role)))
                    Database.AddEnumValue(Enum.Role, i);

                foreach (string i in GetEnumValues(typeof(Currency)))
                    Database.AddEnumValue(Enum.Currency, i);

                Database.AddEnumValue(Enum.Type, "Income");
                Database.AddEnumValue(Enum.Type, "Expense");
                Database.AddEnumValue(Enum.Type, "Transfer");

                Database.AddEnumValue(GetCategoryKey("Income"), "Salary");
                Database.AddEnumValue(GetCategoryKey("Income", "Salary"), "Salary");

                Database.AddEnumValue(GetCategoryKey("Expense"), "Travel");
                Database.AddEnumValue(GetCategoryKey("Expense", "Travel"), "Bus");
                Database.AddEnumValue(GetCategoryKey("Expense", "Travel"), "Train");
                Database.AddEnumValue(GetCategoryKey("Expense", "Travel"), "Car");
                Database.AddEnumValue(GetCategoryKey("Expense", "Travel"), "Air");

                Database.AddEnumValue(GetCategoryKey("Expense"), "Utility");
                Database.AddEnumValue(GetCategoryKey("Expense", "Utility"), "Power");
                Database.AddEnumValue(GetCategoryKey("Expense", "Utility"), "Gas");
                Database.AddEnumValue(GetCategoryKey("Expense", "Utility"), "Water");
                Database.AddEnumValue(GetCategoryKey("Expense", "Utility"), "Internet");
                Database.AddEnumValue(GetCategoryKey("Expense", "Utility"), "Mobile");

                Database.AddEnumValue(GetCategoryKey("Expense"), "Tax");
                Database.AddEnumValue(GetCategoryKey("Expense", "Tax"), "IncomeTax");

                #region Create Users

                // Create Anonymous Internal User and Assign Role
                {
                    Database.CreateUser(true, defaultPublicUsername, defaultPublicPassword, "Anonymous", "User", null, null);

                    Database.SetInternalUser(defaultPublicUsername);

                    Database.AddUserToRole(defaultPublicUsername, defaultPublicRole.ToString());
                }

                // Create Admin User, Assign Role and Add Categories, Sub-Categories to User Settings
                {
                    defaultAdminUsername = string.IsNullOrEmpty(defaultAdminUsername) ? null : defaultAdminUsername.ToLower();

                    defaultAdminPassword = HashPassword(defaultAdminPassword);

                    Database.CreateUser(false, defaultAdminUsername, defaultAdminPassword, "System", "Administrator", "root@localhost", "1234567890");

                    Database.EnableUser(defaultAdminUsername, true);

                    foreach (string type in Database.GetEnumValues(Enum.Type))
                    {
                        List<string> categories = new List<string>();

                        categories.AddRange(Database.GetEnumValues(GetCategoryKey(type)));

                        string json = Serializer<List<string>>.Current.SerializeToJson(categories);

                        Database.CreateUserSettingsAsync(defaultAdminUsername, GetCategoryKey(type), json);

                        foreach (string category in categories)
                        {
                            List<string> subCategories = new List<string>();

                            subCategories.AddRange(Database.GetEnumValues(GetCategoryKey(type, category)));

                            json = Serializer<List<string>>.Current.SerializeToJson(subCategories);

                            Database.CreateUserSettings(defaultAdminUsername, GetCategoryKey(type, category), json);
                        }
                    }

                    Database.AddUserToRole(defaultAdminUsername, defaultAdminRole.ToString());

                    Database.CreateAccount(Database.GetNextId(ID.ACCOUNT), defaultAdminUsername, "Bank", "Default Account", "USD");
                }

                #endregion

                #endregion

                result = true;
            }
            catch (Exception e)
            {
                result = false;

                Log.Error(e);
            }

            return result;
        }

        public bool Uninstall()
        {
            bool result = false;

            try
            {
                if (!IsUserAdmin())
                    throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.Username);

                result = Database.Purge();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region Misc

        public string HashPassword(string password)
        {
            return HashPassword(Encoding.ASCII.GetBytes(password));
        }

        public string HashPassword(byte[] password)
        {
            string result = string.Empty;

            if (PasswordHashIterations > 0)
            {
                result = Hasher.Default.Execute(password, HashType);

                for (int i = 1; i < PasswordHashIterations; i++)
                    result = Hasher.Default.Execute(result, HashType);

                result = result.ToUpper();
            }

            return result;
        }

        public string Hash(string data)
        {
            return Hash(Encoding.ASCII.GetBytes(data));
        }

        public string Hash(byte[] data)
        {
            string result = string.Empty;

            result = Hasher.Default.Execute(data, HashType);

            result = result.ToUpper();

            return result;
        }

        #endregion

        #region Helpers

        private uint GetNextId(string name)
        {
            uint result = 0;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.Username);

            result = Database.GetNextId(name);

            return result;
        }

        private List<string> GetEnumValues(Type o)
        {
            List<string> result = new List<string>();

            foreach (var i in o.GetEnumValues())
                result.Add(i.ToString());

            return result;
        }

        private string GetRandomPassword(int count)
        {
            StringBuilder result = new StringBuilder();

            Random rnd = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < count; i++)
            {
                result.Append(((char)rnd.Next(33, 126)).ToString());

                Thread.Sleep(10);
            }

            return result.ToString();
        }

        private string GetCategoryKey(string type)
        {
            return string.Format("{0}", type);
        }

        private string GetCategoryKey(string type, string category)
        {
            return string.Format("{0}.{1}", type, category);
        }

        #endregion
    }
}