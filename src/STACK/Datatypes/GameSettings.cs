using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace STACK
{
    /// <summary>
    /// Class holding culture, target resolution, which adapter to render on, display mode,
    /// VSync and MultiSampling properties.
    /// </summary>
    public class GameSettings
    {
        public const string CONFIGFILENAME = "settings.xml";

        /// <summary>
        /// Culture 
        /// </summary>
        public string Culture = "en-US";

        /// <summary>
        /// Volume of music
        /// </summary>
        public float MusicVolume = 1;

        /// <summary>
        /// Volume of sound effects
        /// </summary>
        public float SoundEffectVolume = 1;

        /// <summary>
        /// Target Resolution
        /// </summary>
        public Point Resolution = new Point(640, 400);

        /// <summary>
        /// Display Adapter to create the game window on (starting with 0)
        /// </summary>
        public int Adapter = 0;

        /// <summary>
        /// Display mode
        /// </summary>
        public DisplayMode DisplayMode = DisplayMode.BorderlessScale;

        /// <summary>
        /// Enable multisampling
        /// </summary>
        public bool MultiSampling = false;

        /// <summary>
        /// Enable bloom
        /// </summary>
        public bool Bloom = true;

        /// <summary>
        /// Enable vsync
        /// </summary>
        public bool VSync = true;

        /// <summary>
        /// Enable debug mode
        /// </summary>
        public bool Debug = true;

        public bool IsBorderless()
        {
            return DisplayMode.Borderless == DisplayMode ||
               DisplayMode.BorderlessScale == DisplayMode ||
               DisplayMode.BorderlessMaxInteger == DisplayMode;
        }

        public bool IsFullscreen()
        {
            return DisplayMode.Fullscreen == DisplayMode;
        }

        private GraphicsAdapter PreferedGraphicsAdapter
        {
            get
            {
                var GA = GraphicsAdapter.Adapters.ElementAtOrDefault(Adapter);

                if (null != GA)
                {
                    return GA;
                }

                return GraphicsAdapter.DefaultAdapter;
            }
        }

        public void PrepareDevice(object sender, PreparingDeviceSettingsEventArgs eventargs)
        {
            eventargs.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;
            eventargs.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;

            if (!PreferedGraphicsAdapter.IsDefaultAdapter)
            {
                eventargs.GraphicsDeviceInformation.Adapter = PreferedGraphicsAdapter;
            }

            if (IsBorderless())
            {
                var displayMode = PreferedGraphicsAdapter.CurrentDisplayMode;

                eventargs.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = displayMode.Format;
                eventargs.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = displayMode.Width;
                eventargs.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = displayMode.Height;
            }
        }

        public GraphicsDeviceManager CreateGraphicsDeviceManager(Game game)
        {
            var Graphics = new GraphicsDeviceManager(game);
            Graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(PrepareDevice);

            if (!Graphics.GraphicsDevice.Adapter.IsProfileSupported(GraphicsProfile.HiDef) ||
                !Graphics.GraphicsDevice.Adapter.IsProfileSupported(GraphicsProfile.Reach))
            {
                throw new Exception("Graphic profile not supported.");
            }

            return Graphics;
        }

        public void Initialize(GraphicsDeviceManager graphics, Point virtualResolution)
        {
            graphics.PreferMultiSampling = MultiSampling;
            graphics.SynchronizeWithVerticalRetrace = VSync;
            graphics.IsFullScreen = IsFullscreen();

            var TargetResolution = GetTargetResolution(virtualResolution);

            graphics.PreferredBackBufferWidth = TargetResolution.X;
            graphics.PreferredBackBufferHeight = TargetResolution.Y;

            graphics.ApplyChanges();
        }

        public static GameSettings DeserializeFromStream(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(GameSettings));

            return serializer.Deserialize(stream) as GameSettings;
        }

        public static GameSettings LoadFromConfigFile(string directory = null)
        {
            try
            {
                GameSettings InstallationDirectoryGameSettings;
                using (var Stream = TitleContainer.OpenStream(CONFIGFILENAME))
                {
                    InstallationDirectoryGameSettings = DeserializeFromStream(Stream);
                }

                var FileName = GetUserStorageSettingsFileName(directory);
                if (File.Exists(FileName))
                {
                    using (var Stream = File.Open(FileName, FileMode.Open))
                    {
                        var UserStorageGameSettings = DeserializeFromStream(Stream);
                        UserStorageGameSettings.Culture = InstallationDirectoryGameSettings.Culture;
                    }
                }

                return InstallationDirectoryGameSettings;
            }
            catch (Exception e)
            {
                Logging.Log.WriteLine("Could not load game settings: " + e.Message, Logging.LogLevel.Error);
                return new GameSettings();
            }
        }

        private static string GetUserStorageSettingsFileName(string directory)
        {
            return System.IO.Path.Combine(SaveGame.UserStorageFolder(directory), CONFIGFILENAME);
        }

        public void Save(string directory)
        {
            SaveGame.EnsureStorageFolderExists(directory);
            var FileName = GetUserStorageSettingsFileName(directory);

            using (var Writer = new StreamWriter(FileName))
            {
                var Serializer = new XmlSerializer(GetType());
                var XmlNS = new XmlSerializerNamespaces();
                XmlNS.Add(string.Empty, string.Empty);
                Serializer.Serialize(Writer, this, XmlNS);
                Writer.Flush();
            }
        }

        public Point GetTargetResolution(Point virtualResolution)
        {
            if (DisplayMode.Borderless == DisplayMode)
            {
                return Resolution;
            }

            if (DisplayMode.BorderlessMaxInteger == DisplayMode ||
                DisplayMode.WindowMaxInteger == DisplayMode)
            {
                var DisplayMode = PreferedGraphicsAdapter.CurrentDisplayMode;
                var IntegerScaleFactor = Math.Min(DisplayMode.Width / virtualResolution.X, DisplayMode.Height / virtualResolution.Y);

                return new Point(virtualResolution.X * IntegerScaleFactor, virtualResolution.Y * IntegerScaleFactor);
            }

            if (DisplayMode.BorderlessScale == DisplayMode)
            {
                var DisplayMode = PreferedGraphicsAdapter.CurrentDisplayMode;

                return new Point(DisplayMode.Width, DisplayMode.Height);
            }

            return Resolution;
        }

        public void SetCulture()
        {
            SetCurrentCulture(Culture);
        }

        public static string GetCurrentCultureName()
        {
            return Thread.CurrentThread.CurrentUICulture.Name;
        }

        public static void SetCurrentCulture(string name)
        {
            if (!string.IsNullOrEmpty(name) && name != GetCurrentCultureName())
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(name);
            }
        }
    }
}
