using System.Timers;
using MVVM_GMI.Helpers;
using Newtonsoft.Json.Linq;
using @online = MVVM_GMI.Helpers.OnlineRequest.HTTP;

namespace MVVM_GMI.Services.Database
{
    internal class SessionService
    {

        static string _sessionToken { get; set; }
        public static string sessionToken { get { return _sessionToken; } }

        static string _accessToken { get; set; }
        public static string accessToken { get { return _accessToken; } }


        public static void updateSessionToken(string _sessionToken)
        {
            SessionService._sessionToken = _sessionToken;
        }

        static System.Timers.Timer refreshActionToken { get; set; }

        public static async Task<bool> activateSessionAsync(string initialAccessToken)
        {

            _accessToken = initialAccessToken;

            try
            {
                if (refreshActionToken == null)
                {
                    refreshActionToken = new System.Timers.Timer();
                    refreshActionToken.Interval = TimeSpan.FromMinutes(3).TotalMilliseconds;
                    refreshActionToken.Elapsed += (e, s) => { TimerElapsedAsync(); };

                    refreshActionToken.Start();

                }

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message + ex.StackTrace + ex.Source);
                return false;
            }

        }

        static int refreshes = 0;
        private static async void TimerElapsedAsync()
        {

            var x = await online.get(LauncherProperties.host + "/v1/access/newToken", [["launcher-key",LauncherProperties.host],["session-token",_sessionToken]]);

            Console.WriteLine(x.Content); //DEBUG ONLY REMOVE

            if(x.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if(refreshes >= 5) 
                {
                    Console.Error.WriteLine("5 refreshes on access token");
                    //alert user                          
                }

                Thread.Sleep(2000);
                refreshes++;
                TimerElapsedAsync();
            }

            var g = JObject.Parse(x.Content.ToString());
            _accessToken = g["Access Token"].ToString();

        }
    }

}
