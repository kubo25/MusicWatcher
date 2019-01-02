using MusicMetadataLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace MusicWatcher {
    public class ViewModel : INotifyPropertyChanged {
        private readonly NameValueCollection settings = ConfigurationManager.AppSettings;
        private readonly BitmapImage noAlbumArt = new BitmapImage(new Uri("pack://application:,,,/Images/NoAlbumArt.jpg"));
        private readonly string loadedPath;

        private bool multipleTracksSelected = false;
        private List<MusicMetadata> selectedTracks;

        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapImage SelectedTrackAlbumArt {
            get {
                if (SelectedTrack != null && SelectedTrack.BitmapAlbumArt != null) {
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
        public ObservableCollection<MusicMetadata> Tracks { get; set; }

        public ViewModel(string path) {
            loadedPath = path;
        }

        public IEnumerable<double> Init() {
            double progress = 0f;

            Tracks = new ObservableCollection<MusicMetadata>();
            string[] files = Directory.GetFiles(loadedPath);

            foreach (string file in files) {
                Tracks.Add(new MusicMetadata(file));
                progress += 1f / files.Length;
                yield return progress;

            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tracks"));
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

        public void SelectMultipleTracks(List<MusicMetadata> selectedTracks) {
            this.selectedTracks = selectedTracks;
            SelectedTrack = new MusicMetadata(selectedTracks[0]);

            foreach(MusicMetadata track in selectedTracks) {
                SelectedTrack.Title = "";
                SelectedTrack.Track = 0;
                SelectedTrack.Artist = SelectedTrack.Artist == track.Artist ? SelectedTrack.Artist : "";
                SelectedTrack.Album = SelectedTrack.Album == track.Album ? SelectedTrack.Album : "";
                SelectedTrack.Year = SelectedTrack.Year == track.Year ? SelectedTrack.Year : 0;
                SelectedTrack.Genre = SelectedTrack.Genre == track.Genre ? SelectedTrack.Genre : "";
                SelectedTrack.Comment = SelectedTrack.Comment == track.Comment ? SelectedTrack.Comment : "";
                SelectedTrack.AlbumArtist = SelectedTrack.AlbumArtist == track.AlbumArtist ? SelectedTrack.AlbumArtist : "";
                SelectedTrack.Composer = SelectedTrack.Composer == track.Composer ? SelectedTrack.Composer : "";
                SelectedTrack.Discnumber = SelectedTrack.Discnumber == track.Discnumber ? SelectedTrack.Discnumber : 0;

                if (SelectedTrack.AlbumArt != null && !SelectedTrack.AlbumArt.SequenceEqual(track.AlbumArt)) {
                    SelectedTrack.AlbumArt = null;
                    SelectedTrack.BitmapAlbumArt = null;
                }
            }

            multipleTracksSelected = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedTrackAlbumArt"));
        }

        public void DeselectMultipleTracks() {
            selectedTracks = null;
            multipleTracksSelected = false;
        }

        public IEnumerable<double> Save() {
            if (multipleTracksSelected) {
                double progress = 0f;
                foreach (MusicMetadata track in selectedTracks) {
                    track.Title = SelectedTrack.Title != "" ? SelectedTrack.Title : track.Title;
                    track.Track = SelectedTrack.Track != 0 ? SelectedTrack.Track : track.Track;
                    track.Artist = SelectedTrack.Artist != "" ? SelectedTrack.Artist : track.Artist;
                    track.Album = SelectedTrack.Album != "" ? SelectedTrack.Album : track.Album;
                    track.Year = SelectedTrack.Year != 0 ? SelectedTrack.Year : track.Year;
                    track.Genre = SelectedTrack.Genre != "" ? SelectedTrack.Genre : track.Genre;
                    track.Comment = SelectedTrack.Comment != "" ? SelectedTrack.Comment : track.Comment;
                    track.AlbumArtist = SelectedTrack.AlbumArtist != "" ? SelectedTrack.AlbumArtist : track.AlbumArtist;
                    track.Composer = SelectedTrack.Composer != "" ? SelectedTrack.Composer : track.Composer;
                    track.Discnumber = SelectedTrack.Discnumber != 0 ? SelectedTrack.Discnumber : track.Discnumber;
                    
                    if(SelectedTrack.AlbumArt != null) {
                        track.AlbumArt = SelectedTrack.AlbumArt;
                        track.BitmapAlbumArt = SelectedTrack.BitmapAlbumArt;
                        track.AlbumArtDominantColor = SelectedTrack.AlbumArtDominantColor;
                    }

                    track.Save();
                    progress += 1f / selectedTracks.Count;
                    yield return progress;
                }
            }
            else {
                SelectedTrack.Save();
                yield return 1;
            }
        }
    }
}
