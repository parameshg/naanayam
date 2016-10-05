using AutoMapper;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using NLog;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Naanayam.Data
{
    public partial class Database : IDatabase
    {
        #region Circuit Breaker

        private static Policy policy;

        private static Policy CircuitBreaker
        {
            get
            {
                if (policy == null)
                    policy = Policy.Handle<ApplicationException>().CircuitBreaker(1, TimeSpan.FromSeconds(10));

                return policy;
            }
        }

        #endregion

        #region Properties

        public string Name { get { return url.DatabaseName; } }

        private Logger Log { get; set; }

        #endregion

        #region Fields

        private MongoUrlBuilder url;

        private IMongoDatabase db;
        
        #endregion

        #region .ctor

        public Database(string connection)
        {
            Log = LogManager.GetLogger("data");

            CircuitBreaker.Execute(() =>
            {
                try
                {
                    url = new MongoUrlBuilder(connection);

                    db = new MongoClient(connection).GetDatabase(url.DatabaseName);

                    ConventionPack styles = new ConventionPack();

                    styles.Add(new LowerCaseElementNameConvention());

                    ConventionRegistry.Register("conventions", styles, i => true);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Cannot connect to database using connection url '{0}'", connection);

                    throw new ApplicationException("Cannot connect to the specified database", e);
                }
            });

            #region Maps

            Mapper.Initialize(i =>
            {
                i.CreateMap<KeyValuePair<string, string>, Entity.Settings>();
                i.CreateMap<Entity.Settings, KeyValuePair<string, string>>();

                i.CreateMap<User, Entity.User>();
                i.CreateMap<Entity.User, User>();

                i.CreateMap<Account, Entity.Account>();
                i.CreateMap<Entity.Account, Account>();

                i.CreateMap<Transaction, Entity.Transaction>();
                i.CreateMap<Entity.Transaction, Transaction>();
            });

            #endregion
        }

        #endregion

        #region Account

        public List<Account> GetAccounts(string username)
        {
            List<Account> result = new List<Account>();

            try
            {
                var filter = Builders<Entity.Account>.Filter.Empty;

                foreach (var i in db.GetCollection<Entity.Account>(Collection.ACCOUNT).Find(filter).ToList())
                    result.Add(Mapper.Map<Entity.Account, Account>(i));
            }
            catch (Exception e)
            { 
                Log.Error(e);
            }

            return result;
        }

        public bool CreateAccount(uint accountId, string username, string accountName, string accountDescription, string accountCurrency)
        {
            bool result = false;

            try
            {
                db.GetCollection<Entity.Account>(Collection.ACCOUNT).InsertOne(new Entity.Account()
                {
                    ID = accountId,
                    Username = username,
                    Name = accountName,
                    Description = accountDescription,
                    Currency = accountCurrency
                });

                result = true;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool UpdateAccount(uint accountId, string accountName, string accountDescription, string accountCurrency)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.Account>.Filter.Eq(i => i.ID, accountId);

                var update = Builders<Entity.Account>.Update
                            .Set(i => i.Name, accountName)
                            .Set(i => i.Description, accountDescription)
                            .Set(i => i.Currency, accountCurrency);

                UpdateResult x = db.GetCollection<Entity.Account>(Collection.ACCOUNT).UpdateOne(filter, update);

                result = x.IsAcknowledged;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool DeleteAccount(uint accountId)
        {
            bool result = false;

            try
            {
                DeleteResult x = db.GetCollection<Entity.Account>(Collection.ACCOUNT).DeleteOne<Entity.Account>(i => i.ID.Equals(accountId));

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region Transaction

        public List<Transaction> GetTransactions(uint accountId, DateTime? transactionDateFrom = null, DateTime? transactionDateTo = null)
        {
            List<Transaction> result = new List<Transaction>();

            try
            {
                var filter = Builders<Entity.Transaction>.Filter.Empty;

                if (transactionDateFrom.HasValue && transactionDateTo.HasValue)
                {
                    var filterFromDate = Builders<Entity.Transaction>.Filter.AnyGte("timestamp", transactionDateFrom.Value);

                    var filterToDate = Builders<Entity.Transaction>.Filter.AnyLte("timestamp", transactionDateTo.Value);

                    filter = Builders<Entity.Transaction>.Filter.And(filter, filterFromDate, filterToDate);
                }

                foreach (var i in db.GetCollection<Entity.Transaction>(Collection.TRANSACTION).Find(filter).ToList())
                {
                    Transaction o = Mapper.Map<Entity.Transaction, Transaction>(i);

                    o.Amount = ((double)i.Amount) / 100.0;

                    result.Add(o);
                }
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool CreateTransaction(uint transactionId, string username, uint accountId, DateTime transactionDate, int transactionType, string transactionCategory, string transactionDescription, double transactionAmount)
        {
            bool result = false;

            try
            {
                db.GetCollection<Entity.Transaction>(Collection.TRANSACTION).InsertOne(new Entity.Transaction()
                {
                    ID = transactionId,
                    Username = username,
                    Account = accountId,
                    Timestamp = transactionDate,
                    Description = transactionDescription,
                    Type = transactionType,
                    Category = transactionCategory,
                    Amount = (int)(transactionAmount * 100)
                });

                result = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool UpdateTransaction(uint transactionId, DateTime transactionDate, int transactionType, string transactionCategory, string transactionDescription, double transactionAmount)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.Transaction>.Filter.Eq(i => i.ID, transactionId);

                var update = Builders<Entity.Transaction>.Update
                            .Set(i => i.Timestamp, transactionDate)
                            .Set(i => i.Type, transactionType)
                            .Set(i => i.Category, transactionCategory)
                            .Set(i => i.Description, transactionDescription)
                            .Set(i => i.Amount, (int)(transactionAmount * 100));

                UpdateResult x = db.GetCollection<Entity.Transaction>(Collection.TRANSACTION).UpdateOne(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool DeleteTransaction(uint transactionId)
        {
            bool result = false;

            try
            {
                DeleteResult x = db.GetCollection<Entity.Transaction>(Collection.TRANSACTION).DeleteOne<Entity.Transaction>(i => i.ID.Equals(transactionId));

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region User

        public long GetUserCount(bool showAll = false)
        {
            long result = 0;

            try
            {
                var filter = showAll ? Builders<Entity.User>.Filter.Eq(i => i.Internal, false) : Builders<Entity.User>.Filter.And(Builders<Entity.User>.Filter.Eq(i => i.Internal, false), Builders<Entity.User>.Filter.Eq(i => i.Enabled, true));

                result = db.GetCollection<Entity.User>(Collection.USER).Count(filter);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public List<User> GetUsers(int skip = 0, int count = 1, bool showAll = false)
        {
            List<User> result = new List<User>();

            try
            {
                var filter = showAll ? Builders<Entity.User>.Filter.Eq(i => i.Internal, false) : Builders<Entity.User>.Filter.And(Builders<Entity.User>.Filter.Eq(i => i.Internal, false), Builders<Entity.User>.Filter.Eq(i => i.Enabled, true));

                filter = Builders<Entity.User>.Filter.And(filter, Builders<Entity.User>.Filter.Eq(i => i.Internal, false));

                foreach (var i in db.GetCollection<Entity.User>(Collection.USER).Find(filter).Skip(skip).Limit(count).ToList())
                    result.Add(Mapper.Map<Entity.User, User>(i));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public User GetUserByUsername(string username)
        {
            User result = null;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var cursor = db.GetCollection<Entity.User>(Collection.USER).Find(filter);

                Entity.User entity = cursor.FirstOrDefault<Entity.User>();

                if (entity != null)
                    result = Mapper.Map<Entity.User, User>(entity);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool CreateUser(bool userEnabled, string username, string password, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null)
        {
            bool result = false;

            try
            {
                db.GetCollection<Entity.User>(Collection.USER).InsertOne(new Entity.User()
                {
                    Enabled = userEnabled,
                    Username = username,
                    Password = password,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Phone = phone,
                    Provider = loginProvider,
                    ProviderKey = loginProviderKey,
                    Internal = false
                });

                result = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool UpdateUser(bool userEnabled, string username, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var update = Builders<Entity.User>.Update
                            .Set(i => i.Enabled, userEnabled)
                            .Set(i => i.FirstName, firstName)
                            .Set(i => i.LastName, lastName)
                            .Set(i => i.Email, email)
                            .Set(i => i.Phone, phone)
                            .Set(i => i.Provider, loginProvider)
                            .Set(i => i.ProviderKey, loginProviderKey);

                UpdateResult x = db.GetCollection<Entity.User>(Collection.USER).UpdateOne(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool AddUserToRole(string username, string role)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var update = Builders<Entity.User>.Update.AddToSet(i => i.Roles, role);

                UpdateResult x = db.GetCollection<Entity.User>(Collection.USER).UpdateOne(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool RemoveUserFromRole(string username, string role)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var update = Builders<Entity.User>.Update.Pull(i => i.Roles, role);

                UpdateResult x = db.GetCollection<Entity.User>(Collection.USER).UpdateOne(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public List<KeyValuePair<string, string>> GetAllUserSettings(string username)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                foreach (var i in db.GetCollection<Entity.User>(Collection.USER).Find(filter).ToList())
                {
                    foreach (var x in i.Settings)
                        result.Add(new KeyValuePair<string, string>(x.Key, x.Value));
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public string GetUserSettings(string username, string key)
        {
            string result = string.Empty;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                foreach (var i in db.GetCollection<Entity.User>(Collection.USER).Find(filter).ToList())
                {
                    Entity.Settings o = i.Settings.FirstOrDefault(x => x.Key == key);

                    if (o != null)
                        result = o.Value;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool IsUserSettingsExists(string username, string key)
        {
            bool result = false;

            try
            {
                var filter = "{_id:'" + username + "', 'settings._id':'" + key + "'}";

                result = db.GetCollection<Entity.User>(Collection.USER).Find(filter).FirstOrDefault() != null;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool CreateUserSettings(string username, string key, string value)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var update = Builders<Entity.User>.Update.AddToSet(i => i.Settings, new Entity.Settings()
                {
                    Key = key,
                    Value = value
                });

                UpdateResult x = db.GetCollection<Entity.User>(Collection.USER).UpdateOne(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool UpdateUserSettings(string username, string key, string value)
        {
            bool result = false;

            try
            {
                var filter = "{_id:'" + username + "', 'settings._id':'" + key + "'}";

                var update = "{$set:{'settings.$.value':'" + value + "'}}";

                UpdateResult x = db.GetCollection<Entity.User>(Collection.USER).UpdateOne(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool DeleteUserSettings(string username, string key)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var update = "{$pull:{settings:{_id:'" + key + "'}}}";

                UpdateResult x = db.GetCollection<Entity.User>(Collection.USER).UpdateOne(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool ResetUserPasword(string username, string password)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                Entity.User user = db.GetCollection<Entity.User>(Collection.USER).Find(filter).FirstOrDefault();

                if (user != null)
                {
                    var update = Builders<Entity.User>.Update.Set(i => i.Password, password);

                    UpdateResult x = db.GetCollection<Entity.User>(Collection.USER).UpdateOne(filter, update);

                    result = x.IsAcknowledged;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool EnableUser(string username, bool userEnabled)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var update = Builders<Entity.User>.Update.Set(i => i.Enabled, userEnabled);

                UpdateResult x = db.GetCollection<Entity.User>(Collection.USER).UpdateOne(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool SetInternalUser(string username)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var update = Builders<Entity.User>.Update.Set(i => i.Internal, true);

                UpdateResult x = db.GetCollection<Entity.User>(Collection.USER).UpdateOne(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool DeleteUser(string username)
        {
            bool result = false;

            try
            {
                DeleteResult x = db.GetCollection<Entity.User>(Collection.USER).DeleteOne<Entity.User>(i => i.Username.Equals(username));

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region Settings

        public List<KeyValuePair<string, string>> GetSettings()
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            try
            {
                var filter = Builders<Entity.Settings>.Filter.Exists(i => i.Key);

                var cursor = db.GetCollection<Entity.Settings>(Collection.SETTINGS).Find(filter);

                foreach (var i in cursor.ToList())
                    result.Add(new KeyValuePair<string, string>(i.Key, i.Value));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public string GetSettings(string key)
        {
            string result = null;

            try
            {
                var filter = Builders<Entity.Settings>.Filter.Eq(i => i.Key, key);

                var settings = db.GetCollection<Entity.Settings>(Collection.SETTINGS).Find(filter).SingleOrDefault();

                if (settings != null)
                    result = settings.Value;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool CreateSettings(string key, string value)
        {
            bool result = false;

            try
            {
                db.GetCollection<Entity.Settings>(Collection.SETTINGS).InsertOne(new Entity.Settings()
                {
                    Key = key,
                    Value = value,
                });

                result = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool UpdateSettings(string key, string value)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.Settings>.Filter.Eq(i => i.Key, key);

                var update = Builders<Entity.Settings>.Update.Set(i => i.Value, value);

                UpdateResult x = db.GetCollection<Entity.Settings>(Collection.SETTINGS).UpdateOne(filter, update);

                result = x.IsAcknowledged;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool DeleteSettings(string key)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.Settings>.Filter.Eq(i => i.Key, key);

                DeleteResult x = db.GetCollection<Entity.Settings>(Collection.SETTINGS).DeleteOne(filter);

                result = x.IsAcknowledged;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region Enum

        public List<string> GetEnumValues(string name)
        {
            List<string> result = new List<string>();

            try
            {
                var filter = Builders<Entity.Enum>.Filter.Eq(i => i.Name, name);

                var o = db.GetCollection<Entity.Enum>(Collection.ENUM).Find(filter).FirstOrDefault();

                if (o != null)
                    result = o.Values;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool AddEnumValue(string name, string value)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.Enum>.Filter.Eq(i => i.Name, name);

                var update = Builders<Entity.Enum>.Update.AddToSet(i => i.Values, value);

                UpdateResult x = db.GetCollection<Entity.Enum>(Collection.ENUM).UpdateOne(filter, update, new UpdateOptions() { IsUpsert = true });

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool RemoveEnumValue(string name, string value)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.Enum>.Filter.Eq(i => i.Name, name);

                var update = Builders<Entity.Enum>.Update.Pull(i => i.Values, value);

                UpdateResult x = db.GetCollection<Entity.Enum>(Collection.ENUM).UpdateOne(filter, update);

                result = x.IsAcknowledged;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool RemoveEnum(string name)
        {
            bool result = false;

            try
            {
                DeleteResult x = db.GetCollection<Entity.Enum>(Collection.ENUM).DeleteOne<Entity.Enum>(i => i.Name.Equals(name));

                result = x.IsAcknowledged;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region Misc

        public bool Purge()
        {
            bool result = false;

            try
            {
                db.Client.DropDatabase(url.DatabaseName);

                result = true;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public bool CreateIndexes()
        {
            bool result = false;

            try
            {
                // Settings
                {
                    // Text Index on Settings Key

                    IndexKeysDefinition<Entity.Settings> keys = new IndexKeysDefinitionBuilder<Entity.Settings>().Combine(
                                                                 new IndexKeysDefinitionBuilder<Entity.Settings>().Text(i => i.Key));

                    db.GetCollection<Entity.Settings>(Collection.SETTINGS).Indexes.CreateOne(keys);
                }


                // Users
                {
                    // Text Index on Username, FirstName, LastName, Email and Phone

                    IndexKeysDefinition<Entity.User> keys = new IndexKeysDefinitionBuilder<Entity.User>().Combine(
                                                                   new IndexKeysDefinitionBuilder<Entity.User>().Text(i => i.Username),
                                                                   new IndexKeysDefinitionBuilder<Entity.User>().Text(i => i.FirstName),
                                                                   new IndexKeysDefinitionBuilder<Entity.User>().Text(i => i.LastName),
                                                                   new IndexKeysDefinitionBuilder<Entity.User>().Text(i => i.Email),
                                                                   new IndexKeysDefinitionBuilder<Entity.User>().Text(i => i.Phone));

                    db.GetCollection<Entity.User>(Collection.USER).Indexes.CreateOne(keys);
                }

                result = true;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;

        }

        public uint GetNextId(string name)
        {
            uint result = 0;

            try
            {
                var filter = Builders<Entity.Counter>.Filter.Eq(i => i.Name, name);

                var update = Builders<Entity.Counter>.Update.Inc("value", 1);

                Entity.Counter o = Policy.Handle<NullReferenceException>().Retry().Execute<Entity.Counter>(() =>
                {
                    db.GetCollection<Entity.Counter>(Collection.COUNTER).UpdateOne(filter, update, new UpdateOptions() { IsUpsert = true });

                    Entity.Counter counter = db.GetCollection<Entity.Counter>(Collection.COUNTER).FindOneAndUpdate(filter, update, new FindOneAndUpdateOptions<Entity.Counter>() { IsUpsert = true });

                    if (counter == null)
                        throw new NullReferenceException();

                    return counter;
                });

                result = o.Value;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion
    }
}