using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MusicWatcherService {
    public class MusicWatcherService : ServiceBase {
        public const string serviceName = "MusicWatcherService";
        private const string logName = "MusicWatcherServiceLog";
        private const string logSource = "MusicWatcherLogSource";

        private readonly NameValueCollection settings = ConfigurationManager.AppSettings;

        private EventLog log;
        private Watcher watcher;

        public MusicWatcherService() {
            ServiceName = serviceName;
        }

        protected override void OnStart(string[] args) {
            if (!EventLog.SourceExists(logSource, ".")) {
                EventLog.CreateEventSource(logSource, logName);
            }

            log = new EventLog(logName, ".", logSource);

            watcher = new Watcher(settings["WatchFolder"], settings["WatchFileExtensions"].Split(',').ToArray(),log);

            log.WriteEntry(string.Format("Started watching {0}", settings["WatchFolder"]));
        }

        protected override void OnStop() {
            watcher.Dispose();
            log.WriteEntry("Watcher service stopped");
        }
    }
}
