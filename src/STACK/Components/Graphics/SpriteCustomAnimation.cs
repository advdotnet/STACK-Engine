using System;

namespace STACK.Components
{
    /// <summary>
    /// Component for custom sprite animations.
    /// </summary>
    [Serializable]
    public class SpriteCustomAnimation : Component, IPlayAnimation
    {
        public Func<Transform, string, Frames> GetFramesFn { get; private set; }
        string AnimationName = "";
        int Step = 0;
        [NonSerialized]
        Sprite _Sprite = null;
        [NonSerialized]
        Transform _Transform = null;
        Frames Frames = null;
        public bool Playing { get; private set; }
        public bool Looped { get; private set; }
        public string Animation { get { return AnimationName; } }

        private void CacheComponents()
        {
            _Sprite = _Sprite ?? Get<Sprite>();
            _Transform = _Transform ?? Get<Transform>();
        }

        public void StopAnimation()
        {
            Playing = false;
            Step = 0;
            AnimationName = string.Empty;
            Looped = false;
        }

        public override void OnUpdate()
        {
            CacheComponents();

            if (null == _Sprite || !Playing)
            {
                return;
            }

            _Sprite.CurrentFrame = Frames[Step];

            Step++;

            if (Step >= Frames.Count)
            {
                if (!Looped)
                {
                    StopAnimation();
                }
                else
                {
                    Step = 0;
                }
            }
        }

        public void PlayAnimation(string animation, bool looped)
        {
            CacheComponents();
            Frames = GetFramesFn(_Transform, animation);
            if (Frames.Count > 0)
            {
                Looped = looped;
                Playing = true;
                Step = 0;
                AnimationName = animation;
                _Sprite.CurrentFrame = Frames[Step];
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

        public SpriteCustomAnimation SetGetFramesFn(Func<Transform, string, Frames> data) { GetFramesFn = data; return this; }
    }
}
