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

namespace heres
{
    public class Settings
    {
        [PrimaryKey]
        public string Key { get; set; }
        public string Value { get; set; }
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
        private const string uriStrId = @"https://here01-1362.appspot.com/_ah/api/meeting/v1/{0}s/{1}";
        private const string uriStr = @"https://here01-1362.appspot.com/_ah/api/meeting/v1/{0}s";

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
            database.CreateTable<Settings>();

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

        public async Task<CollectionOf<T>> GetItems<T>(object parent) where T : ItemBase, new()
        {
            try
            {
                CollectionOf<T> items = null;
                var uri = new Uri(string.Format(uriStrId, typeof(T).Name.ToLower(), parent == null ? string.Empty : Uri.EscapeDataString(parent.ToString())));
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

        public async Task<long> SaveItem<T>(T item) where T : ItemBase, new()
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
                    // POST https://here01-1362.appspot.com/_ah/api/meeting/v1/meetings

                    var uri = new Uri(string.Format(uriStr, typeof(T).Name.ToLower()));
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
                        item = JsonConvert.DeserializeObject<T>(res);
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

        public async Task<int> DeleteItem<T>(long id) where T : ItemBase, new()
        {
            return database.Delete<T>(id);
        }

        public async Task<int> DeleteItem<T>(T item) where T : ItemBase, new()
        {
            return database.Delete<T>(item.ID);
        }
    }
}
