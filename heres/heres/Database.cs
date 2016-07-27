using heres.poco;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace heres
{
    public class Settings : IID
    {
        [PrimaryKey, AutoIncrement]
        [Newtonsoft.Json.JsonPropertyAttribute("id")]
        public long ID { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }

        public const string email = "email";
        public const string pin = "PIN";
        public const string name = "name";
    }
    /// <summary>
    /// Encapsulates local and remote database
    /// </summary>
    class Database
    {
        static readonly object locker = new object();
        private readonly SQLiteConnection database;
        private readonly HttpClient client;
        const string API_Access_Key = "AIzaSyBBkdUYzIxX9uiYpZv4un8lgq-1CyH6Vc0";
        private const string uriStr = @"https://here01-1362.appspot.com/_ah/api/{0}/v2/{0}s/{1}/{2}";
        private const string uriItem = @"https://here01-1362.appspot.com/_ah/api/{0}/v2/{0}s/{1}/{2}/{3}";

        /// <summary>
        /// Verify the local tables exist
        /// </summary>
        public Database()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            // create the tables
            database.CreateTable<Meeting>();
            database.CreateTable<Person>();
            database.CreateTable<Role>();
            // database.DropTable<Settings>();
            database.CreateTable<Settings>();
            database.CreateTable<User>();

            client = new HttpClient
            {
                MaxResponseContentBufferSize = 256000
            };
        }

        internal void SaveSettings(string key, string value)
        {
            var existing = database.Table<Settings>().FirstOrDefault(s => s.Key == key);
            if (existing != null)
            {
                existing.Value = value;
                database.Update(existing);
            }
            else
            {
                var settings = new Settings
                {
                    Key = key,
                    Value = value
                };
                database.Insert(settings);
            }
        }

        public string GetSetting(string key)
        {
            var existing = database.Table<Settings>().FirstOrDefault(s => s.Key == key);
            return existing == null ? null : existing.Value;
        }

        public IList<T> GetDBItems<T>() where T : new()
        {
            return database.Table<T>().ToList();
        }

        public async Task<CollectionOf<T>> GetItems<T>(object parent, string email, string token = null) where T : ItemBase, new()
        {
            try
            {
                CollectionOf<T> items = null;
                token = token ?? CreateToken(email);
                var uri = new Uri(string.Format(uriItem, typeof(T).Name.ToLower(), parent == null ? string.Empty : Uri.EscapeDataString(parent.ToString()), Uri.EscapeDataString(email), token));
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    items = JsonConvert.DeserializeObject<CollectionOf<T>>(content);
                }
                else
                {
                    items = new CollectionOf<T>
                    {
                        items = new List<T>()
                    };
                }
                return items;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                throw;
            }
        }

        public void SaveDBItem<T>(T item) where T : IID, new()
        {
            try
            {
                if (item.ID != 0)
                {
                    lock (locker)
                    {
                        database.Update(item);
                    }
                }
                else
                {
                    lock (locker)
                    {
                        database.Insert(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Fail");
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<long> SaveItem<T>(T item, string email, string token = null) where T : IID, new()
        {
            try
            {
                if (item.ID != 0)
                {
                    lock (locker)
                    {
                        database.Update(item);
                    }
                    return item.ID;
                }
                else
                {
                    token = token ?? CreateToken(email);
                    var uri = new Uri(string.Format(uriStr, typeof(T).Name.ToLower(), Uri.EscapeDataString(email), token));
                    var json = JsonConvert.SerializeObject(item);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;
                    if (item.ID == 0)
                    {
                        response = await client.PostAsync(uri, content);
                    }
                    else
                    {
                        response = await client.PutAsync(uri, content);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        var res = await response.Content.ReadAsStringAsync();
                        if (string.IsNullOrEmpty(res))
                        {
                            return 0;
                        }
                        item = res == null ? new T() : JsonConvert.DeserializeObject<T>(res);
                    }
                    else
                    {
                        var reason = response.ReasonPhrase;
                        var rcontent = response.Content;
                    }
                    return item.ID;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Fail");
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        internal IList<string> GetEmailAddresses()
        {
            var addresses = (from s in GetDBItems<Settings>()
                             where s.Key == Settings.email && !String.IsNullOrEmpty(s.Value)
                             select s.Value).ToList();
            return addresses;
        }

        public async Task<int> DeleteItem<T>(T item, string email, string token = null, object ID = null) where T : IID, new()
        {
            email = string.IsNullOrEmpty(email) ? PrimaryEmail : email;
            token = string.IsNullOrEmpty(token) ? CreateToken(email) : email;
            var uri = ID == null ? new Uri(string.Format(uriItem, typeof(T).Name.ToLower(),
                                                Uri.EscapeDataString(item.ID.ToString()),
                                                Uri.EscapeDataString(email),
                                                token))
                                                :
                                   new Uri(string.Format(uriStr, typeof(T).Name.ToLower(),
                                                Uri.EscapeDataString(ID.ToString()),
                                                token));

            var response = await client.DeleteAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStringAsync();
            }
            else
            {
                var reason = response.ReasonPhrase;
                var rcontent = response.Content;
            }
            return database.Delete<T>(item.ID);
        }

        public int DeleteDBItem<T>(T item) where T : IID, new()
        {
            return database.Delete<T>(item.ID);
        }

        internal string PrimaryEmail
        {
            get
            {
                {
                    var addresses = (from s in GetDBItems<Settings>()
                                     where s.Key == Settings.email && !String.IsNullOrEmpty(s.Value)
                                     select s).ToList();
                    var min = addresses.Min(s => s.ID);
                    var primaryEmail = addresses.FirstOrDefault(s => s.ID == min).Value;
                    return primaryEmail;
                }
            }
        }

        private static string CalculateMD5Hash(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }


        public string CreateToken(string email)
        {
            var source = $"{GetSetting(Settings.pin)} I'm a clown {email}";
            var hash = CalculateMD5Hash(source);
            return hash;
        }
    }
}
