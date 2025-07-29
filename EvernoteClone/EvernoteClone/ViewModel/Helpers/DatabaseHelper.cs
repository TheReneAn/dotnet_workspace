using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace EvernoteClone.ViewModel.Helpers
{
    public class DatabaseHelper
    {
        private static string dbFile = Path.Combine(Environment.CurrentDirectory, "notesDb.db3");

        public static bool Insert<T>(T item)
        {
            bool result = false;
            using(var db = new SQLite.SQLiteConnection(dbFile))
            {
                db.CreateTable<T>();
                var rowsAffected = db.Insert(item);
                result = rowsAffected > 0;
            }
            return result;
        }

        public static bool Update<T>(T item)
        {
            bool result = false;
            using (var db = new SQLite.SQLiteConnection(dbFile))
            {
                db.CreateTable<T>();
                var rowsAffected = db.Update(item);
                result = rowsAffected > 0;
            }
            return result;
        }

        public static bool Delete<T>(T item)
        {
            bool result = false;
            using (var db = new SQLite.SQLiteConnection(dbFile))
            {
                db.CreateTable<T>();
                var rowsAffected = db.Delete(item);
                result = rowsAffected > 0;
            }
            return result;
        }

        public static List<T> Read<T>() where T : new()
        {
            List<T> items = new List<T>();

            using (var db = new SQLite.SQLiteConnection(dbFile))
            {
                db.CreateTable<T>();
                items = db.Table<T>().ToList();
            }

            return items;
        }
    }
}
