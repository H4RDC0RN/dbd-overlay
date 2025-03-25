using DBDUtilityOverlay.Core.Extensions;
using DBDUtilityOverlay.Core.Languages;
using DBDUtilityOverlay.Core.Utils;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using Application = System.Windows.Application;

namespace DBDUtilityOverlay.Core.Download
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
                instance ??= new DownloadManager();
                return instance;
            }
        }

        public void Download(string language)
        {
            if (GetDownloadedLanguages().Contains(language)) return;
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

        private void DownloadLanguageData(string language)
        {
            language = LanguagesManager.ConvertMexToSpa(language);
            using HttpClient client = new();
            var url = $"{downloadLink}{language}.{traineddata}";
            var content = client.GetByteArrayAsync(url).Result;
            var fileName = $"{language}.{traineddata}";
            SaveTrainedData(fileName, content);
        }

        public List<string> GetDownloadedLanguages()
        {
            var regex = $@"(?<={TessDataFolder}\\).*(?=.{traineddata})";
            return [.. Directory.GetFiles(TessDataFolder.ToProjectPath()).Select(x => Regex.Match(x, regex).Value)];
        }

        public void SaveTrainedData(string fileName, byte[] content)
        {
            if (content != null) File.WriteAllBytes($@"{TessDataFolder.ToProjectPath()}\{fileName}", content);
            else Logger.Log.Warn("Downloaded content is null");
        }
    }
}
