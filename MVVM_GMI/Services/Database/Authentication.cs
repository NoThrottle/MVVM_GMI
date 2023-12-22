using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Rpc;
using MVVM_GMI.Helpers;
using MVVM_GMI.Models;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using @from = MVVM_GMI.Services.ConfigurationService.Launcher;
using @user = MVVM_GMI.Services.UserProfileService;
using online = MVVM_GMI.Helpers.OnlineRequest;
using Google.Protobuf.WellKnownTypes;

namespace MVVM_GMI.Services.Database
{
    public class Authentication : ILauncherProperties
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
        /// Checks if a session already exists, verifies that it isn't expired: <br/>
        /// If it is expired, or doesn't exist, redirects the user to login <br/>
        /// If it is valid, it redirects the user to the launcher.
        /// </summary>
        public string? CheckSession()
        {

            String path = Path.Combine(from.LauncherPath, "session.tkn");

            if (File.Exists(path)){

                string readText = File.ReadAllText(path);
                if (readText != null)
                {
                    var x = VerifySession(readText);
                    if (x != null)
                    {
                        UpdateUserProfileService(x);
                        //Load user profile with username and go to dashboard
                        return x;
                    }
                }
            }

            //login
            return null;
        }

        public void LogOut()
        {
            File.Delete(Path.Combine(from.LauncherPath, "session.tkn"));
        }

        /// <summary>
        /// Updates the User Profile Service
        /// </summary>
        /// <param name="Username"></param>
        void UpdateUserProfileService(string Username)
        {
            user.AuthorizedUsername = Username;
        }

        async Task CreateSessionAsync(String username)
        {
            var token = GetSha512Hash(Guid.NewGuid().ToString());
            var date = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var db = FirestoreService.Database;
            var data = new SessionToken()
            {
                Token = token,
                Username = username,
                Created = date,
                Updated = date,
                Expiry = date + 604800,

            };

            DocumentReference docRef = db.Collection("SessionTokens").Document(token);
            await docRef.SetAsync(data);

            File.WriteAllText(Path.Combine(from.LauncherPath, "session.tkn"), token);

        }

        /// <summary>
        /// Verifies if a session token is valid. Returns the username if it is.
        /// </summary>
        /// <param name="sessionToken"></param>
        /// <returns></returns>
        String? VerifySession(String sessionToken)
        {
            var db = FirestoreService.Database;
            DocumentReference docRef = db.Collection("SessionTokens").Document(sessionToken);
            SessionToken token = docRef.GetSnapshotAsync().Result.ConvertTo<SessionToken>();

            if (token != null)
            {
                if(token.Expiry > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                {
                    UpdateSessionAsync(sessionToken);
                    return token.Username;
                }
            }

            return null;
        }

        /// <summary>
        /// Makes the token last longer updates the dates.
        /// </summary>
        /// <param name="sessionToken"></param>
        void UpdateSessionAsync(String sessionToken)
        {
            var db = FirestoreService.Database;
            DocumentReference docRef = db.Collection("SessionTokens").Document(sessionToken);
            SessionToken token = docRef.GetSnapshotAsync().Result.ConvertTo<SessionToken>();

            var x = new SessionToken() 
            { 
                Token = token.Token,
                Username = token.Username,
                Created = token.Created,
                Updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Expiry = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 604800,      
            
            };

            docRef.SetAsync(x);
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
                var data = new UserCredential()
                {
                    Name = Username,
                    HashedPW = GetSha512Hash(x + Password + Salt),
                    Pepper = x,
                    InviteCodeUsed = Code
                };

                _ = online.WriteToDatabaseAsync("UserData", data.Name, data);

                CreateSessionAsync(Username);
                UpdateUserProfileService(Username);

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


            try
            {
                var x = online.GetFromDatabase<UserCredential>("UserData", Username);
                if (x != null)
                {
                    if (x.HashedPW == GetSha512Hash(x.Pepper + Password + Salt))
                    {
                        CreateSessionAsync(Username);
                        UpdateUserProfileService(Username);
                        return true;
                    }
                }
            }
            catch
            {
                //No Internet or Something
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

    [FirestoreData]
    internal class SessionToken
    {
        [FirestoreProperty]
        public string Token { get; set; }

        [FirestoreProperty]
        public string Username { get; set; }

        [FirestoreProperty]
        public long Created { get; set; }

        [FirestoreProperty]
        public long Updated { get; set; }

        [FirestoreProperty]
        public long Expiry { get; set; }

    }
}
