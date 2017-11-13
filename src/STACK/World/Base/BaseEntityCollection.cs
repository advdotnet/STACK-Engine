using Microsoft.Xna.Framework;
using STACK.Graphics;
using STACK.Input;
using System;
using System.Collections.Generic;

namespace STACK
{

    [Serializable]
    public abstract class BaseEntityCollection : BaseEntity
    {
        internal List<BaseEntity> Items { get; set; }
        protected BaseEntity Parent;

        [NonSerialized]
        protected Dictionary<Type, Component> ComponentCache = new Dictionary<Type, Component>();

        public T Add<T>(bool enabled = true, bool visible = true) where T : Component
        {
            var Component = Activator.CreateInstance<T>();
            return Add(Component, enabled, visible);
        }

        public T Add<T>(T component, bool enabled = true, bool visible = true) where T : Component
        {
            var ComponentType = component.GetType();
            if (ComponentCache.ContainsKey(ComponentType))
            {
                throw new InvalidOperationException("Component type" + ComponentType.Name + " already added.");
            }

            component.Enabled = enabled;
            component.Visible = visible;

            Items.Add(component);
            ComponentCache.Add(ComponentType, component);
            component.Parent = this;

            return component;
        }

        public T Get<T>() where T : Component
        {
            Component Result;

            if (ComponentCache == null)
            {
                ComponentCache = new Dictionary<Type, Component>();
            }

            if (ComponentCache.TryGetValue(typeof(T), out Result))
            {
                return (T)Result;
            }

            foreach (var ComponentType in ComponentCache.Keys)
            {
                if (typeof(T).IsAssignableFrom(ComponentType))
                {
                    return (T)ComponentCache[ComponentType];
                }
            }

            foreach (var Item in Items)
            {
                if (typeof(T).IsAssignableFrom(Item.GetType()))
                {
                    ComponentCache.Add(typeof(T), (Component)Item);
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
    }
}
