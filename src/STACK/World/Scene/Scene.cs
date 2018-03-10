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
        ContentLoader _Content;
        public ContentLoader Content { get { return _Content; } }

        [NonSerialized]
        GameObjectCache _GameObjectCache;

        public GameObjectCache GameObjectCache
        {
            get
            {
                return _GameObjectCache;
            }
        }

        public Scene()
        {
            _GameObjectCache = new GameObjectCache(this);

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
        void OnDeserialized(StreamingContext c)
        {
            _GameObjectCache = new GameObjectCache(this);
            _World = (World)Parent;
        }

        /// <summary>
        /// Gets a list of Portals to the given neighboring scene.
        /// </summary>
        public void GetPassagesTo(string sceneID, ref List<Exit> exits)
        {
            exits.Clear();

            var TargetScene = World[sceneID];

            if (TargetScene == null)
            {
                return;
            }

            GetPassagesTo(TargetScene, ref exits);
        }

        /// <summary>
        /// Gets a list of exits to the given neighboring scene.
        /// </summary>
        public void GetPassagesTo(Scene scene, ref List<Exit> exits)
        {
            exits.Clear();

            for (int i = 0; i < GameObjectCache.Exits.Count; i++)
            {
                var GameObject = World.GetGameObject(GameObjectCache.Exits[i].TargetEntrance);
                if (GameObject != null && GameObject.DrawScene == scene)
                {
                    exits.Add(GameObjectCache.Exits[i]);
                }
            }
        }

        [NonSerialized]
        World _World = null;

        /// <summary>
        /// The World which this scene is assigned to.
        /// </summary>
        public World World
        {
            get
            {
                return _World;
            }

            set
            {
                Parent = value;
                _World = value;
            }
        }

        public override void OnLoadContent(ContentLoader content)
        {
            if (World.Get<ServiceProvider>().Provider != null && _Content == null)
            {
                _Content = new ContentLoader(World.Get<ServiceProvider>().Provider, EngineVariables.ContentPath);
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
            for (int i = 0; i < GameObjectCache.ObjectsToDraw.Count; i++)
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
                for (int i = 0; i < World.Scenes.Count; i++)
                {
                    var CurrentScene = World.Scenes[i];
                    for (int j = 0; j < CurrentScene.GameObjectCache.Entities.Count; j++)
                    {
                        var CurrentEntity = CurrentScene.GameObjectCache.Entities[j];
                        if (CurrentEntity.DrawScene == this || (CurrentEntity.DrawScene == null && CurrentEntity.UpdateScene == this))
                        {
                            CurrentEntity.Notify(message, data);
                        }
                    }
                }
            }
            else
            {
                base.OnNotify<T>(message, data);
            }
        }

        /// <summary>
        /// Returns enabled and interactive GameObjects which collide with the given position.
        /// </summary>
        public Entity GetHitObject(Vector2 position)
        {
            var TransformedPosition = Get<Camera>().TransformInverse(position);

            for (int i = 0; i < GameObjectCache.VisibleObjects.Count; i++)
            {
                var Entity = GameObjectCache.VisibleObjects[i];

                if (Entity.Enabled)
                {
                    var Hotspot = Entity.Get<Hotspot>();
                    if (Hotspot != null)
                    {
                        var Transform = Entity.Get<Transform>();
                        var UseOriginalPosition = (Transform != null && Transform.Absolute);

                        if (Hotspot.IsHit(UseOriginalPosition ? position : TransformedPosition) && Entity.DrawScene == this)
                        {
                            return Entity;
                        }
                    }
                }
            }

            return null;
        }


        public override void OnUnloadContent()
        {
            base.OnUnloadContent();

            if (Content != null)
            {
                Content.Unload();
            }
        }

        public override void OnPropertyChanged(string property)
        {
            if (property == BaseEntity.Properties.DrawOrder && World != null)
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
                entity.OnLoadContent(this.Content);
            }

            if (Initialized)
            {
                GameObjectCache.CacheAll();
            }
        }

        /// <summary>
        /// IMapPosition implementation for pathfinding across scenes.
        /// </summary>
        public bool Equals(StarFinder.IMapPosition b)
        {
            return (b == this);
        }

        public float Cost(StarFinder.IMapPosition parent)
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
            return GameObjectCache.GetEntityById(id);
        }

        public Entity this[string id]
        {
            get
            {
                return GetObject(id);
            }
        }

        /// <summary>
        /// Pushes an array of GameObjects onto this scene.
        /// </summary>        
        public void Push(params Entity[] gameObjects)
        {
            foreach (var Object in gameObjects)
            {
                Push(Object);
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
        }
    }
}
