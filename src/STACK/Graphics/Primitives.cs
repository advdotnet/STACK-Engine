using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace STACK.Graphics
{
    public class PrimitivesRenderer
    {
        BasicEffect BasicEffect;
        Matrix Projection;
        VertexPositionColor[] Vertices = new VertexPositionColor[200];

        public void SetTransformation(Matrix transformation) 
        {
            BasicEffect.Projection = transformation * Projection;
        }

        public PrimitivesRenderer(GraphicsDevice graphicsDevice)
        {
            BasicEffect = new BasicEffect(graphicsDevice);
            BasicEffect.VertexColorEnabled = true;

            Projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight, 0, 0, 1);
            Projection.M41 += -0.5f * Projection.M11;
            Projection.M42 += -0.5f * Projection.M22;

            BasicEffect.Projection = Projection;                        
        }

        public void DrawLine(Vector2 from, Vector2 to, Color color)
        {
            DrawLine(from, to, color, color);
        }

        public void DrawLine(Vector2 from, Vector2 to, Color colorFrom, Color colorTo)
        {            
            Vertices[0].Position = new Vector3(from.X, from.Y, 0);
            Vertices[0].Color = colorFrom;
            Vertices[1].Position = new Vector3(to.X, to.Y, 0);
            Vertices[1].Color = colorTo;

            BasicEffect.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            BasicEffect.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            BasicEffect.CurrentTechnique.Passes[0].Apply();
            BasicEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, Vertices, 0, 1);            
        }

        public void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color color)
        {
            Vertices[0].Position = new Vector3(p1, 0);
            Vertices[0].Color = color;
            Vertices[1].Position = new Vector3(p2, 0);
            Vertices[1].Color = color;
            Vertices[2].Position = new Vector3(p3, 0);
            Vertices[2].Color = color;

            
            BasicEffect.GraphicsDevice.RasterizerState = RasterizerState.CullNone;                        
            BasicEffect.GraphicsDevice.BlendState = BlendState.NonPremultiplied;                        
            BasicEffect.CurrentTechnique.Passes[0].Apply();
            BasicEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, Vertices, 0, 1);
        }

        public void DrawRectangle(Rectangle rect, Color color)
        {
            DrawRectangle(new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y + rect.Height), color);
        }

        public void DrawRectangle(Vector2 p1, Vector2 p2, Color color)
        {
            Vertices[0].Position = new Vector3(p1, 0);
            Vertices[0].Color = color;
            Vertices[1].Position = new Vector3(p2.X, p1.Y, 0);
            Vertices[1].Color = color;
            Vertices[2].Position = new Vector3(p2, 0);
            Vertices[2].Color = color;
            Vertices[3].Position = new Vector3(p2, 0);
            Vertices[3].Color = color;  
            Vertices[4].Position = new Vector3(p1.X, p2.Y, 0);
            Vertices[4].Color = color;
            Vertices[5].Position = new Vector3(p1, 0);
            Vertices[5].Color = color;

            BasicEffect.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            BasicEffect.GraphicsDevice.BlendState = BlendState.NonPremultiplied;                        
            BasicEffect.CurrentTechnique.Passes[0].Apply();
            BasicEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, Vertices, 0, 3);
        }

        public void Dispose()
        {
            BasicEffect.Dispose();
        }

        
    }
}
