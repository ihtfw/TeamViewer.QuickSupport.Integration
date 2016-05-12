namespace TeamViewer.QuickSupport.Integration
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    class UrlDownloader
    {
        private readonly string localCacheDir;

        private readonly string fileName;

        private readonly string url;

        public HttpUtils HttpUtils { get; } = new HttpUtils();

        public UrlDownloader(string localCacheDir, string fileName, string url)
        {
            if (!Directory.Exists(localCacheDir))
            {
                throw new ArgumentException("localCacheDir doesn't exist! " + localCacheDir);
            }

            this.localCacheDir = localCacheDir;
            this.fileName = fileName;
            this.url = url;
        }

        public void Update()
        {
            if (!Directory.Exists(localCacheDir))
            {
                return;
            }

            if (File.Exists(FilePath) && File.Exists(EtagPath))
            {
                var etag = File.ReadAllText(EtagPath);
                var actualEtag = HttpUtils.GetEtagHttpResponse(url);
                if (actualEtag == etag)
                {
                    return;
                }
            }

            Download();
        }

        private string FilePath => Path.Combine(localCacheDir, fileName);

        private string EtagPath => Path.Combine(localCacheDir, Path.GetFileNameWithoutExtension(fileName) + ".etag");

        private void Download()
        {
            var tmp = Path.Combine(localCacheDir, "tmp");
            if (File.Exists(tmp))
            {
                File.Delete(tmp);
            }

            HttpUtils.DownloadFile(url, tmp);
            if (!File.Exists(tmp))
            {
                return;
            }

            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
            File.Move(tmp, FilePath);

            var etag = HttpUtils.GetEtagHttpResponse(url);
            if (File.Exists(EtagPath))
            {
                File.Delete(EtagPath);
            }

            File.WriteAllText(EtagPath, etag);
        }
    }
}