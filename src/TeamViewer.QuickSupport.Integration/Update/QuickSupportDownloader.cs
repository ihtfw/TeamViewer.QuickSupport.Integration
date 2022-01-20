using System;
using System.IO;

namespace TeamViewer.QuickSupport.Integration.Update
{
    public class QuickSupportDownloader
    {
        private string _downloadPath;

        public ProxySettings ProxySettings { get; set; }
        
        public string DownloadUrl { get; set; } = @"http://download.teamviewer.com/download/TeamViewerQS.exe";

        public string DownloadPath
        {
            get
            {
                if (string.IsNullOrEmpty(_downloadPath))
                {
                    _downloadPath = GetDefaultDownloadPath();
                }
                return _downloadPath;
            }
            set => _downloadPath = value;
        }

        public void Update()
        {
            var downloader = new UrlDownloader(Path.GetDirectoryName(DownloadPath), Path.GetFileName(DownloadPath), DownloadUrl)
                {
                    HttpUtils =
                    {
                        ProxySettings = ProxySettings
                    }
                };
            downloader.Update();
        }

        public string GetDefaultDownloadPath()
        {
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var tvDir = Path.Combine(programData, "TeamViewerQuickSupport");
            if (!Directory.Exists(tvDir))
            {
                Directory.CreateDirectory(tvDir);
            }

            return Path.Combine(tvDir, "TeamViewerQS.exe");
        }
    }
}
