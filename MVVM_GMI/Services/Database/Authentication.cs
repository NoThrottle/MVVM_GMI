using Google.Cloud.Firestore;
using MVVM_GMI.Helpers;
using MVVM_GMI.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MVVM_GMI.Services.Database
{
    public class Authentication :ILauncherProperties
    {

        private List<UserCredential> _users = new List<UserCredential>();
        internal List<UserCredential> Users 
        { 
            get 
            {             
                return _users;          
            } 
        
        }

        public Authentication() 
        {
            
        }

        /// <summary>
        /// Creates a new Account
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <param name="Code"></param>
        /// <returns></returns>
        public List<string>? SignUp(string Username, string Password, string Code)
        {
            List<string> result = new List<string>();

            if (string.IsNullOrEmpty(Username))
            {
                result.Add("Enter a username.");
            }

            if (string.IsNullOrEmpty(Password))
            {
                result.Add("Enter a password.");
            }

            if (Username.Length < 6)
            {
                result.Add("Username must be longer than 6 characters.");
            }

            if (Username.Length > 16)
            {
                result.Add("Username must be shorter than 17 characters.");
            }

            if (!IsAlphanumericUnderscore(Username))
            {
                result.Add("Username must only contain alphanumerics and underscores.");
            }

            if (Password.Length < 8)
            {
                result.Add("Password must be at least 8 characters.");
            }

            if (Password.Length > 16)
            {
                result.Add("Password must be shorter than 17 characters.");
            }

            if(result.Count <= 0)
            {

                if (CheckIfUserAlreadyExists(Username))
                {
                    result.Add("Username already exists.");
                    return result;
                }

                var x = Pepper();
                var db = FirestoreService.Database;
                var data = new UserCredential()
                {
                    Name = Username,
                    HashedPW = GetSha512Hash(x + Password + Salt),
                    Pepper = x,
                    InviteCodeUsed = Code
                };

                DocumentReference docRef = db.Collection("UserData").Document(data.Name);
                docRef.SetAsync(data);

                return null;
            }

            return result;
        }

        public bool Login(string Username, string Password)
        {
            if(Username == null || Username.Trim().Length == 0)
            {
                return false;
            }

            if (Password == null || Password.Trim().Length == 0)
            {
                return false;
            }

            var db = FirestoreService.Database;
            DocumentReference docRef = db.Collection("UserData").Document(Username);
            UserCredential userCredential = docRef.GetSnapshotAsync().Result.ConvertTo<UserCredential>();

            if(userCredential != null)
            {
                if(userCredential.HashedPW == GetSha512Hash(userCredential.Pepper + Password + Salt))
                {
                    return true;
                }
            }

            return false;
        }


        bool CheckIfUserAlreadyExists(string Username)
        {
            var db = FirestoreService.Database;
            DocumentReference docRef = db.Collection("UserData").Document(Username);
            UserCredential userCredential = docRef.GetSnapshotAsync().Result.ConvertTo<UserCredential>();

            if (userCredential != null)
            {
                return true;

            }

            return false;
        }

        string Pepper()
        {
            Random random = new Random();
            int num = random.Next(16777216, 268435455);
            return num.ToString("X").ToLower();
        }

        string GetSha512Hash(string input)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);

                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        static bool IsAlphanumericUnderscore(string input)
        {
            // Use a regular expression to check if the string contains only letters, numbers, or underscores
            Regex regex = new Regex("^[a-zA-Z0-9_]+$");
            return regex.IsMatch(input);
        }

        void UpdateData()
        {
            _users = GetUsers();
        }

        List<UserCredential> GetUsers()
        {
            List<UserCredential> UC = new List<UserCredential>();

            var x = GetData();

            foreach(var y in x.Values)
            {
                var user = new UserCredential();
                user.Name = y[0];
                user.HashedPW = y[1];
                user.Pepper = y[2];
                user.InviteCodeUsed = y[3];

                UC.Add(user);
            }

            return _users;
        }

        DefaultSheetsModel? GetData()
        {

            try
            {
                string response = OnlineRequest.Sheets_GetRequest(SheetID, "Authentication!A2:D100", SheetsKey);
                var CONFIG_JSON_RESPONSE = JsonConvert.DeserializeObject<MVVM_GMI.Models.DefaultSheetsModel>(response);

                if (response != null)
                {
                    return CONFIG_JSON_RESPONSE;

                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " Taken Place at GetConfigList", "Error");
                return null;
            }
        }

    }

    [FirestoreData]
    internal class UserCredential
    {
        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string HashedPW { get; set; }

        [FirestoreProperty]
        public string Pepper { get; set; }

        [FirestoreProperty]
        public string InviteCodeUsed { get; set; }

    }
}
