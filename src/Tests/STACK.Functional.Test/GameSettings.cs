using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using System.IO;

namespace STACK.Test
{
    [TestClass]
    public class GameSettingsTests
    {
        /// <summary>
        /// Execute this test in the STACK.Functional.Test test project as SDL2.dll is required
        /// </summary>
        [TestMethod, TestCategory("FileSystem")]
        public void GameSettingsTest()
        {
            const string FILEPATH = "test";

            var Resolution = new Point(10, 10);
            var Volume = 0.4f;
            var Adapter = 1;
            var Mode = DisplayMode.Window;
            var VSync = true;
            var MultiSampling = false;

            var Settings = new GameSettings()
            {
                Resolution = Resolution,
                SoundEffectVolume = Volume,
                MusicVolume = Volume,
                Adapter = Adapter,
                DisplayMode = Mode,
                VSync = VSync,
                MultiSampling = MultiSampling
            };

            Settings.Save(FILEPATH);

            var FileName = System.IO.Path.Combine(SaveGame.UserStorageFolder(FILEPATH), GameSettings.CONFIGFILENAME);

            using (var FileStream = File.Open(FileName, FileMode.Open))
            {
                var DeserializedSettings = GameSettings.DeserializeFromStream(FileStream);
                Assert.AreEqual(Volume, DeserializedSettings.MusicVolume);
                Assert.AreEqual(Volume, DeserializedSettings.SoundEffectVolume);
                Assert.AreEqual(Resolution, DeserializedSettings.Resolution);
                Assert.AreEqual(Adapter, DeserializedSettings.Adapter);
                Assert.AreEqual(Mode, DeserializedSettings.DisplayMode);
                Assert.AreEqual(VSync, DeserializedSettings.VSync);
                Assert.AreEqual(MultiSampling, DeserializedSettings.MultiSampling);
            }

            File.Delete(FileName);
        }
    }
}
