using System;

namespace STACK.Components
{
	/// <summary>
	/// Component for animating an entity based on it's transform component.
	/// </summary>
	[Serializable]
	public class SpriteTransformAnimation : Component, INotify, IUpdate, IInitialize
	{
		private Func<Transform, int, int, int> _setFrameFn;
		private bool _enabled;
		private float _updateOrder;
		private int _step = 0;
		[NonSerialized]
		private Sprite _sprite = null;
		[NonSerialized]
		private Transform _transform = null;

		public Func<Transform, int, int, int> SetFrameFn => _setFrameFn;
		public bool Enabled { get => _enabled; set => _enabled = value; }
		public float UpdateOrder { get => _updateOrder; set => _updateOrder = value; }

		public SpriteTransformAnimation()
		{
			Enabled = true;
		}

		public void Initialize(bool restore)
		{
			CacheComponents();
		}

		public void Notify<T>(string message, T data)
		{
			if (Messages.OrientationChanged == message)
			{
				// maybe reset here, too
			}

			if (Messages.AnimationStateChanged == message)
			{
				_step = 0;
			}
		}

		public void Update()
		{
			if (null == _sprite || null == _transform || IsCustomAnimationPlaying() || !Enabled)
			{
				return;
			}

			SetFrame();
			_step++;
		}

		private void CacheComponents()
		{
			_sprite = _sprite ?? Get<Sprite>();
			_transform = _transform ?? Get<Transform>();
		}

		public void SetFrame()
		{
			if (null == _sprite || null == _transform || IsCustomAnimationPlaying() || !Enabled)
			{
				return;
			}

			_sprite.CurrentFrame = SetFrameFn(_transform, _step, _sprite.CurrentFrame);
		}

		private bool IsCustomAnimationPlaying()
		{
			var customAnimation = Get<SpriteCustomAnimation>();
			return (null != customAnimation && customAnimation.Playing);
		}

		public static SpriteTransformAnimation Create(Entity addTo)
		{
			return addTo.Add<SpriteTransformAnimation>();
		}

		public SpriteTransformAnimation SetSetFrameFn(Func<Transform, int, int, int> data) { _setFrameFn = data; return this; }
	}
}
