using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.MapOverlay.Languages;
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

        private readonly string downloadTessDataLink = "https://raw.github.com/tesseract-ocr/tessdata/main/";
        private readonly string releasesLink = "https://github.com/H4RDC0RN/dbd-overlay/releases/";
        private readonly string githubLink = "github.com";
        private readonly string download = "download";
        private readonly string latest = "latest";

        private readonly string traineddataExtension = ".traineddata";
        private readonly string zip = "zip";

        private readonly string binariesName = "dbd-overlay";

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
            if (GetDownloadedLanguages().Contains(language)) return;
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
            if (GetDownloadedLanguages().Count == 0)
            {
                DownloadLanguage(LanguagesManager.Eng, false);
                Logger.Info($"Default language is downloaded ({LanguagesManager.Eng})");
            }
        }

        public void CheckForUpdate()
        {
            if (!IsConnected) return;

            int.TryParse(CurrentVersion.RemoveRegex(@"\."), out int currentVersionNumber);
            var latestVersion = GetLatestVersion();
            int.TryParse(latestVersion.RemoveRegex(@"\."), out int latestVersionNumber);

            if (currentVersionNumber >= latestVersionNumber)
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
            var zipFilePath = DownloadUpdate(latestVersion);

            ZipFile.ExtractToDirectory(zipFilePath, FileSystem.Update);
            File.Delete(zipFilePath);
            Logger.Info("Zip file with update is unpacked");

            InstallUpdate(latestVersion);
        }

        public List<string> GetDownloadedLanguages()
        {
            var regex = $@"(?<={FileSystem.TessData.Split(@"\").Last()}\\).*(?={traineddataExtension})";
            return Directory.GetFiles(FileSystem.TessData).Select(x => Regex.Match(x, regex).Value).ToList();
        }

        private void DownloadLanguageData(string language)
        {
            language = LanguagesManager.ConvertMexToSpa(language);
            DownloadFile($"{downloadTessDataLink}{language}{traineddataExtension}", $@"{FileSystem.TessData}\{language}{traineddataExtension}");
        }

        private string DownloadUpdate(string version)
        {
            Directory.CreateDirectory(FileSystem.Update);
            var fileName = $"{binariesName}.{zip}";
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
                return client.GetAsync($"{releasesLink}{latest}").Result.RequestMessage.RequestUri.ToString().GetLast(5);
            }
        }

        private void InstallUpdate(string version)
        {
            Logger.Info($"Installation version {version}");

            var exeName = AppDomain.CurrentDomain.FriendlyName;
            var exePath = Assembly.GetEntryAssembly().Location;
            var from = $@"{FileSystem.Update}\{binariesName}";
            var to = string.Empty.ToProjectPath();

            CmdHelper.RunCommand($"taskkill /f /im \"{exeName}\" && " +
                $"timeout /t 1 && " +
                $"xcopy /i /e /y \"{from}\" \"{to}\" && " +
                $"{exePath}");
        }
    }
}
