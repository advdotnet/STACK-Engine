using STACK.Graphics;
using System;

namespace STACK.Components
{
	[Serializable]
	public class ScenePath : Component, IDraw, IContent
	{
		public bool Visible { get; set; }
		public float DrawOrder { get; set; }
		[NonSerialized]
		private readonly bool _loaded = false;
		public string PathFile { get; private set; }

		private Path _path;

		public ScenePath()
		{
			PathFile = string.Empty;
			Visible = true;
		}

		public Path Path
		{
			get => _path;
			set
			{
				if (_path == value)
				{
					return;
				}

				_path = value;

				Parent?.Notify(Messages.ScenePathChanged, value);
			}
		}

		public void Draw(Renderer renderer)
		{
			var drawPath = EngineVariables.DebugPath && Path != null && renderer.Stage != RenderStage.PostBloom;

			if (drawPath)
			{
				Path.Draw(renderer.PrimitivesRenderer.DrawTriangle, renderer.PrimitivesRenderer.DrawLine, Scene.World.Get<Mouse>().Position);
			}
		}

		public void LoadContent(ContentLoader content)
		{
			if (!string.IsNullOrEmpty(PathFile))
			{
				Path = content.Load<Path>(PathFile);
			}
		}

		public void UnloadContent() { }

		private void LoadPath(string file)
		{
			PathFile = file;
			if (_loaded)
			{
				LoadContent(((Scene)Parent).Content);
			}
		}

		public static ScenePath Create(Scene addTo)
		{
			return addTo.Add<ScenePath>();
		}

		public ScenePath SetPathFile(string value) { LoadPath(value); return this; }
		public ScenePath SetPath(Path value) { Path = value; return this; }
	}
}
