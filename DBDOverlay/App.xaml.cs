using DBDOverlay.Core.Download;
using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Core.Reshade;
using DBDOverlay.Core.Utils;
using DBDOverlay.Core.WindowControllers.Loading;
using DBDOverlay.Core.WindowControllers.MapOverlay.Languages;
using DBDOverlay.Properties;
using DBDOverlay.UI.Windows;
using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace DBDOverlay
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Initialize();
        }

        private void Initialize()
        {
            Logger.Info("---Open Application---");
            LoadingWindowController.Window.Show();

            var worker = new BackgroundWorker();
            worker.DoWork += (s, e) =>
            {
                PresetAction("App configuration", HandleExceptions);
                PresetAction("App configuration", AddNumToSendKeys);
                PresetAction("Creating default folders", FileSystem.CreateDefaultFolders);
                PresetAction("Checking for updates", DownloadManager.Instance.CheckForUpdate);
                PresetAction("Checking default language", DownloadManager.Instance.DownloadDefaultLanguage);
                PresetAction("Loading user settings", ReloadSettings);
                PresetAction("Loading ReShade.ini", LoadReshadeIni);
                PresetAction("Initializing Tesseract", ImageReader.Instance.Initialize);
            };
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FinishLoading);
            worker.RunWorkerAsync();
        }

        private void PresetAction(string message, Action action)
        {
            LoadingWindowController.Instance.SetStatus(message);
            action();
        }

        private void FinishLoading(object sender, RunWorkerCompletedEventArgs e)
        {
            Current.MainWindow = new MainWindow();
            LoadingWindowController.Window.Close();
            Current.MainWindow.Show();
        }

        private void ReloadSettings()
        {
            string configPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            if (!File.Exists(configPath))
            {
                Settings.Default.Upgrade();
                Settings.Default.Reload();
                Settings.Default.Save();
            }
        }

        private void LoadReshadeIni()
        {
            var path = Settings.Default.ReshadeIniPath;
            if (!path.Equals(string.Empty))
            {
                ReshadeManager.Instance.Initialize(path);
                var maps = MapNamesContainer.GetReshadeMapsList();
                for (int mapIndex = 0; mapIndex < maps.Count; mapIndex++)
                {
                    var filterIndex = MappingsHandler.GetFilterIndex(mapIndex);
                    if (filterIndex != -1) ReshadeManager.Instance.AddHotKey(maps[mapIndex], filterIndex);
                }
            }
        }

        private void AddNumToSendKeys()
        {
            var info = typeof(SendKeys).GetField("keywords", BindingFlags.Static | BindingFlags.NonPublic);
            var oldKeys = (Array)info.GetValue(null);
            var elementType = oldKeys.GetType().GetElementType();
            var newKeys = Array.CreateInstance(elementType, oldKeys.Length + 10);
            Array.Copy(oldKeys, newKeys, oldKeys.Length);
            for (int i = 0; i < 10; i++)
            {
                var newItem = Activator.CreateInstance(elementType, "NUMPAD" + i, (int)Keys.NumPad0 + i);
                newKeys.SetValue(newItem, oldKeys.Length + i);
            }
            info.SetValue(null, newKeys);
        }

        private void HandleExceptions()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                Logger.Fatal(e.Exception.GetType().Name);
                Logger.Fatal(e.Exception.Message);
                Logger.Fatal(e.Exception.StackTrace);
                Logger.Info("---Close Application with exception---");
            };
        }
    }
}
