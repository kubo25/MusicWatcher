using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MusicWatcherService {
    [RunInstaller(true)]
    public class MusicWatcherServiceInstaller : Installer {
        public MusicWatcherServiceInstaller() {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller {
                Account = ServiceAccount.LocalSystem
            };

            ServiceInstaller serviceInstaller = new ServiceInstaller {
                ServiceName = MusicWatcherService.serviceName,
                StartType = ServiceStartMode.Automatic,
                DisplayName = "Music Watcher Service",
                Description = "Watching the selected folder for new music."
            };

            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }

        protected override void OnCommitted(IDictionary savedState) {
            ServiceController serviceController = new ServiceController(MusicWatcherService.serviceName);
            serviceController.Start();
            serviceController.Dispose();
        }

        protected override void OnBeforeUninstall(IDictionary savedState) {
            ServiceController serviceController = new ServiceController(MusicWatcherService.serviceName);
            serviceController.Stop();
            serviceController.Dispose();
        }
    }
}
