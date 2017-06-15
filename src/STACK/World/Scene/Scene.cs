using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using STACK.Graphics;
using STACK.Input;
using STACK.Components;
using StarFinder;

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

        public Scene()
        {
            Enabled = false;
            Visible = false;

            Camera
                .Create(this);			
        }

        public Scene(string id) : this()
        {
            if (!string.IsNullOrEmpty(id))
            {
                ID = id;
            }
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

            for (int i = 0; i < Exits.Count; i++)
            {
                var GameObject = World.GetGameObject(Exits[i].TargetEntrance);
                if (GameObject != null && GameObject.DrawScene == scene)
                {
                    exits.Add(Exits[i]);
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
                return _World ?? (_World = (World)Parent); 
            }

            set
            {
                Parent = value;
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

        public override void OnInitialize()
        {
            if (World != null)
            {
                World.UpdatePriority();
            }

            base.OnInitialize();
        }        

        /// <summary>
        /// Draws all GameObjects if the Scene is visible.
        /// </summary>  
        public override void OnBeginDraw(Renderer renderer)
        {
            renderer.Begin(Get<Camera>().Transformation * World.Get<Camera>().Transformation);
        }

        [NonSerialized] 
        List<BaseEntity> ObjectsToDraw = null;
        
        public override void OnDraw(Renderer renderer)
        {
            if (ObjectsToDraw == null)
            {
                ObjectsToDraw = new List<BaseEntity>(15);
            }

            ObjectsToDraw.Clear();

			VisibleObjects.Cache();            

            for (int i = 0; i < VisibleObjects.Count; i++)
            {
                ObjectsToDraw.Add(VisibleObjects[i]);
            }

            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Visible)
                {
                    var Component = Items[i] as Component;
                    if (Component != null)
                    {
                        ObjectsToDraw.Add(Component);
                    }
                }
            }            

            ObjectsToDraw.Sort(BaseEntityCollection.ReversePrioritySorter);

            for (int i = 0; i < ObjectsToDraw.Count; i++)
            {
                ObjectsToDraw[i].Draw(renderer);
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
                    for (int j = 0; j < CurrentScene.Entities.Count; j++)
                    {
                        var CurrentEntity = CurrentScene.Entities[j];
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

            for (int i = 0; i < VisibleObjects.Count; i++)
            {
                var Entity = VisibleObjects[i];

                if (Entity.Enabled)
                {
                    var Hotspot = Entity.Get<Hotspot>();
                    if (Hotspot != null)
                    {						
						var Transform = Entity.Get<Transform>();
						var UseOriginalPosition = (Transform != null && Transform.Absolute);

						if (Hotspot.Enabled && Hotspot.IsHit(UseOriginalPosition ? position : TransformedPosition) && Entity.DrawScene == this)
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
            if (property == BaseEntity.Properties.Priority && World != null)
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
            entity.DrawScene = this;            

            if (Loaded)
            {                
                entity.OnLoadContent(this.Content);
            }

            CacheEntities();
			VisibleObjects.Cache();            
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

        [NonSerialized]
        List<Entity> _Entities = null;

        /// <summary>
        /// List of GameObjects.
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

        private void CacheEntities()
        {
            if (_Entities == null)
            {
                _Entities = new List<Entity>(15);
            }

            _Entities.Clear();

            for (int i = 0; i < Items.Count; i++)
            {
                var Entity = Items[i] as Entity;
                if (Entity != null)
                {
                    _Entities.Add(Entity);
                }
            }
        }

        [NonSerialized]
        List<Exit> _Exits = null;

        /// <summary>
        /// Gets a list of Exits.
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

        private void CacheExits()
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

        /// <summary>
        /// Returns an entity with the given ID.
        /// </summary>
        public Entity GetObject(string id)
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

        public Entity this[string id]
        {
            get
            {
                return GetObject(id);
            }
        }

        [NonSerialized]
        VisibleObjectList _VisibleObjects = null;

		internal VisibleObjectList VisibleObjects
        {
            get
            {
                if (_VisibleObjects == null)
                {
					_VisibleObjects = new VisibleObjectList(this); 				
                }

                return _VisibleObjects;
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
            CacheEntities();
        }

    }
}
