using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace STACK.Components
{
    [Serializable]
    public class SpriteData : Component
    {
        public SpriteData()
        {
            Visible = false;
        }

        public float Rotation { get; private set; }
        public Color Color = Color.White;
        public Vector2 Scale = Vector2.One;
        public Vector2 Origin = Vector2.Zero;
        public Vector2 Offset = Vector2.Zero;
        public SpriteEffects Effects = SpriteEffects.None;
        public string Skin = "";
        public string Animation = "";
        public bool OrientationFlip = true;

        public static SpriteData Create(Entity addTo)
        {
            return addTo.Add<SpriteData>();
        }

        public override void OnNotify<T>(string message, T data)
        {
            if (message == Messages.OrientationChanged && OrientationFlip)
            {
                Vector2 Orientation = (Vector2)(object)data;
                if (Orientation.X < 0)
                {
                    Effects = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    Effects = SpriteEffects.None;
                }
            }            
            else if (message == Messages.ColorChanged)
            {
                Color NewColor = (Color)(object)data;
                Color = NewColor;
            }
        }

        public SpriteData SetColor(Color value) { Color = value; return this; }
        public SpriteData SetScale(float valueX, float valueY) { Scale = new Vector2(valueX, valueY); return this; }
        public SpriteData SetOffset(float valueX, float valueY) { Offset = new Vector2(valueX, valueY); return this; }
        public SpriteData SetEffects(SpriteEffects value) { Effects = value; return this; }
        public SpriteData SetRotation(float value) { Rotation = value; return this; }
        public SpriteData SetOrigin(float valueX, float valueY) { Origin = new Vector2(valueX, valueY); return this; }
        public SpriteData SetOrigin(Vector2 value) { Origin = value; return this; }
        public SpriteData SetOrientationFlip(bool value) { OrientationFlip = value;  return this; }
    }    
}
