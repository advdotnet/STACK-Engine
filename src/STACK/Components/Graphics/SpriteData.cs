using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace STACK.Components
{
	[Serializable]
	public class SpriteData : Component, INotify
	{
		public SpriteData()
		{

		}

		private float _rotation;
		public float Rotation => _rotation;
		public Color Color = Color.White;
		public Vector2 Scale = Vector2.One;
		public Vector2 Origin = Vector2.Zero;
		public Vector2 Offset = Vector2.Zero;
		public SpriteEffects Effects = SpriteEffects.None;
		public string Skin = string.Empty;
		public string Animation = string.Empty;
		public bool OrientationFlip { get; set; }

		public static SpriteData Create(Entity addTo)
		{
			return addTo.Add<SpriteData>();
		}

		public void Notify<T>(string message, T data)
		{
			if (message == Messages.OrientationChanged && OrientationFlip)
			{
				var orientation = (Vector2)(object)data;
				if (orientation.X < 0)
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
				var newColor = (Color)(object)data;
				Color = newColor;
			}
		}

		public SpriteData SetColor(Color value) { Color = value; return this; }
		public SpriteData SetScale(float valueX, float valueY) { Scale = new Vector2(valueX, valueY); return this; }
		public SpriteData SetOffset(float valueX, float valueY) { Offset = new Vector2(valueX, valueY); return this; }
		public SpriteData SetEffects(SpriteEffects value) { Effects = value; return this; }
		public SpriteData SetRotation(float value) { _rotation = value; return this; }
		public SpriteData SetOrigin(float valueX, float valueY) { Origin = new Vector2(valueX, valueY); return this; }
		public SpriteData SetOrigin(Vector2 value) { Origin = value; return this; }
		public SpriteData SetOrientationFlip(bool value) { OrientationFlip = value; return this; }
	}
}
