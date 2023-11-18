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

		public bool Initialized => _initialized;
		public bool Loaded => _loaded;
		public string ID => _ID;

		protected string _ID = string.Empty;
		private float _drawOrder = 0;
		private float _updateOrder = 0;
		private bool _interactive = true;
		private bool _enabled = true;
		private bool _visible = true;
		[NonSerialized]
		private bool _initialized = false;
		[NonSerialized]
		private bool _loaded = false;

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
			get => _interactive;
			set
			{
				var changed = _interactive != value;
				if (changed)
				{
					_interactive = value;
					OnPropertyChanged(Properties.Interactive);
				}
			}
		}

		public bool Enabled
		{
			get => _enabled;
			set
			{
				var changed = _enabled != value;
				if (changed)
				{
					_enabled = value;
					OnPropertyChanged(Properties.Enabled);
				}
			}
		}

		public bool Visible
		{
			get => _visible;
			set
			{
				var changed = _visible != value;
				if (changed)
				{
					_visible = value;
					OnPropertyChanged(Properties.Visible);
				}
			}
		}

		public float DrawOrder
		{
			get => _drawOrder;
			set
			{
				var changed = _drawOrder != value;
				if (changed)
				{
					_drawOrder = value;
					OnPropertyChanged(Properties.DrawOrder);
				}
			}
		}

		public float UpdateOrder
		{
			get => _updateOrder;
			set
			{
				var changed = _updateOrder != value;
				if (changed)
				{
					_updateOrder = value;
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
			_loaded = true;
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
			_loaded = false;
		}

		/// <summary>
		/// Called after the content has been loaded. Can be used to programatically create additional resources
		/// or to cache some components.
		/// </summary>
		/// <param name="restore"></param>
		public void Initialize(bool restore)
		{
			OnInitialize(restore);
			_initialized = true;
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