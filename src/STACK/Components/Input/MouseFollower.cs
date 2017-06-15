using System;

namespace STACK.Components
{
    /// <summary>
    /// Synchronizes the Position property of a transform component with the mouse coordinates.
    /// </summary>
    [Serializable]
    public class MouseFollower : Component
    {
        public override void OnUpdate()
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
