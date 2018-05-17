﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using System.IO;

namespace STACK.Test
{
    [TestClass]
    public class GameSettingsTests
    {
        [TestMethod, TestCategory("FileSystem")]
        public void GameSettingsTest()
        {
            const string FILENAME = GameSettings.CONFIGFILENAME;

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

            Settings.Save(FILENAME);

            using (var FileStream = File.Open(FILENAME, FileMode.Open))
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

            File.Delete(FILENAME);
        }
    }
}