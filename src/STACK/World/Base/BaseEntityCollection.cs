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
        internal List<BaseEntity> Items { get; set; }
        protected BaseEntity Parent;

        [NonSerialized]
        protected Dictionary<Type, Component> ComponentCache = new Dictionary<Type, Component>();

        [OnDeserialized]
        void OnDeserialized(StreamingContext c)
        {
            ComponentCache = new Dictionary<Type, Component>();
        }

        public T Add<T>() where T : Component
        {
            var Component = Activator.CreateInstance<T>();
            return Add(Component);
        }

        public T Add<T>(T component) where T : Component
        {
            var ComponentType = component.GetType();
            if (ComponentCache.ContainsKey(ComponentType))
            {
                throw new InvalidOperationException("Component type" + ComponentType.Name + " already added.");
            }

            Items.Add(component);
            ComponentCache.Add(ComponentType, component);
            component.Parent = this;
            if (Initialized)
            {
                OnChangeComponents();
            }

            return component;
        }

        public T Get<T>() where T : Component
        {
            Component Result;
            var Type = typeof(T);

            if (ComponentCache.TryGetValue(Type, out Result))
            {
                return (T)Result;
            }


            foreach (var Item in Items)
            {
                if (Type.IsAssignableFrom(Item.GetType()))
                {
                    ComponentCache.Add(Type, (T)Item);
                    return (T)Item;
                }
            }

            return null;
        }

        public BaseEntityCollection Remove<T>() where T : Component
        {
            Component Component;

            if (ComponentCache.TryGetValue(typeof(T), out Component))
            {
                ComponentCache.Remove(typeof(T));
                Items.Remove(Component);
                OnChangeComponents();
            }

            return this;
        }

        public BaseEntityCollection()
        {
            Items = new List<BaseEntity>(10);
        }

        public static int PrioritySorter(BaseEntity a, BaseEntity b)
        {
            return b.Priority.CompareTo(a.Priority);
        }

        public static int ReversePrioritySorter(BaseEntity a, BaseEntity b)
        {
            return a.Priority.CompareTo(b.Priority);
        }

        public override void OnUpdate()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Update();
            }
        }

        public override void OnDraw(Renderer renderer)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Draw(renderer);
            }
        }

        public override void OnNotify<T>(string message, T data)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Notify(message, data);
            }
        }

        public override void OnLoadContent(ContentLoader content)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].LoadContent(content);
            }
        }

        public override void OnUnloadContent()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].UnloadContent();
            }
        }

        public override void OnHandleInputEvent(Vector2 mouse, InputEvent inputEvent)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].HandleInputEvent(mouse, inputEvent);
            }
        }

        public override void OnInitialize()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Initialize();
            }
        }

        public virtual void OnChangeComponents()
        {

        }
    }
}
