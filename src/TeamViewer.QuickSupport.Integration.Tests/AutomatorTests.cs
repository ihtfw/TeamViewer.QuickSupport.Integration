using System;

namespace TeamViewer.QuickSupport.Integration.Tests
{
    using System.Diagnostics;

    using NUnit.Framework;

    [TestFixture]
    public class AutomatorTests
    {
        [Test]
        public void GetInfoTest()
        {
            var automator = new Automator();

            var info = automator.GetInfo();
            Console.Out.WriteLine(info);
            Assert.IsNotNull(info);
        }
    }
}
