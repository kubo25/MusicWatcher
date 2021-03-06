﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MusicWatcher {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private void Application_Startup(object sender, StartupEventArgs e) {
            if (e.Args.Length > 0) {
                MainWindow window = new MainWindow(e.Args[0]);
                window.Show();
            }
            else {
                MessageBox.Show("No path to a folder provided.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
        }
    }
}
