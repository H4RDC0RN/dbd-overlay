using System.ComponentModel;

namespace DBDOverlay.Core.BackgroundProcesses
{
    public abstract class BaseBackgroundProcess
    {
        public bool IsActive { get; set; } = false;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public void Run()
        {
            IsActive = true;
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += (s, e) =>
            {
                while (IsActive)
                {
                    Action();
                }
            };
            worker.RunWorkerAsync();
        }

        public void Stop()
        {
            IsActive = false;
            worker.CancelAsync();
            worker.Dispose();
        }

        protected abstract void Action();
    }
}
