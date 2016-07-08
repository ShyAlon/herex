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
        static object locker = new object();

        SQLiteConnection database;

        public Database()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            // create the tables
            database.CreateTable<Meeting>();
        }

        public IEnumerable<Meeting> GetItems()
        {
            lock (locker)
            {
                return (from i in database.Table<Meeting>() select i).ToList();
            }
        }

        public IEnumerable<Meeting> GetItemsNotDone()
        {
            lock (locker)
            {
                return database.Query<Meeting>("SELECT * FROM [Meeting] WHERE [Done] = 0");
            }
        }

        public Meeting GetItem(int id)
        {
            lock (locker)
            {
                return database.Table<Meeting>().FirstOrDefault(x => x.ID == id);
            }
        }

        public long SaveItem(Meeting item)
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

        public int DeleteItem(long id)
        {
            lock (locker)
            {
                return database.Delete<Meeting>(id);
            }
        }
    }
}
