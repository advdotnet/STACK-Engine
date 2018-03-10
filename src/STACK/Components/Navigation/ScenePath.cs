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
        private bool Loaded = false;
        public string PathFile { get; private set; }
        Path _Path;

        public ScenePath()
        {
            PathFile = string.Empty;
            Visible = true;
        }

        public Path Path
        {
            get
            {
                return _Path;
            }
            set
            {
                if (_Path == value)
                {
                    return;
                }

                _Path = value;

                Parent?.Notify(Messages.ScenePathChanged, value);
            }
        }

        public void Draw(Renderer renderer)
        {
            bool DrawPath = EngineVariables.DebugPath && Path != null && renderer.Stage != RenderStage.PostBloom;

            if (DrawPath)
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

        void LoadPath(string file)
        {
            PathFile = file;
            if (Loaded)
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
