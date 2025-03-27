using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Languages;
using DBDOverlay.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Application = System.Windows.Application;

namespace DBDOverlay.Core.Download
{
    public class DownloadManager
    {
        public event EventHandler<DownloadEventArgs> Downloading;
        public readonly string TessDataFolder = "Tessdata";
        private readonly string downloadLink = "https://raw.github.com/tesseract-ocr/tessdata/main/";
        private readonly string traineddata = "traineddata";

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
                    Dns.GetHostEntry("github.com");
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public void Download(string language)
        {
            if (GetDownloadedLanguages().Contains(language)) return;
            if (!IsConnected)
            {
                Logger.Log.Error("Can't connect to github");
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
                Download(LanguagesManager.Eng);
            }
        }

        private void DownloadLanguageData(string language)
        {
            language = LanguagesManager.ConvertMexToSpa(language);
            using (var client = new HttpClient())
            {
                var url = $"{downloadLink}{language}.{traineddata}";
                var content = client.GetByteArrayAsync(url).Result;
                var fileName = $"{language}.{traineddata}";
                SaveTrainedData(fileName, content);
            }
        }

        public List<string> GetDownloadedLanguages()
        {
            var regex = $@"(?<={TessDataFolder}\\).*(?=.{traineddata})";
            return Directory.GetFiles(TessDataFolder.ToProjectPath()).Select(x => Regex.Match(x, regex).Value).ToList();
        }

        public void SaveTrainedData(string fileName, byte[] content)
        {
            if (content != null) File.WriteAllBytes($@"{TessDataFolder.ToProjectPath()}\{fileName}", content);
            else Logger.Log.Warn("Downloaded content is null");
        }
    }
}
