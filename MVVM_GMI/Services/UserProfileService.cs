namespace MVVM_GMI.Services
{
    public class UserProfileService
    {

        private static readonly Lazy<UserProfileService> lazyInstance = new Lazy<UserProfileService>(() => new UserProfileService());

        public static UserProfileService Instance => lazyInstance.Value;

        private UserProfileService()
        {
            // Private constructor to prevent instantiation outside the class
        }
        

        private static string? _authorizeUsername { get; set; }
        /// <summary>
        /// Username returned by the database which matches the InstallationID and AuthKey
        /// </summary>
        public static String? AuthorizedUsername 
        {   
            get 
            {
                return _authorizeUsername;
            }             
            set 
            { 
                _authorizeUsername = value;
                Instance.GetUserProfile();
            }        
        }


        void GetUserProfile()
        {




        }



    }

    
    internal class UserProfile() 
    {

        /// <summary>
        /// Donator rank is the increment from 0-tier to the highest tier.
        /// </summary>
        
        public static int DonatorRank { get; set; }

        public static string Clan { get; set; }



    }
}
