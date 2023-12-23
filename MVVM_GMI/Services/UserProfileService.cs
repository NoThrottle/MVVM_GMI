using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

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

    [FirestoreData]
    internal class UserProfile() 
    {

        /// <summary>
        /// Donator rank is the increment from 0-tier to the highest tier.
        /// </summary>
        [FirestoreProperty]
        public static int DonatorRank { get; set; }


        [FirestoreProperty]
        public static string Clan { get; set; }



    }
}
