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
            database.CreateTable<Person>();
            database.CreateTable<Role>();
        }

        public IEnumerable<T> GetItems<T>(long parent = -1) where T : ItemBase,  new()
        {
            lock (locker)
            {
                if(parent < 0)
                {
                    return (from i in database.Table<T>() select i).ToList();
                }
                else
                {
                    return (from i in database.Table<T>()
                            where i.ParentID == parent
                            select i).ToList();
                }
            }
        }

        public T GetItem<T>(int id) where T : ItemBase, new()
        {
            lock (locker)
            {
                return database.Table<T>().FirstOrDefault(x => x.ID == id);
            }
        }

        public long SaveItem<T>(T item) where T : ItemBase,  new()
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
                    item.ID = database.Insert(item);
                    return item.ID;
                }
            }
        }

        public int DeleteItem<T>(long id) where T : ItemBase, new()
        {
            lock (locker)
            {
                return database.Delete<T>(id);
            }
        }

        public int DeleteItem<T>(T item) where T : ItemBase, new()
        {
            lock (locker)
            {
                return database.Delete<T>(item.ID);
            }
        }
    }
}
