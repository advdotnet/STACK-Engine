using Microsoft.Xna.Framework;
using STACK.Graphics;
using STACK.Input;
using System;

namespace STACK
{
    [Serializable]
    public abstract class BaseEntity : IUpdate, IDraw, IInitialize, IContent, IInteractive, INotify
    {
        public static class Properties
        {
            public const string Interactive = "Interactive";
            public const string Enabled = "Enabled";
            public const string Visible = "Visible";
            public const string DrawOrder = "DrawOrder";
            public const string UpdateOrder = "UpdateOrder";
        }

        public bool Initialized { get { return _Initialized; } }
        public bool Loaded { get { return _Loaded; } }
        public string ID { get { return _ID; } }

        protected string _ID = string.Empty;
        float _DrawOrder = 0;
        float _UpdateOrder = 0;
        bool _Interactive = true;
        bool _Enabled = true;
        bool _Visible = true;
        [NonSerialized]
        bool _Initialized = false;
        [NonSerialized]
        bool _Loaded = false;

        public BaseEntity(string id)
                : this()
        {
            if (!string.IsNullOrEmpty(id))
            {
                _ID = id;
            }
        }

        public BaseEntity()
        {
            if (string.IsNullOrEmpty(ID))
            {
                _ID = GetType().FullName;
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
                if (Changed)
                {
                    _Interactive = value;
                    OnPropertyChanged(Properties.Interactive);
                }
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
                if (Changed)
                {
                    _Enabled = value;
                    OnPropertyChanged(Properties.Enabled);
                }
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
                if (Changed)
                {
                    _Visible = value;
                    OnPropertyChanged(Properties.Visible);
                }
            }
        }

        public float DrawOrder
        {
            get
            {
                return _DrawOrder;
            }
            set
            {
                bool Changed = _DrawOrder != value;
                if (Changed)
                {
                    _DrawOrder = value;
                    OnPropertyChanged(Properties.DrawOrder);
                }
            }
        }

        public float UpdateOrder
        {
            get
            {
                return _UpdateOrder;
            }
            set
            {
                bool Changed = _UpdateOrder != value;
                if (Changed)
                {
                    _UpdateOrder = value;
                    OnPropertyChanged(Properties.UpdateOrder);
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
            _Loaded = true;
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
            _Loaded = false;
        }

        /// <summary>
        /// Called after the content has been loaded. Can be used to programatically create additional resources
        /// or to cache some components.
        /// </summary>
        /// <param name="restore"></param>
        public void Initialize(bool restore)
        {
            OnInitialize(restore);
            _Initialized = true;
        }

        public void SetEnabled(bool value) { Enabled = value; }
        public void SetVisible(bool value) { Visible = value; }
        public void SetDrawOrder(float value) { DrawOrder = value; }
        public void SetUpdateOrder(float value) { UpdateOrder = value; }

        public virtual void OnUpdate() { }
        public virtual void OnNotify<T>(string message, T data) { }
        public virtual void OnLoadContent(ContentLoader content) { }
        public virtual void OnUnloadContent() { }
        public virtual void OnHandleInputEvent(Vector2 mouse, InputEvent inputEvent) { }
        public virtual void OnBeginDraw(Renderer renderer) { }
        public virtual void OnDraw(Renderer renderer) { }
        public virtual void OnEndDraw(Renderer renderer) { }
        public virtual void OnPropertyChanged(string property) { }
        public virtual void OnInitialize(bool restore) { }
    }
}