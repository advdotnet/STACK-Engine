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
		private List<IUpdate> _updateCompontents;
		[NonSerialized]
		private List<IDraw> _drawCompontents;
		[NonSerialized]
		private List<IInitialize> _initializeCompontents;
		[NonSerialized]
		private List<IContent> _contentCompontents;
		[NonSerialized]
		private List<IInteractive> _interactiveCompontents;
		[NonSerialized]
		private List<INotify> _notifyCompontents;

		public List<IUpdate> UpdateCompontents => _updateCompontents;
		public List<IDraw> DrawCompontents => _drawCompontents;
		public List<IInitialize> InitializeCompontents => _initializeCompontents;
		public List<IContent> ContentCompontents => _contentCompontents;
		public List<IInteractive> InteractiveCompontents => _interactiveCompontents;
		public List<INotify> NotifyCompontents => _notifyCompontents;

		private void Cache()
		{
			_updateCompontents = new List<IUpdate>();
			_drawCompontents = new List<IDraw>();
			_initializeCompontents = new List<IInitialize>();
			_contentCompontents = new List<IContent>();
			_interactiveCompontents = new List<IInteractive>();
			_notifyCompontents = new List<INotify>();

			foreach (var component in Components)
			{
				AddRemoveComponentInterface(true, component);
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
		private void OnDeserialized(StreamingContext c)
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
			_updateCompontents = new List<IUpdate>();
			_drawCompontents = new List<IDraw>();
			_initializeCompontents = new List<IInitialize>();
			_contentCompontents = new List<IContent>();
			_interactiveCompontents = new List<IInteractive>();
			_notifyCompontents = new List<INotify>();
		}

		public Component this[int index] => Components[index];
		public int Count => Components.Count;

		private T Add<T>(T component) where T : Component
		{
			var componentType = component.GetType();
			if (ComponentTypeCache.ContainsKey(componentType))
			{
				throw new InvalidOperationException("Component type" + componentType.Name + " already added.");
			}

			Components.Add(component);
			AddRemoveComponentInterface(true, component);
			ComponentTypeCache.Add(componentType, component);

			return component;
		}

		public T Add<T>() where T : Component
		{
			var component = Activator.CreateInstance<T>();
			return Add(component);
		}

		public T Get<T>() where T : Component
		{
			var type = typeof(T);

			if (ComponentTypeCache.TryGetValue(type, out var result))
			{
				return (T)result;
			}

			foreach (var component in Components)
			{
				if (type.IsAssignableFrom(component.GetType()))
				{
					ComponentTypeCache.Add(type, (T)component);
					return (T)component;
				}
			}

			return null;
		}

		public T GetInterface<T>()
		{
			var type = typeof(T);

			if (type.IsInterface)
			{
				foreach (var component in Components)
				{
					if (type.IsAssignableFrom(component.GetType()))
					{
						return (T)(object)component;
					}
				}
			}

			return default;
		}

		public bool Remove<T>() where T : Component
		{
			var component = Get<T>();

			if (null != component)
			{
				Remove(component);
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
			if (!(component is T item))
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

		public IEnumerator<Component> GetEnumerator() => ((IEnumerable<Component>)Components).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Component>)Components).GetEnumerator();
	}
}
