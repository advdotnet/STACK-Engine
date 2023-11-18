using STACK.Logging;
using System;

namespace STACK.Components
{
	/// <summary>
	/// Component for custom sprite animations.
	/// </summary>
	[Serializable]
	public class SpriteCustomAnimation : Component, IPlayAnimation, IUpdate, IInitialize
	{
		private Action<Transform, string, Frames> _getFramesAction;
		private string _animationName = string.Empty;
		private int _step = 0;
		[NonSerialized]
		private Sprite _sprite = null;
		[NonSerialized]
		private Transform _transform = null;
		private readonly Frames _frames = Frames.Empty;
		private bool _playing;
		private bool _looped;
		private bool _enabled;
		private float _updateOrder;

		public Action<Transform, string, Frames> GetFramesAction => _getFramesAction;
		public bool Playing => _playing;
		public bool Looped => _looped;
		public string Animation => _animationName;
		public bool Enabled { get => _enabled; set => _enabled = value; }
		public float UpdateOrder { get => _updateOrder; set => _updateOrder = value; }

		public SpriteCustomAnimation()
		{
			Enabled = true;
		}

		public void Initialize(bool restore)
		{
			CacheComponents();
		}

		private void CacheComponents()
		{
			_sprite = Get<Sprite>();
			_transform = Get<Transform>();
		}

		public void StopAnimation()
		{
			_playing = false;
			_step = 0;
			_animationName = string.Empty;
			_looped = false;
		}

		public void Update()
		{
			if (null == _sprite || !Playing)
			{
				return;
			}

			_sprite.CurrentFrame = _frames[_step];

			_step++;

			if (_step >= _frames.Count)
			{
				if (!Looped)
				{
					StopAnimation();
				}
				else
				{
					_step = 0;
				}
			}
		}

		public void PlayAnimation(string animation, bool looped)
		{
			// avoid memory allocation
			_frames.Clear();
			GetFramesAction(_transform, animation, _frames);
			if (_frames.Count > 0)
			{
				_looped = looped;
				_playing = true;
				_step = 0;
				_animationName = animation;
				_sprite.CurrentFrame = _frames[_step];
			}
			else
			{
				Log.WriteLine("Custom animation not found: " + animation, LogLevel.Notice);
			}
		}

		public static SpriteCustomAnimation Create(Entity addTo)
		{
			return addTo.Add<SpriteCustomAnimation>();
		}

		public SpriteCustomAnimation SetGetFramesAction(Action<Transform, string, Frames> value) { _getFramesAction = value; return this; }
	}
}
