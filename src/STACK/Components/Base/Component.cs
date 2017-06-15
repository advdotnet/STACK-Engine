using System;

namespace STACK
{
    [Serializable]
    public abstract class Component : BaseEntity
    {
        public BaseEntityCollection Parent { get; internal set; }
        [NonSerialized]
        Entity CastedEntity = null;
        [NonSerialized]
        Scene CastedScene = null;

        public Entity Entity
        {
            get
            {
                return CastedEntity ?? (CastedEntity = (Entity)Parent);
            }
        }

        public Scene Scene
        {
            get
            {
                return CastedScene ?? (CastedScene = (Scene)Parent);
            }
        }

        public T Get<T>() where T : Component
        {
            return Entity.Get<T>();
        }    
    
        public void NotifyParent<T>(string message, T data) 
        {
            if (Parent != null)
            {
                Parent.Notify(message, data);
            }
        }
    }    
}
