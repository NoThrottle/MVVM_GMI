using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using @ext = MVVM_GMI.Helpers.Extensions;
using @online = MVVM_GMI.Helpers.OnlineRequest;
using @user = MVVM_GMI.Services.UserProfileService;

namespace MVVM_GMI.Services.Database
{
    public static class API
    {
        private static string? RSA_PUBLIC_KEY;

        public static class Auth
        {
            static string apiHost = LauncherProperties.host + "/v1/auth";

            static string[] tokenHeader = GetTokenHeader();
            static string[] GetTokenHeader()
            {
                return ["session-token", SessionService.sessionToken];
            }
            //-----Login-----

            /// <summary>
            /// Attempts to login using a session token.
            /// Returns true if successful, false if not.
            /// </summary>
            /// <returns></returns>
            public static async Task<bool> LoginAsync()
            {

                await EnsureRSA_Async();

                string? x = await LoginUsingSessionAsync();

                if (x != null)
                {
                    SessionService.updateSessionToken(x);
                    WriteSessionFile(x);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Logs in using a session token.
            /// Returns the session token if successful, false if not.
            /// </summary>
            static async Task<string?> LoginUsingSessionAsync()
            {
                string? token = CheckSession();

                if (token == null)
                {
                    return null;
                }

                string content = "{\"Session Token\" : \"" + token + "\"}";
                HttpResponseMessage? t = await online.HTTP.post(apiHost + "/sessionlogin", content, [LauncherProperties.LauncherKeyHeader]);

                if (t == null)
                {
                    return null;
                }

                if (t.StatusCode == HttpStatusCode.OK)
                {
                    return JObject.Parse(t.Content.ToString())["Session Token"].ToString();
                }

                return null;
            }

            /// <summary>
            /// Bool - Was the request successful?
            /// Bool 2 - Was the user able to log in?
            /// String - The error message (if the request was not successful)
            /// </summary>
            internal static async Task<APIResponse> LoginNormal(LoginCredentials credentials)
            {
                credentials.password = ext.RSAEncrypt(credentials.password, RSA_PUBLIC_KEY);

                HttpResponseMessage? t = await online.HTTP.post(
                    apiHost + "/login", 
                    JObject.FromObject(credentials).ToString() ,
                    [LauncherProperties.LauncherKeyHeader]);

                if (t == null)
                {
                    return new APIResponse()
                    {
                        Sent = false,
                        Success = false,
                        Error = null,
                    };
                }

                if (t.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    return new APIResponse()
                    {
                        Sent = true, 
                        Success = false,
                        Error = [t.Content.ToString()]

                    };
                }

                if (t.StatusCode == HttpStatusCode.BadRequest)
                {

                    return new APIResponse()
                    {
                        Sent = true,
                        Success = false,
                        Error = ["One or more fields are empty, or does not meet the criteria"]

                    };

                }

                if (t.StatusCode == HttpStatusCode.NotFound)
                {
                    return new APIResponse()
                    {
                        Sent = true,
                        Success = false,
                        Error = [t.Content.ToString()]

                    };
                }

                if (t.StatusCode == HttpStatusCode.OK)
                {
                    var g = JObject.Parse(t.Content.ToString())["Session Token"].ToString();
                    SessionService.updateSessionToken(g);
                    WriteSessionFile(g);

                    return new APIResponse()
                    {
                        Sent = true,
                        Success = true,
                        Error = null,

                    };
                }

                throw new Exception(IncorrectResponse(t));
            }

            //(bool success, string[] ErrorResponse)
            internal static async Task<APIResponse> Register(RegisterCredentials credentials)
            {

                HttpResponseMessage? t = await online.HTTP.post(
                    apiHost + "/register", 
                    JObject.FromObject(credentials).ToString(),
                    [LauncherProperties.LauncherKeyHeader]);

                if (t == null)
                {
                    return new APIResponse()
                    {
                        Sent = false,
                        Success = false,
                    };
                }

                if (t.StatusCode != HttpStatusCode.BadRequest)
                {

                    var x = JObject.Parse(t.Content.ToString());
                    var h = (JArray)x["Error"];
                    List<string> errorList = new List<string>();

                    foreach (var i in h)
                    {
                        errorList.Add(i.ToString());
                    }

                    return new APIResponse()
                    {
                        Sent = true,
                        Success = false,
                        Error = errorList.ToArray()
                    };

                }


                if (t.StatusCode == HttpStatusCode.OK)
                {
                    var g = JObject.Parse(t.Content.ToString())["Session Token"].ToString();
                    SessionService.updateSessionToken(g);
                    WriteSessionFile(g);

                    return new APIResponse()
                    {
                        Sent = true,
                        Success = true,
                    };
                }

                throw new Exception(IncorrectResponse(t));
            }

            /// <summary>
            /// Bool 1 - Was the request successful?
            /// Bool 2 - Is the user a qualified member?
            /// MembershipRequest - The membership request of the user (if the user is not a qualified member)
            /// </summary>
            /// <returns></returns>
            internal static async Task<APIResponse> Membership()
            {
                //402, 403, 404, 503, 200

                HttpResponseMessage? t = await online.HTTP.get(apiHost + "/membership", [LauncherProperties.LauncherKeyHeader,tokenHeader]);

                if (t == null) //No Response
                {
                    return new APIResponse()
                    {
                        Sent = false,
                        Success = false,
                    };
                }

                if (t.StatusCode == HttpStatusCode.PaymentRequired) //402
                {
                    Membership mem = JsonConvert.DeserializeObject<Membership>(t.Content.ToString());

                    return new APIResponse()
                    {
                        Sent = true,
                        Success = true,
                        Error = null,
                        Content = (false, mem)
                    };
                }

                if (t.StatusCode == HttpStatusCode.OK) //200
                {
                    await SessionService.activateSessionAsync(JObject.Parse(t.Content.ToString())["Access Token"].ToString());

                    return new APIResponse()
                    {
                        Sent = true,
                        Success = true,
                        Error = null,
                        Content = (true, (Membership?) null)

                    };

                }

                if (t.StatusCode == HttpStatusCode.Forbidden || t.StatusCode == HttpStatusCode.NotFound) //403 404
                {
                    return new APIResponse()
                    {
                        Sent = true,
                        Success = false,
                        Error = [t.Content.ToString()],
                        Content = (false, (Membership?)null)
                    };
                }

                if (t.StatusCode == HttpStatusCode.ServiceUnavailable) //503
                {
                    return new APIResponse()
                    {
                        Sent = true,
                        Success = false,
                        Error = [t.Content.ToString()],
                        Content = (false, (Membership?)null)
                    };
                }

                throw new Exception(IncorrectResponse(t));
            }

            /// <summary>
            /// Bool 1 - Was the request successful?
            /// Bool 2 - Was the user able to submit the payment?
            /// String - The error message (if the request was not successful)
            /// Membership - The membership of the user (if the user was able to submit the payment)
            /// </summary>
            /// <param name="payment"></param>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            internal static async Task<(bool,bool,string?,Membership?)> SubmitPayment(Payment payment)
            {
                HttpResponseMessage? t = await online.HTTP.post(
                    apiHost + "/membership/submitPayment", 
                    JObject.FromObject(payment).ToString(), 
                    [LauncherProperties.LauncherKeyHeader, tokenHeader]);

                if (t == null)
                {
                    return (false, false, null, null);
                }

                switch (t.StatusCode)
                {
                    case HttpStatusCode.OK: //200
                        return (true, true, null, JsonConvert.DeserializeObject<Membership>(t.ToString()));

                    case HttpStatusCode.Accepted: //202
                        return (true, false, t.Content.ToString(), null);

                    case HttpStatusCode.BadRequest: //400
                        return (false, false, t.Content.ToString(), null);

                    case HttpStatusCode.Forbidden: //403
                        return (false, false, t.Content.ToString(), null);

                    case HttpStatusCode.ServiceUnavailable: //503
                        return (false, false, t.Content.ToString(), null);

                    default:
                        throw new Exception(IncorrectResponse(t));
                }

            }

            public static async Task<bool> Welcome()
            {
                HttpResponseMessage? t = await online.HTTP.get(apiHost + "/membership/welcome", [LauncherProperties.LauncherKeyHeader, tokenHeader]);

                if (t != null && t.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }

                return false;

            }

            static async Task<bool> EnsureRSA_Async()
            {

                if (RSA_PUBLIC_KEY != null)
                {
                    return true;
                }

                HttpResponseMessage? t = await online.HTTP.get(apiHost + "/publicKey", [LauncherProperties.LauncherKeyHeader]);

                if (t == null)
                {
                    return false;
                }

                if (t.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }

                JObject obj = JObject.Parse(t.Content.ToString());

                string key = obj["Key"].ToString();
                string sig = obj["Signature"].ToString();

                using (var rsa = new RSACryptoServiceProvider())
                {

                    rsa.ImportFromPem(key);

                    byte[] dataBytes = Encoding.UTF8.GetBytes(key);
                    byte[] signature = Encoding.UTF8.GetBytes(sig);
                    byte[] hash = SHA256.Create().ComputeHash(dataBytes);

                    if (rsa.VerifyHash(hash, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                    {
                        RSA_PUBLIC_KEY = key;
                        return true;
                    }

                }

                return false;
            }


        }

        public static class Profile
        {

            private static string apiHost = LauncherProperties.host + "/v1/profile";

            static string[] accessTokenHeader = GetTokenHeader();
            static string[] usernameHeader = GetUsername();
            static string[] GetTokenHeader()
            {
                return ["session-token", SessionService.sessionToken];
            }
            static string[] GetUsername()
            {
                return ["username", UserProfileService.AuthorizedUsername ?? throw new Exception("No Username")];
            }

            internal static async Task<APIResponse> CreateInviteCodeAsync()
            {

                HttpResponseMessage? t = await online.HTTP.get(
                    apiHost + "/createInviteCode",
                    [LauncherProperties.LauncherKeyHeader, accessTokenHeader, usernameHeader]);

                if (t == null ) // No Response
                {
                    return new APIResponse()
                    {
                        Sent = false,
                        Success = false,
                    };
                }

                if (t.StatusCode == HttpStatusCode.Forbidden) // 403
                {
                    return new APIResponse()
                    {
                        Sent = true,
                        Success = false,
                        Error = [t.Content.ToString()],
                    };
                }

                if (t.StatusCode == HttpStatusCode.ServiceUnavailable) // 503
                {
                    return new APIResponse()
                    {
                        Sent = true,
                        Success = false
                    };
                }

                if (t.StatusCode == HttpStatusCode.OK) // 200
                {
                    InviteCode code = JsonConvert.DeserializeObject<InviteCode>(t.Content.ToString());

                    return new APIResponse()
                    {
                        Sent = true,
                        Success = true,
                        Content = code
                    };
                }


                throw new Exception(IncorrectResponse(t));
            }

            //Get Invited Users
            //Get User Profile
            //Update Email
        }

        public static class Launcher
        {
            //Mods List
            //Minecraft Info
            //Launcher Info
        }

        /// <summary>
        /// Checks if a session already exists, returns the token if it does.
        /// </summary>
        static string? CheckSession()
        {
            String path = Path.Combine(ConfigurationService.Instance.fromLauncher.LauncherPath, "session.tkn");

            if (File.Exists(path)){

                string readText = File.ReadAllText(path);
                if (!string.IsNullOrEmpty(readText))
                {
                    return readText;
                }
            }

            return null;
        }

        public static void LogOut()
        {
            File.Delete(Path.Combine(ConfigurationService.Instance.fromLauncher.LauncherPath, "session.tkn"));
            user.AuthorizedUsername = null;
        }

        /// <summary>
        /// Updates the User Profile Service
        /// </summary>
        /// <param name="Username"></param>
        static void UpdateUserProfileService(string Username)
        {
            user.AuthorizedUsername = Username;
        }

        static void WriteSessionFile(String token)
        {
            File.WriteAllText(Path.Combine(ConfigurationService.Instance.fromLauncher.LauncherPath, "session.tkn"),
                token);
        }


        //public async Task<string[]?> CreateInviteCode()
        //{

        //    return [code, DateTimeOffset.FromUnixTimeSeconds(c.DateExpiry).ToLocalTime().ToString("dd/MM/yyyy h:mm tt")];
        //}


        static string IncorrectResponse(HttpResponseMessage t)
        {
            return "API returned an incorrect response: \n" + "Status Code:" + t.StatusCode + "\n" + "Message: \n" + t.Content;
        }


    }

    internal class InviteCode
    {
        [JsonProperty("Invite Code")]
        public long Code { get; set; }
        
        public long Expiry { get; set; }

    }
    
    internal class SessionToken
    {
        
        public string Token { get; set; }

        public long Expiry { get; set; }

    }

    
    internal class Invited
    {
        
        public string Username { get; set; }

        public string Code { get; set; }
        
        public long Date { get; set; }

    }

    internal class LoginCredentials
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    internal class RegisterCredentials
    {
        public string username { get; set; }
        public string password { get; set; }
        public string inviteCode { get; set; }
    }

    internal class Payment
    {
        [JsonProperty("Reference Code")]
        public string ReferenceCode { get; set; }

        public string Email { get; set; }
    }

    internal class Membership
    {
        [JsonProperty("Membership Status")]
        public MembershipStatus UserMembership { get; set; }

        [JsonProperty("Membership Request")]
        public MembershipRequest UserMembershipRequest { get; set; }
    }

    internal class MembershipStatus
    {

        public bool QualifiedMember { get; set; }

        public bool hasErrorResponse { get; set; }

        public bool isWelcomed { get; set; }

        public bool hasSubmitted { get; set; }
    }

    internal class MembershipRequest
    {
        public string Username { get; set; }

        public string ReferenceCode { get; set; }

        public string Email { get; set; }

    }

    /// <summary>
    /// Universal API Response object for all API requests.
    /// </summary>
    internal class APIResponse
    {
        /// <summary>
        /// Was the request sent? Will only be false due to an internet error <br/>
        /// Did the server respond?
        /// </summary>
        public bool Sent { get; set; }

        /// <summary>
        /// Was the request successful? Will only be false due to an incorrect/unauthorized request <br/>
        /// Did the server respond with a success code? Or as expected?
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error message if the request was not successful
        /// </summary>
        public string[]? Error {
            get
            {
                return Error ?? ["Request was not set due to an unknown reason"];
            } 
            set
            {
                Error = value;
            }
        }

        /// <summary>
        /// Content of the response if the request was successful
        /// </summary>
        public object? Content { get; set; } = null;
    }
}
