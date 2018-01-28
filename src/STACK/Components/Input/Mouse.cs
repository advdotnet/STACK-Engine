﻿using Microsoft.Xna.Framework;
using STACK.Input;
using System;

namespace STACK.Components
{
    /// <summary>
    /// Component holding the mouse position as well as the object which is 
    /// currently under the mouse.
    /// </summary>
    [Serializable]
    public class Mouse : Component
    {
        public Vector2 Position { get; set; }
        public Entity ObjectUnderMouse { get; private set; }

        public Mouse()
        {
            Visible = false;
        }

        public static Mouse Create(World addTo)
        {
            return addTo.Add<Mouse>();
        }

        public override void OnHandleInputEvent(Vector2 mouse, InputEvent inputEvent)
        {
            if (inputEvent.Type == InputEventType.MouseMove)
            {
                Position = InputEvent.IntToVector2(inputEvent.Param);

                ObjectUnderMouse = World.GetObjectAtPosition(Position);
            }
        }

        public override void OnUpdate()
        {
            ObjectUnderMouse = World.GetObjectAtPosition(Position);
        }

        public Mouse SetPosition(Vector2 value) { Position = value; return this; }
        public Mouse SetPosition(float x, float y) { Position = new Vector2(x, y); return this; }
    }
}
