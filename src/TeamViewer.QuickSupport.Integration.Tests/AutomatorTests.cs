using System;

namespace TeamViewer.QuickSupport.Integration.Tests
{
    using System.Diagnostics;
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class AutomatorTests
    {
        [Test]
        public void GetInfoTest()
        {
            var automator = new Automator
                            {
                                AlternativePathToTeamViewer = @"C:\ProgramData\TeamViewerQuickSupport\TeamViewerQS.exe"
                            };

            var info = automator.GetInfo();
            Console.Out.WriteLine(info);
            Assert.IsNotNull(info);
        }
    }
}