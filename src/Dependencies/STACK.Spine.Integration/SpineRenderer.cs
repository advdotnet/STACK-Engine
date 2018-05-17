using Microsoft.Xna.Framework.Graphics;
using Spine;
using STACK.Components;
using STACK.Graphics;
using STACK.Logging;


namespace STACK.Spine
{
    /// <summary>
    /// Provides classes and methods for rendering.
    /// </summary>
    public class SpineRenderer : Component, IContent, IWorldAutoAdd
    {
        public SkeletonMeshRenderer SkeletonRenderer;
        public GraphicsDevice GraphicsDevice;
        public Effect NormalmapEffect;

        public void ApplyNormalmapEffectParameter(Lightning settings, Texture2D normalMap, Renderer renderer)
        {
            NormalmapEffect.Parameters["MatrixTransform"].SetValue(renderer.Projection * renderer.TransformationMatrix);
            NormalmapEffect.Parameters["LightPosition"].SetValue(settings.LightPosition);
            NormalmapEffect.Parameters["LightColor"].SetValue(settings.LightColor);
            NormalmapEffect.Parameters["AmbientColor"].SetValue(settings.AmbientColor);
            NormalmapEffect.Parameters["DrawNormals"].SetValue(settings.DrawNormals);

            if (EngineVariables.DrawNormals)
            {
                NormalmapEffect.Parameters["DrawNormals"].SetValue(1f);
            }

            NormalmapEffect.Parameters["CellShading"].SetValue(settings.CellShading);

            GraphicsDevice.Textures[1] = normalMap;
        }

        public void LoadContent(ContentLoader content)
        {
            Log.WriteLine("Constructing renderer");
            GraphicsDevice = ((IGraphicsDeviceService)content.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

            SkeletonRenderer = new SkeletonMeshRenderer(GraphicsDevice)
            {
                PremultipliedAlpha = false
            };

            Log.WriteLine("Loading normalmap effect");
            NormalmapEffect = content.Load<Effect>(STACK.content.shaders.Normalmap);
        }

        public void UnloadContent()
        {
            NormalmapEffect.Dispose();
        }
    }
}
