using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace MVVM_GMI.Helpers
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] arr, int size)
        {
            for (var i = 0; i < arr.Length / size + 1; i++)
            {
                yield return arr.Skip(i * size).Take(size);
            }
        }

        public static string CutToMaxLength(string input, int maxLength)
        {
            if (input.Length > maxLength)
            {
                return input.Substring(0, maxLength);
            }
            else
            {
                return input;
            }
        }

        internal static string? getDetailsFromLog4j(string logEventString)
        {
            var t = ParseLog4jEvent(logEventString);

            if(t == null)
            {
                return null;
            }

            var x = DateTimeOffset.FromUnixTimeMilliseconds(t.Timestamp).ToLocalTime().ToString("dd/M/yy HH:mm");

            return "[" + x + "] " + t.Logger + " - " + t.Level + " : " + t.Message;
        }


        static List<string> logsBuilder = new List<string>();

        internal static Log4jEvent? ParseLog4jEvent(string logEventString)
        {


            if (logEventString.Contains("<log4j:Event"))
            {
                logsBuilder.Clear();
                var t = logEventString.Replace("<log4j:Event", "<Event");
                logsBuilder.Add(t);
                return null;
            }
            else if (logEventString.Contains("</log4j:Event>"))
            {
                logsBuilder.Add("</Event>");
            }
            else
            {
                var m = new Regex("<log4j:").Replace(logEventString, "<");
                var n = new Regex("</log4j:").Replace(m, "</");
                logsBuilder.Add(n);
                return null;
            }

            string Event = string.Join("", logsBuilder);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Event);

            string logger = doc.DocumentElement.Attributes["logger"].Value;
            long timestamp = long.Parse(doc.DocumentElement.Attributes["timestamp"].Value);
            string level = doc.DocumentElement.Attributes["level"].Value;
            string thread = doc.DocumentElement.Attributes["thread"].Value;
            string message = doc.DocumentElement.SelectSingleNode("Message").InnerText;

            // Create and return a Log4jEvent object
            return new Log4jEvent
            {
                Timestamp = timestamp,
                Logger = logger,
                Level = level,
                Message = message,
                Thread = thread
                // Add more properties based on your log data structure
            };

        }

        internal static void RestartApplication()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location.Replace(".dll", ".exe"));
            Application.Current.Shutdown();
        }

        public static string CalculateMD5(this string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }

    internal class Log4jEvent
    {
        public long Timestamp { get; set; }
        public string Logger { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Thread { get; set; }
        // Add more properties based on your log data structure
    }
}
