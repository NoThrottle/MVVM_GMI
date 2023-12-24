using Google.Cloud.Firestore;

namespace MVVM_GMI.Models
{
    [FirestoreData]
    public class ModEntry
    {
        [FirestoreProperty]
        public string Name { get; set; } //Title

        [FirestoreProperty]
        public string Description { get; set; } //Description

        [FirestoreProperty]
        public string IconURL { get; set; }

        [FirestoreProperty]
        public string Categories { get; set; }

        //--------//

        [FirestoreProperty]
        public bool ClientSide { get; set; }

        [FirestoreProperty]
        public bool IsRequired { get; set; }

        [FirestoreProperty]
        public bool ServerSide { get; set; }

        [FirestoreProperty]
        public string DownloadURL { get; set; }

        [FirestoreProperty]
        public string VersionNumber { get; set; }

        [FirestoreProperty]
        public string DatePublished { get; set; }

        [FirestoreProperty]
        public int Size { get; set; }

    }

    public class Mod
    {

        public string Name { get; set; } //Title

        public string Description { get; set; } //Description

        public bool ClientSide { get; set; }

        public bool ServerSide { get; set; }

        public string[] Categories { get; set; }

        public string[] Versions { get; set; }

        public string[] Loaders { get; set; }

        public string IconURL { get; set; }


    }

    public class ModVersion
    {
        public string VersionID { get; set; }

        public string DownloadURL { get; set; }

        public string VersionNumber { get; set; }

        public string DatePublished { get; set; }

        public int Size { get; set; }

        public List<Dependency> Dependencies { get; set; }

    }

    public class Dependency
    {
        public string VersionID { get; set; }

        public string ProjectID { get; set; }

        public string DependencyType { get; set; }


    }
}
