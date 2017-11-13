using System;

namespace STACK.Components
{
    /// <summary>
    /// Component for animating an entity based on it's transform component.
    /// </summary>
    [Serializable]
    public class SpriteTransformAnimation : Component
    {
        public Func<Transform, int, int, int> SetFrameFn { get; private set; }
        int Step = 0;
        [NonSerialized]
        Sprite _Sprite = null;
        [NonSerialized]
        Transform _Transform = null;

        public override void OnNotify<T>(string message, T data)
        {
            if (Messages.OrientationChanged == message)
            {
                // maybe reset here, too
            }

            if (Messages.AnimationStateChanged == message)
            {
                Step = 0;
            }
        }

        public override void OnUpdate()
        {
            CacheComponents();

            if (null == _Sprite || null == _Transform || IsCustomAnimationPlaying() || !Enabled)
            {
                return;
            }

            SetFrame();
            Step++;
        }

        private void CacheComponents()
        {
            _Sprite = _Sprite ?? Get<Sprite>();
            _Transform = _Transform ?? Get<Transform>();
        }

        public void SetFrame()
        {
            CacheComponents();

            if (null == _Sprite || null == _Transform || IsCustomAnimationPlaying() || !Enabled)
            {
                return;
            }

            _Sprite.CurrentFrame = SetFrameFn(_Transform, Step, _Sprite.CurrentFrame);
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

        public SpriteTransformAnimation SetSetFrameFn(Func<Transform, int, int, int> data) { SetFrameFn = data; return this; }
    }
}
