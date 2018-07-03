using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using STACK.Input;
using System;

namespace STACK.Components
{
    /// <summary>
    /// Dispatches incoming input events to various delegates.
    /// </summary>
    [Serializable]
    public class InputDispatcher : Component, IInteractive
    {
        public InputDispatcher()
        {

        }

        public Action<Vector2> OnMouseMove { get; set; }
        public Action<Vector2, MouseButton> OnMouseDown { get; set; }
        public Action<Vector2, MouseButton> OnMouseUp { get; set; }
        public Action<Keys> OnKeyUp { get; set; }
        public Action<Keys> OnKeyDown { get; set; }
        public Action<Vector2, int> OnMouseScroll { get; set; }

        public void HandleInputEvent(Vector2 mouse, InputEvent inputEvent)
        {
            inputEvent.Dispatch(mouse, OnMouseMove, OnMouseDown, OnMouseUp, OnKeyDown, OnKeyUp, OnMouseScroll);
        }

        public static InputDispatcher Create(BaseEntityCollection addTo)
        {
            return addTo.Add<InputDispatcher>();
        }

        public InputDispatcher SetOnMouseMoveFn(Action<Vector2> value) { OnMouseMove = value; return this; }
        public InputDispatcher SetOnMouseDownFn(Action<Vector2, MouseButton> value) { OnMouseDown = value; return this; }
        public InputDispatcher SetOnMouseUpFn(Action<Vector2, MouseButton> value) { OnMouseUp = value; return this; }
        public InputDispatcher SetOnKeyUpFn(Action<Keys> value) { OnKeyUp = value; return this; }
        public InputDispatcher SetOnKeyDownFn(Action<Keys> value) { OnKeyDown = value; return this; }
        public InputDispatcher SetOnMouseScrollFn(Action<Vector2, int> value) { OnMouseScroll = value; return this; }
    }
}
