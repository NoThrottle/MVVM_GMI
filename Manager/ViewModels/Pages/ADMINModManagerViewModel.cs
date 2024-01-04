using Google.Protobuf.Compiler;
using MVVM_GMI.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.PointOfService;
using Wpf.Ui.Controls;
using online = MVVM_GMI.Helpers.OnlineRequest;

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


        string downloadLink = "";

        string versionNumber = "";

        private ModEntry? WorkingModEntry = null;





        [RelayCommand]
        async Task SearchModAsync()
        {

            downloadLink = "";
            versionNumber = "";
            IsRequired = "False";
            DatePublished = "";
            FileSize = "";

            ModVersions.Clear();

            String x = "";

            try
            {
                x = await online.GetJsonAsync("https://api.modrinth.com/v2/project/" + ProjectID);
            }
            catch
            {
                System.Windows.MessageBox.Show("Error, does not exist or something.");
                return;
            }

            if(x == null)
            {
                System.Windows.MessageBox.Show("Error, does not exist or something.");
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

            MinecraftVersions.Clear();

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

            ClientLoaders.Clear();

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


        [RelayCommand]
        async Task GetVersionsAsync()
        {

            String x;

            try
            {
                x = await online.GetJsonAsync("https://api.modrinth.com/v2/project/" + ProjectID + "/version?loaders=[\"" + ClientLoaders[SelIndexLoaders] +"\"]&game_versions=[\"" + MinecraftVersions[SelIndexMinecraftVersions] + "\"]");
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

            ModVersions.Clear();

            foreach(var version in obj)
            {
                ModVersions.Add(version["id"].ToString());
            }
        }

        [RelayCommand]
        async Task QueryVersionAsync()
        {
            String x;

            try
            {
                x = await online.GetJsonAsync("https://api.modrinth.com/v2/version/" + ModVersions[SelIndexModVersions]);
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

            var obj = JObject.Parse(x);
            FileSize = obj["files"][0]["size"].ToString();
            DatePublished = obj["date_published"].ToString();
            downloadLink = obj["files"][0]["url"].ToString();
            versionNumber = obj["version_number"].ToString();
        }

        [RelayCommand]
        void AddModToList()
        {
            var x = new ModEntry()
            {
                Name = ModName,
                Description = ModDescription,
                IconURL = IconURL,
                Categories = Category,

                ClientSide = bool.Parse(IsClientSide ?? "false"),
                IsRequired = bool.Parse(IsRequired),
                ServerSide = bool.Parse(IsServerSide ?? "false"),
                DownloadURL = downloadLink,
                VersionNumber = versionNumber,
                DatePublished = DatePublished,
                Size = int.Parse(FileSize),

                projectID = ProjectID,
                versionID = ModVersions[SelIndexModVersions]

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
    }
}
