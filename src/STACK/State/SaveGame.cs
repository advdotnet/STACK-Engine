using SDL2;
using STACK.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace STACK
{
	[Serializable]
	public class SaveGame
	{
		private const string _fileExtension = ".dat";

		public byte[] World;
		public string Name = "SaveGame";
		public string Culture;
		public DateTime Date;
		public byte[] Screenshot;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Nicht verwendete Parameter entfernen", Justification = "<Ausstehend>")]
		public SaveGame(string name, byte[] world, byte[] screen)
		{
			World = world;
			Name = name;
			Culture = GameSettings.GetCurrentCultureName();
			//ScreenShot = screen;
			Date = DateTime.Now;
		}

		/// <summary>
		/// Stores the current state of all scenes to the given filename.
		/// </summary>        
		public static SaveGame SaveToFile(string folder, string name, World world, byte[] screen)
		{
			Log.WriteLine("Saving game " + name);
			EnsureStorageFolderExists(folder);

			var fileName = GetFileName(name, folder);
			var serializedWorld = State.Serialization.SaveState(world);
			var saveGame = new SaveGame(name, serializedWorld, screen);

			State.Serialization.SaveToFile(GetFilePath(folder, fileName), saveGame);

			return saveGame;
		}

		private static string GetFilePath(string folder, string filename)
		{
			return System.IO.Path.Combine(UserStorageFolder(folder), filename + _fileExtension);
		}

		private static string GetFileName(string name, string folder)
		{
			var index = 1;
			var filename = "game" + index;
			var found = false;

			foreach (var existingSaveGame in GetSaveGames(folder))
			{
				if (existingSaveGame.Value.Name == name)
				{
					filename = existingSaveGame.Key;
					found = true;
				}
			}

			if (!found)
			{
				while (File.Exists(GetFilePath(folder, filename)))
				{
					index++;
					filename = "game" + index;
				}
			}

			return filename;
		}

		public static string ExistsStateByName(string folder, string name)
		{
			var saves = GetSaveGames(folder);
			var save = saves.FirstOrDefault(s => s.Value != null && s.Value.Name.Equals(name));

			if (!save.Equals(default(KeyValuePair<string, SaveGame>)))
			{
				return save.Key;
			}

			return null;
		}

		public static SaveGame LoadFromFile(string folder, string fileName)
		{
			try
			{
				return State.Serialization.LoadFromFile<SaveGame>(GetFilePath(folder, fileName));
			}
			catch (Exception e)
			{
				Log.WriteLine("Could not open savegame " + fileName + ": " + e.Message, LogLevel.Error);
			}

			return null;
		}

		internal static Dictionary<string, SaveGame> GetSaveGames(string folder)
		{
			EnsureStorageFolderExists(folder);

			var files = Directory
				.EnumerateFiles(UserStorageFolder(folder), "*" + _fileExtension)
				.ToArray();

			return files
				.Select(file => new
				{
					Filename = System.IO.Path.GetFileNameWithoutExtension(file),
					Savegame = LoadFromFile(folder, System.IO.Path.GetFileNameWithoutExtension(file))
				})
				.Where(t => t.Savegame != null)
				.ToDictionary(t => t.Filename, t => t.Savegame);
		}

		public static string UserStorageFolder(string subdirectory = "STACK")
		{
			var platform = SDL.SDL_GetPlatform();

			if (platform.Equals("Windows"))
			{
				return System.IO.Path.Combine(
					Environment.GetFolderPath(
						Environment.SpecialFolder.MyDocuments
					),
					"SavedGames",
					subdirectory
				);
			}
			else if (platform.Equals("Mac OS X"))
			{
				var osConfigDir = Environment.GetEnvironmentVariable("HOME");
				if (string.IsNullOrEmpty(osConfigDir))
				{
					return "."; // Oh well.
				}
				osConfigDir += "/Library/Application Support";

				return System.IO.Path.Combine(osConfigDir, subdirectory);
			}
			else if (platform.Equals("Linux"))
			{
				var osConfigDir = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
				if (string.IsNullOrEmpty(osConfigDir))
				{
					osConfigDir = Environment.GetEnvironmentVariable("HOME");
					if (string.IsNullOrEmpty(osConfigDir))
					{
						return "."; // Oh well.
					}
					osConfigDir += "/.local/share";
				}

				return System.IO.Path.Combine(osConfigDir, subdirectory);
			}

			throw new Exception("SDL platform unhandled: " + platform);
		}

		public static void EnsureStorageFolderExists(string folder)
		{
			var storageFolder = UserStorageFolder(folder);

			if (!Directory.Exists(storageFolder))
			{
				Directory.CreateDirectory(storageFolder);
			}
		}
	}
}
