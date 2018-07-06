using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Media;
using STACK.Components;

namespace STACK.Test
{
    [TestClass]
    public class AudioManagerTest
    {
        AudioManager Manager;

        [TestInitialize]
        public void TestInitialize()
        {
            Manager = new AudioManager();
            MediaPlayer.Volume = 1;
        }

        [TestMethod]
        public void SetMaxMusicVolumeFirst()
        {
            Manager.MaxMusicVolume = 1f;
            Manager.MusicVolume = 0.5f;

            Assert.AreEqual(0.5f, MediaPlayer.Volume);
        }

        [TestMethod]
        public void SetMaxMusicVolumeLast()
        {
            Manager.MusicVolume = 1f;
            Manager.MaxMusicVolume = 0.5f;

            Assert.AreEqual(0.5f, MediaPlayer.Volume);
        }

        [TestMethod]
        public void SetMusicEffectiveVolume()
        {
            Manager.MaxMusicVolume = 0.5f;
            Manager.MusicVolume = 0.5f;

            Assert.AreEqual(0.25f, MediaPlayer.Volume);
        }

        [TestMethod]
        public void SetMusicVolumeClamp()
        {
            Manager.MaxMusicVolume = 1.5f;
            Manager.MusicVolume = 1.5f;

            Assert.AreEqual(1f, MediaPlayer.Volume);
        }

        [TestMethod]
        public void SetMaxSoundEffectVolumeFirst()
        {
            Manager.MaxSoundEffectVolume = 1f;
            Manager.SoundEffectVolume = 0.5f;

            Assert.AreEqual(0.5f, Manager.SoundEffectVolume);
        }

        [TestMethod]
        public void SetMaxSoundEffectVolumeLast()
        {
            Manager.SoundEffectVolume = 1f;
            Manager.MaxSoundEffectVolume = 0.5f;

            Assert.AreEqual(0.5f, Manager.EffectiveSoundEffectVolume);
        }

        [TestMethod]
        public void SetSoundEffectEffectiveVolume()
        {
            Manager.MaxSoundEffectVolume = 0.5f;
            Manager.SoundEffectVolume = 0.5f;

            Assert.AreEqual(0.25f, Manager.EffectiveSoundEffectVolume);
        }

        [TestMethod]
        public void SetSoundEffectEffectiveVolumeTwice()
        {
            Manager.MaxSoundEffectVolume = 0.5f;
            Manager.SoundEffectVolume = 0.5f;

            Assert.AreEqual(0.25f, Manager.EffectiveSoundEffectVolume);

            Manager.MaxSoundEffectVolume = 0.25f;

            Assert.AreEqual(0.125f, Manager.EffectiveSoundEffectVolume);
        }

        [TestMethod]
        public void SetSoundEffectVolumeClamp()
        {
            Manager.MaxSoundEffectVolume = 1.5f;
            Manager.SoundEffectVolume = 1.5f;

            Assert.AreEqual(1f, Manager.SoundEffectVolume);
        }
    }
}
