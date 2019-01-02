using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TagLib;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace MusicMetadataLibrary {
    public class MusicMetadata : INotifyPropertyChanged {
        private string path;

        public string FileName { get; }

        private string _Title;
        public string Title {
            get { return _Title; }
            set {
                if (_Title != value) {
                    _Title = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Title"));
                }
            }
        }

        private string _Artist;
        public string Artist {
            get { return _Artist; }
            set {
                if (_Artist != value) {
                    _Artist = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Artist"));
                }
            }
        }

        private string _Album;
        public string Album {
            get { return _Album; }
            set {
                if (_Album != value) {
                    _Album = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Album"));
                }
            }
        }

        private uint _Year;
        public uint Year {
            get { return _Year; }
            set {
                if (_Year != value) {
                    _Year = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Year"));
                }
            }
        }

        private uint _Track;
        public uint Track {
            get { return _Track; }
            set {
                if (_Track != value) {
                    _Track = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Track"));
                }
            }
        }

        private string _Genre;
        public string Genre {
            get { return _Genre; }
            set {
                if (_Genre != value) {
                    _Genre = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Genre"));
                }
            }
        }

        private string _Comment;
        public string Comment {
            get { return _Comment; }
            set {
                if (_Comment != value) {
                    _Comment = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Comment"));
                }
            }
        }

        private string _AlbumArtist;
        public string AlbumArtist {
            get { return _AlbumArtist; }
            set {
                if (_AlbumArtist != value) {
                    _AlbumArtist = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AlbumArtist"));
                }
            }
        }

        private string _Composer;
        public string Composer {
            get { return _Composer; }
            set {
                if (_Composer != value) {
                    _Composer = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Composer"));
                }
            }
        }

        private uint _Discnumber;
        public uint Discnumber {
            get { return _Discnumber; }
            set {
                if (_Discnumber != value) {
                    _Discnumber = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Discnumber"));
                }
            }
        }

        private byte[] _AlbumArt;
        public byte[] AlbumArt {
            get { return _AlbumArt; }
            set {
                _AlbumArt = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AlbumArt"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AlbumArtSize"));
            }
        }

        public BitmapImage BitmapAlbumArt { get; set; }

        public System.Windows.Media.Color AlbumArtDominantColor { get; set; }
        public string AlbumArtSize {
            get {
                if (AlbumArt != null) {
                    double size = AlbumArt.Length;
                    if (AlbumArt.Length >= 1024 * 1024) {
                        size /= 1024 * 1024;
                        return size.ToString("0.00") + "MB";
                    }
                    else if (AlbumArt.Length >= 1024) {
                        size /= 1024;
                        return size.ToString("0.00") + "kB";
                    }
                    else {
                        return size.ToString() + "B";
                    }
                }
                else {
                    return "No album art.";
                }
            }
        }

        private TagLib.File file;

        public event PropertyChangedEventHandler PropertyChanged;

        public MusicMetadata() { }

        public MusicMetadata(MusicMetadata original) {
            path = original.path;
            file = TagLib.File.Create(path);

            FileName = original.FileName;
            _Title = original.Title;
            _Artist = original.Artist;
            _Album = original.Album;
            _Year = original.Year;
            _Track = original.Track;
            _Genre = original.Genre;
            _Comment = original.Comment;
            _AlbumArtist = original.AlbumArtist;
            _Composer = original.Composer;
            _Discnumber = original.Discnumber;
            _AlbumArt = original.AlbumArt;
            BitmapAlbumArt = original.BitmapAlbumArt;
            AlbumArtDominantColor = original.AlbumArtDominantColor;
        }

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
                _AlbumArt = file.Tag.Pictures[0].Data.Data;
                ConvertAlbumArt();
            }
        }

        private void ConvertAlbumArt() {
            BitmapAlbumArt = new BitmapImage();
            Bitmap bitmap;
            using (MemoryStream stream = new MemoryStream(AlbumArt)) {
                stream.Position = 0;
                BitmapAlbumArt.BeginInit();
                BitmapAlbumArt.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                BitmapAlbumArt.CacheOption = BitmapCacheOption.OnLoad;
                BitmapAlbumArt.UriSource = null;
                BitmapAlbumArt.StreamSource = stream;
                BitmapAlbumArt.EndInit();
                bitmap = (Bitmap)Image.FromStream(stream);
            }
            BitmapAlbumArt.Freeze();

            AlbumArtDominantColor = CalculateAverageColor(bitmap);
            bitmap.Dispose();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BitmapAlbumArt"));
        }

        public void CreateNewAlbumArt(string file, bool isBase64) {
            if (isBase64) {
                AlbumArt = Convert.FromBase64String(Regex.Replace(file, ".*,", string.Empty));
                ConvertAlbumArt();
            } else {
                AlbumArt = System.IO.File.ReadAllBytes(file);
                ConvertAlbumArt();
            }
        }

        //Originated from https://stackoverflow.com/questions/6177499/how-to-determine-the-background-color-of-document-when-there-are-3-options-usin/6185448#6185448
        private System.Windows.Media.Color CalculateAverageColor(Bitmap bm) {
            int width = bm.Width;
            int height = bm.Height;
            int red;
            int green;
            int blue;
            int white = 0;
            int black = 0;
            int minDiversion = 15; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
            int dropped = 0; // keep track of dropped pixels
            long[] totals = new long[] { 0, 0, 0 };
            int bppModifier = bm.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4; // cutting corners, will fail on anything else but 32 and 24 bit images

            BitmapData srcData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, bm.PixelFormat);
            int stride = srcData.Stride;
            IntPtr Scan0 = srcData.Scan0;

            unsafe {
                byte* p = (byte*)(void*)Scan0;

                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        int idx = (y * stride) + x * bppModifier;
                        red = p[idx + 2];
                        green = p[idx + 1];
                        blue = p[idx];
                        if (Math.Abs(red - green) > minDiversion || Math.Abs(red - blue) > minDiversion || Math.Abs(green - blue) > minDiversion) {
                            totals[2] += red;
                            totals[1] += green;
                            totals[0] += blue;
                        }
                        else {
                            if (red == 0) {
                                black += 1;
                            } else {
                                white = 0;
                            }
                            dropped++;
                        }
                    }
                }
            }

            int count = width * height - dropped;
            byte avgR;
            byte avgG;
            byte avgB;
            if (count > 0) {
                avgR = (byte)(totals[2] / count);
                avgG = (byte)(totals[1] / count);
                avgB = (byte)(totals[0] / count);
            } else {
                if (black > white) {
                    avgR = 0;
                    avgG = 0;
                    avgB = 0;
                } else {
                    avgR = 255;
                    avgG = 255;
                    avgB = 255;
                }
            }

            return System.Windows.Media.Color.FromRgb(avgR, avgG, avgB);
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
