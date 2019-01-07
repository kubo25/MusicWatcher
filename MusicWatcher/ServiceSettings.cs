using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MusicWatcher {
    public class ServiceSettings : INotifyPropertyChanged {
        private const string serviceName = "MusicWatcherService";
        private readonly ServiceController service = new ServiceController(serviceName);
        private readonly Configuration serviceConfiguration;

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

            RegistryKey hklm = Registry.LocalMachine;
            hklm = hklm.OpenSubKey(@"System\CurrentControlSet\Services\" + serviceName);
            string path = hklm.GetValue("ImagePath").ToString();
            path = Regex.Match(path, "\"(.*)\"").Groups[1].ToString();

            serviceConfiguration = ConfigurationManager.OpenExeConfiguration(path);
            WatchFolder = serviceConfiguration.AppSettings.Settings["WatchFolder"].Value;
        }

        private void CheckServiceStatus (bool shouldStop) {
            ServiceControllerStatus status = shouldStop ? ServiceControllerStatus.Stopped : ServiceControllerStatus.Running;
            service.WaitForStatus(status);
            IsServiceRunning = !shouldStop;
        }

        public void ApplySettings() {
            serviceConfiguration.AppSettings.Settings["WatchFolder"].Value = WatchFolder;
            serviceConfiguration.Save();

            RestartService();
        }

        private async Task StartService() {
            service.Start();
            await Task.Run(() => CheckServiceStatus(false));
        }

        private async Task StopService() {
            service.Stop();
            await Task.Run(() => CheckServiceStatus(true));
        }

        private async void RestartService() {
            if (IsServiceRunning) {
                await StopService();
            }
            await StartService();
        }

        public async void ToggleService() {
            if (IsServiceRunning) {
                await StopService();
            }
            else {
                await StartService();
            }
        }
    }
}
