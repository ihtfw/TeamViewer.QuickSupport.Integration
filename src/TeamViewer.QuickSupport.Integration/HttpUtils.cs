namespace TeamViewer.QuickSupport.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    class HttpUtils
    {
        private ProxySettings proxySettings;

        public ProxySettings ProxySettings { get { return proxySettings ?? (proxySettings = new ProxySettings()); } set { proxySettings = value; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="System.Net.WebException ">If url not exist</exception>
        public string GetEtagHttpResponse(string url)
        {
            return GetEtagHttpResponse(new Uri(url));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="System.Net.WebException ">If url not exist</exception>
        public string GetEtagHttpResponse(Uri uri)
        {
            var headers = GetHttpResponseHeaders(uri);
            string etag;
            if (headers.TryGetValue("ETag", out etag))
            {
                return etag;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns> 
        /// <exception cref="System.Net.WebException ">If url not exist</exception>
        public Dictionary<string, string> GetHttpResponseHeaders(string url)
        {
            return GetHttpResponseHeaders(new Uri(url));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="System.Net.WebException ">If url not exist</exception>
        public Dictionary<string, string> GetHttpResponseHeaders(Uri uri)
        {
            var headers = new Dictionary<string, string>();
            var webRequest = WebRequest.Create(uri);
            if (ProxySettings.Use)
            {
                var webProxy = new WebProxy(ProxySettings.Address, true);
                var cred = new NetworkCredential(ProxySettings.Login, ProxySettings.Password);
                webProxy.Credentials = cred;
                webRequest.Proxy = webProxy;
            }
            webRequest.Method = "HEAD";
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                foreach (string header in webResponse.Headers)
                {
                    headers.Add(header, webResponse.Headers[header]);
                }
            }

            return headers;
        }

        public void DownloadFile(string url, string fileName)
        {
            using (var client = new WebClient())
            {
                if (ProxySettings.Use)
                {
                    var webProxy = new WebProxy(ProxySettings.Address, true);
                    var cred = new NetworkCredential(ProxySettings.Login, ProxySettings.Password);
                    webProxy.Credentials = cred;
                    client.Proxy = webProxy;
                }
                client.DownloadFile(url, fileName);
            }
        }

    }
}