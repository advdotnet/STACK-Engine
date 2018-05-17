using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
    /// <summary>
    /// Base class for AudioEmitter and AudioListener
    /// </summary>
    [Serializable]
    public abstract class AudioTransmission : Component, IInitialize, INotify
    {
        public float Scale { get; set; }
        public bool UpdatePositionWithTransform { get; set; }
        [NonSerialized]
        Transform Transform;

        public AudioTransmission()
        {
            Scale = 200;
            UpdatePositionWithTransform = true;
        }

        protected abstract void SetPosition(Vector3 position);

        public void Notify<T>(string message, T data)
        {
            if (Messages.PositionChanged == message)
            {
                UpdatePositionFromTransform();
            }
        }

        protected void UpdatePositionFromTransform()
        {
            if (UpdatePositionWithTransform && null != Transform)
            {
                var NewPosition = new Vector3(Transform.Position.X / Scale, Transform.Position.Y / Scale, 0);
                SetPosition(NewPosition);
            }
        }

        public void Initialize(bool restore)
        {
            CacheTransients();
            UpdatePositionFromTransform();
        }

        private void CacheTransients()
        {
            Transform = Get<Transform>();
        }
    }
}
