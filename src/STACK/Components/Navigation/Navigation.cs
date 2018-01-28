using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace STACK.Components
{
    [Serializable]
    public class Navigation : Component
    {
        public List<Vector2> WayPoints
        {
            get
            {
                return _WayPoints;
            }
        }

        private List<Vector2> _WayPoints;
        private float _Scale = 1f;
        public float Scale
        {
            get
            {
                return _Scale;
            }
            set
            {
                if (value != _Scale)
                {
                    _Scale = value;
                    _IsDirty = true;
                }
            }
        }
        public bool ApplyScale { get; set; }
        public bool RestrictPosition { get; set; }
        public bool ApplyColoring { get; set; }
        public bool UseScenePath { get; set; }
        Path _Path;

        public Path Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;

                if (RestrictPosition && Path != null)
                {
                    var Transform = Parent.Get<Transform>();
                    if (Transform != null)
                    {
                        if (!_Path.Contains(Transform.Position))
                        {
                            Transform.Position = Path.GetClosestPoint(Transform.Position);
                        }
                    }
                }

                _IsDirty = true;
            }
        }

        public Navigation()
        {
            Scale = 1f;
            ApplyScale = true;
            ApplyColoring = false;
            RestrictPosition = true;
            UseScenePath = true;
            _WayPoints = new List<Vector2>(5);
        }

        public override void OnNotify<T>(string message, T data)
        {
            if (message == Messages.ScenePathChanged && UseScenePath)
            {
                var ScenePath = Entity.DrawScene.Get<ScenePath>();
                Path = ScenePath == null ? null : ScenePath.Path;
            }

            if (message == Messages.SceneEnter && UseScenePath)
            {
                // sanity check
                var Scripts = Entity.Get<Scripts>();
                if (Scripts != null)
                {
                    if (Scripts.HasScript(ActorScripts.GOTOSCRIPTID)) throw new Exception();
                }

                Scene Scene = (Scene)(object)data;
                if (Scene != null)
                {
                    var ScenePath = Scene.Get<ScenePath>();
                    Path = (ScenePath == null) ? null : ScenePath.Path;
                }
            }
        }

        public List<Vector2> FindPath(Vector2 target)
        {
            var Transform = Get<Transform>();

            if (Path == null || Transform == null)
            {
                WayPoints.Clear();
                WayPoints.Add(target);
            }
            else
            {
                Path.FindPath(Transform.Position, target, ref _WayPoints);
            }

            return _WayPoints;
        }

        private Color LastColor = Color.Transparent;
        private Vector2 LastPosition = Vector2.Zero;
        private bool _IsDirty = false;

        public override void OnUpdate()
        {
            var Transform = Get<Transform>();

            if (Transform == null || Path == null || Path.Mesh == null || (Transform.Position == LastPosition && !_IsDirty))
            {
                return;
            }

            LastPosition = Transform.Position;

            if (ApplyScale)
            {
                Transform.Scale = Path.GetScale(Transform.Position.Y) * Scale;
            }

            if (ApplyColoring)
            {
                var Position = Transform.Position;
                var WithinPath = Path.Contains(Transform.Position);

                if (!WithinPath)
                {
                    var ClosestPoint = Path.GetClosestPoint(Position);
                    WithinPath = (ClosestPoint - Position).LengthSquared() < 3;
                    Position = ClosestPoint;
                }
                else
                {
                    var Color = Path.GetVertexData(Position).Color;

                    if (LastColor != Color)
                    {
                        NotifyParent(Messages.ColorChanged, Color);
                        LastColor = Color;
                    }
                }
            }

            _IsDirty = false;
        }

        public static Navigation Create(Entity addTo)
        {
            return addTo.Add<Navigation>();
        }

        public Navigation SetScale(float value) { Scale = value; return this; }
        public Navigation SetApplyScale(bool value) { ApplyScale = value; return this; }
        public Navigation SetUseScenePath(bool value) { UseScenePath = value; return this; }
        public Navigation SetRestrictPosition(bool value) { RestrictPosition = value; return this; }
        public Navigation SetApplyColoring(bool value) { ApplyColoring = value; return this; }
        public Navigation SetPath(Path value) { Path = value; return this; }
    }
}
