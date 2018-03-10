using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace STACK.Debug
{
    /// <summary>
    /// Debug Console control.
    /// </summary>
    public class ConsoleControl
    {
        TomShane.Neoforce.Controls.Window WindowUIControl;
        TomShane.Neoforce.Controls.Console ConsoleUIControl;
        Manager Manager;
        bool CursorShown = false;

        /// <summary>
        /// Returns if the console window is currently visible or sets the visibility state.
        /// </summary>
        public bool Visible
        {
            get
            {
                return WindowUIControl.Visible;
            }
            set
            {
                if (value)
                {
                    CursorShown = Manager.ShowSoftwareCursor;
                    WindowUIControl.Show();
                    ConsoleUIControl.TextBox.Text = "";
                    ConsoleUIControl.TextBox.Focused = true;
                    Manager.ShowSoftwareCursor = true;
                    WindowUIControl.BringToFront();
                }
                else
                {
                    WindowUIControl.Hide();
                    ConsoleUIControl.TextBox.Focused = false;
                    Manager.ShowSoftwareCursor = CursorShown;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current text input.
        /// </summary>
        public string Input
        {
            get
            {
                return ConsoleUIControl.TextBox.Text;
            }
            set
            {
                ConsoleUIControl.TextBox.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the cursor position.
        /// </summary>
        public int CursorPosition { get; set; }

        public ConsoleControl(StackEngine engine, ConsoleMessageEventHandler messageSent, KeyEventHandler keyUp)
        {
            Manager = engine.Renderer.GUIManager;
            WindowUIControl = new TomShane.Neoforce.Controls.Window(Manager);
            WindowUIControl.Init();
            WindowUIControl.Text = "STACK Engine Console";
            WindowUIControl.Width = 480;
            WindowUIControl.Resizable = true;
            WindowUIControl.MinimumHeight = 240;
            WindowUIControl.MinimumWidth = 320;
            WindowUIControl.Height = 200;
            WindowUIControl.IconVisible = false;
            WindowUIControl.Alpha = 215;
            WindowUIControl.CloseButtonVisible = false;
            WindowUIControl.Closing += (object sender, WindowClosingEventArgs e) => e.Cancel = true;
            WindowUIControl.Visible = false;
            WindowUIControl.Color = Color.Black;
            WindowUIControl.Center(engine.Game.VirtualResolution);

            ConsoleUIControl = new TomShane.Neoforce.Controls.Console(Manager);
            ConsoleUIControl.Channels.Add(new ConsoleChannel((byte)Console.Channel.System, "System", Color.White));
            ConsoleUIControl.Channels.Add(new ConsoleChannel((byte)Console.Channel.Debug, "Debug", Color.LightGray));
            ConsoleUIControl.Channels.Add(new ConsoleChannel((byte)Console.Channel.Error, "Error", Color.IndianRed));
            ConsoleUIControl.Channels.Add(new ConsoleChannel((byte)Console.Channel.Notice, "Notice", Color.DimGray));
            ConsoleUIControl.Channels.Add(new ConsoleChannel((byte)Console.Channel.Warning, "Warning", Color.DeepPink));
            ConsoleUIControl.Channels.Add(new ConsoleChannel((byte)Console.Channel.User, "User", Color.Gray));
            ConsoleUIControl.SelectedChannel = (byte)Console.Channel.User;
            ConsoleUIControl.ChannelsVisible = false;
            ConsoleUIControl.ClientArea.Left = 10;
            ConsoleUIControl.Color = Color.Black;
            ConsoleUIControl.MessageFormat = ConsoleMessageFormats.TimeStamp;
            ConsoleUIControl.TextBox.Color = Color.Black;
            ConsoleUIControl.Sender = "User";
            ConsoleUIControl.TextBox.KeyUp += keyUp;
            ConsoleUIControl.TextBox.AutoSelection = false;
            ConsoleUIControl.Width = WindowUIControl.ClientWidth - ConsoleUIControl.Left;
            ConsoleUIControl.Height = WindowUIControl.ClientHeight;
            ConsoleUIControl.Parent = WindowUIControl;

            ConsoleUIControl.MessageSent += messageSent;

            WindowUIControl.Resize += new ResizeEventHandler(OnWindowResize);

            Manager.Add(WindowUIControl);
        }

        void OnWindowResize(object sender, ResizeEventArgs e)
        {
            ConsoleUIControl.Width = WindowUIControl.ClientWidth - ConsoleUIControl.Left;
            ConsoleUIControl.Height = WindowUIControl.ClientHeight;
            ConsoleUIControl.Invalidate();
        }

        /// <summary>
        /// Writes a string to the console using given channel.
        /// </summary>
        /// <param name="message">The string to add.</param>
        /// <param name="channel">The channel to use.</param>
        public void AddLine(object message, Console.Channel channel = Console.Channel.Notice)
        {
            ConsoleUIControl.MessageBuffer.Add(new ConsoleMessage("STACK", message.ToString(), (byte)channel));
        }
    }
}
