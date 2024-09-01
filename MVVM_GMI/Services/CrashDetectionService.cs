using MVVM_GMI.Models;
using MVVM_GMI.Services.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui;
using @online = MVVM_GMI.Helpers.OnlineRequest;

namespace MVVM_GMI.Services
{
    public sealed class CrashDetectionService
    {
        #region Boiler Plate
        private CrashDetectionService()
        {

        }

        private static readonly object lockObj = new object();

        private static CrashDetectionService instance = null;
        public static CrashDetectionService Instance
        {
            get
            {
                lock (lockObj)
                    {
                        if (instance == null)
                        {   
                            instance = new CrashDetectionService();
                        }
                        return instance;
                    }
            }
        } 
        

        private static List<string> existingFiles = new List<string>();

        #endregion

        /// <summary>
        /// Checks for existing crash reports in the Minecraft directory.
        /// This will be used to compare with the current crash reports.
        /// </summary>
        public void CheckExistingCrashReports()
        {
            var path = ConfigurationService.Instance.fromLauncher.MinecraftPath;

            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    if (file.Contains("hs_err_pid"))
                    {
                        existingFiles.Add(file);

                    }
                }
            }
            else
            {
                MessageBox.Show("The path to the Minecraft directory is invalid. Please check the path in the settings.");
            }
        }

        private string NewestFilePath = "";

        /// <summary>
        /// Checks if there are new crash reports in the Minecraft directory. 
        /// You must run CheckExistingCrashReports() before running this method.
        /// </summary>
        /// <returns>
        /// True - if there are new crash reports.
        /// False - if there are no new crash reports.
        /// </returns>
        public bool CheckCurrentCrashReports()
        {
            var path = ConfigurationService.Instance.fromLauncher.MinecraftPath;

            List<string> newFiles = new List<string>();

            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    if (file.Contains("hs_err_pid"))
                    {
                        newFiles.Add(file);
                    }
                }
            }
            else
            {
                MessageBox.Show("On closing, the path to the Minecraft directory is invalid. Please check the path in the settings.");
            }

            if (newFiles.Count == 0)
            {
                return false;
            }

            List<string> stranger = new List<string>();

            foreach (var file in newFiles)
            {
                if (!existingFiles.Contains(file))
                {
                    stranger.Add(file);

                }
            }

            if (stranger.Count > 0)
            {
                List<FileInfo> strangersInfo = new List<FileInfo>();

                foreach (var filePaths in stranger)
                {
                    FileInfo info = new FileInfo(filePaths);
                    strangersInfo.Add(info);
                }

                NewestFilePath = strangersInfo
                    .OrderByDescending(f => f.LastWriteTime)
                    .First().FullName;

                return true;
            }

            return false;

        }

        //If not the same list, prompt the user to upload the new reports.
        async public void UploadNewCrashReport(IContentDialogService dialogService)
        {
            var lines = File.ReadLines(NewestFilePath);
            bool isOutOfMemory = false;

            string Content = "";

            string NormalContent = 
                "Your game closed because it crashed! We recommend submitting your crash report to be inspected by our developers. \n" +
                "\n" +
                "Note, these information will be included in the report:\n" +
                "- Your System information\n" +
                "- Anything in your logs, which may include the name of mods or files involved with the minecraft process.\n" +
                "- Other process related information\n" +
                "\n" +
                "The report wont include personal information.";

            string OOMContent = "Your game closed because it ran out of memory!\n\n" +
                "You allocated " + ConfigurationService.Instance.fromMinecraft.MaxRamAllocation + " MB, and this may not be enough.\n" +
                "\n" +
                "What you can do:\n" +
                "Consider increasing it!\n" +
                "Consider closing programs you're not currently using, or increasing the ram you allocate for Minecraft.\n" +
                "\n" +
                "You may still report this crash to our developers!\n" +
                "\n" +
                "Note, these information will be included in the report:\n" +
                "- Your System information\n" +
                "- Anything in your logs, which may include the name of mods or files involved with the minecraft process.\n" +
                "- Other process related information\n" +
                "\n" +
                "The report wont include personal information.";

            foreach (var line in lines)
            {
                if (line.Contains("Out of Memory Error") || line.Contains("insufficient memory for the Java Runtime Environment"))
                {
                    Content = OOMContent;
                    break;
                }
            }
          
            if (Content == "")
            {
                Content = NormalContent;
            }

            var x = await dialogService.ShowAsync(
                new Wpf.Ui.Controls.ContentDialog()
                {
                    Title = "Oh no, You crashed!",
                    Content = Content,
                    PrimaryButtonText = "Submit Report",
                    CloseButtonText = "Close"
                }, new CancellationToken()
            );

            if (x == Wpf.Ui.Controls.ContentDialogResult.Primary)
            {
                try
                {

                    var time = DateTime.Now;
                    _ = await online.WriteToDatabaseAsync("Crash Reports", UserProfileService.AuthorizedUsername + new DateTimeOffset(time).ToUnixTimeSeconds(), new CrashReport()
                    {
                        Contents = string.Join("\n", lines),
                        Timestamp = time.ToString()
                    });
                }
                catch
                {
                    var y = await dialogService.ShowAsync(
                        new Wpf.Ui.Controls.ContentDialog()
                        {
                            Title = "Unable to send crash report!",
                            Content = "Unable to send the crash report, do you wanna try again?",
                            PrimaryButtonText = "Try again",
                            CloseButtonText = "Close"
                        }, new CancellationToken());

                    if (y == Wpf.Ui.Controls.ContentDialogResult.Primary)
                    {
                        UploadNewCrashReport(dialogService);
                    }
                }   
            }

        }
    }
}
