using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Graphics;

namespace STACK.Test
{
    [TestClass]
    public class DisplaySettingsTests
    {
        [TestMethod]
        public void DisplaySettingsTest()
        {
            var Settings = new DisplaySettings(new Point(640, 400), new Point(1280, 800));

            Assert.AreEqual(1280, Settings.Viewport.Width);
            Assert.AreEqual(800, Settings.Viewport.Height);
            Assert.AreEqual(2, Settings.ScaleMatrix.M11);
            Assert.AreEqual(2, Settings.ScaleMatrix.M22);

            Settings.OnClientSizeChanged(640, 500);

            Assert.AreEqual(0, Settings.Viewport.X);
            Assert.AreEqual(50, Settings.Viewport.Y);
            Assert.AreEqual(640, Settings.Viewport.Width);
            Assert.AreEqual(400, Settings.Viewport.Height);
        }
    }        
}
