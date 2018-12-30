using MusicMetadataLibrary;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MusicWatcher {
    public class ViewModel : INotifyPropertyChanged {
        private readonly NameValueCollection settings = ConfigurationManager.AppSettings;
        private readonly BitmapImage noAlbumArt = new BitmapImage(new Uri("pack://application:,,,/Images/NoAlbumArt.jpg"));

        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapImage SelectedTrackAlbumArt {
            get {
                if (SelectedTrack.BitmapAlbumArt != null) {
                    return SelectedTrack.BitmapAlbumArt;
                }
                else {
                    return noAlbumArt;
                }
            }
        }

        private MusicMetadata _SelectedTrack { get; set; }
        public MusicMetadata SelectedTrack {
            get { return _SelectedTrack; }
            set {
                _SelectedTrack = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedTrack"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedTrackAlbumArt"));
            }
        }
        public List<MusicMetadata> Tracks { get; set; }

        public ViewModel(string path) {
            Tracks = new List<MusicMetadata>();
            string[] files = Directory.GetFiles(path);
            foreach (string file in files) {
                Tracks.Add(new MusicMetadata(file));
            }
            SelectedTrack = Tracks[0];
        }

        public void Dispose() {
            foreach (MusicMetadata metadata in Tracks) {
                metadata.Dispose();
            }
        }

        public void CreateNewAlbumArt(string file, bool isBase64) {
            SelectedTrack.CreateNewAlbumArt(file, isBase64);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedTrackAlbumArt"));
        }
    }
}
