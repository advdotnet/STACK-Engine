using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using System;

namespace STACK
{
    /// <summary>
    /// ContentManager which adds functionality to load spine skeletons.
    /// </summary>
    public class ContentLoader : ContentManager, TextureLoader
    {
        public ContentLoader(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            RootDirectory = EngineVariables.ContentPath;
        }

        public ContentLoader(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
        {
            RootDirectory = EngineVariables.ContentPath;
        }

        public override T Load<T>(string assetName)
        {
            if (typeof(T) == typeof(Path))
            {
                return (T)(object)Path.LoadFromFile(RootDirectory + "/" + assetName);
            }

            if (typeof(T) == typeof(Skeleton))
            {
                var Atlas = new Atlas(RootDirectory + "/" + assetName + ".atlas", this);
                var Json = new SkeletonJson(Atlas);
                var Skeleton = new Skeleton(Json.ReadSkeletonData(RootDirectory + "/" + assetName + ".json"));

                return (T)(object)Skeleton;
            }

            return base.Load<T>(assetName);
        }

        public void Load(AtlasPage page, string path)
        {
            IGraphicsDeviceService Service = (IGraphicsDeviceService)this.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
            var Texture = Util.LoadTexture(Service.GraphicsDevice, path);

            page.rendererObject = Texture;
            page.width = Texture.Width;
            page.height = Texture.Height;
        }

        public void Unload(Object texture)
        {
            ((Texture2D)texture).Dispose();
        }
    }

}
