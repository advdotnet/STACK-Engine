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

        [NonSerialized]
        List<Entity> _VisibleObjects = null;

        /// <summary>
        /// List of visible
        /// </summary>
        public List<Entity> VisibleObjects
        {
            get
            {
                if (_VisibleObjects == null)
                {
                    CacheVisibleObjects();
                }

                return _VisibleObjects;
            }
        }

        public void CacheVisibleObjects()
        {
            if (_VisibleObjects == null)
            {
                _VisibleObjects = new List<Entity>(20);
            }

            _VisibleObjects.Clear();

            if (Scene.World != null)
            {
                for (int i = 0; i < Scene.World.Scenes.Count; i++)
                {
                    var CurrentScene = Scene.World.Scenes[i];

                    for (int j = 0; j < CurrentScene.GameObjectCache.Entities.Count; j++)
                    {
                        var Entity = CurrentScene.GameObjectCache.Entities[j];
                        if (Entity.DrawScene == Scene && Entity.Visible) // || (Entity.DrawScene == null && Entity.UpdateScene == this)
                        {
                            _VisibleObjects.Add(Entity);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Entities.Count; i++)
                {
                    var entity = Entities[i];

                    if (entity.Visible && (entity.DrawScene == null || entity.DrawScene == Scene))
                    {
                        _VisibleObjects.Add(entity);
                    }
                }
            }

            _VisibleObjects.Sort(BaseEntityCollection.PrioritySorter);
        }

        [NonSerialized]
        List<Exit> _Exits = null;

        /// <summary>
        /// List of entities with an Exit component.
        /// </summary>
        public List<Exit> Exits
        {
            get
            {
                if (_Exits == null)
                {
                    CacheExits();
                }

                return _Exits;
            }
        }

        public void CacheExits()
        {
            if (_Exits == null)
            {
                _Exits = new List<Exit>(3);
            }

            _Exits.Clear();

            for (int i = 0; i < Entities.Count; i++)
            {
                var Exit = Entities[i].Get<Exit>();
                if (Exit != null)
                {
                    _Exits.Add(Exit);
                }
            }
        }

        [NonSerialized]
        List<IDraw> _ObjectsToDraw = null;

        /// <summary>
        /// List of Entities / Components to draw.
        /// </summary>
        public List<IDraw> ObjectsToDraw
        {
            get
            {
                if (_ObjectsToDraw == null)
                {
                    CacheObjectsToDraw();
                }

                return _ObjectsToDraw;
            }
        }

        public void CacheObjectsToDraw(bool onlySort = false)
        {
            if (_ObjectsToDraw == null)
            {
                _ObjectsToDraw = new List<IDraw>(15);
            }

            if (!onlySort)
            {
                ObjectsToDraw.Clear();

                // VisibleObjects list is priority sorted - add in reverse
                for (int i = VisibleObjects.Count - 1; i >= 0; i--)
                {
                    ObjectsToDraw.Add(VisibleObjects[i]);
                }

                for (int i = 0; i < Components.Count; i++)
                {
                    var DrawableComponent = Components[i] as IDraw;
                    if (null != DrawableComponent)
                    {
                        ObjectsToDraw.Add(DrawableComponent);
                    }
                }
            }

            ObjectsToDraw.Sort(BaseEntityCollection.ReversePrioritySorter);
        }

        [NonSerialized]
        List<Entity> _Entities = null;

        /// <summary>
        /// List of Entities of this Scene.
        /// </summary>
        public List<Entity> Entities
        {
            get
            {
                if (_Entities == null)
                {
                    CacheEntities();
                }

                return _Entities;
            }
        }

        public void CacheEntities()
        {
            if (_Entities == null)
            {
                _Entities = new List<Entity>(15);
            }

            _Entities.Clear();

            for (int i = 0; i < Scene.Items.Count; i++)
            {
                var Entity = Scene.Items[i] as Entity;
                if (Entity != null)
                {
                    _Entities.Add(Entity);
                }
            }
        }

        [NonSerialized]
        List<Component> _Components = null;

        /// <summary>
        /// List of Components of this Scene.
        /// </summary>
        public List<Component> Components
        {
            get
            {
                if (_Components == null)
                {
                    CacheComponents();
                }

                return _Components;
            }
        }

        public void CacheComponents()
        {
            if (_Components == null)
            {
                _Components = new List<Component>(5);
            }

            _Components.Clear();

            for (int i = 0; i < Scene.Components.Count; i++)
            {
                var Component = Scene.Components[i];
                _Components.Add(Component);
            }
        }

        public Entity GetEntityById(string id)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i].ID.Equals(id))
                {
                    return Entities[i];
                }
            }

            return null;
        }
    }
}
