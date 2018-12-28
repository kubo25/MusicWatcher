using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using TagLib;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace MusicMetadataLibrary {
    public class MusicMetadata : INotifyPropertyChanged {
        private string path;
        public string FileName { get; }

        private string _Title { get; set; }
        public string Title {
            get { return _Title; }
            set {
                if (_Title != value) {
                    _Title = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Title"));
                }
            }
        }

        private string _Artist { get; set; }
        public string Artist {
            get { return _Artist; }
            set {
                if (_Artist != value) {
                    _Artist = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Artist"));
                }
            }
        }

        private string _Album { get; set; }
        public string Album {
            get { return _Album; }
            set {
                if (_Album != value) {
                    _Album = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Album"));
                }
            }
        }

        private uint _Year { get; set; }
        public uint Year {
            get { return _Year; }
            set {
                if (_Year != value) {
                    _Year = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Year"));
                }
            }
        }

        private uint _Track { get; set; }
        public uint Track {
            get { return _Track; }
            set {
                if (_Track != value) {
                    _Track = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Track"));
                }
            }
        }

        private string _Genre { get; set; }
        public string Genre {
            get { return _Genre; }
            set {
                if (_Genre != value) {
                    _Genre = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Genre"));
                }
            }
        }

        private string _Comment { get; set; }
        public string Comment {
            get { return _Comment; }
            set {
                if (_Comment != value) {
                    _Comment = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Comment"));
                }
            }
        }

        private string _AlbumArtist { get; set; }
        public string AlbumArtist {
            get { return _AlbumArtist; }
            set {
                if (_AlbumArtist != value) {
                    _AlbumArtist = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("AlbumArtist"));
                }
            }
        }

        private string _Composer { get; set; }
        public string Composer {
            get { return _Composer; }
            set {
                if (_Composer != value) {
                    _Composer = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Composer"));
                }
            }
        }

        private uint _Discnumber { get; set; }
        public uint Discnumber {
            get { return _Discnumber; }
            set {
                if (_Discnumber != value) {
                    _Discnumber = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Discnumber"));
                }
            }
        }

        public byte[] AlbumArt { get; set; }

        private TagLib.File file;

        public event PropertyChangedEventHandler PropertyChanged;

        public MusicMetadata(string path) {
            this.path = path;
            file = TagLib.File.Create(path);

            FileName = Path.GetFileName(path);
            _Title = file.Tag.Title;
            _Artist = file.Tag.FirstPerformer;
            _Album = file.Tag.Album;
            _Year = file.Tag.Year;
            _Track = file.Tag.Track;
            _Genre = file.Tag.FirstGenre;
            _Comment = file.Tag.Comment;
            _AlbumArtist = file.Tag.FirstAlbumArtist;
            _Composer = file.Tag.FirstComposer;
            _Discnumber = file.Tag.Disc;

            if (file.Tag.Pictures.Length > 0) {
                AlbumArt = file.Tag.Pictures[0].Data.Data;
            }
        }

        public void Save() {
            Tag tag = new TagLib.Id3v2.Tag {
                Title = Title,
                Performers = new string[] { Artist },
                Album = Album,
                Year = Year,
                Track = Track,
                Genres = new string[] { Genre },
                Comment = Comment,
                AlbumArtists = new string[] { AlbumArtist },
                Composers = new string[] { Composer },
                Disc = Discnumber
            };

            ByteVector artVector = new ByteVector(AlbumArt);
            IPicture[] picture = new IPicture[] { new Picture(artVector) };

            file.RemoveTags(TagTypes.AllTags); //To remove all unused tags like old pictures first delete all tags
            file.Save();
            Dispose();

            file = TagLib.File.Create(path);
            tag.CopyTo(file.Tag, true);
            file.Tag.Pictures = picture;
            file.Save();
        }

        public void Dispose() {
            file.Dispose();
        }
    }
}
