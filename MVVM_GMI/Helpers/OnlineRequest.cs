﻿using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using Windows.Media.Protection.PlayReady;

namespace MVVM_GMI.Helpers
{
    public class OnlineRequest
    {

        public static class HTTP
        {
            public static async Task<HttpResponseMessage?> get(string URL, List<String[]>? header = null)
            {
                List<string> ip = new List<string>();

                foreach (var kvp in header)
                {
                    ip.Add(kvp[0] + " : " + kvp[1]);
                }

                MessageBox.Show("GET - getting " + URL + "\n with headers \n" + String.Join(Environment.NewLine, ip));

                using (HttpClient client = new HttpClient())
                {
                    try
                    {

                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Add("Accept", "application/json");

                        if (header != null)
                        {
                            foreach (var kvp in header)
                            {
                                client.DefaultRequestHeaders.Add(kvp[0], kvp[1]);
                            }
                        }

                        var res = client.GetAsync(URL).Result;

                        MessageBox.Show("GET - with value\n" + res.StatusCode + "\n" + res.Content.ReadAsStringAsync().Result);
                        return res;
                    }
                    catch (HttpRequestException ex)
                    {
                        MessageBox.Show("GET - without value " + ex.StatusCode);
                        return null;
                    }
                }
            }

            public static async Task<HttpResponseMessage> post(string URL, string Content, List<String[]>? header = null)
            {

                List<string> ip = new List<string>();

                foreach (var kvp in header)
                {
                    ip.Add(kvp[0] + " : " + kvp[1]);
                }

                MessageBox.Show("POST - posting " + URL + "\n with headers \n" + String.Join(Environment.NewLine, ip) + "\n with body \n" + Content);

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        MessageBox.Show("POST - trying");

                        client.DefaultRequestHeaders.Accept.Clear();
                        //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                        client.DefaultRequestHeaders.Add("Accept", "application/json");

                        if (header != null)
                        {
                            foreach (var kvp in header)
                            {
                                client.DefaultRequestHeaders.Add(kvp[0], kvp[1]);
                            }
                        }

                        MessageBox.Show("POST - sending");

                        var jsonContent = new StringContent(Content, Encoding.UTF8, "application/json");
                        var res = client.PostAsync(URL, jsonContent).Result;

                        MessageBox.Show("POST - with value\n" + res.StatusCode + "\n" + res.Content.ReadAsStringAsync().Result);

                        return res;
                    }
                    catch (HttpRequestException ex)
                    {
                        return null;
                    }
                }
            }
        }

        internal static async Task<string>? GetJsonAsync(string url, List<String[]>? header = null)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("UserCredential-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:102.0) Gecko/20100101 Firefox/102.0");

                    if (header != null)
                    {
                        foreach (var kvp in header)
                        {
                            client.DefaultRequestHeaders.Add(kvp[0], kvp[1]);
                        }
                    }

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

    }
    

}
