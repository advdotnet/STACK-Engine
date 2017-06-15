using STACK.Components;
using System;

namespace STACK
{   
    [Serializable]
    public class Entity : BaseEntityCollection
    {        
        public Scene DrawScene;

        [NonSerialized]
        private Scene _UpdateScene;

        public World World
        {
            get
            {                
                return UpdateScene.World;
            }
        }

        public Scene UpdateScene
        {
            get
            {
                return _UpdateScene ?? (_UpdateScene = (Scene)Parent);
            }
            set
            {
                Parent = value;
            }
        }        

        public Entity() 
        {
            
        }

        public Entity(string id) : this()
        {
            if (!string.IsNullOrEmpty(id))
            {
                ID = id;
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
    }
}
