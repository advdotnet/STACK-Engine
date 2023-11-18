using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace STACK.Components
{
	[Serializable]
	public class Navigation : Component, INotify, IUpdate
	{
		public bool Enabled { get; set; }
		public float UpdateOrder { get; set; }

		public List<Vector2> WayPoints => _wayPoints;

		private List<Vector2> _wayPoints;
		private float _scale = 1f;
		public float Scale
		{
			get => _scale;
			set
			{
				if (value != _scale)
				{
					_scale = value;
					_isDirty = true;
				}
			}
		}
		public bool ApplyScale { get; set; }
		public bool RestrictPosition { get; set; }
		public bool ApplyColoring { get; set; }
		public bool UseScenePath { get; set; }

		private Path _path;

		public Path Path
		{
			get => _path;
			set
			{
				_path = value;

				if (RestrictPosition && Path != null)
				{
					var transform = Parent.Get<Transform>();
					if (transform != null)
					{
						if (!_path.Contains(transform.Position))
						{
							transform.Position = Path.GetClosestPoint(transform.Position);
						}
					}
				}

				_isDirty = true;
			}
		}

		public Navigation()
		{
			Scale = 1f;
			ApplyScale = true;
			ApplyColoring = false;
			RestrictPosition = true;
			Enabled = true;
			UseScenePath = true;
			_wayPoints = new List<Vector2>(5);
		}

		public void Notify<T>(string message, T data)
		{
			if (message == Messages.ScenePathChanged && UseScenePath)
			{
				var scenePath = Entity.DrawScene.Get<ScenePath>();
				Path = scenePath?.Path;
			}

			if (message == Messages.SceneEnter && UseScenePath)
			{
				// sanity check
				var scripts = Entity.Get<Scripts>();
				if (scripts != null)
				{
					if (scripts.HasScript(ActorScripts.GOTOSCRIPTID))
					{
						throw new Exception();
					}
				}

				var scene = (Scene)(object)data;
				if (scene != null)
				{
					var scenePath = scene.Get<ScenePath>();
					Path = scenePath?.Path;
				}
			}
		}

		public List<Vector2> FindPath(Vector2 target)
		{
			var transform = Get<Transform>();

			if (Path == null || transform == null || !RestrictPosition)
			{
				WayPoints.Clear();
				WayPoints.Add(target);
			}
			else
			{
				Path.FindPath(transform.Position, target, ref _wayPoints);
			}

			return _wayPoints;
		}

		private Color _lastColor = Color.Transparent;
		private Vector2 _lastPosition = Vector2.Zero;
		private bool _isDirty = false;

		public void Update()
		{
			var transform = Get<Transform>();

			if (transform == null || Path == null || Path.Mesh == null || (transform.Position == _lastPosition && !_isDirty))
			{
				return;
			}

			_lastPosition = transform.Position;

			if (ApplyScale)
			{
				transform.Scale = Path.GetScale(transform.Position.Y) * Scale;
			}

			if (ApplyColoring)
			{
				var position = transform.Position;
				var withinPath = Path.Contains(transform.Position);

				if (withinPath)
				{
					var color = Path.GetVertexData(position).Color;

					if (_lastColor != color)
					{
						Parent?.Notify(Messages.ColorChanged, color);
						_lastColor = color;
					}
				}
			}

			_isDirty = false;
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
