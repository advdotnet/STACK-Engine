using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace STACK
{

    [Serializable]
    public class ComponentList : IEnumerable<Component>
    {
        internal List<Component> Components = new List<Component>();
        [NonSerialized]
        internal Dictionary<Type, Component> ComponentTypeCache = new Dictionary<Type, Component>();

        [NonSerialized]
        List<IUpdate> _UpdateCompontents;
        [NonSerialized]
        List<IDraw> _DrawCompontents;
        [NonSerialized]
        List<IInitialize> _InitializeCompontents;
        [NonSerialized]
        List<IContent> _ContentCompontents;
        [NonSerialized]
        List<IInteractive> _InteractiveCompontents;
        [NonSerialized]
        List<INotify> _NotifyCompontents;

        public List<IUpdate> UpdateCompontents { get { return _UpdateCompontents; } }
        public List<IDraw> DrawCompontents { get { return _DrawCompontents; } }
        public List<IInitialize> InitializeCompontents { get { return _InitializeCompontents; } }
        public List<IContent> ContentCompontents { get { return _ContentCompontents; } }
        public List<IInteractive> InteractiveCompontents { get { return _InteractiveCompontents; } }
        public List<INotify> NotifyCompontents { get { return _NotifyCompontents; } }

        void Cache()
        {
            _UpdateCompontents = new List<IUpdate>();
            _DrawCompontents = new List<IDraw>();
            _InitializeCompontents = new List<IInitialize>();
            _ContentCompontents = new List<IContent>();
            _InteractiveCompontents = new List<IInteractive>();
            _NotifyCompontents = new List<INotify>();

            foreach (var Component in Components)
            {
                AddRemoveComponentInterface(true, Component);
            }
        }

        [OnSerializing]
        public void OnSerializing(StreamingContext context)
        {
            if (null != Components && Components.Count == 0)
            {
                Components = null;
            }
        }

        [OnSerialized]
        public void OnSerialized(StreamingContext context)
        {
            if (null == Components)
            {
                Components = new List<Component>();
            }
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext c)
        {
            ComponentTypeCache = new Dictionary<Type, Component>();
            if (null == Components)
            {
                Components = new List<Component>();
            }
            Cache();
        }

        public ComponentList()
        {
            ComponentTypeCache = new Dictionary<Type, Component>();
            _UpdateCompontents = new List<IUpdate>();
            _DrawCompontents = new List<IDraw>();
            _InitializeCompontents = new List<IInitialize>();
            _ContentCompontents = new List<IContent>();
            _InteractiveCompontents = new List<IInteractive>();
            _NotifyCompontents = new List<INotify>();
        }

        public Component this[int index] { get { return Components[index]; } }
        public int Count { get { return Components.Count; } }

        private T Add<T>(T component) where T : Component
        {
            var ComponentType = component.GetType();
            if (ComponentTypeCache.ContainsKey(ComponentType))
            {
                throw new InvalidOperationException("Component type" + ComponentType.Name + " already added.");
            }

            Components.Add(component);
            AddRemoveComponentInterface(true, component);
            ComponentTypeCache.Add(ComponentType, component);

            return component;
        }

        public T Add<T>() where T : Component
        {
            var Component = Activator.CreateInstance<T>();
            return Add(Component);
        }

        public T Get<T>() where T : Component
        {
            Component Result;
            var Type = typeof(T);

            if (ComponentTypeCache.TryGetValue(Type, out Result))
            {
                return (T)Result;
            }

            foreach (var Component in Components)
            {
                if (Type.IsAssignableFrom(Component.GetType()))
                {
                    ComponentTypeCache.Add(Type, (T)Component);
                    return (T)Component;
                }
            }

            return null;
        }

        public T GetInterface<T>()
        {
            var Type = typeof(T);

            if (Type.IsInterface)
            {
                foreach (var Component in Components)
                {
                    if (Type.IsAssignableFrom(Component.GetType()))
                    {
                        return (T)(object)Component;
                    }
                }
            }

            return default(T);
        }

        public bool Remove<T>() where T : Component
        {
            var Component = Get<T>();

            if (null != Component)
            {
                Remove(Component);
                return true;
            }

            return false;
        }

        public bool Remove(Component component)
        {
            ComponentTypeCache.Remove(component.GetType());
            AddRemoveComponentInterface(false, component);
            Components.Remove(component);

            return true;
        }

        private void AddRemoveComponentInterface(bool add, Component component)
        {
            AddOrRemove(UpdateCompontents, component, add);
            AddOrRemove(DrawCompontents, component, add);
            AddOrRemove(InitializeCompontents, component, add);
            AddOrRemove(ContentCompontents, component, add);
            AddOrRemove(InteractiveCompontents, component, add);
            AddOrRemove(NotifyCompontents, component, add);
        }

        private void AddOrRemove<T>(List<T> list, Component component, bool add) where T : class
        {
            var item = component as T;
            if (null == item)
            {
                return;
            }

            if (add)
            {
                list.Add(item);
            }
            else
            {
                list.Remove(item);
            }
        }

        public IEnumerator<Component> GetEnumerator()
        {
            return ((IEnumerable<Component>)Components).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Component>)Components).GetEnumerator();
        }
    }
}
