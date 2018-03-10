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
        const string FILE_EXTENSION = ".dat";

        public byte[] World;
        public string Name = "SaveGame";
        public string Culture;
        public DateTime Date;
        public byte[] Screenshot;

        private SaveGame()
        {
            Culture = GameSettings.GetCurrentCultureName();
        }

        public SaveGame(string name, byte[] world, byte[] screen)
        {
            World = world;
            Name = name;
            //ScreenShot = screen;
            Date = DateTime.Now;
        }

        /// <summary>
        /// Stores the current state of all scenes to the given filename.
        /// </summary>        
        public static void SaveToFile(string folder, string name, World world, byte[] screen)
        {
            Log.WriteLine("Saving game " + name);
            SaveGame.EnsureStorageFolderExists(folder);

            int index = 1;
            string Filename = "game" + index;
            bool Found = false;

            foreach (var ExistingSaveGame in GetSaveGames(folder))
            {
                if (ExistingSaveGame.Value.Name == name)
                {
                    Filename = ExistingSaveGame.Key;
                    Found = true;
                }
            }

            if (!Found)
            {
                while (File.Exists(GetFilePath(folder, Filename)))
                {
                    index++;
                    Filename = "game" + index;
                }
            }

            var SerializedWorld = STACK.State.Serialization.SaveState<World>(world);

            STACK.State.Serialization.SaveToFile<SaveGame>(GetFilePath(folder, Filename), new SaveGame(name, SerializedWorld, screen));
        }

        private static string GetFilePath(string folder, string filename)
        {
            return System.IO.Path.Combine(SaveGame.UserStorageFolder(folder), filename + FILE_EXTENSION);
        }

        public static SaveGame LoadFromFile(string folder, string name)
        {
            Log.WriteLine("Loading game " + name);
            try
            {
                return STACK.State.Serialization.LoadFromFile<SaveGame>(GetFilePath(folder, name));
            }
            catch (Exception e)
            {
                Log.WriteLine("Could not open savegame " + name + ": " + e.Message, LogLevel.Error);
            }

            return null;
        }

        internal static Dictionary<string, SaveGame> GetSaveGames(string folder)
        {
            EnsureStorageFolderExists(folder);

            var Files = Directory
                .EnumerateFiles(SaveGame.UserStorageFolder(folder), "*" + FILE_EXTENSION)
                .ToArray();

            return Files
                .Select(file => new
                {
                    Filename = System.IO.Path.GetFileNameWithoutExtension(file),
                    Savegame = LoadFromFile(folder, System.IO.Path.GetFileNameWithoutExtension(file))
                })
                .Where(t => t.Savegame != null)
                .ToDictionary(t => t.Filename, t => t.Savegame);
        }

        internal static string UserStorageFolder(string subdirectory = "STACK")
        {
            string platform = SDL.SDL_GetPlatform();
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
                string osConfigDir = Environment.GetEnvironmentVariable("HOME");
                if (String.IsNullOrEmpty(osConfigDir))
                {
                    return "."; // Oh well.
                }
                osConfigDir += "/Library/Application Support";
                return System.IO.Path.Combine(osConfigDir, subdirectory);
            }
            else if (platform.Equals("Linux"))
            {
                string osConfigDir = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
                if (String.IsNullOrEmpty(osConfigDir))
                {
                    osConfigDir = Environment.GetEnvironmentVariable("HOME");
                    if (String.IsNullOrEmpty(osConfigDir))
                    {
                        return "."; // Oh well.
                    }
                    osConfigDir += "/.local/share";
                }
                return System.IO.Path.Combine(osConfigDir, subdirectory);
            }

            throw new Exception("SDL platform unhandled: " + platform);
        }

        private static void EnsureStorageFolderExists(string folder)
        {
            var Folder = SaveGame.UserStorageFolder(folder);

            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }
        }
    }
}
