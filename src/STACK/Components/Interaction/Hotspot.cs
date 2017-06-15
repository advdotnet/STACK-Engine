using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
    [Serializable]
    public abstract class Hotspot : Component
    {
        public abstract bool IsHit(Vector2 mouse);
        public string Caption { get; protected set; }        
    }   
}
