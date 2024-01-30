using System.Net.Http;
using System.Security.Policy;

namespace MVVM_GMI.Helpers
{
    public class OnlineRequest
    {

        public static class HTTP
        {
            public static async Task<HttpResponseMessage> get(string URL, List<String[]>? header = null)
            {
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


                        return await client.GetAsync(URL);
                    }
                    catch (HttpRequestException ex)
                    {
                        return null;
                    }
                }
            }

            public static async Task<HttpResponseMessage> post(string URL, string Content, List<String[]>? header = null)
            {
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

                        return await client.PostAsync(URL, new StringContent(Content));
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
                        foreach(var kvp in header)
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
