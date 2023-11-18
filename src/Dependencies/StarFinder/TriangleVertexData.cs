using Microsoft.Xna.Framework;
using System;

namespace StarFinder
{

	/// <summary>
	/// Stores common data for each triangle vertex like color or scale.
	/// </summary>
	[Serializable]
	public struct TriangleVertexData : IScalable<TriangleVertexData>
	{
		public Color Color { get; set; }
		public Vector2 Scale { get; set; }

		public TriangleVertexData(Color color, Vector2 scale)
		{
			Color = color;
			Scale = scale;
		}

		public TriangleVertexData Default()
		{
			return new TriangleVertexData()
			{
				Scale = Vector2.One,
				Color = Color.White
			};
		}

		public TriangleVertexData Multiply(float scale)
		{
			return new TriangleVertexData()
			{
				Scale = Scale * scale,
				Color = Color * scale
			};
		}

		public TriangleVertexData Add(TriangleVertexData t)
		{
			return new TriangleVertexData()
			{
				Scale = Scale + t.Scale,
				Color = new Color(Color.R + t.Color.R, Color.G + t.Color.G, Color.B + t.Color.B, Color.A + t.Color.A)
			};
		}
	}
}
