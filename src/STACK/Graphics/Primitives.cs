using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace STACK.Graphics
{
	public class PrimitivesRenderer
	{
		private readonly BasicEffect _basicEffect;
		private Matrix _projection;
		private readonly VertexPositionColor[] _vertices = new VertexPositionColor[200];

		public void SetTransformation(Matrix transformation)
		{
			_basicEffect.Projection = transformation * _projection;
		}

		public PrimitivesRenderer(GraphicsDevice graphicsDevice)
		{
			_basicEffect = new BasicEffect(graphicsDevice)
			{
				VertexColorEnabled = true
			};

			_projection = Matrix.CreateOrthographicOffCenter(0,
				graphicsDevice.PresentationParameters.BackBufferWidth,
				graphicsDevice.PresentationParameters.BackBufferHeight,
				0,
				0,
				1);

			_projection.M41 += -0.5f * _projection.M11;
			_projection.M42 += -0.5f * _projection.M22;

			_basicEffect.Projection = _projection;
		}

		public void DrawLine(Vector2 from, Vector2 to, Color color)
		{
			DrawLine(from, to, color, color);
		}

		public void DrawLine(Vector2 from, Vector2 to, Color colorFrom, Color colorTo)
		{
			_vertices[0].Position = new Vector3(from.X, from.Y, 0);
			_vertices[0].Color = colorFrom;
			_vertices[1].Position = new Vector3(to.X, to.Y, 0);
			_vertices[1].Color = colorTo;

			_basicEffect.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			_basicEffect.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
			_basicEffect.CurrentTechnique.Passes[0].Apply();
			_basicEffect.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _vertices, 0, 1);
		}

		public void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color color)
		{
			_vertices[0].Position = new Vector3(p1, 0);
			_vertices[0].Color = color;
			_vertices[1].Position = new Vector3(p2, 0);
			_vertices[1].Color = color;
			_vertices[2].Position = new Vector3(p3, 0);
			_vertices[2].Color = color;


			_basicEffect.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			_basicEffect.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
			_basicEffect.CurrentTechnique.Passes[0].Apply();
			_basicEffect.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, 1);
		}

		public void DrawRectangle(Rectangle rect, Color color)
		{
			DrawRectangle(new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y + rect.Height), color);
		}

		public void DrawRectangle(Vector2 p1, Vector2 p2, Color color)
		{
			_vertices[0].Position = new Vector3(p1, 0);
			_vertices[0].Color = color;
			_vertices[1].Position = new Vector3(p2.X, p1.Y, 0);
			_vertices[1].Color = color;
			_vertices[2].Position = new Vector3(p2, 0);
			_vertices[2].Color = color;
			_vertices[3].Position = new Vector3(p2, 0);
			_vertices[3].Color = color;
			_vertices[4].Position = new Vector3(p1.X, p2.Y, 0);
			_vertices[4].Color = color;
			_vertices[5].Position = new Vector3(p1, 0);
			_vertices[5].Color = color;

			_basicEffect.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			_basicEffect.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
			_basicEffect.CurrentTechnique.Passes[0].Apply();
			_basicEffect.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, 3);
		}

		public void Dispose()
		{
			_basicEffect.Dispose();
		}
	}
}
