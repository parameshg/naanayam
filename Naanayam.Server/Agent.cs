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
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                username = string.IsNullOrEmpty(username) ? Context.User : username;

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
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                uint id = GetNextId(ID.TRANSACTION);

                result = Database.CreateAccount(id, Context.User, name, description, currency);
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
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

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
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

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

        public List<Transaction> GetTransactions(uint account, DateTime? transactionDateFrom = null, DateTime? transactionDateTo = null)
        {
            List<Transaction> result = new List<Transaction>();

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                result.AddRange(Database.GetTransactions(account, transactionDateFrom, transactionDateTo));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool CreateTransaction(uint account, DateTime timestamp, TransactionType type, string category, string description, double amount)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                uint id = GetNextId(ID.TRANSACTION);

                result = Database.CreateTransaction(id, Context.User, account, timestamp, (int)type, category, description, account);
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
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

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
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                result = Database.DeleteTransaction(id);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #region Category

        public Dictionary<string, List<string>> GetTransactionCategories()
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                string json = GetUserSettings(Enum.TransactionCategory);

                foreach(var i in Serializer<Dictionary<string, List<string>>>.Current.DeserializeFromJson(json))
                    result.Add(i.Key, new List<string>(i.Value));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool AddTransactionCategory(string category)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                Dictionary<string, List<string>> o = new Dictionary<string, List<string>>();

                foreach (var i in GetTransactionCategories())
                    o.Add(i.Key, new List<string>(i.Value));

                o.Add(category, new List<string>());

                string json = Serializer<Dictionary<string, List<string>>>.Current.SerializeToJson(o);

                if (IsUserSettingsExists(Enum.TransactionCategory))
                    UpdateUserSettings(Enum.TransactionCategory, json);
                else
                    CreateUserSettings(Enum.TransactionCategory, json);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool RemoveTransactionCategory(string category)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                Dictionary<string, List<string>> o = new Dictionary<string, List<string>>();

                foreach (var i in GetTransactionCategories())
                    o.Add(i.Key, new List<string>(i.Value));

                o.Remove(category);

                string json = Serializer<Dictionary<string, List<string>>>.Current.SerializeToJson(o);

                UpdateUserSettings(Enum.TransactionCategory, json);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public List<string> GetTransactionCategories(string category)
        {
            List<string> result = new List<string>();

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                foreach(var i in GetTransactionCategories())
                {
                    if (i.Key.Equals(category))
                    {
                        result.AddRange(i.Value);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool AddTransactionCategory(string category, string subCategory)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                Dictionary<string, List<string>> o = new Dictionary<string, List<string>>();

                foreach (var i in GetTransactionCategories())
                    o.Add(i.Key, new List<string>(i.Value));

                foreach(var i in o)
                {
                    if (i.Key.Equals(category))
                    {
                        i.Value.Add(subCategory);
                        break;
                    }
                }

                string json = Serializer<Dictionary<string, List<string>>>.Current.SerializeToJson(o);

                if (IsUserSettingsExists(Enum.TransactionCategory))
                    UpdateUserSettings(Enum.TransactionCategory, json);
                else
                    CreateUserSettings(Enum.TransactionCategory, json);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool RemoveTransactionCategory(string category, string subCategory)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                Dictionary<string, List<string>> o = new Dictionary<string, List<string>>();

                foreach (var i in GetTransactionCategories())
                    o.Add(i.Key, new List<string>(i.Value));

                foreach (var i in o)
                {
                    if (i.Key.Equals(category))
                    {
                        i.Value.Remove(subCategory);
                        break;
                    }
                }

                string json = Serializer<Dictionary<string, List<string>>>.Current.SerializeToJson(o);

                if (IsUserSettingsExists(Enum.TransactionCategory))
                    UpdateUserSettings(Enum.TransactionCategory, json);
                else
                    CreateUserSettings(Enum.TransactionCategory, json);
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
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                result.AddRange(Database.GetEnumValues(Enum.TransactionType));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool AddTransactionType(string transactionType)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                result = Database.AddEnumValue(Enum.TransactionType, transactionType);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool RemoveTransactionType(string transactionType)
        {
            bool result = false;

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                result = Database.RemoveEnumValue(Enum.TransactionType, transactionType);
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
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            result = Database.GetUserCount();

            return result;
        }

        public List<User> GetUsers(int page = 1, int count = 10)
        {
            List<User> result = null;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            result = Database.GetUsers((page - 1) * count, count);

            return result;
        }

        public bool IsUserExists(string username = null)
        {
            bool result = false;

            username = string.IsNullOrEmpty(username) ? Context.User : username;

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
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            username = string.IsNullOrEmpty(username) ? Context.User : username;

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
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            username = string.IsNullOrEmpty(username) ? Context.User : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.GetUserByUsername(username);

            result = result != null && result.Enabled ? result : null;

            return result;
        }

        public bool CreateUser(string username, string password, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.User);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            password = HashPassword(password);

            result = Database.CreateUser(false, username, password, firstName, lastName, email, phone, loginProvider, loginProviderKey);

            if (result)
            {
                Dictionary<string, List<string>> category = new Dictionary<string, List<string>>();

                category.Add("Travel", new List<string>(new string[] { "Car", "Bus", "Train" }));

                category.Add("Utility", new List<string>(new string[] { "Mobile", "Internet", "Electricity" }));

                category.Add("Tax", new List<string>(new string[] { "Income Tax", "Sales Tax" }));

                string json = Serializer<Dictionary<string, List<string>>>.Current.SerializeToJson(category);

                Database.CreateUserSettings(username, Enum.TransactionCategory, json);
            }

            return result;
        }

        public bool UpdateUser(string username, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.User);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.UpdateUser(false, username, firstName, lastName, email, phone, loginProvider, loginProviderKey);

            return result;
        }

        public bool AddUserToRole(string username, string role)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.User);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.AddUserToRole(username, role);

            return result;
        }

        public bool RemoveUserFromRole(string username, string role)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.User);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.RemoveUserFromRole(username, role);

            return result;
        }

        public List<KeyValuePair<string, string>> GetAllUserSettings(string username)
        {
            List<KeyValuePair<string, string>> result = null;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            username = string.IsNullOrEmpty(username) ? Context.User : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.GetAllUserSettings(username);

            return result;
        }

        public string GetUserSettings(string key, string username = null)
        {
            string result = string.Empty;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            username = string.IsNullOrEmpty(username) ? Context.User : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.GetUserSettings(username, key);

            return result;
        }

        public bool IsUserSettingsExists(string key, string username = null)
        {
            bool result = false;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            username = string.IsNullOrEmpty(username) ? Context.User : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.IsUserSettingsExists(username, key);

            return result;
        }

        public bool CreateUserSettings(string key, string value, string username = null)
        {
            bool result = false;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            username = string.IsNullOrEmpty(username) ? Context.User : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.CreateUserSettings(username, key, value);

            return result;
        }

        public bool UpdateUserSettings(string key, string value, string username = null)
        {
            bool result = false;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            username = string.IsNullOrEmpty(username) ? Context.User : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.UpdateUserSettings(username, key, value);

            return result;
        }

        public bool DeleteUserSettings(string value, string username = null)
        {
            bool result = false;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            username = string.IsNullOrEmpty(username) ? Context.User : username;

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.DeleteUserSettings(username, value);

            return result;
        }

        public bool ChangeUserPasword(string username, string oldPassword, string newPassword)
        {
            bool result = false;

            if (string.IsNullOrEmpty(username))
                throw new InvalidParameterException("username");

            if (string.IsNullOrEmpty(oldPassword))
                throw new InvalidParameterException("oldPassword");

            if (string.IsNullOrEmpty(newPassword))
                throw new InvalidParameterException("newPassword");

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

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

        public bool ResetUserPasword(string username, string password)
        {
            bool result = false;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            if (string.IsNullOrEmpty(username))
                throw new InvalidParameterException("username");

            if (string.IsNullOrEmpty(password))
                throw new InvalidParameterException("password");

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.User);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

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
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

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
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            User o = Database.GetUserByUsername(username);

            if (o != null)
                result = o.Provider == loginProvider && o.ProviderKey == loginProviderKey;

            return result;
        }

        public bool EnableUser(string username)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.User);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.EnableUser(username, true);

            return result;
        }

        public bool DisableUser(string username)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.User);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

            result = Database.EnableUser(username, false);

            return result;
        }

        public bool DeleteUser(string username)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.User);

            username = string.IsNullOrEmpty(username) ? null : username.ToLower();

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

        public List<Report.CategoryValue> GetMonthlyExpensesByCategoryReport(uint accountId, DateTime? transactionDateFrom, DateTime? transactionDateTo)
        {
            List<Report.CategoryValue> result = new List<Report.CategoryValue>();

            try
            {
                if (!IsUserExists())
                    throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

                Dictionary<string, List<string>> categories = GetTransactionCategories();

                List<Transaction> transactions = GetTransactions(accountId, transactionDateFrom, transactionDateTo);

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
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            result.AddRange(Database.GetSettings());

            return result;
        }

        public string GetSettings(string key)
        {
            string result = string.Empty;

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            result = Database.GetSettings(key);

            return result;
        }

        public bool CreateSettings(string key, string value)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.User);

            result = Database.CreateSettings(key, value);

            return result;
        }

        public bool UpdateSettings(string key, string value)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.User);

            result = Database.UpdateSettings(key, value);

            return result;
        }

        public bool DeleteSettings(string key)
        {
            bool result = false;

            if (!IsUserAdmin())
                throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.User);

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

                    Dictionary<string, List<string>> category = new Dictionary<string, List<string>>();

                    category.Add("Travel", new List<string>(new string[] { "Car", "Bus", "Train" }));

                    category.Add("Utility", new List<string>(new string[] { "Mobile", "Internet", "Electricity" }));

                    category.Add("Tax", new List<string>(new string[] { "Income Tax", "Sales Tax" }));

                    string json = Serializer<Dictionary<string, List<string>>>.Current.SerializeToJson(category);

                    Database.CreateUserSettings(Constants.User.ADMINISTRATOR, Enum.TransactionCategory, json);

                    Database.AddUserToRole(defaultAdminUsername, defaultAdminRole.ToString());

                    Database.CreateAccount(GetNextId(ID.ACCOUNT), defaultAdminUsername, "Bank", "Default Account", "USD");
                }

                #endregion

                // Create Enum Values

                foreach (string i in GetEnumValues(typeof(Role)))
                    Database.AddEnumValue(Enum.Role, i);

                foreach (string i in GetEnumValues(typeof(Currency)))
                    Database.AddEnumValue(Enum.Currency, i);

                Database.AddEnumValue(Enum.TransactionType, "Income");
                Database.AddEnumValue(Enum.TransactionType, "Expense");
                Database.AddEnumValue(Enum.TransactionType, "Transfer");

                #endregion
            }
            catch (Exception e)
            {
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
                    throw new InsufficientPrevilegeException(ErrorMessage.INSUFFICIENT_USER_PREVILEGE, Context.User);

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

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

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

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

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
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            result = Database.GetNextId(name);

            return result;
        }

        private List<string> GetEnumValues(Type o)
        {
            List<string> result = new List<string>();

            if (!IsUserExists())
                throw new UserNotFoundException(ErrorMessage.USER_NOT_FOUND, Context.User);

            foreach (var i in o.GetEnumValues())
                result.Add(i.ToString());

            return result;
        }

        private string GetRandomPassword(int n)
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