using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace STACK.Graphics
{

    public enum DisplayMode
    {
        /// <summary>
        /// Game is displayed in a window using the setting's target resolution.
        /// Scaling happens if the target resolution does not match the game's virtual resolution.
        /// </summary>
        Window,

        /// <summary>
        /// Fullscreen mode using the target resolution.
        /// </summary>
        Fullscreen,

        /// <summary>
        /// The desktop resolution is used, but the target resolution is kept, possibly resulting in black bars
        /// at the border.
        /// </summary>
        Borderless,

        /// <summary>
        /// The desktop resolution is used and the game is upscaled.
        /// </summary>
        BorderlessScale
    }

    /// <summary>
    /// Class holding target resolution, which adapter to render on, display mode,
    /// VSync and MultiSampling properties.
    /// </summary>
    public class GraphicSettings
    {
        public const string CONFIGFILENAME = "graphicsettings.xml";

        /// <summary>
        /// Target Resolution
        /// </summary>
        public Point Resolution = new Point(640, 400);

        /// <summary>
        /// Adapter to create the game window on (enumeration starting with 0)
        /// </summary>
        public int Adapter = 0;

        /// <summary>
        /// Display mode
        /// </summary>
        public DisplayMode DisplayMode = DisplayMode.Window;

        /// <summary>
        /// Enable multisampling
        /// </summary>
        public bool MultiSampling = false;

        /// <summary>
        /// Enable vsync
        /// </summary>
        public bool VSync = true;

        public bool IsBorderless()
        {
            return DisplayMode.Borderless == DisplayMode ||
               DisplayMode.BorderlessScale == DisplayMode;
        }

        public bool IsFullscreen()
        {
            return DisplayMode.Fullscreen == DisplayMode;
        }

        public void PrepareDevice(object sender, PreparingDeviceSettingsEventArgs eventargs)
        {
            eventargs.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;
            eventargs.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;

            if (Adapter != 0 && GraphicsAdapter.Adapters.ElementAtOrDefault(Adapter) != null)
            {
                eventargs.GraphicsDeviceInformation.Adapter = GraphicsAdapter.Adapters[Adapter];
            }

            if (IsBorderless())
            {
                var displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

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

        public void Initialize(GraphicsDeviceManager graphics)
        {
            graphics.PreferMultiSampling = MultiSampling;
            graphics.SynchronizeWithVerticalRetrace = VSync;
            graphics.IsFullScreen = IsFullscreen();

            graphics.PreferredBackBufferWidth = Resolution.X;
            graphics.PreferredBackBufferHeight = Resolution.Y;

            graphics.ApplyChanges();
        }

        public static GraphicSettings DeserializeFromStream(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(GraphicSettings));
            return serializer.Deserialize(stream) as GraphicSettings;
        }

        public static GraphicSettings LoadFromConfigFile()
        {
            try
            {
                using (var stream = TitleContainer.OpenStream(CONFIGFILENAME))
                {
                    return DeserializeFromStream(stream);
                }
            }
            catch
            {
                return new GraphicSettings();
            }
        }

        public void Save(string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(GetType());
                var xmlns = new XmlSerializerNamespaces();
                xmlns.Add(string.Empty, string.Empty);
                serializer.Serialize(writer, this, xmlns);
                writer.Flush();
            }
        }

        public Point? GetTargetResolution()
        {
            if (DisplayMode.Borderless == DisplayMode)
            {
                return Resolution;
            }
            return null;
        }
    }
}
