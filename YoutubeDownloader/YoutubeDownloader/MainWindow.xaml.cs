using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using YoutubeDownloader.Properties;

namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private string _downloadFolder;
        private string _appDataFolder;
        private readonly Downloader _downloader;

        public MainWindow()
        {
            InitializeComponent();
            CreateLocalAppFolder();

            _downloader = new Downloader(_appDataFolder);

            SetDownloadFolder();
        }

        private void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if(result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Debug.WriteLine(fbd.SelectedPath);
                    Settings.Default.DownloadFolder = fbd.SelectedPath;
                    Settings.Default.Save();
                    SetDownloadFolder();
                }
            }
            Debug.WriteLine($"Setting: {(string)Settings.Default["DownloadFolder"]}");
        }

        private void SetDownloadFolder()
        {
            Settings.Default.DownloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            _downloadFolder = Settings.Default.DownloadFolder;
            _downloader.SetSource();
            FolderLocationTextBlock.Text = _downloadFolder;
        }

        private void Reset()
        {
            Debug.WriteLine("Reset");
            Settings.Default.Reset();
            SetDownloadFolder();
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", _downloadFolder);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            LinkTextBox.IsEnabled = false;
            DownloadButton.IsEnabled = false;
            _downloader.Download(LinkTextBox.Text);
            LinkTextBox.Text = "";
            LinkTextBox.IsEnabled = true;
            DownloadButton.IsEnabled = true;
        }

        private void CreateLocalAppFolder()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _appDataFolder = Path.Combine(localAppData, @"KProducts\YoutubeDownloader");

            if (!Directory.Exists(_appDataFolder))
                Directory.CreateDirectory(_appDataFolder);
        }
    }
}
