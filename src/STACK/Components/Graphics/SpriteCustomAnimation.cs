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
        Action<Transform, string, Frames> _GetFramesAction;
        string _AnimationName = "";
        int Step = 0;
        [NonSerialized]
        Sprite _Sprite = null;
        [NonSerialized]
        Transform _Transform = null;
        Frames Frames = Frames.Empty;
        bool _Playing;
        bool _Looped;
        bool _Enabled;
        float _UpdateOrder;

        public Action<Transform, string, Frames> GetFramesAction { get { return _GetFramesAction; } }
        public bool Playing { get { return _Playing; } }
        public bool Looped { get { return _Looped; } }
        public string Animation { get { return _AnimationName; } }
        public bool Enabled { get { return _Enabled; } set { _Enabled = value; } }
        public float UpdateOrder { get { return _UpdateOrder; } set { _UpdateOrder = value; } }

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
            _Sprite = Get<Sprite>();
            _Transform = Get<Transform>();
        }

        public void StopAnimation()
        {
            _Playing = false;
            Step = 0;
            _AnimationName = string.Empty;
            _Looped = false;
        }

        public void Update()
        {
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
            // avoid memory allocation
            Frames.Clear();
            GetFramesAction(_Transform, animation, Frames);
            if (Frames.Count > 0)
            {
                _Looped = looped;
                _Playing = true;
                Step = 0;
                _AnimationName = animation;
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

        public SpriteCustomAnimation SetGetFramesAction(Action<Transform, string, Frames> value) { _GetFramesAction = value; return this; }
    }
}
