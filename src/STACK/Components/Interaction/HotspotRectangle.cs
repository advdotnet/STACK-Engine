using Microsoft.Xna.Framework;
using STACK.Graphics;
using System;
using System.Diagnostics;

namespace STACK.Components
{
    /// <summary>
    /// A hotspot described by a rectangle.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Rectangle}")]
    public class HotspotRectangle : Hotspot
    {
        public Rectangle Rectangle { get; protected set; }

        public override bool IsHit(Vector2 mouse)
        {
            return Rectangle.Contains(mouse);
        }

        public override void OnDraw(Renderer renderer)
        {
            if (EngineVariables.DebugPath)
            {
                renderer.SpriteBatch.Draw(renderer.WhitePixelTexture, Rectangle, new Color(255, 255, 255, 100));
            }
        }

        public static HotspotRectangle Create(Entity addTo)
        {
            return addTo.Add<HotspotRectangle>();
        }

        public HotspotRectangle SetRectangle(int x, int y, int width, int height) { Rectangle = new Rectangle(x, y, width, height); return this; }
        public HotspotRectangle SetRectangle(Rectangle value) { Rectangle = value; return this; }
        public HotspotRectangle SetCaption(string value) { Caption = value; return this; }
    }
}
