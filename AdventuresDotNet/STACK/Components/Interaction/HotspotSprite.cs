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
        public bool PixelPerfect { get; set; }

        public override bool IsHit(Vector2 mouse)
        {
            var Sprite = Get<Sprite>();

            if (Sprite != null)
            {
                return PixelPerfect ? Sprite.IsPixelHit(mouse) : Sprite.IsRectangleHit(mouse);
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
