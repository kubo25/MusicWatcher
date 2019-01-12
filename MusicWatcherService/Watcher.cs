using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicWatcherService {
    public class Watcher {
        private EventLog Log { get; set; }
        private readonly FileSystemWatcher watcher;
        private readonly string[] watchFileExtensions;
        
        public Watcher(string path, string[] fileExtensions, EventLog log) {
            Log = log;
            watchFileExtensions = fileExtensions;

            watcher = new FileSystemWatcher(path);
            watcher.Created += OnCreated;
            watcher.EnableRaisingEvents = true;
        }

        private void OnCreated(object sender, FileSystemEventArgs e) {
            Log.WriteEntry(string.Format("New file detected: {0}", e.FullPath));
            try {
                FileAttributes attr = File.GetAttributes(e.FullPath);
                if (attr.HasFlag(FileAttributes.Directory)) {
                    WaitUntilTransferDone(e.FullPath);
                }
            }
            catch(Exception ex) {
                Log.WriteEntry(string.Format("Failed reading new directory with exception: {0}", ex.ToString()));
            }
        }

        private void WaitUntilTransferDone(string path) {
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            int currentAmount = files.Length;
            int previousAmount = -1;

            while(previousAmount != currentAmount) {
                Thread.Sleep(1000);
                files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                previousAmount = currentAmount;
                currentAmount = files.Length;
            }

            ReadNewDirectory(path, files);
        }

        private void ReadNewDirectory(string path, string[] files) {
            if (files.Any(x => watchFileExtensions.Contains(Path.GetExtension(x)))) {
                Log.WriteEntry(string.Format("New folder with music: {0}", path));
                Log.WriteEntry(string.Format("Files({0}):{1}{2}", files.Length, Environment.NewLine, string.Join("," + Environment.NewLine, files)));

                string appPath = AppDomain.CurrentDomain.BaseDirectory + "/MusicWatcher.exe";
                string argument = "\"" + path + "\"";

                ApplicationLoader.StartProcessAndBypassUAC(appPath + " " + argument, out ApplicationLoader.PROCESS_INFORMATION procInfo);
            }
        }

        public void Dispose() {
            watcher.Dispose();
        }
    }
}
