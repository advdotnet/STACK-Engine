using System;

namespace STACK
{
	[Serializable]
	[Flags]
	public enum RenderStage
	{
		PreBloom,
		Bloom,
		PostBloom
	}

	public enum MouseButton
	{
		Left, Right
	}

	[Flags]
	public enum Alignment
	{
		Center = 0,
		Left = 1,
		Right = 2,
		Top = 4,
		Bottom = 8
	}

	public enum DisplayMode
	{
		/// <summary>
		/// Game is displayed in a window using the setting's target resolution.
		/// Scaling happens if the target resolution does not match the game's virtual resolution.
		/// </summary>
		Window,

		/// <summary>
		/// Game is displayed in a window sized the maximum integer multiple of the game's virtual resolution
		/// fitting on the preferred adapter.
		/// </summary>
		WindowMaxInteger,

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
		BorderlessScale,

		/// <summary>
		/// Desktop resolution with max integer scaling factor
		/// </summary>
		BorderlessMaxInteger
	}
}
