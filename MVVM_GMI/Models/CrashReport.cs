using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_GMI.Models
{
    [FirestoreData]
    public class CrashReport
    {
        [FirestoreProperty]
        public required string Contents { get; set; }
        [FirestoreProperty]
        public required string Timestamp { get; set; }

    }
}
