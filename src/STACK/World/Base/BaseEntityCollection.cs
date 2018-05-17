using Microsoft.Xna.Framework;
using STACK.Graphics;
using STACK.Input;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace STACK
{

    [Serializable]
    public abstract class BaseEntityCollection : BaseEntity
    {
        internal BaseEntity Parent;

        List<BaseEntity> _Items;
        internal List<BaseEntity> Items { get { return _Items; } }

        ComponentList _Components;
        internal ComponentList Components { get { return _Components; } }

        public BaseEntityCollection()
        {
            _Items = new List<BaseEntity>(5);
            _Components = new ComponentList();
        }

        [OnSerializing]
        public void OnSerializing(StreamingContext context)
        {
            if (null != _Items && _Items.Count == 0)
            {
                _Items = null;
            }

            if (null != _Components && _Components.Count == 0)
            {
                _Components = null;
            }
        }

        [OnSerialized]
        public void OnSerialized(StreamingContext context)
        {
            if (null == _Items)
            {
                _Items = new List<BaseEntity>(5);
            }

            if (null == _Components)
            {
                _Components = new ComponentList();
            }
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext c)
        {
            OnSerialized(c);
        }

        public T Add<T>() where T : Component
        {
            var Component = Components.Add<T>();

            Component.Parent = this;
            if (Initialized)
            {
                OnChangeComponents();
            }

            return Component;
        }

        public T Get<T>() where T : Component
        {
            return Components.Get<T>();
        }

        public T GetInterface<T>()
        {
            return Components.GetInterface<T>();
        }

        public BaseEntityCollection Remove<T>() where T : Component
        {
            var Removed = Components.Remove<T>();

            if (Removed)
            {
                OnChangeComponents();
            }

            return this;
        }

        public static int PrioritySorter(BaseEntity a, BaseEntity b)
        {
            return b.DrawOrder.CompareTo(a.DrawOrder);
        }

        public static int ReversePrioritySorter(BaseEntity a, BaseEntity b)
        {
            return a.DrawOrder.CompareTo(b.DrawOrder);
        }

        public static int ReversePrioritySorter(IDraw a, IDraw b)
        {
            return a.DrawOrder.CompareTo(b.DrawOrder);
        }

        public override void OnUpdate()
        {
            for (int i = 0; i < Components.UpdateCompontents.Count; i++)
            {
                var UpdateComponent = Components.UpdateCompontents[i];
                if (UpdateComponent.Enabled)
                {
                    UpdateComponent.Update();
                }

            }

            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Update();
            }
        }

        public override void OnDraw(Renderer renderer)
        {
            for (int i = 0; i < Components.DrawCompontents.Count; i++)
            {
                var DrawComponent = Components.DrawCompontents[i];
                if (DrawComponent.Visible)
                {
                    DrawComponent.Draw(renderer);
                }
            }

            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Draw(renderer);
            }
        }

        public override void OnNotify<T>(string message, T data)
        {
            for (int i = 0; i < Components.NotifyCompontents.Count; i++)
            {
                Components.NotifyCompontents[i].Notify(message, data);
            }

            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Notify(message, data);
            }
        }

        public override void OnLoadContent(ContentLoader content)
        {
            for (int i = 0; i < Components.ContentCompontents.Count; i++)
            {
                Components.ContentCompontents[i].LoadContent(content);
            }

            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].LoadContent(content);
            }
        }

        public override void OnUnloadContent()
        {
            for (int i = 0; i < Components.ContentCompontents.Count; i++)
            {
                Components.ContentCompontents[i].UnloadContent();
            }

            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].UnloadContent();
            }
        }

        public override void OnHandleInputEvent(Vector2 mouse, InputEvent inputEvent)
        {
            for (int i = 0; i < Components.InteractiveCompontents.Count; i++)
            {
                Components.InteractiveCompontents[i].HandleInputEvent(mouse, inputEvent);
            }

            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].HandleInputEvent(mouse, inputEvent);
            }
        }

        public override void OnInitialize(bool restore)
        {
            for (int i = 0; i < Components.InitializeCompontents.Count; i++)
            {
                Components.InitializeCompontents[i].Initialize(restore);
            }

            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Initialize(restore);
            }
        }

        public virtual void OnChangeComponents()
        {

        }
    }
}
