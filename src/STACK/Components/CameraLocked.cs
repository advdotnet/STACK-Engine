using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
	/// <summary>
	/// When an entity with this component changes its DrawScene it is made sure that 
	/// the new Scene is visible and the old Scene set to invisible.
	/// 
	/// Also the DrawScene's camera will scroll to ensure the entity is in the visible region.
	/// </summary>
	[Serializable]
	public class CameraLocked : Component, INotify, IUpdate, IInitialize
	{
		public bool Enabled { get; set; }
		public float Acceleration { get; set; }
		public float Damping { get; set; }
		public bool Scroll { get; set; }
		public bool CenterCharacter { get; set; }
		public float UpdateOrder { get; set; }

		private bool _newSceneEntered = false;

		public CameraLocked()
		{
			Acceleration = 8f;
			Damping = 3.0f;
			Scroll = true;
			CenterCharacter = true;
			Enabled = true;
		}

		public void Initialize(bool restore)
		{
			CacheTransients();
		}

		public void Notify<T>(string message, T data)
		{
			if (!Enabled)
			{
				return;
			}

			if (message == Messages.SceneEnter)
			{
				var scene = (Scene)(object)data;

				Entity.DrawScene.Visible = false;
				Entity.DrawScene.Enabled = false;

				scene.Visible = true;
				scene.Enabled = true;

				_newSceneEntered = true;
			}

			if (Messages.SceneEntered == message)
			{
				CacheTransients();
			}
		}

		private void CacheTransients()
		{
			_resolution = Entity.World.Get<RenderSettings>().VirtualResolution;
			_transform = Get<Transform>();
			var backgroundObject = Entity.DrawScene.GetObject(Location.BACKGROUND_ENTITY_ID);
			_hasBackground = (null != backgroundObject);
			if (_hasBackground)
			{
				var backgroundSprite = backgroundObject.Get<Sprite>();
				_backgroundWidth = backgroundSprite.Texture.Width / backgroundSprite.Columns;
				_backgroundHeight = backgroundSprite.Texture.Height / backgroundSprite.Rows;
			}
			_camera = Entity.DrawScene.Get<Camera>();
		}

		[NonSerialized]
		private Point _resolution;
		[NonSerialized]
		private Transform _transform;
		[NonSerialized]
		private int _backgroundWidth, _backgroundHeight;
		[NonSerialized]
		private bool _hasBackground;
		[NonSerialized]
		private Camera _camera;

		public void Update()
		{
			if (!Scroll || !_hasBackground)
			{
				return;
			}

			var transformedPosition = _camera.Transform(_transform.Position);
			var delta = Vector2.Zero;

			if (_newSceneEntered && CenterCharacter)
			{
				var newX = _transform.Position.X - _resolution.X / 2;
				newX = Math.Max(0, newX);
				newX = Math.Min(_backgroundWidth - _resolution.X, newX);

				var newY = _transform.Position.Y - _resolution.Y / 2;
				newY = Math.Max(0, newY);
				newY = Math.Min(_backgroundHeight - _resolution.Y, newY);

				_camera.Position = new Vector2(newX, newY);

				_newSceneEntered = false;
			}

			var shouldScrollLeft = _camera.Position.X > 0 && transformedPosition.X < _resolution.X / 2f - (_resolution.X / 15f);
			var shouldScrollRight = _camera.Position.X < _backgroundWidth - _resolution.X && transformedPosition.X > _resolution.X - _resolution.X / 2f + (_resolution.X / 15f);

			if (shouldScrollLeft || shouldScrollRight)
			{
				delta = new Vector2((transformedPosition.X / (_resolution.X / 2f) - 1) * Acceleration, 0);
				var damp = (shouldScrollLeft ? _camera.Position.X : _backgroundWidth - _resolution.X - _camera.Position.X) / (_resolution.X / 2f);
				delta *= Math.Min(1, damp * Damping);
			}
			else
			{
				_camera.Position = _camera.Position.ToInt();
			}

			_camera.Move(delta);
		}

		public static CameraLocked Create(Entity addTo)
		{
			return addTo.Add<CameraLocked>();
		}

		public CameraLocked SetAcceleration(float value) { Acceleration = value; return this; }
		public CameraLocked SetDamping(float value) { Damping = value; return this; }
		public CameraLocked SetScroll(bool value) { Scroll = value; return this; }
		public CameraLocked SetCenterCharacter(bool value) { CenterCharacter = value; return this; }
		public CameraLocked SetEnabled(bool value) { Enabled = value; return this; }
	}
}
