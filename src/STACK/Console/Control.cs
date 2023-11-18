using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace STACK.Debug
{
	/// <summary>
	/// Debug Console control.
	/// </summary>
	public class ConsoleControl
	{
		private readonly TomShane.Neoforce.Controls.Window _windowUIControl;
		private readonly TomShane.Neoforce.Controls.Console _consoleUIControl;
		private readonly Manager _manager;
		private bool _cursorShown = false;

		/// <summary>
		/// Returns if the console window is currently visible or sets the visibility state.
		/// </summary>
		public bool Visible
		{
			get => _windowUIControl.Visible;
			set
			{
				if (value)
				{
					_cursorShown = _manager.ShowSoftwareCursor;
					_windowUIControl.Show();
					_consoleUIControl.TextBox.Text = string.Empty;
					_consoleUIControl.TextBox.Focused = true;
					_manager.ShowSoftwareCursor = true;
					_windowUIControl.BringToFront();
				}
				else
				{
					_windowUIControl.Hide();
					_consoleUIControl.TextBox.Focused = false;
					_manager.ShowSoftwareCursor = _cursorShown;
				}
			}
		}

		/// <summary>
		/// Gets or sets the current text input.
		/// </summary>
		public string Input
		{
			get => _consoleUIControl.TextBox.Text;
			set => _consoleUIControl.TextBox.Text = value;
		}

		/// <summary>
		/// Gets or sets the cursor position.
		/// </summary>
		public int CursorPosition { get; set; }

		public ConsoleControl(StackEngine engine, ConsoleMessageEventHandler messageSent, KeyEventHandler keyUp)
		{
			_manager = engine.Renderer.GUIManager;
			_windowUIControl = new TomShane.Neoforce.Controls.Window(_manager);
			_windowUIControl.Init();
			_windowUIControl.Text = "STACK Engine Console";
			_windowUIControl.Width = 480;
			_windowUIControl.Resizable = true;
			_windowUIControl.MinimumHeight = 240;
			_windowUIControl.MinimumWidth = 320;
			_windowUIControl.Height = 200;
			_windowUIControl.IconVisible = false;
			_windowUIControl.Alpha = 215;
			_windowUIControl.CloseButtonVisible = false;
			_windowUIControl.Closing += (object sender, WindowClosingEventArgs e) => e.Cancel = true;
			_windowUIControl.Visible = false;
			_windowUIControl.Color = Color.Black;
			_windowUIControl.Center(engine.Game.VirtualResolution);

			_consoleUIControl = new TomShane.Neoforce.Controls.Console(_manager);
			_consoleUIControl.Channels.Add(new ConsoleChannel((byte)Console.Channel.System, "System", Color.White));
			_consoleUIControl.Channels.Add(new ConsoleChannel((byte)Console.Channel.Debug, "Debug", Color.LightGray));
			_consoleUIControl.Channels.Add(new ConsoleChannel((byte)Console.Channel.Error, "Error", Color.IndianRed));
			_consoleUIControl.Channels.Add(new ConsoleChannel((byte)Console.Channel.Notice, "Notice", Color.DimGray));
			_consoleUIControl.Channels.Add(new ConsoleChannel((byte)Console.Channel.Warning, "Warning", Color.DeepPink));
			_consoleUIControl.Channels.Add(new ConsoleChannel((byte)Console.Channel.User, "User", Color.Gray));
			_consoleUIControl.SelectedChannel = (byte)Console.Channel.User;
			_consoleUIControl.ChannelsVisible = false;
			_consoleUIControl.ClientArea.Left = 10;
			_consoleUIControl.Color = Color.Black;
			_consoleUIControl.MessageFormat = ConsoleMessageFormats.TimeStamp;
			_consoleUIControl.TextBox.Color = Color.Black;
			_consoleUIControl.Sender = "User";
			_consoleUIControl.TextBox.KeyUp += keyUp;
			_consoleUIControl.TextBox.AutoSelection = false;
			_consoleUIControl.Width = _windowUIControl.ClientWidth - _consoleUIControl.Left;
			_consoleUIControl.Height = _windowUIControl.ClientHeight;
			_consoleUIControl.Parent = _windowUIControl;

			_consoleUIControl.MessageSent += messageSent;

			_windowUIControl.Resize += new ResizeEventHandler(OnWindowResize);

			_manager.Add(_windowUIControl);
		}

		private void OnWindowResize(object sender, ResizeEventArgs e)
		{
			_consoleUIControl.Width = _windowUIControl.ClientWidth - _consoleUIControl.Left;
			_consoleUIControl.Height = _windowUIControl.ClientHeight;
			_consoleUIControl.Invalidate();
		}

		/// <summary>
		/// Writes a string to the console using given channel.
		/// </summary>
		/// <param name="message">The string to add.</param>
		/// <param name="channel">The channel to use.</param>
		public void AddLine(object message, Console.Channel channel = Console.Channel.Notice)
		{
			_consoleUIControl.MessageBuffer.Add(new ConsoleMessage("STACK", message.ToString(), (byte)channel));
		}
	}
}
