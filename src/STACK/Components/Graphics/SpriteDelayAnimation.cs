using System;

namespace STACK.Components
{
	/// <summary>
	/// Makes a sprite cycle through its frames using a configurable delay in game ticks.
	/// The animation can be looped.
	/// </summary>
	[Serializable]
	public class SpriteDelayAnimation : Component, IUpdate
	{
		private bool _looped;
		private int _delay;
		private bool _enabled;
		private float _updateOrder;
		private int _timer = 0;

		public bool Looped => _looped;
		public int Delay { get => _delay; set => _delay = value; }
		public bool Enabled { get => _enabled; set => _enabled = value; }
		public float UpdateOrder { get => _updateOrder; set => _updateOrder = value; }

		public SpriteDelayAnimation()
		{
			Enabled = true;
		}

		public void Update()
		{
			_timer++;

			if (_timer >= Delay)
			{
				var sprite = Get<Sprite>();

				if (sprite == null)
				{
					return;
				}

				if (Looped && sprite.CurrentFrame == sprite.TotalFrames)
				{
					sprite.CurrentFrame = 1;
				}
				else
				{
					sprite.CurrentFrame++;
				}

				_timer = 0;
			}
		}

		public static SpriteDelayAnimation Create(Entity addTo)
		{
			return addTo.Add<SpriteDelayAnimation>();
		}

		public SpriteDelayAnimation SetLooped(bool value) { _looped = value; return this; }
		public SpriteDelayAnimation SetDelay(int value) { Delay = value; return this; }
	}
}
