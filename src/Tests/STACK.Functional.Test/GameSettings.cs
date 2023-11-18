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
			const string filePath = "test";

			var resolution = new Point(10, 10);
			var volume = 0.4f;
			var adapter = 1;
			var mode = DisplayMode.Window;
			var vSync = true;
			var multiSampling = false;

			var settings = new GameSettings()
			{
				Resolution = resolution,
				SoundEffectVolume = volume,
				MusicVolume = volume,
				Adapter = adapter,
				DisplayMode = mode,
				VSync = vSync,
				MultiSampling = multiSampling
			};

			settings.Save(filePath);

			var fileName = System.IO.Path.Combine(SaveGame.UserStorageFolder(filePath), GameSettings.CONFIGFILENAME);

			using (var fileStream = File.Open(fileName, FileMode.Open))
			{
				var deserializedSettings = GameSettings.DeserializeFromStream(fileStream);
				Assert.AreEqual(volume, deserializedSettings.MusicVolume);
				Assert.AreEqual(volume, deserializedSettings.SoundEffectVolume);
				Assert.AreEqual(resolution, deserializedSettings.Resolution);
				Assert.AreEqual(adapter, deserializedSettings.Adapter);
				Assert.AreEqual(mode, deserializedSettings.DisplayMode);
				Assert.AreEqual(vSync, deserializedSettings.VSync);
				Assert.AreEqual(multiSampling, deserializedSettings.MultiSampling);
			}

			File.Delete(fileName);
		}
	}
}
