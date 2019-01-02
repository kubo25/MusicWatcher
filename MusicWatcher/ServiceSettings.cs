using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MusicWatcher {
    public class ServiceSettings : INotifyPropertyChanged {
        private const string serviceName = "MusicWatcherService";
        private readonly ServiceController service = new ServiceController(serviceName);

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _IsServiceRunning;
        public bool IsServiceRunning {
            get { return _IsServiceRunning; }
            private set {
                _IsServiceRunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsServiceRunnning"));
            }
        }

        private string _WatchFolder;
        public string WatchFolder {
            get { return _WatchFolder; }
            set {
                _WatchFolder = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WatchFolder"));
            }
        }

        public ServiceSettings() {
            IsServiceRunning = service.Status == ServiceControllerStatus.Running;
            //IsServiceRunning = true;
            WatchFolder = @"D:\Anime";
        }

        private void CheckServiceStatus (bool shouldStop) {
            ServiceControllerStatus status = shouldStop ? ServiceControllerStatus.Stopped : ServiceControllerStatus.Running;
            service.WaitForStatus(status);
            IsServiceRunning = !shouldStop;
        }

        public async void ToggleService() {
            if (IsServiceRunning) {
                service.Stop();
                await Task.Run(() => {
                    CheckServiceStatus(true);
                });
            }
            else {
                service.Start();
                await Task.Run(() => {
                    CheckServiceStatus(false);
                });
            }
        }
    }
}
