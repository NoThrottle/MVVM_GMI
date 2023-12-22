using Google.Cloud.Firestore;
using MVVM_GMI.Models;
using MVVM_GMI.Services;
using Newtonsoft.Json;
using System;
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
        public static T GetFromDatabase<T>(string Collection, string Document)
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
