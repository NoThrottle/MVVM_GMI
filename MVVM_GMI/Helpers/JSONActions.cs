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
                else if (item["Action"].ToString() == "Download To Directory")
                {
                    await DownloadToDirectoryAsync(item);
                }
                else if (item["Action"].ToString() == "Move")
                {
                    Move(item);
                }
                else if (item["Action"].ToString() == "Create Directory")
                {
                    CreateDirectory(item);
                }
                else if (item["Action"].ToString() == "Extract to Directory")
                {
                    ExtractToDirectory(item);
                }
                else if (item["Action"].ToString() == "Write File")
                {
                    WriteFile(item);
                }
            }
        }


        void EditFile(JObject action)
        {
            string path = parsePath(action["Path"].ToString());
            Directory.CreateDirectory(path);
        }

        void CreateDirectory(JObject action)
        {
            string path = parsePath(action["Path"].ToString());
            Directory.CreateDirectory(path);
        }

        void Move(JObject action)
        {
            string source = parsePath(action["Source"].ToString());
            string destination = parsePath(action["Destination"].ToString());

            Directory.Move(source, destination);
        }

        string parsePath(string path)
        {

            if (!string.IsNullOrEmpty(path))
            {
                if (path.Contains("~"))
                {
                    var cut = path.Substring(2);
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

            string source = parsePath(action["Source"].ToString());
            string destination = parsePath(action["Destination"].ToString());

            Directory.CreateDirectory(destination);
            System.IO.Compression.ZipFile.ExtractToDirectory(source, destination,true);
        }

        void WriteText(JObject action)
        {
            string content = action["Content"].ToString();
            string path = parsePath(action["Path"].ToString());
            string filename = action["Filename"].ToString();

            Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(path, filename), content);

        }

        void WriteFile(JObject action)
        {
            string content = action["Bytes"].ToString();
            string path = parsePath(action["Path"].ToString());
            string filename = action["Filename"].ToString();

            byte[] bytes = Convert.FromBase64String(content);

            Directory.CreateDirectory(path);
            File.WriteAllBytes(Path.Combine(path, filename), bytes);

        }

        void DeleteFile(JObject action)
        {

            string path = parsePath(action["Path"].ToString());
            string filename = action["Filename"].ToString();

            File.Delete(Path.Combine(path, filename));
        }

        async Task DownloadToDirectoryAsync(JObject action)
        {
            string url = action["URL"].ToString();
            string destination = parsePath(action["Destination"].ToString());


            using (HttpClient client = new HttpClient())
            {
                try
                {
                    byte[] fileBytes = await client.GetByteArrayAsync(url);
                    await File.WriteAllBytesAsync(destination, fileBytes);
                }
                catch (HttpRequestException ex)
                {
                    throw;
                }
            }
        }

    }

    /// <summary>
    /// Document format as to how the JSON Action is stored in Firebase
    /// </summary>
    
    public class JSONActionDocument
    {
        
        public string Title { get; set; }

        
        public string JSONString { get; set; }
    }

    /// <summary>
    /// Document format for the creation of JSON Actions
    /// </summary>
    public class JSONActionModels
    {

        public class WriteText
        {
            public string Action { get; } = "Write Text";
            public string Content { get; set; }
            public string Path { get; set; }
            public string Filename { get; set; }

        }
        public class WriteFile
        {
            public string Action { get; } = "Write File";
            public string Bytes { get; set; }
            public string Path { get; set; }
            public string Filename { get; set; }

        }

        public class DeleteFile
        {
            public string Action { get; } = "Delete File";
            public string Path { get; set; }
            public string Filename { get; set; }

        }

        public class CreateDirectory
        {
            public string Action { get; } = "Create Directory";
            public string Path { get; set; }

        }

        public class Move
        {
            public string Action { get; } = "Move";
            public string Source { get; set; }
            public string Destination { get; set; }

        }

        public class ExtractToDirectory
        {
            public string Action { get; } = "Extract to Directory";
            public string Source { get; set; }
            public string Destination { get; set; }

        }

        public class DownloadToDirectory
        {
            public string Action { get; } = "Download To Directory";
            public string URL { get; set; }
            public string Destination { get; set; }

        }

        public class EditFile
        {
            public string Action { get; } = "Edit File";
            public string Path { get; set; }
            public string Filename { get; set; }

            //Java Properties
            //JSON
            //XML
            //TOML
            public string Type { get; set; }

            public List<EditFileChange> Changes { get; set; }

        }

        public class EditFileChange
        {
            /// <summary>
            /// Key to edit must be the last one in this list.
            /// </summary>
            List<string> KeyStructure {  get; set; }

            /// <summary>
            /// Value can be Text or Numbers.
            /// Make sure to encapsulate in "" all text.
            /// Booleans will br treated based on type.
            /// </summary>
            object Value { get; set; }
        }
    }
}
