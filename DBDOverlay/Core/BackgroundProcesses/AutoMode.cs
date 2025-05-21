﻿using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Core.WindowControllers.MapOverlay;
using System;
using Application = System.Windows.Application;

namespace DBDOverlay.Core.BackgroundProcesses
{
    public class AutoMode : BaseBackgroundProcess
    {
        public bool IsMapNameMode { get; set; } = true;

        public event EventHandler<NewMapEventArgs> NewMapRecognized;
        private static AutoMode instance;

        public static AutoMode Instance
        {
            get
            {
                if (instance == null)
                    instance = new AutoMode();
                return instance;
            }
        }

        protected AutoMode()
        {
            NewMapRecognized += MapOverlayController.Instance.HandleNewMapRecognized;
        }

        protected override void Action()
        {
            if (IsMapNameMode)
            {
                var mapInfo = ImageReader.Instance.GetMapInfo(true);
                if (MapOverlayController.Instance.CanMapOverlayBeApplied(mapInfo))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NewMapRecognized?.Invoke(this, new NewMapEventArgs(mapInfo));
                    });
                }
            }
            else
            {
                IsMapNameMode = ImageReader.Instance.IsMatchFinished();
            }
        }
    }
}
