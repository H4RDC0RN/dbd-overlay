using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Languages;
using DBDOverlay.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using Application = System.Windows.Application;

namespace DBDOverlay.Core.Download
{
    public class DownloadManager
    {
        public event EventHandler<DownloadEventArgs> Downloading;

        private readonly string githubLink = "github.com";
        private readonly string downloadTessDataLink = "https://raw.github.com/tesseract-ocr/tessdata/main/";
        private readonly string releasesLink = "https://github.com/H4RDC0RN/dbd-overlay/releases/";
        private readonly string download = "download";
        private readonly string latest = "latest";

        public readonly string TessDataFolder = "Tessdata";
        private readonly string UpdateFolder = "Update";

        private readonly string traineddataExtension = ".traineddata";
        private readonly string zipExtension = ".zip";

        private readonly string binariesName = "dbd-overlay-";

        private static DownloadManager instance;

        public static DownloadManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new DownloadManager();
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

        public void DownloadLanguage(string language)
        {
            if (GetDownloadedLanguages().Contains(language)) return;
            if (!IsConnected)
            {
                Logger.Log.Error($"Can't connect to {githubLink}");
                return;
            }
            var worker = new BackgroundWorker();

            worker.DoWork += (s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Downloading?.Invoke(this, new DownloadEventArgs(true, language));
                });

                DownloadLanguageData(language);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Downloading?.Invoke(this, new DownloadEventArgs(false, language));
                });
            };

            worker.RunWorkerAsync();
        }

        public void DownloadDefaultLanguage()
        {
            Directory.CreateDirectory(TessDataFolder.ToProjectPath());
            if (GetDownloadedLanguages().Count == 0)
            {
                DownloadLanguage(LanguagesManager.Eng);
            }
        }

        public void CheckForUpdate()
        {
            if (!IsConnected)
            {
                Logger.Log.Error($"Can't connect to {githubLink}");
                return;
            }

            int.TryParse(CurrentVersion.RemoveRegex(@"\."), out int currentVersionNumber);
            var latestVersion = GetLatestVersion();
            int.TryParse(latestVersion.RemoveRegex(@"\."), out int latestVersionNumber);

            if (currentVersionNumber >= latestVersionNumber)
            {
                var updatePath = UpdateFolder.ToProjectPath();
                if (Directory.Exists(updatePath)) Directory.Delete(updatePath, true);
                return;
            }

            var zipFilePath = DownloadUpdate(latestVersion);
            ZipFile.ExtractToDirectory(zipFilePath, UpdateFolder.ToProjectPath());
            File.Delete(zipFilePath);
            InstallUpdate(latestVersion);
        }

        public List<string> GetDownloadedLanguages()
        {
            var regex = $@"(?<={TessDataFolder}\\).*(?={traineddataExtension})";
            return Directory.GetFiles(TessDataFolder.ToProjectPath()).Select(x => Regex.Match(x, regex).Value).ToList();
        }

        private void DownloadLanguageData(string language)
        {
            language = LanguagesManager.ConvertMexToSpa(language);
            DownloadFile($"{downloadTessDataLink}{language}{traineddataExtension}", $"{TessDataFolder.ToProjectPath()}/{language}{traineddataExtension}");
        }

        private string DownloadUpdate(string version)
        {
            Directory.CreateDirectory(UpdateFolder.ToProjectPath());
            var fileName = $"{binariesName}{version}{zipExtension}";
            var path = $"{UpdateFolder.ToProjectPath()}/{fileName}";
            DownloadFile($"{releasesLink}{download}/{version}/{fileName}", path);
            return path;
        }

        private void DownloadFile(string url, string path)
        {
            using (var client = new HttpClient())
            {
                SaveFile(path, client.GetByteArrayAsync(url).Result);
            }
        }

        private void SaveFile(string path, byte[] content)
        {
            if (content != null) File.WriteAllBytes(path, content);
            else Logger.Log.Warn("Downloaded content is null");
        }

        private string GetLatestVersion()
        {
            using (var client = new HttpClient())
            {
                return client.GetAsync($"{releasesLink}{latest}").Result.RequestMessage.RequestUri.ToString().GetLast(5);
            }
        }

        private void InstallUpdate(string version)
        {
            var exeName = AppDomain.CurrentDomain.FriendlyName;
            var exePath = Assembly.GetEntryAssembly().Location;
            var from = $"{UpdateFolder.ToProjectPath()}/{binariesName}{version}";
            var to = string.Empty.ToProjectPath();
            CmdHelper.RunCommand($"taskkill /f /im \"{exeName}\" && " +
                $"timeout /t 1 && " +
                $"xcopy /i /e /y \"{from}\" \"{to}\" && " +
                $"{exePath}");
        }
    }
}
