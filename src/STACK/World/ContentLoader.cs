using Microsoft.Xna.Framework.Content;
using System;

namespace STACK
{
    /// <summary>
    /// ContentManager which adds functionality to load spine skeletons.
    /// </summary>
    public class ContentLoader : ContentManager
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

            return base.Load<T>(assetName);
        }
    }
}
