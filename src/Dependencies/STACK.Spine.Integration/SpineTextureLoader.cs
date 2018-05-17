using Microsoft.Xna.Framework.Graphics;
using Spine;
using System;

namespace STACK
{
    /// <summary>
    /// SpineTextureLoader
    /// </summary>
    public class SpineTextureLoader : Component, TextureLoader, IContent, IWorldAutoAdd
    {
        [NonSerialized]
        GraphicsDevice GraphicsDevice;
        string RootDirectory;

        public void Load(AtlasPage page, string path)
        {
            var Texture = Util.LoadTexture(GraphicsDevice, path);

            page.rendererObject = Texture;
            page.width = Texture.Width;
            page.height = Texture.Height;
        }

        public Skeleton Load(string assetName)
        {
            var Atlas = new Atlas(RootDirectory + "/" + assetName + ".atlas", this);
            var Json = new SkeletonJson(Atlas);
            var Skeleton = new Skeleton(Json.ReadSkeletonData(RootDirectory + "/" + assetName + ".json"));

            return Skeleton;
        }

        public void LoadContent(ContentLoader content)
        {
            RootDirectory = content.RootDirectory;
            var DeviceService = (IGraphicsDeviceService)content.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
            GraphicsDevice = DeviceService.GraphicsDevice;
        }

        public void Unload(Object texture)
        {
            ((Texture2D)texture).Dispose();
        }

        public void UnloadContent()
        {

        }
    }
}
