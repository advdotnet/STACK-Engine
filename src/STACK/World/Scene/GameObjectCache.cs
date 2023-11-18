using STACK.Components;
using System;
using System.Collections.Generic;

namespace STACK
{
	/// <summary>
	/// Stores several lists of game objects for performance reasons.
	/// </summary>   
	[Serializable]
	public class GameObjectCache
	{
		private Scene Scene { get; set; }
		[NonSerialized]
		private List<Entity> _visibleObjects = null;
		[NonSerialized]
		private List<Exit> _exits = null;
		[NonSerialized]
		private List<IDraw> _objectsToDraw = null;
		[NonSerialized]
		private List<Entity> _entities = null;
		[NonSerialized]
		private List<Component> _components = null;

		public GameObjectCache(Scene scene)
		{
			Scene = scene;
		}

		public void CacheAll()
		{
			CacheEntities();
			CacheVisibleObjects();
			CacheObjectsToDraw();
			CacheExits();
		}

		/// <summary>
		/// List of visible
		/// </summary>
		public List<Entity> VisibleObjects
		{
			get
			{
				if (_visibleObjects == null)
				{
					CacheVisibleObjects();
				}

				return _visibleObjects;
			}
		}

		public void CacheVisibleObjects()
		{
			if (_visibleObjects == null)
			{
				_visibleObjects = new List<Entity>(20);
			}

			_visibleObjects.Clear();

			if (Scene.World != null)
			{
				for (var i = 0; i < Scene.World.Scenes.Count; i++)
				{
					var currentScene = Scene.World.Scenes[i];

					for (var j = 0; j < currentScene.GameObjectCache.Entities.Count; j++)
					{
						var entity = currentScene.GameObjectCache.Entities[j];
						if (entity.DrawScene == Scene && entity.Visible) // || (Entity.DrawScene == null && Entity.UpdateScene == this)
						{
							_visibleObjects.Add(entity);
						}
					}
				}
			}
			else
			{
				for (var i = 0; i < Entities.Count; i++)
				{
					var entity = Entities[i];

					if (entity.Visible && (entity.DrawScene == null || entity.DrawScene == Scene))
					{
						_visibleObjects.Add(entity);
					}
				}
			}

			_visibleObjects.Sort(BaseEntityCollection.PrioritySorter);
		}

		/// <summary>
		/// List of entities with an Exit component.
		/// </summary>
		public List<Exit> Exits
		{
			get
			{
				if (_exits == null)
				{
					CacheExits();
				}

				return _exits;
			}
		}

		public void CacheExits()
		{
			if (_exits == null)
			{
				_exits = new List<Exit>(3);
			}

			_exits.Clear();

			for (var i = 0; i < Entities.Count; i++)
			{
				var exit = Entities[i].Get<Exit>();
				if (exit != null)
				{
					_exits.Add(exit);
				}
			}
		}

		/// <summary>
		/// List of Entities / Components to draw.
		/// </summary>
		public List<IDraw> ObjectsToDraw
		{
			get
			{
				if (_objectsToDraw == null)
				{
					CacheObjectsToDraw();
				}

				return _objectsToDraw;
			}
		}

		public void CacheObjectsToDraw(bool onlySort = false)
		{
			if (_objectsToDraw == null)
			{
				_objectsToDraw = new List<IDraw>(15);
			}

			if (!onlySort)
			{
				ObjectsToDraw.Clear();

				// VisibleObjects list is priority sorted - add in reverse
				for (var i = VisibleObjects.Count - 1; i >= 0; i--)
				{
					ObjectsToDraw.Add(VisibleObjects[i]);
				}

				for (var i = 0; i < Components.Count; i++)
				{
					if (Components[i] is IDraw drawableComponent)
					{
						ObjectsToDraw.Add(drawableComponent);
					}
				}
			}

			ObjectsToDraw.Sort(BaseEntityCollection.ReversePrioritySorter);
		}

		/// <summary>
		/// List of Entities of this Scene.
		/// </summary>
		public List<Entity> Entities
		{
			get
			{
				if (_entities == null)
				{
					CacheEntities();
				}

				return _entities;
			}
		}

		public void CacheEntities()
		{
			if (_entities == null)
			{
				_entities = new List<Entity>(15);
			}

			_entities.Clear();

			for (var i = 0; i < Scene.Items.Count; i++)
			{
				if (Scene.Items[i] is Entity entity)
				{
					_entities.Add(entity);
				}
			}
		}

		/// <summary>
		/// List of Components of this Scene.
		/// </summary>
		public List<Component> Components
		{
			get
			{
				if (_components == null)
				{
					CacheComponents();
				}

				return _components;
			}
		}

		public void CacheComponents()
		{
			if (_components == null)
			{
				_components = new List<Component>(5);
			}

			_components.Clear();

			for (var i = 0; i < Scene.Components.Count; i++)
			{
				var component = Scene.Components[i];
				_components.Add(component);
			}
		}

		public Entity GetEntityById(string id, bool initialized)
		{
			if (initialized)
			{
				for (var i = 0; i < Entities.Count; i++)
				{
					if (Entities[i].ID.Equals(id))
					{
						return Entities[i];
					}
				}
			}
			else
			{
				for (var i = 0; i < Scene.Items.Count; i++)
				{
					if (Scene.Items[i] is Entity entity && entity.ID.Equals(id))
					{
						return entity;
					}
				}
			}

			return null;
		}
	}
}
