using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
    /// <summary>
    /// A hotspot which is always hit.
    /// </summary>
    [Serializable]
    public class HotspotPersistent : Hotspot
    {
        public override bool IsHit(Vector2 mouse)
        {
            return true;
        }

        public static HotspotPersistent Create(Entity addTo)
        {
            return addTo.Add<HotspotPersistent>();
        }

        public HotspotPersistent SetCaption(string value) { Caption = value; return this; }
    }
}
