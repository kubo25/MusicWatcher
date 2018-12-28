using MusicMetadataLibrary;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWatcher {
    public class ViewModel {
        private readonly NameValueCollection settings = ConfigurationManager.AppSettings;

        public MusicMetadata SelectedTrack { get; set; }
        public List<MusicMetadata> Tracks { get; set; }

        public ViewModel(string path) {
            Tracks = new List<MusicMetadata>();
            string[] files = Directory.GetFiles(path);
            foreach (string file in files) {
                Tracks.Add(new MusicMetadata(file));
            }
            SelectedTrack = Tracks[0];
        }
    }
}
