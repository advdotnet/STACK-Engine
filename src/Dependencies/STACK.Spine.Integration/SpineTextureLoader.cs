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
		private GraphicsDevice _graphicsDevice;
		private string _rootDirectory;

		public void Load(AtlasPage page, string path)
		{
			var texture = Util.LoadTexture(_graphicsDevice, path);

			page.rendererObject = texture;
			page.width = texture.Width;
			page.height = texture.Height;
		}

		public Skeleton Load(string assetName)
		{
			var atlas = new Atlas(_rootDirectory + "/" + assetName + ".atlas", this);
			var json = new SkeletonJson(atlas);
			var skeleton = new Skeleton(json.ReadSkeletonData(_rootDirectory + "/" + assetName + ".json"));

			return skeleton;
		}

		public void LoadContent(ContentLoader content)
		{
			_rootDirectory = content.RootDirectory;
			var deviceService = (IGraphicsDeviceService)content.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
			_graphicsDevice = deviceService.GraphicsDevice;
		}

		public void Unload(object texture)
		{
			((Texture2D)texture).Dispose();
		}

		public void UnloadContent()
		{

		}
	}
}
