using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamViewer.QuickSupport.Integration
{
    using System.IO;

    public class QuickSupportDownloader
    {
        private string downloadPath;

        public ProxySettings ProxySettings { get; set; }
        
        public string DownloadUrl { get; set; } = @"http://download.teamviewer.com/download/TeamViewerQS.exe";

        public string DownloadPath
        {
            get
            {
                if (string.IsNullOrEmpty(downloadPath))
                {
                    downloadPath = GetDefaultDownloadPath();
                }
                return downloadPath;
            }
            set
            {
                downloadPath = value;
            }
        }

        public void Update()
        {
            var urlDownloader = new UrlDownloader(Path.GetDirectoryName(DownloadPath), Path.GetFileName(DownloadPath), DownloadUrl);
            urlDownloader.HttpUtils.ProxySettings = ProxySettings;
            urlDownloader.Update();
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
