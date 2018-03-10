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
        bool _Looped;
        int _Delay;
        bool _Enabled;
        float _UpdateOrder;
        int _Timer = 0;

        public bool Looped { get { return _Looped; } }
        public int Delay { get { return _Delay; } set { _Delay = value; } }
        public bool Enabled { get { return _Enabled; } set { _Enabled = value; } }
        public float UpdateOrder { get { return _UpdateOrder; } set { _UpdateOrder = value; } }

        public SpriteDelayAnimation()
        {
            Enabled = true;
        }

        public void Update()
        {
            _Timer++;

            if (_Timer >= Delay)
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

                _Timer = 0;
            }
        }

        public static SpriteDelayAnimation Create(Entity addTo)
        {
            return addTo.Add<SpriteDelayAnimation>();
        }

        public SpriteDelayAnimation SetLooped(bool value) { _Looped = value; return this; }
        public SpriteDelayAnimation SetDelay(int value) { Delay = value; return this; }
    }
}
