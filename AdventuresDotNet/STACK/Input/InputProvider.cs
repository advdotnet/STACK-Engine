using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using STACK.Graphics;
using System;

namespace STACK.Input
{
    
    /// <summary>    
    /// </summary>
    public abstract class InputProvider
    {
        public delegate void InputHandler(InputEvent check);    
        public event InputHandler Handler;
        public DisplaySettings DisplaySettings { get; set; }

        protected void Notify(InputEvent @event)
        {
            // avoids racing conditions
            var Temp = Handler;

            if (Temp != null)
            {
                Temp(@event);
            }
        }        

        public abstract void Dispatch(bool paused);
        public abstract KeyboardState KeyboardState { get; }
        public abstract MouseState MouseState { get; }
    }

    /// <summary>
    /// Captures input events send by the OS to the application using XNA Input methods.
    /// </summary>
    public class UserInputProvider : InputProvider
    {
        KeyboardState _KeyboardState, OldKeyboardState;
        MouseState _MouseState, OldMouseState;
        long TimeStamp;
        InputQueue Queue = new InputQueue();                

        public override KeyboardState KeyboardState
        {
            get
            {
                return _KeyboardState;
            }
        }

        public override MouseState MouseState
        {
            get
            {
                return _MouseState;
            }            
        }

        bool IsInsideWindow(int x, int y)
        {
            return DisplaySettings.Viewport.Bounds.Contains(x, y);            
        }

        public override void Dispatch(bool paused)
        {
            Update();     
    
            while (Queue.Count > 0)
            {
                var Event = Queue.Dequeue();
                Event.Paused = paused;
                Notify(Event);
            }
        }        

        void Update()
        {
            if (Queue == null)
            {
                return;
            }

            TimeStamp += (long)(GameSpeed.TickDuration);

            OldKeyboardState = _KeyboardState;
            OldMouseState = _MouseState;

            _KeyboardState = Keyboard.GetState();
            _MouseState = Mouse.GetState();

            var ScreenCoordinates = new Vector2(_MouseState.X, _MouseState.Y);
            var WorldCoordinates = DisplaySettings.TransformPoint(_MouseState.X, _MouseState.Y);

            _MouseState = _MouseState.UpdatePosition((int)WorldCoordinates.X, (int)WorldCoordinates.Y);

            // mouse move
            if (IsInsideWindow((int)ScreenCoordinates.X, (int)ScreenCoordinates.Y))
            {

                if (OldMouseState.X != _MouseState.X || OldMouseState.Y != _MouseState.Y)
                {
                    Queue.Enqueue(InputEvent.MouseMove(TimeStamp, _MouseState.X, _MouseState.Y));
                }

                // mouse click left
                if (_MouseState.LeftButton == ButtonState.Pressed && OldMouseState.LeftButton == ButtonState.Released)
                {                    
                    Queue.Enqueue(InputEvent.MouseClick(ButtonState.Pressed, TimeStamp, MouseButton.Left));                    
                }

                if (_MouseState.LeftButton == ButtonState.Released && OldMouseState.LeftButton == ButtonState.Pressed)
                {
                    Queue.Enqueue(InputEvent.MouseClick(ButtonState.Released, TimeStamp, MouseButton.Left));                    
                }

                // mouse click right
                if (_MouseState.RightButton == ButtonState.Pressed && OldMouseState.RightButton == ButtonState.Released)
                {
                    Queue.Enqueue(InputEvent.MouseClick(ButtonState.Pressed, TimeStamp, MouseButton.Right));                    
                }

                if (_MouseState.RightButton == ButtonState.Released && OldMouseState.RightButton == ButtonState.Pressed)
                {
                    Queue.Enqueue(InputEvent.MouseClick(ButtonState.Released, TimeStamp, MouseButton.Right));
                }
            }

            // key down
            foreach (Keys Key in _KeyboardState.GetPressedKeys())
            {
                if (!OldKeyboardState.IsKeyDown(Key))
                {
                    Queue.Enqueue(InputEvent.KeyPress(KeyState.Down, TimeStamp, Key));
                }
            }

            // key up
            foreach (Keys Key in OldKeyboardState.GetPressedKeys())
            {
                if (!_KeyboardState.IsKeyDown(Key))
                {
                    Queue.Enqueue(InputEvent.KeyPress(KeyState.Up, TimeStamp, Key));
                }
            }
        }
    }

    /// <summary>
    /// Reconstructs input events using a given recorded demo file.
    /// </summary>
    public class PlaybackInputProvider : InputProvider
    {
        readonly Stream Reader;
        readonly DeflateStream ZipStream;
        readonly BinaryFormatter Formatter;
        long TimeStamp;
        InputQueue Queue = new InputQueue();

        bool end = false;
        InputEvent CurrentEvent;        

        public PlaybackInputProvider(string filename)
        {
            Reader = File.Open(filename, FileMode.Open, FileAccess.Read);
            ZipStream = new DeflateStream(Reader, CompressionMode.Decompress, true);
            Formatter = new BinaryFormatter();
            CurrentEvent = LoadInputEventFromStream();
        }

        public override KeyboardState KeyboardState
        {
            get
            {
                throw new NotImplementedException();                
            }
        }

        public override MouseState MouseState
        {
            get
            {
                throw new NotImplementedException();                
            }
        }

        public InputEvent LoadInputEventFromStream()
        {
            InputEvent Result = new InputEvent();

            try
            {
                Result = (InputEvent)Formatter.Deserialize(ZipStream);
                ZipStream.Flush();
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                end = true;
                ZipStream.Close();
                ZipStream.Dispose();
                Reader.Dispose();
            }

            return Result;
        }


        void Update()
        {
            TimeStamp += (long)GameSpeed.TickDuration;

            // make sure to enqueue all events for the current timestamp
            while (CurrentEvent.Timestamp <= TimeStamp && !end)
            {
                Queue.Enqueue(CurrentEvent);
                CurrentEvent = LoadInputEventFromStream();
            }
        }

        public override void Dispatch(bool paused)
        {
            Update();
            while (Queue.Count > 0)
            {
                var Event = Queue.Dequeue();
                Event.Paused = paused;
                Notify(Event);
            }
        }
    }
}
