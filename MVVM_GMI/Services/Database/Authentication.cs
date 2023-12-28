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
using @user = MVVM_GMI.Services.UserProfileService;
using @online = MVVM_GMI.Helpers.OnlineRequest;
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
            //ConfigurationService.Instance.PropertiesExist();
        }

        internal Membership GetMembership(string Username)
        {
            var x = online.GetFromDatabaseAsync<UserCredential>("UserData", Username).Result;
            return x.UserMembership;
        }

        internal void UpdateMembership(string Username, bool QualifiedMember, bool hasError, bool hasSubmitted, bool isWelcomed)
        {
            var x = online.GetFromDatabaseAsync<UserCredential>("UserData", Username).Result;

            var newMem = new Membership()
            {
                QualifiedMember = QualifiedMember,
                hasErrorResponse = hasError,
                hasSubmitted = hasSubmitted,
                isWelcomed = isWelcomed
            };

            var update = new UserCredential()
            {
                Name = x.Name,
                InviteCodeUsed = x.InviteCodeUsed,
                HashedPW = x.HashedPW,
                Pepper = x.Pepper,
                UserMembership = newMem
            };

            online.WriteToDatabaseAsync("UserData",Username,update);
        }

        internal async Task<bool> SubmitMembershipRequestAsync(string Username, string Reference, string Email)
        {
            var x = new MembershipRequest()
            {
                Email = Email,
                ReferenceCode = Reference,
                Username = Username,
            };

            try
            {
                await online.WriteToDatabaseAsync("MembershipRequest", Username, x);
                UpdateMembership(Username,false,false,true,false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the membership request of the user (if it exists)
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        internal MembershipRequest? GetMembershipRequest(string Username)
        {
            return online.GetFromDatabaseAsync<MembershipRequest>("MembershipRequest", Username).Result;
        }

        /// <summary>
        /// Checks if a session already exists, verifies that it isn't expired: <br/>
        /// If it is expired, or doesn't exist, redirects the user to login <br/>
        /// If it is valid, it redirects the user to the launcher.
        /// </summary>
        public string? CheckSession()
        {
            String path = Path.Combine(ConfigurationService.Instance.fromLauncher.LauncherPath, "session.tkn");

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
            File.Delete(Path.Combine(ConfigurationService.Instance.fromLauncher.LauncherPath, "session.tkn"));
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

            File.WriteAllText(Path.Combine(ConfigurationService.Instance.fromLauncher.LauncherPath, "session.tkn"), token);

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

                var mem = new Membership()
                {
                    QualifiedMember = false,
                    hasSubmitted = false,
                    isWelcomed = false,
                    hasErrorResponse = false,
                };

                var x = Pepper();
                var data = new UserCredential()
                {
                    Name = Username,
                    HashedPW = GetSha512Hash(x + Password + Salt),
                    Pepper = x,
                    InviteCodeUsed = Code,
                    UserMembership = mem,
                    
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
                var x = online.GetFromDatabaseAsync<UserCredential>("UserData", Username).Result;
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

        [FirestoreProperty]
        public Membership UserMembership {  get; set; }

    }

    [FirestoreData]
    internal class Membership
    {
        [FirestoreProperty]
        public bool QualifiedMember { get; set; }

        [FirestoreProperty]
        public bool hasErrorResponse { get; set; }

        [FirestoreProperty]
        public bool isWelcomed { get; set; }

        [FirestoreProperty]
        public bool hasSubmitted { get; set; }
    }

    [FirestoreData]
    internal class MembershipRequest
    {
        [FirestoreProperty]
        public string Username { get; set; }

        [FirestoreProperty]
        public string ReferenceCode { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }

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
