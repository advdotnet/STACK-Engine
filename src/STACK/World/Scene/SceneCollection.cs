using Microsoft.Xna.Framework;
using STACK.Logging;
using StarFinder;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace STACK
{
	/// <summary>
	/// </summary>
	[Serializable]
	public abstract class SceneCollection : BaseEntityCollection
	{
		[NonSerialized]
		private readonly AStar<Scene> _sceneFinder;
		[NonSerialized]
		internal List<Scene> _Scenes = null;
		[NonSerialized]
		private List<Scene> _findPathResult = new List<Scene>();
		[NonSerialized]
		protected Dictionary<string, Entity> EntityIDCache = new Dictionary<string, Entity>();

		public List<Scene> Scenes
		{
			get
			{
				if (_Scenes == null)
				{
					CacheScenes();
				}

				return _Scenes;
			}
		}

		public SceneCollection()
		{
			_sceneFinder = new AStar<Scene>(GetSceneNeighbors);
		}

		/// <summary>
		/// Returns the neighbors of given scene by getting the target scenes of its portals.
		/// </summary>
		internal List<Scene> GetSceneNeighbors(Scene scene)
		{
			var result = new List<Scene>();

			for (var i = 0; i < scene.GameObjectCache.Exits.Count; i++)
			{
				if (!string.IsNullOrEmpty(scene.GameObjectCache.Exits[i].TargetEntrance))
				{
					result.Add(GetGameObject(scene.GameObjectCache.Exits[i].TargetEntrance).DrawScene);
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the subsequent IDs of all scenes between source and target scene.
		/// If there is no path, null is returned.
		/// </summary>
		public void FindPath(string from, string to, ref List<string> result)
		{
			FindPath(GetScene(from), GetScene(to), ref result);
		}

		/// <summary>
		/// Returns the subsequent IDs of all scenes between source and target scene.
		/// If there is no path, an empty list is returned.
		/// </summary>
		public void FindPath(Scene from, Scene to, ref List<string> result)
		{
			result.Clear();

			_sceneFinder.Search(from, to, ref _findPathResult);

			if (_findPathResult.Count == 0)
			{
				return;
			}

			for (var i = 0; i < _findPathResult.Count; i++)
			{
				result.Add(_findPathResult[i].ID);
			}
		}

		/// <summary>
		/// Returns the GameObject with the given ID.
		/// </summary>
		public Entity GetGameObject(string id)
		{

			if (EntityIDCache.TryGetValue(id, out var result))
			{
				return result;
			}

			for (var i = 0; i < Scenes.Count; i++)
			{
				result = Scenes[i].GetObject(id);
				if (result != null)
				{
					EntityIDCache.Add(id, result);

					return result;
				}
			}

			return null;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext c)
		{
			EntityIDCache = new Dictionary<string, Entity>();
		}

		public void InvalidateEntityIDCache(Entity entity)
		{
			if (null != entity)
			{
				EntityIDCache.Remove(entity.ID);
			}
		}

		/// <summary>
		/// Returns the scene with the given ID or null.
		/// </summary>
		public Scene this[string id] => GetScene(id);

		/// <summary>
		/// Returns the scene with the given ID or null.
		/// </summary>
		public Scene GetScene(string id)
		{
			for (var i = 0; i < Scenes.Count; i++)
			{
				if (Scenes[i].ID == id)
				{
					return Scenes[i];
				}
			}

			return null;
		}

		public void UpdatePriority()
		{
			Items.Sort(PrioritySorter);
			Scenes.Sort(PrioritySorter);
		}

		public override void OnInitialize(bool restore)
		{
			UpdatePriority();
			base.OnInitialize(restore);
		}

		/// <summary>
		/// Pushes an array of scenes.
		/// </summary>        
		public void Push(params BaseEntity[] scenes)
		{
			foreach (var scene in scenes)
			{
				Push(scene);
			}
		}

		/// <summary>
		/// Pushes a scene and sets its manager to this instance.
		/// </summary
		public virtual bool Push(BaseEntity scene)
		{
			if (GetScene(scene.ID) != null)
			{
				throw new ArgumentException("Scene with same ID has already been added.");
			}

			if (!Items.Contains(scene))
			{
				Log.WriteLine("Adding Scene " + scene.ID);
				Items.Add(scene);
				CacheScenes();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Unloads and removes a scene.
		/// </summary>        
		public virtual void Pop(Scene scene)
		{
			if (Items.Contains(scene))
			{
				scene.UnloadContent();
				Items.Remove(scene);
				CacheScenes();
			}
		}

		private void CacheScenes()
		{
			if (_Scenes == null)
			{
				_Scenes = new List<Scene>(Items.Count);
			}

			_Scenes.Clear();

			for (var i = 0; i < Items.Count; i++)
			{
				if (Items[i] is Scene scene)
				{
					_Scenes.Add(scene);
				}
			}

			_Scenes.Sort(PrioritySorter);
		}

		/// <summary>
		/// Gets the first object at the given position
		/// </summary>
		public Entity GetObjectAtPosition(Vector2 position)
		{
			Entity pick;

			for (var i = 0; i < Scenes.Count; i++)
			{
				if (Scenes[i].Enabled && Scenes[i].Interactive)
				{
					pick = Scenes[i].GetHitObject(position);

					if (pick != null)
					{
						return pick;
					}
				}
			}

			return null;
		}
	}
}
