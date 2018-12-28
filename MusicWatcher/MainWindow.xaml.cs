﻿using MahApps.Metro.Controls;
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

            ColorizeWindow();
        }

        private void OnClose(object sender, CancelEventArgs e) {
            foreach (MusicMetadata metadata in Model.Tracks) {
                metadata.Dispose();
            }
        }

        private void ColorizeWindow() {
            ThemeManagerHelper.CreateAppStyleBy(Model.SelectedTrack.AlbumArtDominantColor, true);
            Application.Current.MainWindow.Activate();
        }

        private void SaveCommand(object sender, ExecutedRoutedEventArgs e) {
            Model.SelectedTrack.Save();
        }

        private void SelectedTrackChanged(object sender, SelectionChangedEventArgs e) {
            ColorizeWindow();
        }

        private void ImageDrop(object sender, DragEventArgs e) {
            Model.SelectedTrack.PropertyChanged += ImageChanged;

            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                Model.SelectedTrack.CreateNewAlbumArt((e.Data.GetData(DataFormats.FileDrop) as string[])[0], false);
            } else if (e.Data.GetDataPresent(DataFormats.Text)) {
                Model.SelectedTrack.CreateNewAlbumArt((e.Data.GetData(DataFormats.Text) as string), true);
            }
        }

        private void ImageChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "BitmapAlbumArt") {
                ColorizeWindow();
                Model.SelectedTrack.PropertyChanged -= ImageChanged;
            }
        }
    }
}
