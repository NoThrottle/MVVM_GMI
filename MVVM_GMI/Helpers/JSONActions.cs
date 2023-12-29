using Google.Cloud.Firestore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_GMI.Helpers
{
    internal class JSONActions
    {

        string appDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Iterates over a json to do actions
        /// </summary>
        /// <param name="JSON"></param>
        public async Task DoActionAsync(string JSON)
        {
            var x = JArray.Parse(JSON);

            foreach (JObject item in x)
            {
                if (item["Action"].ToString() == "Write Text")
                {
                    WriteText(item);
                }
                else if (item["Action"].ToString() == "Delete File")
                {
                    DeleteFile(item);
                }
                else if (item["Action"].ToString() == "Download To Location")
                {
                    await DownloadToLocationAsync(item);
                }
                else if (item["Action"].ToString() == "Move")
                {
                    Move(item);
                }
                else if (item["Action"].ToString() == "Create Directory")
                {
                    CreateDirectory(item);
                }
                else if (item["Action"].ToString() == "Extract To Directory")
                {
                    ExtractToDirectory(item);
                }
            }
        }


        void CreateDirectory(JObject action)
        {
            string path = parsePath(action["Path"].ToString());
            Directory.CreateDirectory(path);
        }

        void Move(JObject action)
        {
            string source = parsePath(action["Source Path"].ToString());
            string destination = parsePath(action["Destination Path"].ToString());

            Directory.Move(source, destination);
        }

        string parsePath(string path)
        {

            if (!string.IsNullOrEmpty(path))
            {
                if (path.Contains("%AppData%"))
                {
                    var cut = path.Substring(10);
                    return Path.Combine(appDataRoaming, cut);
                }
                else
                {
                    return path;
                }
            }

            return path;
        }

        void ExtractToDirectory(JObject action)
        {

            string source = parsePath(action["Source Path"].ToString());
            string destination = parsePath(action["Destination Path"].ToString());
            
            Directory.CreateDirectory(destination);
            System.IO.Compression.ZipFile.ExtractToDirectory(source, destination);
        }

        void WriteText(JObject action)
        {
            string content = action["Content"].ToString();
            string path = parsePath(action["Path"].ToString());
            string filename = action["Filename"].ToString();

            Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(path, filename), content);

        }

        void DeleteFile(JObject action)
        {

            string path = parsePath(action["Path"].ToString());
            string filename = action["Filename"].ToString();

            File.Delete(Path.Combine(path, filename));
        }

        async Task DownloadToLocationAsync(JObject action)
        {
            string url = action["URL"].ToString();
            string path = parsePath(action["Path"].ToString());
            string filename = action["Filename"].ToString();

            Directory.CreateDirectory(path);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    byte[] fileBytes = await client.GetByteArrayAsync(url);
                    await File.WriteAllBytesAsync(Path.Combine(path, filename), fileBytes);
                }
                catch (HttpRequestException ex)
                {
                    throw;
                }
            }
        }

    }

    [FirestoreData]
    internal class JSONActionDocument
    {
        [FirestoreProperty]
        public string Title { get; set; }

        [FirestoreProperty]
        public string JSONString { get; set; }
    }
}
