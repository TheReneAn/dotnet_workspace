using EvernoteClone.Model;
using Newtonsoft.Json;
using SQLite;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Windows;

namespace EvernoteClone.ViewModel.Helpers
{
    public class DatabaseHelper
    {
        // private static string dbFile = Path.Combine(Environment.CurrentDirectory, "notesDb.db3");
        private static string dbPath = "";

        public static async Task<bool> Insert<T>(T item)
        {
            // Local Database
            //bool result = false;
            //using (SQLiteConnection conn = new SQLiteConnection(dbFile))
            //{
            //    conn.CreateTable<T>();
            //    int rows = conn.Insert(item);
            //    if (rows > 0)
            //        result = true;
            //}
            //return result;

            // Firebase Realtime Database
            var jsonBody = JsonConvert.SerializeObject(item);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var result = await client.PostAsync($"{dbPath}{item.GetType().Name.ToLower()}.json", content);

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static async Task<bool> Update<T>(T item) where T : HasId
        {
            // Local Database
            //bool result = false;
            //using (SQLiteConnection conn = new SQLiteConnection(dbFile))
            //{
            //    conn.CreateTable<T>();
            //    int rows = conn.Update(item);
            //    if (rows > 0)
            //        result = true;
            //}
            //return result;

            // Firebase Realtime Database
            var jsonBody = JsonConvert.SerializeObject(item);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var result = await client.PatchAsync($"{dbPath}{item.GetType().Name.ToLower()}/{item.Id}.json", content);

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static async Task<bool> Delete<T>(T item) where T : HasId
        {
            // Local Database
            //bool result = false;
            //using (SQLiteConnection conn = new SQLiteConnection(dbFile))
            //{
            //    conn.CreateTable<T>();
            //    int rows = conn.Delete(item);
            //    if (rows > 0)
            //        result = true;
            //}
            //return result;

            // Firebase Realtime Database
            using (var client = new HttpClient())
            {
                var result = await client.DeleteAsync($"{dbPath}{item.GetType().Name.ToLower()}/{item.Id}.json");

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static async Task<List<T>> Read<T>() where T : HasId
        {
            // Local Database
            //List<T> items;
            //using (SQLiteConnection conn = new SQLiteConnection(dbFile))
            //{
            //    conn.CreateTable<T>();
            //    items = conn.Table<T>().ToList();
            //}
            //return items;


            // Firebase Realtime Database
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync($"{dbPath}{typeof(T).Name.ToLower()}.json");
                var jsonResult = await result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                {
                    var objects = JsonConvert.DeserializeObject<Dictionary<string, T>>(jsonResult);
                    if (objects == null)
                    {
                        // If no data exists, deserialize returns null. Return an empty list instead.
                        return new List<T>();
                    }

                    List<T> lists = new List<T>();
                    foreach (var obj in objects)
                    {
                        obj.Value.Id = obj.Key; // Set the Id from the key

                        T item = obj.Value;
                        if (item != null)
                        {
                            lists.Add(item);
                        }
                    }

                    return lists;
                }
                else
                {
                    return new List<T>();
                }
            }
        }
    }
}
