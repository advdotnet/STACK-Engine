using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
        
    [Serializable]
    public class SpriteTransformAnimation : Component
    {        
        public Func<State, Vector2, int, int> SetFrameFn { get; private set; }
        int Step = 0;
        [NonSerialized]
        Sprite _Sprite = null;
        [NonSerialized]
        Transform _Transform = null;

        public override void OnNotify<T>(string message, T data)
        {
            if (Messages.OrientationChanged == message)
            {

            }

            if (Messages.AnimationStateChanged == message)
            {
                Step = 0;
            }
        }

        public override void OnUpdate()
		{
            _Sprite = _Sprite ?? Get<Sprite>();
            _Transform = _Transform ?? Get<Transform>();

			if (null == _Sprite || null == _Transform)
			{
				return;
			}

            _Sprite.CurrentFrame = SetFrameFn(_Transform.State, _Transform.Orientation, Step++);
		}

		public static SpriteTransformAnimation Create(Entity addTo)
        {
			return addTo.Add<SpriteTransformAnimation>();
        }

        public SpriteTransformAnimation SetSetFrameFn(Func<State, Vector2, int, int> data) { SetFrameFn = data;  return this; }		
    }    
}
