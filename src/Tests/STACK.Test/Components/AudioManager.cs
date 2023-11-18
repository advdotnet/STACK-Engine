using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Media;
using STACK.Components;

namespace STACK.Test
{
	[TestClass]
	public class AudioManagerTest
	{
		private AudioManager _manager;

		[TestInitialize]
		public void TestInitialize()
		{
			_manager = new AudioManager();
			MediaPlayer.Volume = 1;
		}

		[TestMethod, TestCategory("GPU")]
		public void SetMaxMusicVolumeFirst()
		{
			_manager.MaxMusicVolume = 1f;
			_manager.MusicVolume = 0.5f;

			Assert.AreEqual(0.5f, MediaPlayer.Volume);
		}

		[TestMethod, TestCategory("GPU")]
		public void SetMaxMusicVolumeLast()
		{
			_manager.MusicVolume = 1f;
			_manager.MaxMusicVolume = 0.5f;

			Assert.AreEqual(0.5f, MediaPlayer.Volume);
		}

		[TestMethod, TestCategory("GPU")]
		public void SetMusicEffectiveVolume()
		{
			_manager.MaxMusicVolume = 0.5f;
			_manager.MusicVolume = 0.5f;

			Assert.AreEqual(0.25f, MediaPlayer.Volume);
		}

		[TestMethod, TestCategory("GPU")]
		public void SetMusicVolumeClamp()
		{
			_manager.MaxMusicVolume = 1.5f;
			_manager.MusicVolume = 1.5f;

			Assert.AreEqual(1f, MediaPlayer.Volume);
		}

		[TestMethod, TestCategory("GPU")]
		public void SetMaxSoundEffectVolumeFirst()
		{
			_manager.MaxSoundEffectVolume = 1f;
			_manager.SoundEffectVolume = 0.5f;

			Assert.AreEqual(0.5f, _manager.SoundEffectVolume);
		}

		[TestMethod, TestCategory("GPU")]
		public void SetMaxSoundEffectVolumeLast()
		{
			_manager.SoundEffectVolume = 1f;
			_manager.MaxSoundEffectVolume = 0.5f;

			Assert.AreEqual(0.5f, _manager.EffectiveSoundEffectVolume);
		}

		[TestMethod, TestCategory("GPU")]
		public void SetSoundEffectEffectiveVolume()
		{
			_manager.MaxSoundEffectVolume = 0.5f;
			_manager.SoundEffectVolume = 0.5f;

			Assert.AreEqual(0.25f, _manager.EffectiveSoundEffectVolume);
		}

		[TestMethod, TestCategory("GPU")]
		public void SetSoundEffectEffectiveVolumeTwice()
		{
			_manager.MaxSoundEffectVolume = 0.5f;
			_manager.SoundEffectVolume = 0.5f;

			Assert.AreEqual(0.25f, _manager.EffectiveSoundEffectVolume);

			_manager.MaxSoundEffectVolume = 0.25f;

			Assert.AreEqual(0.125f, _manager.EffectiveSoundEffectVolume);
		}

		[TestMethod, TestCategory("GPU")]
		public void SetSoundEffectVolumeClamp()
		{
			_manager.MaxSoundEffectVolume = 1.5f;
			_manager.SoundEffectVolume = 1.5f;

			Assert.AreEqual(1f, _manager.SoundEffectVolume);
		}
	}
}
