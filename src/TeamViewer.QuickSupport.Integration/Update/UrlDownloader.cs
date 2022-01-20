using System;
using System.IO;

namespace TeamViewer.QuickSupport.Integration.Update
{
    class UrlDownloader
    {
        private readonly string _localCacheDir;

        private readonly string _fileName;

        private readonly string _url;

        public HttpUtils HttpUtils { get; } = new();

        public UrlDownloader(string localCacheDir, string fileName, string url)
        {
            if (!Directory.Exists(localCacheDir))
            {
                throw new ArgumentException("localCacheDir doesn't exist! " + localCacheDir);
            }

            _localCacheDir = localCacheDir;
            _fileName = fileName;
            _url = url;
        }

        public void Update()
        {
            if (!Directory.Exists(_localCacheDir))
            {
                return;
            }

            if (File.Exists(FilePath) && File.Exists(EtagPath))
            {
                var etag = File.ReadAllText(EtagPath);
                var actualEtag = HttpUtils.GetEtagHttpResponse(_url);
                if (actualEtag == etag)
                {
                    return;
                }
            }

            Download();
        }

        private string FilePath => Path.Combine(_localCacheDir, _fileName);

        private string EtagPath => Path.Combine(_localCacheDir, Path.GetFileNameWithoutExtension(_fileName) + ".etag");

        private void Download()
        {
            var tmp = Path.Combine(_localCacheDir, "tmp");
            if (File.Exists(tmp))
            {
                File.Delete(tmp);
            }

            HttpUtils.DownloadFile(_url, tmp);
            if (!File.Exists(tmp))
            {
                return;
            }

            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
            File.Move(tmp, FilePath);

            var etag = HttpUtils.GetEtagHttpResponse(_url);
            if (File.Exists(EtagPath))
            {
                File.Delete(EtagPath);
            }

            File.WriteAllText(EtagPath, etag);
        }
    }
}