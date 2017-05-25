using Microsoft.Xna.Framework;
using STACK.Graphics;
using STACK.Input;
using System;

namespace STACK
{
    [Serializable]
    public abstract class BaseEntity
    {
        public static class Properties
        {
            public const string Interactive = "Interactive";
            public const string Enabled = "Enabled";
            public const string Visible = "Visible";
            public const string Priority = "Priority";
        }

        protected bool Initialized { get; private set; }
        protected bool Loaded { get; private set; }
        public string ID { get; protected set; }
        float _Priority = 0;
        bool _Interactive = true;
        bool _Enabled = true;
        bool _Visible = true;

        public BaseEntity(string id)
            : this()
        {
            if (!string.IsNullOrEmpty(id))
            {
                ID = id;
            }
        }

        public BaseEntity()
        {
            if (string.IsNullOrEmpty(ID))
            {
                ID = GetType().FullName;
            }
        }

        public bool Interactive
        {
            get
            {
                return _Interactive;
            }
            set
            {
                bool Changed = _Interactive != value;
                _Interactive = value;
                OnPropertyChanged(Properties.Interactive);
            }
        }

        public bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                bool Changed = _Enabled != value;
                _Enabled = value;
                OnPropertyChanged(Properties.Enabled);
            }
        }

        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                bool Changed = _Visible != value;
                _Visible = value;
                OnPropertyChanged(Properties.Visible);
            }
        }

        public float Priority
        {
            get
            {
                return _Priority;
            }
            set
            {
                bool Changed = _Priority != value;
                _Priority = value;
                if (Changed)
                {
                    OnPropertyChanged(Properties.Priority);
                }
            }
        }

        public void Update()
        {
            if (Enabled)
            {
                OnUpdate();
            }
        }

        public void HandleInputEvent(Vector2 mouse, InputEvent inputEvent)
        {
            if (Interactive)
            {
                OnHandleInputEvent(mouse, inputEvent);
            }
        }

        public void Draw(Renderer renderer)
        {
            if (Visible)
            {
                OnBeginDraw(renderer);
                OnDraw(renderer);
                OnEndDraw(renderer);
            }
        }

        public void LoadContent(ContentLoader content)
        {
            OnLoadContent(content);
            Loaded = true;
        }

        public void Notify<T>(string message, T data)
        {
            if (Initialized)
            {
                OnNotify(message, data);
            }
        }

        public void UnloadContent()
        {
            OnUnloadContent();
            Loaded = false;
        }

        public void Initialize()
        {
            OnInitialize();
            Initialized = true;
        }

        public void SetEnabled(bool value) { Enabled = value; }
        public void SetVisible(bool value) { Visible = value; }
        public void SetPriority(float value) { Priority = value; }

        public virtual void OnUpdate() { }
        public virtual void OnNotify<T>(string message, T data) { }
        public virtual void OnLoadContent(ContentLoader content) { }
        public virtual void OnUnloadContent() { }
        public virtual void OnHandleInputEvent(Vector2 mouse, InputEvent inputEvent) { }
        public virtual void OnBeginDraw(Renderer renderer) { }
        public virtual void OnDraw(Renderer renderer) { }
        public virtual void OnEndDraw(Renderer renderer) { }
        public virtual void OnPropertyChanged(string property) { }
        public virtual void OnInitialize() { }
    }
}
