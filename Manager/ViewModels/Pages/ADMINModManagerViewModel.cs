using CmlLib.Core.Version;
using Google.Protobuf.Compiler;
using MVVM_GMI.Models;
using MVVM_Manager.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.PointOfService;
using Wpf.Ui.Controls;
using online = MVVM_GMI.Helpers.OnlineRequest;
using from = MVVM_GMI.Services.ConfigurationService;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class ADMINModManagerViewModel : ObservableObject, INavigationAware
    {
        #region Boiler Plate
        private bool _isInitialized = false;

        public void OnNavigatedTo()
        {
            FetchModListAsync();

            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {

            _isInitialized = true;
        }
        #endregion

        [ObservableProperty]
        public ObservableCollection<ModEntry> _Mods = new ObservableCollection<ModEntry>();

        [ObservableProperty]
        string _projectID = "";

        [ObservableProperty]
        string _isModrinth = "True";

        [ObservableProperty]
        string _iconURL = "";

        [ObservableProperty]
        string _modName = "";

        [ObservableProperty]
        string _modDescription = "";

        [ObservableProperty]
        string _category = "";

        [ObservableProperty]
        string? _isClientSide = "";

        [ObservableProperty]
        string? _isServerSide = "";

        [ObservableProperty]
        ObservableCollection<String> _clientLoaders = new ObservableCollection<string>();

        [ObservableProperty]
        int _selIndexLoaders = -1;

        [ObservableProperty]
        ObservableCollection<String> _minecraftVersions = new ObservableCollection<string>();

        [ObservableProperty]
        int _selIndexMinecraftVersions = -1;

        [ObservableProperty]
        ObservableCollection<String> _modVersions = new ObservableCollection<string>();

        [ObservableProperty]
        int _selIndexModVersions = -1;

        [ObservableProperty]
        string _fileSize = "";

        [ObservableProperty]
        string _datePublished = "";

        [ObservableProperty]

        string _isRequired = "False";

        [ObservableProperty]
        string _downloadLink = "";

        string versionNumber = "";

        private ModEntry? WorkingModEntry = null;


        #region Mod Entry List

        [ObservableProperty]
        int _modEntrySelectedIndex = -1;

        [RelayCommand]
        async Task OpenModEntryAsync()
        {
            ClearFields(true);

            var x = Mods[ModEntrySelectedIndex];

            ProjectID = x.projectID;
            ModName = x.Name;
            IconURL = x.IconURL;
            ModDescription = x.Description;
            Category = x.Categories;
            IsClientSide = x.ClientSide.ToString();
            IsServerSide = x.ServerSide.ToString();
            IsRequired = x.IsRequired.ToString();           

            if(x.Actions != null)
            {
                var t = JArray.Parse(x.Actions);
                JsonActionsArray = t;
                JsonActionsAggregate = t.ToString();
            }

            await QueryVersionAsync(x.versionID);

            if (DownloadLink != x.DownloadURL)
            {
                System.Windows.MessageBox.Show("Download link mismatch. Will use saved download link");
                DownloadLink = x.DownloadURL;
            }
        }

        [RelayCommand]
        void DeselectModEntry()
        {
            ModEntrySelectedIndex = -1;
            ClearFields();
        }

        #endregion

        [RelayCommand]
        void ClearFields(bool Searching = false)
        {

            if(!Searching)
            {
                ProjectID = "";
                IsModrinth = "True";
            }

            IconURL = "";
            ModName = "";
            ModDescription = "";
            Category = "";
            IsClientSide = "";
            IsServerSide = "";


            ClientLoaders.Clear();
            SelIndexLoaders = -1;
            MinecraftVersions.Clear();
            SelIndexMinecraftVersions = -1;
            ModVersions.Clear();
            SelIndexModVersions = -1;
            FileSize = "";
            DatePublished = "";
            DownloadLink = "";
            versionNumber = "";
            IsRequired = "";

            JsonActionsAggregate = "";
            JsonActionTextBox = "";
            JsonActionsArray.Clear();
        }

        [ObservableProperty]
        string _jsonActionsAggregate = "";

        [ObservableProperty]
        string _jsonActionTextBox = "";

        JArray JsonActionsArray = new JArray();

        [RelayCommand]
        void AddAction()
        {

            JToken x;

            try
            {
                x = JToken.Parse(JsonActionTextBox);
            }
            catch
            {
                System.Windows.MessageBox.Show("Invalid Json");
                return;
            }

            JsonActionsArray.Add(x);
            JsonActionsAggregate = JsonActionsArray.ToString();

        }

        [RelayCommand]
        async Task SearchModAsync()
        {

            ClearFields(true);

            if (bool.Parse(IsModrinth))
            {
                await SearchModrinthAsync();
            }
            else
            {
                await SearchCurseforgeAsync();
            }

        }

        #region Search Mod Tools

        async Task SearchModrinthAsync()
        {
            String x = "";

            try
            {
                x = await online.GetJsonAsync("https://api.modrinth.com/v2/project/" + ProjectID);
            }
            catch
            {
                System.Windows.MessageBox.Show("Error, does not exist or something. Modrinth - NetError");
                return;
            }

            if (x == null)
            {
                System.Windows.MessageBox.Show("Error, does not exist or something. Modrinth - Null");
                return;
            }

            JObject obj = JObject.Parse(x);

            IconURL = obj["icon_url"].ToString() ?? String.Empty;
            ModName = obj["title"].ToString() ?? String.Empty;
            ModDescription = obj["description"].ToString() ?? String.Empty;
            try
            {
                Category = new CultureInfo("en-US", false).TextInfo.ToTitleCase((obj["categories"][0]).ToString());
            }
            catch
            {
                Category = "None";
            }

            switch ((obj["client_side"].ToString()))
            {
                case "required":
                    IsClientSide = "True";
                    break;
                case "optional":
                    IsClientSide = "True";
                    break;
                case "unsupported":
                    IsClientSide = "False";
                    break;
            }

            switch ((obj["server_side"].ToString()))
            {
                case "required":
                    IsServerSide = "True";
                    break;
                case "optional":
                    IsServerSide = "True";
                    break;
                case "unsupported":
                    IsServerSide = "False";
                    break;
            }

            int b = 0;
            foreach (var p in (JArray)obj["game_versions"])
            {
                if (p.ToString().ToLower() == "1.20.1")
                {
                    SelIndexMinecraftVersions = b;
                }
                MinecraftVersions.Add(p.ToString());
                b++;
            }

            int c = 0;
            foreach (var p in (JArray)obj["loaders"])
            {
                if (p.ToString().ToLower() == "fabric")
                {
                    SelIndexLoaders = c;
                }
                ClientLoaders.Add(p.ToString());
                c++;
            }
        }

        async Task SearchCurseforgeAsync()
        {
            String x;

            try
            {
                x = await online.GetJsonAsync("https://api.curseforge.com/v1/mods/" + ProjectID, [LauncherProperties.CurseforgeKey]);
            }
            catch
            {
                System.Windows.MessageBox.Show("Error, does not exist or something. CF - NetError");
                return;
            }

            if (x == null)
            {
                System.Windows.MessageBox.Show("Error, does not exist or something. CF - Null");
                return;
            }

            var obj = JObject.Parse(x)["data"];

            ModName = obj["name"].ToString();
            IconURL = obj["logo"]["thumbnailUrl"].ToString();
            ModDescription = obj["summary"].ToString();

            try
            {
                Category = new CultureInfo("en-US", false).TextInfo.ToTitleCase((obj["categories"][0]["name"]).ToString());
            }
            catch
            {
                Category = "None";
            }

            IsClientSide = "True";
            IsServerSide = "True";

            foreach (var t in obj["latestFilesIndexes"])
            {
                if (!MinecraftVersions.Contains(t["gameVersion"].ToString()))
                {
                    MinecraftVersions.Add(t["gameVersion"].ToString());
                }

                var g = CurseforgeLoader_IntToName((t["modLoader"] ?? "0").ToString());

                if (!ClientLoaders.Contains(g))
                {
                    ClientLoaders.Add(g);
                }
            }

        }

        string CurseforgeLoader_IntToName(string Number)
        {

            /*
                0=Any
                1=Forge
                2=Cauldron
                3=LiteLoader
                4=Fabric
                5=Quilt
                6=NeoForge
             */

            var x = int.Parse(Number);

            switch (x)
            {
                case 0:
                    return "Any";
                case 1:
                    return "Forge";
                case 2:
                    return "Cauldron";
                case 3:
                    return "LiteLoader";
                case 4:
                    return "Fabric";
                case 5:
                    return "Quilt";
                case 6:
                    return "NeoForge";
                default:
                    return "null";

            }
        }
        int CurseforgeLoader_NameToInt(string Name)
        {

            /*
                0=Any
                1=Forge
                2=Cauldron
                3=LiteLoader
                4=Fabric
                5=Quilt
                6=NeoForge
             */

            switch (Name)
            {
                case "Any":
                    return 0;
                case "Forge":
                    return 1;
                case "Cauldron":
                    return 2;
                case "LiteLoader":
                    return 3;
                case "Fabric":
                    return 4;
                case "Quilt":
                    return 5;
                case "NeoForge":
                    return 6;
                default:
                    return -1;

            }
        }

        #endregion



        [RelayCommand]
        async Task GetVersionsAsync()
        {

            if (bool.Parse(IsModrinth))
            {
                await GetVersionModrinthAsync();
            }
            else
            {
                await GetVersionCurseforgeAsync();
            }
            
        }

        async Task GetVersionModrinthAsync()
        {
            String x;

            try
            {
                x = await online.GetJsonAsync("https://api.modrinth.com/v2/project/" + ProjectID + "/version?loaders=[\"" + ClientLoaders[SelIndexLoaders] + "\"]&game_versions=[\"" + MinecraftVersions[SelIndexMinecraftVersions] + "\"]");
            }
            catch
            {
                System.Windows.MessageBox.Show("Error, does not exist or something.");
                return;
            }

            if (x == null)
            {
                System.Windows.MessageBox.Show("Error, does not exist or something.");
                return;
            }

            JArray obj = JArray.Parse(x);

            foreach (var version in obj)
            {
                ModVersions.Add(version["id"].ToString());
            }
        }

        async Task GetVersionCurseforgeAsync()
        {
            String x;

            try
            {
                x = await online.GetJsonAsync("https://api.curseforge.com/v1/mods/" + ProjectID + "/files?modLoaderType=" + CurseforgeLoader_NameToInt(ClientLoaders[SelIndexLoaders]) + "&gameVersion=" + MinecraftVersions[SelIndexMinecraftVersions], [LauncherProperties.CurseforgeKey]);
            }
            catch
            {
                System.Windows.MessageBox.Show("Error, does not exist or something.");
                return;
            }

            if (x == null)
            {
                System.Windows.MessageBox.Show("Error, does not exist or something.");
                return;
            }

            JArray obj = (JArray)JObject.Parse(x)["data"];


            foreach (var version in obj)
            {
                ModVersions.Add(version["id"].ToString());
            }
        }




        [RelayCommand]
        async Task QueryVersionAsync(string? version = null)
        {
            if (bool.Parse(IsModrinth))
            {
                await QueryVersionModrinthAsync(version);
            }
            else
            {
                await QueryVersionCurseforgeAsync(version);
            }


        }

        async Task QueryVersionModrinthAsync(string? version = null)
        {
            String x;

            if (version == null)
            {
                try
                {
                    x = await online.GetJsonAsync("https://api.modrinth.com/v2/version/" + ModVersions[SelIndexModVersions]);
                }
                catch
                {
                    System.Windows.MessageBox.Show("Error, does not exist or something.");
                    return;
                }
            }
            else
            {
                try
                {
                    x = await online.GetJsonAsync("https://api.modrinth.com/v2/version/" + version);
                }
                catch
                {
                    System.Windows.MessageBox.Show("Error, does not exist or something.");
                    return;
                }
            }

            if (x == null)
            {
                System.Windows.MessageBox.Show("Error, does not exist or something.");
                return;
            }

            var obj = JObject.Parse(x);
            FileSize = obj["files"][0]["size"].ToString();
            DatePublished = obj["date_published"].ToString();
            DownloadLink = obj["files"][0]["url"].ToString();
            versionNumber = obj["version_number"].ToString();
        }

        async Task QueryVersionCurseforgeAsync(string? version = null)
        {
            String x;

            if (version == null)
            {
                try
                {
                    x = await online.GetJsonAsync("https://api.curseforge.com/v1/mods/" + ProjectID + "/files/" + ModVersions[SelIndexModVersions], [LauncherProperties.CurseforgeKey]);
                }
                catch
                {
                    System.Windows.MessageBox.Show("Error, does not exist or something. CF - Query Version Net Error");
                    return;
                }
            }
            else
            {
                try
                {
                    x = await online.GetJsonAsync("https://api.curseforge.com/v1/mods/" + ProjectID + "/files/" + version, [LauncherProperties.CurseforgeKey]);
                }
                catch
                {
                    System.Windows.MessageBox.Show("Error, does not exist or something. CF - Query Version Net Error");
                    return;
                }
            }

            if (x == null)
            {
                System.Windows.MessageBox.Show("Error, does not exist or something. CF - Query Version Null");
                return;
            }

            var obj = JObject.Parse(x)["data"];

            FileSize = obj["fileLength"].ToString();
            DatePublished = obj["fileDate"].ToString();
            DownloadLink = obj["downloadUrl"].ToString();
            versionNumber = obj["displayName"].ToString();
        }




        [RelayCommand]
        void AddModToList()
        {
            System.Windows.MessageBox.Show(IsClientSide + "dad" + IsRequired + "ad" + IsServerSide);

            var x = new ModEntry()
            {
                Name = ModName,
                Description = ModDescription,
                IconURL = IconURL,
                Categories = Category,

                ClientSide = (IsClientSide != null && (IsClientSide.ToLower() == "true" || IsClientSide.ToLower() == "false")) ? bool.Parse(IsClientSide) : false,
                IsRequired = (IsRequired != null && (IsRequired.ToLower() == "true" || IsRequired.ToLower() == "false")) ? bool.Parse(IsRequired) : false,
                ServerSide = (IsServerSide != null && (IsServerSide.ToLower() == "true" || IsServerSide.ToLower() == "false")) ? bool.Parse(IsServerSide) : false,
                DownloadURL = DownloadLink,
                VersionNumber = versionNumber,
                DatePublished = DatePublished,
                Size = int.Parse(FileSize),

                projectID = ProjectID,
                versionID = ModVersions[SelIndexModVersions],

                Actions = JsonActionsArray.ToString()

            };

            AddEntryAsync(x);
        }

        async Task AddEntryAsync(ModEntry modEntry)
        {
            await online.WriteToDatabaseAsync("Mods",modEntry.Name,modEntry);
            FetchModListAsync();
        }

        void RemoveEntry(string ModName)
        {
            online.DeleteFromDatabaseAsync("Mods",ModName);
            FetchModListAsync();
        }

        async Task FetchModListAsync()
        {

            Mods.Clear();

            var modList = await online.GetAllFromDatabaseAsync<ModEntry>("Mods");

            foreach (var mod in modList)
            {
                Mods.Add(mod);
            }
        }



        [RelayCommand]
        void DownloadAllServerMods()
        {
            List<ModEntry> toDL = new List<ModEntry>();

            foreach(var mod in Mods)
            {
                if (mod.ServerSide)
                {
                    toDL.Add(mod);
                }
            }

            DownloadModsAsync("E:/HSMC Launcher/allServerMods", toDL);
        }

        async Task<bool> DownloadModsAsync(string _Path, List<ModEntry> mods)
        {
            try
            {
                Directory.Delete(_Path, true);
            }
            catch
            {

            }

            Directory.CreateDirectory(_Path);

            var x = mods;

            int MaxQueue = from.Instance.fromLauncher.SimultaneousDownloads;
            int inQueue = 0;

            string entryName = "";
            int max = x.Count() - 1;
            int prog = 0;

            _ = Task.Run(() =>
            {
                foreach (var entry in x)
                {

                    while (inQueue > MaxQueue)
                    {
                        Thread.Sleep(250);
                    }

                    Task.Run(async () =>
                    {

                        inQueue += 1;

                        entryName = entry.Name;
                        var o = Path.Combine(_Path, entry.Name + entry.projectID + entry.versionID + ".jar");
                        await online.DownloadFileAsync(entry.DownloadURL, o);
                        prog++;

                        inQueue--;

                    });

                }

            });

            while (prog != max)
            {

                var t = System.Windows.MessageBox.Show(("Downloading Mods: " + prog + "/" + x.Count() + " - Mod: " + entryName), "Downloading");

                Thread.Sleep(1000);
            }

            return true;
        }
    }
}
