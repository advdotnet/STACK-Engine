﻿using Microsoft.Xna.Framework;
using STACK.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace STACK.Components
{
    /// <summary>
    /// A hotspot described by one or multiple rectangles.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Rectangle}")]
    public class HotspotRectangle : Hotspot
    {
        public List<Rectangle> Rectangles { get; protected set; }

        public HotspotRectangle()
        {
            Rectangles = new List<Rectangle>();
        }

        public override bool IsHit(Vector2 mouse)
        {
            foreach (var Rectangle in Rectangles)
            {
                if (Rectangle.Contains(mouse))
                {
                    return true;
                }
            }

            return false;
        }

        public override void OnEndDraw(Renderer renderer)
        {
            if (EngineVariables.DebugPath)
            {
                foreach (var Rectangle in Rectangles)
                {
                    renderer.SpriteBatch.Draw(renderer.WhitePixelTexture, Rectangle, new Color(255, 255, 255, 100));
                }
            }
        }

        public static HotspotRectangle Create(Entity addTo)
        {
            return addTo.Add<HotspotRectangle>();
        }

        public HotspotRectangle AddRectangle(int x, int y, int width, int height) { Rectangles.Add(new Rectangle(x, y, width, height)); return this; }
        public HotspotRectangle AddRectangle(Rectangle value) { Rectangles.Add(value); return this; }
        public HotspotRectangle SetRectangle(Rectangle value) { Rectangles.Clear(); Rectangles.Add(value); return this; }
        public HotspotRectangle SetRectangle(int x, int y, int width, int height) { return SetRectangle(new Rectangle(x, y, width, height)); }
        public HotspotRectangle SetCaption(string value) { Caption = value; return this; }
    }
}
