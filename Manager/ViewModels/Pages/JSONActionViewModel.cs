using Microsoft.Win32;
using MVVM_GMI.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.IO;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class JSONActionViewModel : ObservableObject, INavigationAware
    {

        IContentDialogService _contentDialogService;
        private bool _isInitialized = false;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            _isInitialized = true;
        }

        public JSONActionViewModel(IContentDialogService contentDialog)
        {
            _contentDialogService = contentDialog;
        }

        [ObservableProperty]
        int _selectedIndex = 0;

        [ObservableProperty]
        int _selectedListIndex = 0;


        List<object> actions = new List<object>();


        [ObservableProperty]
        string _url = "";

        [ObservableProperty]
        string _path = "";

        [ObservableProperty]
        string _source = "";

        [ObservableProperty]
        string _destination = "";

        [ObservableProperty]
        string _filename = "";

        [ObservableProperty]
        string _content = "";


        [RelayCommand]
        void ClearFields()
        {
            Url = String.Empty;
            Path = String.Empty;
            Source = String.Empty;
            Destination = String.Empty;
            Filename = String.Empty;
            Content = String.Empty;
        }


        //DeleteFile = 0
        //WriteText = 1
        //CreateDirectory = 2
        //ExtractToDirectory = 3
        //Move = 4
        //DownloadToDirectory = 5
        [RelayCommand]
        void CreateAction()
        {
            switch (SelectedIndex)
            {
                case 0:
                    var x = new JSONActionModels.DeleteFile()
                    {
                        Filename = Filename,
                        Path = Path,
                    };
                    actions.Add(x);
                    TranslateToVisual("Delete File",[Filename, Path]);
                    break;
                case 1:
                    var y = new JSONActionModels.WriteText()
                    {
                        Content = Content,
                        Filename = Filename,
                        Path = Path,
                    };
                    actions.Add(y);
                    TranslateToVisual("Write Text",[Content, Filename, Path]);
                    break;
                case 2:
                    var z = new JSONActionModels.CreateDirectory()
                    {
                        Path = Path,
                    };
                    actions.Add(z);
                    TranslateToVisual("Create Directory",[Path]);
                    break;
                case 3:
                    var a = new JSONActionModels.ExtractToDirectory()
                    {
                        Source = Source,
                        Destination = Destination,
                    };
                    actions.Add(a);
                    TranslateToVisual("Extract To Directory",[Source, Destination]);
                    break;
                case 4:
                    var b = new JSONActionModels.Move()
                    {
                        Source = Source,
                        Destination = Destination
                    };
                    actions.Add(b);
                    TranslateToVisual("Move",[Source, Destination]);
                    break;
                case 5:
                    var c = new JSONActionModels.DownloadToDirectory()
                    {
                        URL = Url,
                        Destination = Destination,
                    };
                    actions.Add(c);
                    TranslateToVisual("Download To Directory",[Url, Destination]);
                    break;
                case 6:
                    var d = new JSONActionModels.WriteFile()
                    {
                        Bytes = Convert.ToBase64String(File.ReadAllBytes(Source)),
                        Path = Path,
                        Filename = Filename,
                    };
                    actions.Add(d);
                    TranslateToVisual("Write File", [Source, Path, Filename]);
                    break;
            }

            ClearFields();
            TranslateToJSONString();
        }

        [RelayCommand]
        void BrowseFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
			if(openFileDialog.ShowDialog() == true) { 
				Source = openFileDialog.FileName;
		    }
        }

        [RelayCommand]
        void DeleteItemFromList()
        {
            int y = SelectedListIndex;

            if (y < 0)
            {
                return;
            }

            ActionsDisplay.RemoveAt(y);
            actions.RemoveAt(y);
            SelectedListIndex = y - 1;
            TranslateToJSONString();

        }


        [RelayCommand]
        void MoveUpFromList()
        {
            if (actions.Count == 0 || SelectedListIndex <= 0)
            {
                return;
            }

            int y = SelectedListIndex;

            var init = ActionsDisplay[y];
            ActionsDisplay.RemoveAt(y);
            ActionsDisplay.Insert(y-1, init);

            var init2 = actions[y];
            actions.RemoveAt(y);
            actions.Insert(y - 1, init2);

            SelectedListIndex = y - 1;
            TranslateToJSONString();

        }

        [RelayCommand]
        void MoveDownFromList()
        {
            if (actions.Count == 0 || SelectedListIndex == actions.Count-1 || SelectedListIndex < 0)
            {
                return;
            }

            int y = SelectedListIndex;

            var init = ActionsDisplay[y];
            ActionsDisplay.RemoveAt(y);
            ActionsDisplay.Insert(y + 1, init);

            var init2 = actions[y];
            actions.RemoveAt(y);
            actions.Insert(y + 1, init2);

            SelectedListIndex = y+1;
            TranslateToJSONString();
        }


        void TranslateToVisual(string Action, string[] parameters)
        {
            ActionsDisplay.Add(new ActionDisplay() { Action = Action, Parameters = parameters});
        }

        [ObservableProperty]
        ObservableCollection<ActionDisplay> _actionsDisplay = new ObservableCollection<ActionDisplay>();


        [ObservableProperty]
        string _jsonString = "";

        void TranslateToJSONString()
        {
            JArray array = JArray.FromObject(actions);
            JsonString = array.ToString();
        }

        [RelayCommand]
        void ResetAll()
        {
            JsonString = string.Empty;
            actions.Clear();
            ActionsDisplay.Clear();
        }

        [ObservableProperty]
        string _scriptName = string.Empty;

        [RelayCommand]
        async Task PublishScript()
        {
            var x = new JSONActionDocument() { Title=ScriptName, JSONString = JsonString };
            await OnlineRequest.WriteToDatabaseAsync("JsonActions",ScriptName,x);
        }

    }

    public class ActionDisplay
    {
        public string Action { get; set; }
        public string[] Parameters { get; set; }

    }
}

