using Google.Cloud.Firestore;
using MVVM_GMI.Models;
using MVVM_GMI.Services;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace MVVM_GMI.Helpers
{
    public class OnlineRequest
    {
        /// <summary>
        /// Gets data from FireStore and parses it into a Type. Throws an exception if it cannot find it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Collection"></param>
        /// <param name="Document"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<T> GetFromDatabaseAsync<T>(string Collection, string Document)
        {
            var db = FirestoreService.Database;
            DocumentReference docRef = db.Collection(Collection).Document(Document);
            var res = docRef.GetSnapshotAsync().Result.ConvertTo<T>();

            if (res != null)
            {
                return res;
            }

            throw new Exception();
        }

        /// <summary>
        /// Returns ALL the documents in a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Collection"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<List<T>> GetAllFromDatabaseAsync<T>(string Collection)
        {
            List<T> list = new List<T>();

            var db = FirestoreService.Database;
            var docRef = db.Collection(Collection);
            var res = await docRef.GetSnapshotAsync();

            foreach (var snap in res.Documents)
            {
                list.Add(snap.ConvertTo<T>());
            }

            if (list != null)
            {
                return list;
            }

            throw new Exception();
        }

        /// <summary>
        /// Writes to Firestore. Passes any and all errors.
        /// </summary>
        /// <param name="Collection"></param>
        /// <param name="Document"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<WriteResult> WriteToDatabaseAsync(string Collection, string Document, object data)
        {
            try
            {
                var db = FirestoreService.Database;
                DocumentReference docRef = db.Collection(Collection).Document(Document);
                return await docRef.SetAsync(data);
            }
            catch (Exception ex) 
            {
                throw;
            }            
        }

        /// <summary>
        /// Deletes from Firestore. Passes any and all errors.
        /// </summary>
        /// <param name="Collection"></param>
        /// <param name="Document"></param>
        /// <returns></returns>
        public static async Task<WriteResult> DeleteFromDatabaseAsync(string Collection, string Document)
        {
            try
            {
                var db = FirestoreService.Database;
                DocumentReference docRef = db.Collection(Collection).Document(Document);
                return await docRef.DeleteAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal static async Task<string>? GetJsonAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("UserCredential-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:102.0) Gecko/20100101 Firefox/102.0");
                    string json = await client.GetStringAsync(url);
                    return json;
                }
                catch (HttpRequestException ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Download file from url to path.
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        internal static async Task DownloadFileAsync(string fileUrl, string destinationPath)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Download the file and save it to the specified destination path
                    byte[] fileBytes = await client.GetByteArrayAsync(fileUrl);
                    await System.IO.File.WriteAllBytesAsync(destinationPath, fileBytes);
                }
                catch (HttpRequestException ex)
                {
                    throw;
                }
            }
        }

        internal static string Sheets_GetRequest(String sheetid, String range, String key)
        {
            try
            {
                System.Net.Http.HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("UserCredential-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:102.0) Gecko/20100101 Firefox/102.0");

                var rawJSON = client.GetStringAsync("https://sheets.googleapis.com/v4/spreadsheets/" + sheetid + "/values/" + range + "?key=" + key);
                Console.WriteLine("Requesting to: " + "https://sheets.googleapis.com/v4/spreadsheets/" + sheetid + "/values/" + range + "?key=" + key);

                return rawJSON.Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        internal static async Task<bool> Sheets_AppendRequestAsync(String sheetid, String range, String key, DefaultSheetsModel data)
        {
            try
            {

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("UserCredential-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:102.0) Gecko/20100101 Firefox/102.0");

                    string url = $"https://sheets.googleapis.com/v4/spreadsheets/{sheetid}/values/{range}?key={key}";

                    // Create StringContent from the JSON payload
                    StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                    // Perform the POST request
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content
                        string result = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Request successful. Response: " + result);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.StatusCode);
                        return false;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

    }
}
