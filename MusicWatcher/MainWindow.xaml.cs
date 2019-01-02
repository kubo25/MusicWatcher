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
using System.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using System.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MusicWatcher {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        public ViewModel Model { get; set; }

        public MainWindow(string path) {
            InitializeComponent();

            Model = new ViewModel(path);
            DataContext = Model;

            Model.PropertyChanged += ImageChanged;
        }
        private async void Refresh() {
            ProgressDialogController controller = await this.ShowProgressAsync("Loading tracks", "");

            await Task.Run(() => {
                foreach (double progress in Model.Init()) {
                    controller.SetProgress(progress);
                    controller.SetMessage(string.Format("Loading: {0:N0}%", progress * 100));
                }
            });

            await controller.CloseAsync();
        }


        private void OnLoad(object sender, RoutedEventArgs e) {
            Refresh();
        }

        public void RefreshCommand(object sender, ExecutedRoutedEventArgs e) {
            Refresh();
        }

        private void RefreshClick(object sender, RoutedEventArgs e) {
            Refresh();
        }

        private void OnClose(object sender, CancelEventArgs e) {
            Model.Dispose();
        }

        private void ColorizeWindow() {
            ThemeManagerHelper.CreateAppStyleBy(Model.SelectedTrack.AlbumArtDominantColor, true);
            Application.Current.MainWindow.Activate();
        }

        private async void Save() {
            ProgressDialogController controller = await this.ShowProgressAsync("Saving changes", "");

            await Task.Run(() => {
                foreach (double progress in Model.Save()) {
                    controller.SetProgress(progress);
                    controller.SetMessage(string.Format("Saving: {0:N0}%", progress * 100));
                }
            });

            await controller.CloseAsync();
        }

        private void SaveCommand(object sender, ExecutedRoutedEventArgs e) {
            Save();
        }

        private void SaveClick(object sender, RoutedEventArgs e) {
            Save();
        }

        private void SelectedTrackChanged(object sender, SelectionChangedEventArgs e) {
            if (TrackGrid.SelectedItems.Count > 1) {
                List<MusicMetadata> selectedTracks = TrackGrid.SelectedItems.Cast<MusicMetadata>().ToList();
                Model.SelectMultipleTracks(selectedTracks);
            }
            else {
                Model.DeselectMultipleTracks();
            }
        }

        private void ImageDrop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                Model.CreateNewAlbumArt((e.Data.GetData(DataFormats.FileDrop) as string[])[0], false);
            } else if (e.Data.GetDataPresent(DataFormats.Text)) {
                Model.CreateNewAlbumArt((e.Data.GetData(DataFormats.Text) as string), true);
            }
        }

        private void ImageChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "SelectedTrackAlbumArt") {
                Dispatcher.Invoke(() => ColorizeWindow());
            }
        }

        private void ServiceToggleClick(object sender, RoutedEventArgs e) {
            Model.ServiceSettings.ToggleService();
        }

        private void BrowseClick(object sender, RoutedEventArgs e) {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog {
                Title = "My Title",
                IsFolderPicker = true,
                InitialDirectory = Model.ServiceSettings.WatchFolder,

                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                DefaultDirectory = Model.ServiceSettings.WatchFolder,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                Model.ServiceSettings.WatchFolder = dialog.FileName;
            }
        }
    }
}
