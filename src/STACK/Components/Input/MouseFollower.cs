using System;

namespace STACK.Components
{
    /// <summary>
    /// Synchronizes the Position property of a transform component with the mouse coordinates.
    /// </summary>
    [Serializable]
    public class MouseFollower : Component, IUpdate
    {
        bool _Enabled;
        float _UpdateOrder;

        public bool Enabled { get { return _Enabled; } set { _Enabled = value; } }
        public float UpdateOrder { get { return _UpdateOrder; } set { _UpdateOrder = value; } }

        public MouseFollower()
        {
            Enabled = true;
        }

        public void Update()
        {
            var Position = Entity.UpdateScene.World.Get<Mouse>().Position;
            Get<Transform>().Position = Position;
        }

        public static MouseFollower Create(Entity addTo)
        {
            return addTo.Add<MouseFollower>();
        }
    }
}
