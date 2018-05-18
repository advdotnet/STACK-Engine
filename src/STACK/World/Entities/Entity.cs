using STACK.Components;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace STACK
{
    [Serializable]
    [DebuggerDisplay("ID = {ID}")]
    public class Entity : BaseEntityCollection
    {
        public Scene _DrawScene;
        [NonSerialized]
        private Scene _UpdateScene;

        public Scene DrawScene
        {
            get
            {
                return _DrawScene;
            }
            set
            {
                if (value != _DrawScene)
                {
                    var previous = _DrawScene;
                    _DrawScene = value;

                    if (_DrawScene != null && _DrawScene.Initialized)
                    {
                        _DrawScene.GameObjectCache.CacheVisibleObjects();
                        _DrawScene.GameObjectCache.CacheObjectsToDraw();
                    }

                    if (previous != null && previous.Initialized)
                    {
                        previous.GameObjectCache.CacheVisibleObjects();
                        previous.GameObjectCache.CacheObjectsToDraw();
                    }
                }
            }
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext c)
        {
            _UpdateScene = (Scene)Parent;
        }

        public World World
        {
            get
            {
                return _UpdateScene.World;
            }
        }

        public Scene UpdateScene
        {
            get
            {
                return _UpdateScene;
            }
            set
            {
                if (value != _UpdateScene)
                {
                    var previous = _UpdateScene;
                    Parent = value;
                    _UpdateScene = value;

                    previous?.Pop(this);
                    if (null != _UpdateScene && !_UpdateScene.Items.Contains(this))
                    {
                        _UpdateScene.Push(this);
                    }
                }
            }
        }

        public Entity()
        {

        }

        public Entity(string id) : this()
        {
            if (!string.IsNullOrEmpty(id))
            {
                _ID = id;
            }
        }

        public void EnterScene(string name)
        {
            EnterScene(World[name]);
        }

        public virtual void EnterScene(Scene scene)
        {
            DrawScene.Notify(Messages.EntityLeavesScene, this);
            Notify(Messages.SceneEnter, scene);
            DrawScene = scene;
            Notify(Messages.SceneEntered, scene);
            DrawScene.Notify(Messages.EntityEntersScene, this);
        }

        public override void OnPropertyChanged(string property)
        {
            if (Properties.DrawOrder == property || Properties.Visible == property)
            {
                if (UpdateScene != DrawScene)
                {
                    if (DrawScene != null && DrawScene.Initialized)
                    {
                        DrawScene.GameObjectCache.CacheVisibleObjects();
                        DrawScene.GameObjectCache.CacheObjectsToDraw(Properties.DrawOrder == property);
                    }
                }

                if (UpdateScene != null && UpdateScene.Initialized)
                {
                    UpdateScene.GameObjectCache.CacheVisibleObjects();
                    UpdateScene.GameObjectCache.CacheObjectsToDraw(Properties.DrawOrder == property);
                }
            }

            base.OnPropertyChanged(property);
        }
    }
}
