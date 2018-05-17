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
            var Sprite = Get<SpineSprite>();

            if (Sprite != null)
            {
                return PixelPerfect ? Sprite.IsPixelHit(mouse) : Sprite.IsRectangleHit(mouse);
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
