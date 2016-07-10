using heres.poco;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;

namespace heres
{
    class Database
    {
        static readonly object locker = new object();

        private readonly SQLiteConnection database;

        public Database()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            // create the tables
            database.CreateTable<Meeting>();
        }

        public IEnumerable<T> GetItems<T>() where T : new()
        {
            lock (locker)
            {
                return (from i in database.Table<T>() select i).ToList();
            }
        }

        public T GetItem<T>(int id) where T : IItem, new()
        {
            lock (locker)
            {
                return database.Table<T>().FirstOrDefault(x => x.ID == id);
            }
        }

        public long SaveItem<T>(T item) where T : IItem,  new()
        {
            lock (locker)
            {
                if (item.ID != 0)
                {
                    database.Update(item);
                    return item.ID;
                }
                else
                {
                    return database.Insert(item);
                }
            }
        }

        public int DeleteItem<T>(long id) where T : IItem, new()
        {
            lock (locker)
            {
                return database.Delete<T>(id);
            }
        }

        public int DeleteItem<T>(T item) where T : IItem, new()
        {
            lock (locker)
            {
                return database.Delete<T>(item.ID);
            }
        }
    }
}
