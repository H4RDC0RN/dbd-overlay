using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Utils;
using DBDOverlay.Core.WindowControllers.Loading;
using DBDOverlay.Core.WindowControllers.MapOverlay.Languages;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Application = System.Windows.Application;

namespace DBDOverlay.Core.Download
{
    public class DownloadManager
    {
        public event EventHandler<DownloadEventArgs> Downloading;

        private readonly string downloadTessDataLink = "https://raw.github.com/tesseract-ocr/tessdata/main/";
        private readonly string releasesLink = "https://github.com/H4RDC0RN/dbd-overlay/releases/";
        private readonly string githubLink = "github.com";
        private readonly string download = "download";
        private readonly string latest = "latest";

        private static DownloadManager instance;

        public static DownloadManager Instance
        {
            get
            {
                if (instance == null) instance = new DownloadManager();
                return instance;
            }
        }

        private bool IsConnected
        {
            get
            {
                try
                {
                    Dns.GetHostEntry(githubLink);
                    return true;
                }
                catch
                {
                    Logger.Error($"Can't connect to {githubLink}");
                    return false;
                }
            }
        }

        public string CurrentVersion
        {
            get
            {
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return currentVersion.Substring(0, currentVersion.Length - 2);
            }
        }

        public void DownloadLanguage(string language, bool inBackground = true)
        {
            if (FileSystem.GetDownloadedLanguagesAbbs().Contains(language)) return;
            if (!IsConnected) return;

            if (inBackground)
            {
                var worker = new BackgroundWorker();
                worker.DoWork += (s, e) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Downloading?.Invoke(this, new DownloadEventArgs(true));
                    });

                    DownloadLanguageData(language);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Downloading?.Invoke(this, new DownloadEventArgs(false));
                    });
                };
                worker.RunWorkerAsync();
            }
            else
            {
                DownloadLanguageData(language);
            }
        }

        public void DownloadDefaultLanguage()
        {
            if (FileSystem.GetDownloadedLanguagesAbbs().Count == 0)
            {
                LoadingWindowController.Instance.SetStatus("Downloading default language");
                DownloadLanguage(LanguagesManager.Eng, false);
                Logger.Info($"Default language is downloaded ({LanguagesManager.Eng})");
            }
        }

        public void CheckForUpdate()
        {
            if (!IsConnected) return;

            var latestVersion = GetLatestVersion();

            var current = new Version(CurrentVersion);
            var latest = new Version(latestVersion);

            if (current >= latest)
            {
                Logger.Info($"Application has latest version ({CurrentVersion})");
                var updatePath = FileSystem.Update;
                if (Directory.Exists(updatePath))
                {
                    Directory.Delete(updatePath, true);
                    Logger.Info("Update folder is deleted");
                }
                return;
            }

            Logger.Info($"New application version is available ({latestVersion})");
            Logger.Info($"Current version is ({CurrentVersion})");
            LoadingWindowController.Instance.SetStatus($"Downloading version {latestVersion}");
            var zipFilePath = DownloadUpdate(latestVersion);

            LoadingWindowController.Instance.SetStatus("Extracting archive");
            ZipFile.ExtractToDirectory(zipFilePath, FileSystem.Update);
            File.Delete(zipFilePath);
            Logger.Info("Zip file with update is unpacked");

            InstallUpdate(latestVersion);
        }

        private void DownloadLanguageData(string language)
        {
            language = LanguagesManager.ConvertMexToSpa(language);
            var fileName = FileSystem.BuildTrainedDataFileFullName(language);
            DownloadFile($"{downloadTessDataLink}{fileName}", $@"{FileSystem.TessData}\{fileName}");
        }

        private string DownloadUpdate(string version)
        {
            Directory.CreateDirectory(FileSystem.Update);
            var fileName = FileSystem.GetBinariesName(true);
            var path = $@"{FileSystem.Update}\{fileName}";
            DownloadFile($"{releasesLink}{download}/{version}/{fileName}", path);
            return path;
        }

        private void DownloadFile(string url, string path)
        {
            Logger.Info($"Start downloading file from '{url}'");
            using (var client = new HttpClient())
            {
                SaveFile(path, client.GetByteArrayAsync(url).Result);
            }
        }

        private void SaveFile(string path, byte[] content)
        {
            if (content != null)
            {
                File.WriteAllBytes(path, content);
                Logger.Info($"Downloaded file is saved to '{path}'");
            }
            else Logger.Warn("Downloaded content is null");
        }

        private string GetLatestVersion()
        {
            using (var client = new HttpClient())
            {
                var message = client.GetAsync($"{releasesLink}{latest}").Result.RequestMessage;
                if (message == null) return string.Empty;
                return message.RequestUri.ToString().ExtractVersion();
            }
        }

        private void InstallUpdate(string version)
        {
            Logger.Info($"Installation version {version}");
            LoadingWindowController.Instance.SetStatus($"Installing version {version}");

            var exeName = AppDomain.CurrentDomain.FriendlyName;
            var exePath = Assembly.GetEntryAssembly().Location;
            var from = $@"{FileSystem.Update}\{FileSystem.GetBinariesName()}";
            var to = string.Empty.ToProjectPath();

            CmdHelper.RunCommand($"taskkill /f /im \"{exeName}\" && " +
                $"timeout /t 1 && " +
                $"xcopy /i /e /y \"{from}\" \"{to}\" && " +
                $"{exePath}".EscapeCmdArg());
        }
    }
}
