using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
	/// <summary>
	/// Hotspot described by a sprite component. Can be rectangular or pixel perfect.
	/// </summary>
	[Serializable]
	public class HotspotSprite : Hotspot
	{
		/// <summary>
		/// If true, the texture is checked whether there is a collision with visible pixel (alpha > 0.4). 
		/// If false, the texture's rectangle is used for collision detection.
		/// </summary>
		public bool PixelPerfect { get; set; }

		public override bool IsHit(Vector2 mouse)
		{
			var sprite = Get<Sprite>();

			if (sprite != null)
			{
				return PixelPerfect ? sprite.IsPixelHit(mouse) : sprite.IsRectangleHit(mouse);
			}

			return false;
		}

		public static HotspotSprite Create(Entity addTo)
		{
			return addTo.Add<HotspotSprite>();
		}

		public HotspotSprite SetPixelPerfect(bool value) { PixelPerfect = value; return this; }
		public HotspotSprite SetCaption(string value) { Caption = value; return this; }
	}
}
