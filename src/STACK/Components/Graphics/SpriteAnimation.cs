using System;

namespace STACK.Components
{
    /// <summary>
    /// Makes a sprite cycle through its frames using a configurable delay in game ticks.
    /// The animation can be looped.
    /// </summary>
    [Serializable]
    public class SpriteDelayAnimation : Component
    {
        public bool Looped { get; private set; }
		public int Delay { get; set; }

		private int Timer = 0;

		public override void OnUpdate()
		{
			Timer++;

			if (Timer >= Delay)
			{
				var Sprite = Get<Sprite>();

				if (Sprite == null)
				{
					return;
				}

				if (Looped && Sprite.CurrentFrame == Sprite.TotalFrames)
				{
					Sprite.CurrentFrame = 1;
				}
				else
				{
					Sprite.CurrentFrame++;
				}

				Timer = 0;
			}
		}

		public static SpriteDelayAnimation Create(Entity addTo)
        {
			return addTo.Add<SpriteDelayAnimation>();
        }

		public SpriteDelayAnimation SetLooped(bool value) { Looped = value; return this; }
		public SpriteDelayAnimation SetDelay(int value) { Delay = value; return this; }        
    }    
}
