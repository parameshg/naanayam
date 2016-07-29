using AutoMapper;
using MongoDB.Driver;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naanayam.Data
{
    public partial class Database : IDatabase
    {
        #region Account

        public async Task<List<Account>> GetAccountsAsync(string accountUsername)
        {
            List<Account> result = new List<Account>();

            try
            {
                var filter = Builders<Entity.Account>.Filter.Empty;

                var cursor = await db.GetCollection<Entity.Account>(Collection.ACCOUNT).FindAsync(filter);

                await cursor.ForEachAsync(i => { result.Add(Mapper.Map<Entity.Account, Account>(i)); });
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> CreateAccountAsync(uint accountId, string accountUsername, string accountName, string accountDescription, string accountCurrency)
        {
            bool result = false;

            try
            {
                await db.GetCollection<Entity.Account>(Collection.ACCOUNT).InsertOneAsync(new Entity.Account()
                {
                    ID = accountId,
                    Username = accountUsername,
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

        public async Task<bool> UpdateAccountAsync(uint accountId, string accountName, string accountDescription, string accountCurrency)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.Account>.Filter.Eq(i => i.ID, accountId);

                var update = Builders<Entity.Account>.Update
                            .Set(i => i.Name, accountName)
                            .Set(i => i.Description, accountDescription)
                            .Set(i => i.Currency, accountCurrency);

                UpdateResult x = await db.GetCollection<Entity.Account>(Collection.ACCOUNT).UpdateOneAsync(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> DeleteAccountAsync(uint accountId)
        {
            bool result = false;

            try
            {
                DeleteResult x = await db.GetCollection<Entity.Account>(Collection.ACCOUNT).DeleteOneAsync<Entity.Account>(i => i.ID.Equals(accountId));

                result = x.IsAcknowledged;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region Transaction

        public async Task<List<Transaction>> GetTransactionsAsync(uint accountId, DateTime? transactionDateFrom = null, DateTime? transactionDateTo = null)
        {
            List<Transaction> result = new List<Transaction>();

            try
            {
                var filter = Builders<Entity.Transaction>.Filter.Eq(i => i.Account, accountId);

                if (transactionDateFrom.HasValue && transactionDateTo.HasValue)
                {
                    var filterFromDate = Builders<Entity.Transaction>.Filter.AnyGte("timestamp", transactionDateFrom.Value);

                    var filterToDate = Builders<Entity.Transaction>.Filter.AnyLte("timestamp", transactionDateTo.Value);

                    filter = Builders<Entity.Transaction>.Filter.And(filter, filterFromDate, filterToDate);
                }

                var cursor = await db.GetCollection<Entity.Transaction>(Collection.TRANSACTION).FindAsync(filter);

                await cursor.ForEachAsync(i =>
                {
                    Transaction o = Mapper.Map<Entity.Transaction, Transaction>(i);

                    o.Amount = ((double)i.Amount) / 100.0;

                    result.Add(o);
                });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> CreateTransactionAsync(uint transactionId, string transactionUser, uint accountId, DateTime transactionDate, int transactionType, string transactionCategory, string transactionDescription, double transactionAmount)
        {
            bool result = false;

            try
            {
                await db.GetCollection<Entity.Transaction>(Collection.TRANSACTION).InsertOneAsync(new Entity.Transaction()
                {
                    ID = transactionId,
                    Username = transactionUser,
                    Account = accountId,
                    Timestamp = transactionDate,
                    Description = transactionDescription,
                    Type = transactionType,
                    Category = transactionCategory,
                    Amount = (int)(transactionAmount * 100)
                });

                result = true;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> UpdateTransactionAsync(uint transactionId, DateTime transactionDate, int transactionType, string transactionCategory, string transactionDescription, double transactionAmount)
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

                UpdateResult x = await db.GetCollection<Entity.Transaction>(Collection.TRANSACTION).UpdateOneAsync(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> DeleteTransactionAsync(uint transactionId)
        {
            bool result = false;

            try
            {
                DeleteResult x = await db.GetCollection<Entity.Transaction>(Collection.TRANSACTION).DeleteOneAsync<Entity.Transaction>(i => i.ID.Equals(transactionId));

                result = x.IsAcknowledged;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region User

        public async Task<long> GetUserCountAsync(bool showAll = false)
        {
            long result = 0;

            try
            {
                var filter = showAll ? Builders<Entity.User>.Filter.Eq(i => i.Internal, false) : Builders<Entity.User>.Filter.And(Builders<Entity.User>.Filter.Eq(i => i.Internal, false), Builders<Entity.User>.Filter.Eq(i => i.Enabled, true));

                result = await db.GetCollection<Entity.User>(Collection.USER).CountAsync(filter);
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<List<User>> GetUsersAsync(int skip = 0, int count = 1, bool showAll = false)
        {
            List<User> result = new List<User>();

            try
            {
                var filter = showAll ? Builders<Entity.User>.Filter.Eq(i => i.Internal, false) : Builders<Entity.User>.Filter.And(Builders<Entity.User>.Filter.Eq(i => i.Internal, false), Builders<Entity.User>.Filter.Eq(i => i.Enabled, true));

                filter = Builders<Entity.User>.Filter.And(filter, Builders<Entity.User>.Filter.Eq(i => i.Internal, false));

                var cursor = db.GetCollection<Entity.User>(Collection.USER).FindAsync(filter, new FindOptions<Entity.User>() { Skip = skip, Limit = count }).GetAwaiter().GetResult();

                await cursor.ForEachAsync(i => { result.Add(Mapper.Map<Entity.User, User>(i)); });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            User result = null;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var cursor = db.GetCollection<Entity.User>(Collection.USER).FindAsync(filter).GetAwaiter().GetResult();

                await cursor.ForEachAsync(i => { result = Mapper.Map<Entity.User, User>(i); });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> CreateUserAsync(bool userEnabled, string username, string password, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null)
        {
            bool result = false;

            try
            {
                await db.GetCollection<Entity.User>(Collection.USER).InsertOneAsync(new Entity.User()
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

        public async Task<bool> UpdateUserAsync(bool userEnabled, string username, string firstName, string lastName, string email, string phone, string loginProvider = null, string loginProviderKey = null)
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

                UpdateResult x = await db.GetCollection<Entity.User>(Collection.USER).UpdateOneAsync(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> AddUserToRoleAsync(string username, string role)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var update = Builders<Entity.User>.Update.AddToSet(i => i.Roles, role);

                UpdateResult x = await db.GetCollection<Entity.User>(Collection.USER).UpdateOneAsync(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> RemoveUserFromRoleAsync(string username, string role)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var update = Builders<Entity.User>.Update.Pull(i => i.Roles, role);

                UpdateResult x = await db.GetCollection<Entity.User>(Collection.USER).UpdateOneAsync(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<List<KeyValuePair<string, string>>> GetAllUserSettingsAsync(string username)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var cursor = db.GetCollection<Entity.User>(Collection.USER).FindAsync(filter).GetAwaiter().GetResult();

                await cursor.ForEachAsync(i =>
                {
                    foreach (var x in i.Settings)
                        result.Add(new KeyValuePair<string, string>(x.Key, x.Value));
                });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<string> GetUserSettingsAsync(string username, string settingsName)
        {
            string result = string.Empty;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var cursor = db.GetCollection<Entity.User>(Collection.USER).FindAsync(filter).GetAwaiter().GetResult();

                await cursor.ForEachAsync(i =>
                {
                    Entity.Settings o = i.Settings.FirstOrDefault(x => x.Key == settingsName);

                    if (o != null)
                        result = o.Value;
                });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> IsUserSettingsExistsAsync(string username, string key)
        {
            bool result = false;

            try
            {
                result = !string.IsNullOrEmpty(await GetUserSettingsAsync(username, key));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> CreateUserSettingsAsync(string username, string key, string value)
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

                UpdateResult x = await db.GetCollection<Entity.User>(Collection.USER).UpdateOneAsync(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> UpdateUserSettingsAsync(string username, string key, string value)
        {
            bool result = false;

            try
            {
                var filter = "{_id:'" + username + "', 'settings._id':'" + key + "'}";

                var update = "{$set:{'settings.$.value':'" + value + "'}}";

                UpdateResult x = await db.GetCollection<Entity.User>(Collection.USER).UpdateOneAsync(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> DeleteUserSettingsAsync(string username, string key)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var update = "{$pull:{settings:{_id:'" + key + "'}}}";

                UpdateResult x = await db.GetCollection<Entity.User>(Collection.USER).UpdateOneAsync(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> ResetUserPaswordAsync(string username, string password)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var cursor = await db.GetCollection<Entity.User>(Collection.USER).FindAsync(filter);

                Entity.User user = null;

                await cursor.ForEachAsync(i => { user = i; });

                if (user != null)
                {
                    var update = Builders<Entity.User>.Update.Set(i => i.Password, password);

                    UpdateResult x = await db.GetCollection<Entity.User>(Collection.USER).UpdateOneAsync(filter, update);

                    result = x.IsAcknowledged;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> EnableUserAsync(string username, bool userEnabled)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var update = Builders<Entity.User>.Update.Set(i => i.Enabled, userEnabled);

                UpdateResult x = await db.GetCollection<Entity.User>(Collection.USER).UpdateOneAsync(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> SetInternalUserAsync(string username)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.User>.Filter.Eq(i => i.Username, username);

                var update = Builders<Entity.User>.Update.Set(i => i.Internal, true);

                UpdateResult x = await db.GetCollection<Entity.User>(Collection.USER).UpdateOneAsync(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            bool result = false;

            try
            {
                DeleteResult x = await db.GetCollection<Entity.User>(Collection.USER).DeleteOneAsync<Entity.User>(i => i.Username.Equals(username));

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

        public async Task<List<KeyValuePair<string, string>>> GetSettingsAsync()
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            try
            {
                var filter = Builders<Entity.Settings>.Filter.Exists(i => i.Key);

                var cursor = db.GetCollection<Entity.Settings>(Collection.SETTINGS).FindAsync(filter).GetAwaiter().GetResult();

                await cursor.ForEachAsync(i => { result.Add(new KeyValuePair<string, string>(i.Key, i.Value)); });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<string> GetSettingsAsync(string key)
        {
            string result = null;

            try
            {
                var filter = Builders<Entity.Settings>.Filter.Eq(i => i.Key, key);

                var cursor = db.GetCollection<Entity.Settings>(Collection.SETTINGS).FindAsync(filter).GetAwaiter().GetResult();

                await cursor.ForEachAsync(i => { result = i.Value; });
            }
            catch(Exception e)
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
                await db.GetCollection<Entity.Settings>(Collection.SETTINGS).InsertOneAsync(new Entity.Settings()
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

        public async Task<bool> UpdateSettingsAsync(string key, string value)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.Settings>.Filter.Eq(i => i.Key, key);

                var update = Builders<Entity.Settings>.Update.Set(i => i.Value, value);

                UpdateResult x = await db.GetCollection<Entity.Settings>(Collection.SETTINGS).UpdateOneAsync(filter, update);

                result = x.IsAcknowledged;
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
                var filter = Builders<Entity.Settings>.Filter.Eq(i => i.Key, key);

                DeleteResult x = await db.GetCollection<Entity.Settings>(Collection.SETTINGS).DeleteOneAsync(filter);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        #endregion

        #region Enum

        public async Task<List<string>> GetEnumValuesAsync(string name)
        {
            List<string> result = new List<string>();

            try
            {
                var filter = Builders<Entity.Enum>.Filter.Eq(i => i.Name, name);

                var cursor = db.GetCollection<Entity.Enum>(Collection.ENUM).FindAsync(filter).GetAwaiter().GetResult();

                await cursor.ForEachAsync(i => { result = i.Values; });
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> AddEnumValueAsync(string name, string value)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.Enum>.Filter.Eq(i => i.Name, name);

                var update = Builders<Entity.Enum>.Update.AddToSet(i => i.Values, value);

                UpdateResult x = await db.GetCollection<Entity.Enum>(Collection.ENUM).UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });

                result = x.IsAcknowledged;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> RemoveEnumValueAsync(string name, string value)
        {
            bool result = false;

            try
            {
                var filter = Builders<Entity.Enum>.Filter.Eq(i => i.Name, name);

                var update = Builders<Entity.Enum>.Update.Pull(i => i.Values, value);

                UpdateResult x = await db.GetCollection<Entity.Enum>(Collection.ENUM).UpdateOneAsync(filter, update);

                result = x.IsAcknowledged;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> RemoveEnumAsync(string name)
        {
            bool result = false;

            try
            {
                DeleteResult x = await db.GetCollection<Entity.Enum>(Collection.ENUM).DeleteOneAsync<Entity.Enum>(i => i.Name.Equals(name));

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

        public async Task<uint> GetNextIdAsync(string name)
        {
            uint result = 0;

            try
            {
                var filter = Builders<Entity.Counter>.Filter.Eq(i => i.Name, name);

                var update = Builders<Entity.Counter>.Update.Inc("value", 1);

                Entity.Counter o = await Policy.Handle<NullReferenceException>().RetryAsync().ExecuteAsync<Entity.Counter>(async () =>
                {
                    Entity.Counter counter = null;

                    counter = await db.GetCollection<Entity.Counter>(Collection.COUNTER).FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Entity.Counter>() { IsUpsert = true });

                    if (counter == null)
                        throw new NullReferenceException();

                    return counter;
                });

                //Entity.Counter o = _db.GetCollection<Entity.Counter>(Collection.Counter).FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Entity.Counter>() { IsUpsert = true }).GetAwaiter().GetResult();

                result = o.Value;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> CreateIndexesAsync()
        {
            bool result = false;

            try
            {
                // Settings
                {
                    // Text Index on Settings Key

                    IndexKeysDefinition<Entity.Settings> keys = new IndexKeysDefinitionBuilder<Entity.Settings>().Combine(
                                                                 new IndexKeysDefinitionBuilder<Entity.Settings>().Text(i => i.Key));

                    await db.GetCollection<Entity.Settings>(Collection.SETTINGS).Indexes.CreateOneAsync(keys);
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

                    await db.GetCollection<Entity.User>(Collection.USER).Indexes.CreateOneAsync(keys);
                }

                result = true;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return result;
        }

        public async Task<bool> PurgeAsync()
        {
            bool result = false;

            try
            {
                await db.Client.DropDatabaseAsync(url.DatabaseName);

                result = true;
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