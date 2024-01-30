
namespace MVVM_GMI.Models
{
    
    public class ModEntry
    {
        
        public string Name { get; set; } //Title

        
        public string Description { get; set; } //Description

        
        public string IconURL { get; set; }

        
        public string Categories { get; set; }

        //--------//

        
        public string projectID { get; set; }

        
        public string versionID { get; set; }

        
        public bool ClientSide { get; set; }

        
        public bool IsRequired { get; set; }

        
        public bool ServerSide { get; set; }

        
        public string DownloadURL { get; set; }

        
        public string VersionNumber { get; set; }

        
        public string DatePublished { get; set; }

        
        public int Size { get; set; }

        
        public string Actions { get; set; }

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

        public string Actions { get; set; }

    }

}
