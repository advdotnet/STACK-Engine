using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Graphics;
using System.IO;

namespace STACK.Test
{
    [TestClass]
    public class GraphicsSettingsTests
    {
        [TestMethod, TestCategory("FileSystem")]
        public void ResolutionSettingsTest()
        {
            const string FILENAME = GraphicSettings.CONFIGFILENAME;

            var Resolution = new Point(10, 10);
            var Adapter = 1;
            var Mode = DisplayMode.Window;
            var VSync = true;
            var MultiSampling = false;

            var Settings = new GraphicSettings()
            {
                Resolution = Resolution,
                Adapter = Adapter,
                DisplayMode = Mode,
                VSync = VSync,
                MultiSampling = MultiSampling
            };

            Settings.Save(FILENAME);

            using (var FileStream = File.Open(FILENAME, FileMode.Open))
            {
                var DeserializedSettings = GraphicSettings.DeserializeFromStream(FileStream);
                Assert.AreEqual(Resolution, DeserializedSettings.Resolution);
                Assert.AreEqual(Adapter, DeserializedSettings.Adapter);
                Assert.AreEqual(Mode, DeserializedSettings.DisplayMode);
                Assert.AreEqual(VSync, DeserializedSettings.VSync);
                Assert.AreEqual(MultiSampling, DeserializedSettings.MultiSampling);
            }

            File.Delete(FILENAME);
        }
    }
}
