using System.IO;
using System.Windows;
using MediaToolkit;
using MediaToolkit.Model;
using VideoLibrary;
using YoutubeDownloader.Properties;

namespace YoutubeDownloader
{
    class Downloader
    {
        private string _downloadFolder;
        private readonly string _appDataFolder;
        private readonly YouTube _youtube;
        private string _videosFolder;

        public Downloader(string appDataFolder)
        {
            _appDataFolder = appDataFolder;
            SetSource();
            CreateVideosFolder();

            _youtube = YouTube.Default;
        }

        public void SetSource()
        {
            _downloadFolder = @Settings.Default.DownloadFolder;
        }

        public void Download(string url)
        {
            YouTubeVideo video;

            if (_downloadFolder.Equals("none"))
            {
                MessageBox.Show("Set a download folder!");
                return;
            }

            try
            {
                video = _youtube.GetVideo(url);
            }
            catch
            {
                MessageBox.Show("Invalid URL");
                return;
            }

            if (video == null)
                return;
            
            File.WriteAllBytes(Path.Combine(_videosFolder, video.FullName), video.GetBytes());

            var inputFile = new MediaFile { Filename = Path.Combine(_videosFolder, video.FullName) };
            var outputFile = new MediaFile { Filename = Path.Combine(_downloadFolder, $"{video.FullName}.mp3") };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                engine.Convert(inputFile, outputFile);
            }

            File.Delete(Path.Combine(_videosFolder, video.FullName));
            MessageBox.Show("Done!");
        }

        private void CreateVideosFolder()
        {
            _videosFolder = Path.Combine(_appDataFolder, @"Videos");

            if (!Directory.Exists(_videosFolder))
                Directory.CreateDirectory(_videosFolder);
        }
    }
}
