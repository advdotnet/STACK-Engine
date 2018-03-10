﻿using System;
using System.Runtime.Serialization;

namespace STACK
{
    [Serializable]
    public abstract class Component
    {
        private BaseEntityCollection _Parent;

        public BaseEntityCollection Parent
        {
            get
            {
                return _Parent;
            }
            internal set
            {
                _Parent = value;
                CacheTransients();
            }
        }

        [NonSerialized]
        Entity CastedEntity = null;
        [NonSerialized]
        Scene CastedScene = null;
        [NonSerialized]
        World ParentWorld = null;

        [OnDeserialized]
        void OnDeserialized(StreamingContext c)
        {
            CacheTransients();
        }

        private void CacheTransients()
        {
            CastedEntity = _Parent as Entity;
            CastedScene = _Parent as Scene;
            ParentWorld = _Parent as World;
        }

        public Entity Entity
        {
            get
            {
                return CastedEntity;
            }
        }

        public Scene Scene
        {
            get
            {
                return CastedScene;
            }
        }

        public World World
        {
            get
            {
                return ParentWorld;
            }
        }

        public T Get<T>() where T : Component
        {
            return Entity.Get<T>();
        }
    }
}
