using Microsoft.Xna.Framework;
using STACK.Components;
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
        AStar<Scene> _SceneFinder;
        [NonSerialized]
        internal List<Scene> _Scenes = null;

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
            _SceneFinder = new AStar<Scene>(GetSceneNeighbors);
        }

        /// <summary>
        /// Returns the neighbors of given scene by getting the target scenes of its portals.
        /// </summary>
        internal List<Scene> GetSceneNeighbors(Scene scene)
        {
            var Result = new List<Scene>();

            for (int i = 0; i < scene.GameObjectCache.Exits.Count; i++)
            {
                if (!string.IsNullOrEmpty(scene.GameObjectCache.Exits[i].TargetEntrance))
                {
                    Result.Add(GetGameObject(scene.GameObjectCache.Exits[i].TargetEntrance).DrawScene);
                }
            }

            return Result;
        }

        /// <summary>
        /// Returns the subsequent IDs of all scenes between source and target scene.
        /// If there is no path, null is returned.
        /// </summary>
        public void FindPath(string from, string to, ref List<string> result)
        {
            FindPath(this.GetScene(from), this.GetScene(to), ref result);
        }

        [NonSerialized]
        List<Scene> FindPathResult = new List<Scene>();

        /// <summary>
        /// Returns the subsequent IDs of all scenes between source and target scene.
        /// If there is no path, an empty list is returned.
        /// </summary>
        public void FindPath(Scene from, Scene to, ref List<string> result)
        {
            result.Clear();

            _SceneFinder.Search(from, to, ref FindPathResult);

            if (FindPathResult.Count == 0)
            {
                return;
            }

            for (int i = 0; i < FindPathResult.Count; i++)
            {
                result.Add(FindPathResult[i].ID);
            }
        }

        /// <summary>
        /// Returns the GameObject with the given ID.
        /// </summary>
        public Entity GetGameObject(string id)
        {
            Entity Result;

            if (EntityIDCache.TryGetValue(id, out Result))
            {
                return Result;
            }

            for (int i = 0; i < Scenes.Count; i++)
            {
                Result = Scenes[i].GetObject(id);
                if (Result != null)
                {
                    EntityIDCache.Add(id, Result);

                    return Result;
                }
            }

            return null;
        }

        [NonSerialized]
        protected Dictionary<string, Entity> EntityIDCache = new Dictionary<string, Entity>();

        [OnDeserialized]
        void OnDeserialized(StreamingContext c)
        {
            EntityIDCache = new Dictionary<string, Entity>();
        }

        /// <summary>
        /// Returns the scene with the given ID or null.
        /// </summary>
        public Scene this[string id]
        {
            get
            {
                return GetScene(id);
            }
        }

        /// <summary>
        /// Returns the scene with the given ID or null.
        /// </summary>
        public Scene GetScene(string id)
        {
            for (int i = 0; i < Scenes.Count; i++)
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

        public override void OnInitialize()
        {
            UpdatePriority();
            base.OnInitialize();
        }

        /// <summary>
        /// Pushes an array of scenes.
        /// </summary>        
        public void Push(params BaseEntity[] scenes)
        {
            foreach (var Scene in scenes)
            {
                Push(Scene);
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

                if (scene is Component)
                {
                    Add((Component)scene);
                }
                else
                {
                    Items.Add(scene);
                    CacheScenes();
                }

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

            for (int i = 0; i < Items.Count; i++)
            {
                var Scene = Items[i] as Scene;
                if (Scene != null)
                {
                    _Scenes.Add(Scene);
                }
            }

            _Scenes.Sort(PrioritySorter);
        }

        /// <summary>
        /// Gets the first object at the given position
        /// </summary>
        public Entity GetObjectAtPosition(Vector2 position)
        {
            Entity Pick;

            for (int i = 0; i < Scenes.Count; i++)
            {
                if (Scenes[i].Enabled && Scenes[i].Interactive)
                {
                    Pick = Scenes[i].GetHitObject(position);

                    if (Pick != null)
                    {
                        return Pick;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Restores a snapshot of a list of scenes from the given file.
        /// </summary>        
        protected void RestoreState(List<BaseEntity> scenes, IServiceProvider provider, ContentLoader content)
        {
            Items.Clear();
            Scenes.Clear();
            ComponentCache.Clear();

            foreach (var Scene in scenes)
            {
                Push(Scene);
            }

            Get<ServiceProvider>().SetProvider(provider);
            Get<SkipContent>().SetInterfaceFromServiceProvider(provider);

            LoadContent(content);
        }
    }
}
