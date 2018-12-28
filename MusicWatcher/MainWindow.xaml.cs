using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using MusicMetadataLibrary;
using ColorThiefDotNet;
using System.ComponentModel;

namespace MusicWatcher {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        private static string testPath = @"D:\Hudba\A Perfect Circle\Eat the Elephant";

        public ViewModel Model { get; set; }

        private ColorThief colorThief = new ColorThief();
        
        public MainWindow() {
            InitializeComponent();

            Model = new ViewModel(testPath);
            DataContext = Model;

            ShowAlbumArt();
        }

        private void OnClose(object sender, CancelEventArgs e) {
            foreach (MusicMetadata metadata in Model.Tracks) {
                metadata.Dispose();
            }
        }

        private void ShowAlbumArt() {
            if (Model.SelectedTrack.AlbumArt != null) { 
                BitmapImage image = new BitmapImage();
                System.Drawing.Bitmap bitmap;
                using (MemoryStream stream = new MemoryStream(Model.SelectedTrack.AlbumArt)) {
                    stream.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = stream;
                    image.EndInit();
                    bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(stream);
                }
                image.Freeze();
                AlbumArt.Source = image;

                QuantizedColor stolenColor = colorThief.GetColor(bitmap);
                bitmap.Dispose();
                System.Windows.Media.Color color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(stolenColor.Color.ToHexString());
                ThemeManagerHelper.CreateAppStyleBy(color, true);
                Application.Current.MainWindow.Activate();
            }
        }

        private byte[] ConvertImage() {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapImage)AlbumArt.Source));
            using (MemoryStream stream = new MemoryStream()) {
                encoder.Save(stream);
                data = stream.ToArray();
            }
            return data;
        }

        private void SaveCommand(object sender, ExecutedRoutedEventArgs e) {
            Model.SelectedTrack.Save();
        }
    }
}
