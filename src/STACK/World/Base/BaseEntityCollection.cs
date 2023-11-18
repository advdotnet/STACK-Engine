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
		private List<BaseEntity> _items;
		internal List<BaseEntity> Items => _items;

		private ComponentList _components;
		internal ComponentList Components => _components;

		public BaseEntityCollection()
		{
			_items = new List<BaseEntity>(5);
			_components = new ComponentList();
		}

		[OnSerializing]
		public void OnSerializing(StreamingContext context)
		{
			if (null != _items && _items.Count == 0)
			{
				_items = null;
			}

			if (null != _components && _components.Count == 0)
			{
				_components = null;
			}
		}

		[OnSerialized]
		public void OnSerialized(StreamingContext context)
		{
			if (null == _items)
			{
				_items = new List<BaseEntity>(5);
			}

			if (null == _components)
			{
				_components = new ComponentList();
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext c)
		{
			OnSerialized(c);
		}

		public T Add<T>() where T : Component
		{
			var component = Components.Add<T>();

			component.Parent = this;
			if (Initialized)
			{
				OnChangeComponents();
			}

			return component;
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
			var removed = Components.Remove<T>();

			if (removed)
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
			for (var i = 0; i < Components.UpdateCompontents.Count; i++)
			{
				var updateComponent = Components.UpdateCompontents[i];
				if (updateComponent.Enabled)
				{
					updateComponent.Update();
				}

			}

			for (var i = 0; i < Items.Count; i++)
			{
				Items[i].Update();
			}
		}

		public override void OnDraw(Renderer renderer)
		{
			for (var i = 0; i < Components.DrawCompontents.Count; i++)
			{
				var drawComponent = Components.DrawCompontents[i];
				if (drawComponent.Visible)
				{
					drawComponent.Draw(renderer);
				}
			}

			for (var i = 0; i < Items.Count; i++)
			{
				Items[i].Draw(renderer);
			}
		}

		public override void OnNotify<T>(string message, T data)
		{
			for (var i = 0; i < Components.NotifyCompontents.Count; i++)
			{
				Components.NotifyCompontents[i].Notify(message, data);
			}

			for (var i = 0; i < Items.Count; i++)
			{
				Items[i].Notify(message, data);
			}
		}

		public override void OnLoadContent(ContentLoader content)
		{
			for (var i = 0; i < Components.ContentCompontents.Count; i++)
			{
				Components.ContentCompontents[i].LoadContent(content);
			}

			for (var i = 0; i < Items.Count; i++)
			{
				Items[i].LoadContent(content);
			}
		}

		public override void OnUnloadContent()
		{
			for (var i = 0; i < Components.ContentCompontents.Count; i++)
			{
				Components.ContentCompontents[i].UnloadContent();
			}

			for (var i = 0; i < Items.Count; i++)
			{
				Items[i].UnloadContent();
			}
		}

		public override void OnHandleInputEvent(Vector2 mouse, InputEvent inputEvent)
		{
			for (var i = 0; i < Components.InteractiveCompontents.Count; i++)
			{
				Components.InteractiveCompontents[i].HandleInputEvent(mouse, inputEvent);
			}

			for (var i = 0; i < Items.Count; i++)
			{
				Items[i].HandleInputEvent(mouse, inputEvent);
			}
		}

		public override void OnInitialize(bool restore)
		{
			for (var i = 0; i < Components.InitializeCompontents.Count; i++)
			{
				Components.InitializeCompontents[i].Initialize(restore);
			}

			for (var i = 0; i < Items.Count; i++)
			{
				Items[i].Initialize(restore);
			}
		}

		public virtual void OnChangeComponents()
		{

		}
	}
}
