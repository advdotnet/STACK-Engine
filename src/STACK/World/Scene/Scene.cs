using Microsoft.Xna.Framework;
using STACK.Components;
using STACK.Graphics;
using STACK.Input;
using StarFinder;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace STACK
{
	/// <summary>
	/// A scene represents a base node in the game hierarchy containing game objects.
	/// </summary>   
	[Serializable]
	public class Scene : BaseEntityCollection, IMapPosition
	{
		[NonSerialized]
		private World _world = null;
		[NonSerialized]
		private ContentLoader _content;
		public ContentLoader Content => _content;

		[NonSerialized]
		private GameObjectCache _gameObjectCache;

		public GameObjectCache GameObjectCache => _gameObjectCache;

		public Scene()
		{
			_gameObjectCache = new GameObjectCache(this);

			Enabled = false;
			Visible = false;

			Camera
				.Create(this);
		}

		public Scene(string id) : this()
		{
			if (!string.IsNullOrEmpty(id))
			{
				_ID = id;
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext c)
		{
			_gameObjectCache = new GameObjectCache(this);
			_world = (World)Parent;
		}

		/// <summary>
		/// Gets a list of Portals to the given neighboring scene.
		/// </summary>
		public void GetPassagesTo(string sceneID, ref List<Exit> exits)
		{
			exits.Clear();

			var targetScene = World[sceneID];

			if (targetScene == null)
			{
				return;
			}

			GetPassagesTo(targetScene, ref exits);
		}

		/// <summary>
		/// Gets a list of exits to the given neighboring scene.
		/// </summary>
		public void GetPassagesTo(Scene scene, ref List<Exit> exits)
		{
			exits.Clear();

			for (var i = 0; i < GameObjectCache.Exits.Count; i++)
			{
				var gameObject = World.GetGameObject(GameObjectCache.Exits[i].TargetEntrance);
				if (gameObject != null && gameObject.DrawScene == scene)
				{
					exits.Add(GameObjectCache.Exits[i]);
				}
			}
		}

		/// <summary>
		/// The World which this scene is assigned to.
		/// </summary>
		public World World
		{
			get => _world;

			set
			{
				Parent = value;
				_world = value;
			}
		}

		public override void OnLoadContent(ContentLoader content)
		{
			if (World.Get<ServiceProvider>().Provider != null && _content == null)
			{
				_content = new ContentLoader(World.Get<ServiceProvider>().Provider, EngineVariables.ContentPath);
			}

			base.OnLoadContent(content);
		}

		public override void OnHandleInputEvent(Vector2 mouse, InputEvent inputEvent)
		{
			base.OnHandleInputEvent(Get<Camera>().Transform(World.Get<Mouse>().Position), inputEvent);
		}

		public override void OnInitialize(bool restore)
		{
			base.OnInitialize(restore);

			GameObjectCache.CacheAll();
		}

		/// <summary>
		/// Draws all GameObjects if the Scene is visible.
		/// </summary>  
		public override void OnBeginDraw(Renderer renderer)
		{
			renderer.Begin(Get<Camera>().Transformation * World.Get<Camera>().Transformation);
		}

		public override void OnDraw(Renderer renderer)
		{
			for (var i = 0; i < GameObjectCache.ObjectsToDraw.Count; i++)
			{
				GameObjectCache.ObjectsToDraw[i].Draw(renderer);
			}
		}

		public override void OnEndDraw(Renderer renderer)
		{
			renderer.End();
		}

		public override void OnNotify<T>(string message, T data)
		{
			// notify drawn entities, too

			if (World != null)
			{
				for (var i = 0; i < World.Scenes.Count; i++)
				{
					var currentScene = World.Scenes[i];
					for (var j = 0; j < currentScene.GameObjectCache.Entities.Count; j++)
					{
						var currentEntity = currentScene.GameObjectCache.Entities[j];
						if (currentEntity.DrawScene == this || (currentEntity.DrawScene == null && currentEntity.UpdateScene == this))
						{
							currentEntity.Notify(message, data);
						}
					}
				}
			}
			else
			{
				base.OnNotify(message, data);
			}
		}

		/// <summary>
		/// Returns enabled and interactive GameObjects which collide with the given position.
		/// </summary>
		public Entity GetHitObject(Vector2 position)
		{
			var transformedPosition = Get<Camera>().TransformInverse(position);

			for (var i = 0; i < GameObjectCache.VisibleObjects.Count; i++)
			{
				var entity = GameObjectCache.VisibleObjects[i];

				if (entity.Enabled)
				{
					var hotspot = entity.Get<Hotspot>();
					if (hotspot != null)
					{
						var transform = entity.Get<Transform>();
						var useOriginalPosition = (transform != null && transform.Absolute);

						if (hotspot.IsHit(useOriginalPosition ? position : transformedPosition) && entity.DrawScene == this)
						{
							return entity;
						}
					}
				}
			}

			return null;
		}

		public override void OnUnloadContent()
		{
			base.OnUnloadContent();

			Content?.Unload();
		}

		public override void OnPropertyChanged(string property)
		{
			if (property == Properties.DrawOrder && World != null)
			{
				World.UpdatePriority();
			}
		}

		/// <summary>
		/// Pushes an Entity onto this scene.
		/// </summary>        
		public void Push(Entity entity)
		{
			Items.Add(entity);

			entity.UpdateScene = this;
			entity.DrawScene = entity.DrawScene ?? this;

			if (Loaded)
			{
				entity.OnLoadContent(Content);
			}

			if (Initialized)
			{
				GameObjectCache.CacheAll();
			}
		}

		/// <summary>
		/// IMapPosition implementation for pathfinding across scenes.
		/// </summary>
		public bool Equals(IMapPosition b)
		{
			return (b == this);
		}

		public float Cost(IMapPosition parent)
		{
			return (parent == null) ? 0 : 1;
		}

		public override void OnChangeComponents()
		{
			GameObjectCache.CacheComponents();
			GameObjectCache.CacheObjectsToDraw();
		}

		/// <summary>
		/// Returns an entity with the given ID.
		/// </summary>
		public Entity GetObject(string id)
		{
			return GameObjectCache.GetEntityById(id, Initialized);
		}

		public Entity this[string id] => GetObject(id);

		/// <summary>
		/// Pushes an array of GameObjects onto this scene.
		/// </summary>        
		public void Push(params Entity[] gameObjects)
		{
			foreach (var @object in gameObjects)
			{
				Push(@object);
			}
		}

		/// <summary>
		/// Unloads and removes the given GameObject.
		/// </summary> 
		public void Pop(Entity gameObject)
		{
			gameObject.OnUnloadContent();
			Items.Remove(gameObject);

			if (Initialized)
			{
				GameObjectCache.CacheAll();
			}

			World?.InvalidateEntityIDCache(gameObject);
		}
	}
}
