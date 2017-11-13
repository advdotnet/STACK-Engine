using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
    [Serializable]
    public class Interaction : Component
    {
        public Vector2 Position { get; set; }
        public Directions8 Direction { get; set; }
        public Func<Interactions> GetInteractionsFn { get; set; }

        /// <summary>
        /// If there is only one interaction, select it automatically
        /// </summary>
		public bool AutoUseOnlyInteraction { get; set; }

        public Interaction()
        {
            AutoUseOnlyInteraction = false;
            Direction = Directions8.None;
        }

        public Interactions GetInteractions()
        {
            return GetInteractionsFn != null ? GetInteractionsFn() : Interactions.None;
        }

        public static Interaction Create(Entity addTo)
        {
            return addTo.Add<Interaction>();
        }

        public Interaction SetGetInteractionsFn(Func<Interactions> value) { GetInteractionsFn = value; return this; }
        public Interaction SetPosition(float x, float y) { Position = new Vector2(x, y); return this; }
        public Interaction SetPosition(Vector2 value) { Position = value; return this; }
        public Interaction SetDirection(Directions8 value) { Direction = value; return this; }
        public Interaction SetAutoUseOnlyInteraction(bool value) { AutoUseOnlyInteraction = value; return this; }
    }
}
