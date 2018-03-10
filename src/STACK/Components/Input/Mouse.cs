using Microsoft.Xna.Framework;
using STACK.Input;
using System;

namespace STACK.Components
{
    /// <summary>
    /// Component holding the mouse position as well as the object which is 
    /// currently under the mouse.
    /// </summary>
    [Serializable]
    public class Mouse : Component, IUpdate, IInteractive
    {
        Vector2 _Position;
        Entity _ObjectUnderMouse;
        bool _Enabled;
        float _UpdateOrder;

        public Vector2 Position { get { return _Position; } set { _Position = value; } }
        public Entity ObjectUnderMouse { get { return _ObjectUnderMouse; } }
        public bool Enabled { get { return _Enabled; } set { _Enabled = value; } }
        public float UpdateOrder { get { return _UpdateOrder; } set { _UpdateOrder = value; } }

        public Mouse()
        {
            Enabled = true;
        }

        public static Mouse Create(World addTo)
        {
            return addTo.Add<Mouse>();
        }

        public void HandleInputEvent(Vector2 mouse, InputEvent inputEvent)
        {
            if (inputEvent.Type == InputEventType.MouseMove)
            {
                Position = InputEvent.IntToVector2(inputEvent.Param);

                _ObjectUnderMouse = World.GetObjectAtPosition(Position);
            }
        }

        public void Update()
        {
            _ObjectUnderMouse = World.GetObjectAtPosition(Position);
        }

        public Mouse SetPosition(Vector2 value) { Position = value; return this; }
        public Mouse SetPosition(float x, float y) { Position = new Vector2(x, y); return this; }
    }
}
