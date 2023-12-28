namespace MVVM_GMI.Services
{
    internal class WebsocketService
    {

        private static readonly Lazy<WebsocketService> lazyInstance = new Lazy<WebsocketService>(() => new WebsocketService());

        public static WebsocketService Instance => lazyInstance.Value;

        private WebsocketService()
        {
            // Private constructor to prevent instantiation outside the class
        }

        /// <summary>
        /// False if ended with error, True if gracefully ended
        /// </summary>
        public event Action<bool>? Ended;

        public event Action<int>? UptimeUpdated;

        public event Action<bool>? ReAuthUser;


        private string? Authenticate()
        {

            


            return null;
        }
    }
}
