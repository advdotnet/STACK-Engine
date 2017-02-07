using System;
using Microsoft.Xna.Framework;

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
                Scale = this.Scale * scale,
                Color = this.Color * scale
            };
        }

        public TriangleVertexData Add(TriangleVertexData t)
        {
            return new TriangleVertexData()
            {
                Scale = this.Scale + t.Scale,
                Color = new Color(this.Color.R + t.Color.R, this.Color.G + t.Color.G, this.Color.B + t.Color.B, this.Color.A + t.Color.A)
            };
        }
    }    
}
