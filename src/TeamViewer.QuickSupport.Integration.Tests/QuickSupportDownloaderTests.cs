using TeamViewer.QuickSupport.Integration.Update;

namespace TeamViewer.QuickSupport.Integration.Tests
{
    using System.IO;

    using NUnit.Framework;

    [TestFixture]
    public class QuickSupportDownloaderTests
    {
        [Test]
        public void UpdateTest()
        {
            var sut = new QuickSupportDownloader();
            sut.Update();

            var path = sut.DownloadPath;

            Assert.IsTrue(File.Exists(path));
        }
    }
}
