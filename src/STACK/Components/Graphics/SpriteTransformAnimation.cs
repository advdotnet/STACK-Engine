using System;

namespace STACK.Components
{
    /// <summary>
    /// Component for animating an entity based on it's transform component.
    /// </summary>
    [Serializable]
    public class SpriteTransformAnimation : Component, INotify, IUpdate, IInitialize
    {
        Func<Transform, int, int, int> _SetFrameFn;
        bool _Enabled;
        float _UpdateOrder;
        int _Step = 0;
        [NonSerialized]
        Sprite _Sprite = null;
        [NonSerialized]
        Transform _Transform = null;

        public Func<Transform, int, int, int> SetFrameFn { get { return _SetFrameFn; } }
        public bool Enabled { get { return _Enabled; } set { _Enabled = value; } }
        public float UpdateOrder { get { return _UpdateOrder; } set { _UpdateOrder = value; } }

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
                _Step = 0;
            }
        }

        public void Update()
        {
            if (null == _Sprite || null == _Transform || IsCustomAnimationPlaying() || !Enabled)
            {
                return;
            }

            SetFrame();
            _Step++;
        }

        private void CacheComponents()
        {
            _Sprite = _Sprite ?? Get<Sprite>();
            _Transform = _Transform ?? Get<Transform>();
        }

        public void SetFrame()
        {
            if (null == _Sprite || null == _Transform || IsCustomAnimationPlaying() || !Enabled)
            {
                return;
            }

            _Sprite.CurrentFrame = SetFrameFn(_Transform, _Step, _Sprite.CurrentFrame);
        }

        private bool IsCustomAnimationPlaying()
        {
            var CustomAnimation = Get<SpriteCustomAnimation>();
            return (null != CustomAnimation && CustomAnimation.Playing);
        }

        public static SpriteTransformAnimation Create(Entity addTo)
        {
            return addTo.Add<SpriteTransformAnimation>();
        }

        public SpriteTransformAnimation SetSetFrameFn(Func<Transform, int, int, int> data) { _SetFrameFn = data; return this; }
    }
}
