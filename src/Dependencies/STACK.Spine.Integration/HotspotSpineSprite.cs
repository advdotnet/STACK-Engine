using Microsoft.Xna.Framework;
using STACK.Components;
using System;

namespace STACK.Spine
{
	/// <summary>
	/// Hotspot described by a spine sprite component.
	/// </summary>
	[Serializable]
	public class HotspotSpineSprite : Hotspot
	{
		public bool PixelPerfect { get; set; }

		public override bool IsHit(Vector2 mouse)
		{
			var sprite = Get<SpineSprite>();

			if (sprite != null)
			{
				return PixelPerfect ? sprite.IsPixelHit(mouse) : sprite.IsRectangleHit(mouse);
			}

			return false;
		}

		public static HotspotSpineSprite Create(Entity addTo)
		{
			return addTo.Add<HotspotSpineSprite>();
		}

		public HotspotSpineSprite SetPixelPerfect(bool value) { PixelPerfect = value; return this; }
		public HotspotSpineSprite SetCaption(string value) { Caption = value; return this; }
	}
}
