using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace STACK.Functional.Test
{
    public class TestEngine : StackEngine, IDisposable
    {
        protected TestInputProvider Input;
        protected GraphicsDeviceServiceMock GraphicsDevice;

        public TestEngine(StackGame game, IServiceProvider services, TestInputProvider input)
            : base(game, services, input)
        {
            Input = input;
        }

        public bool IsInteractive
        {
            get
            {
                return Game.World.Interactive;
            }
        }

        public void Tick()
        {
            Update();
        }

        public virtual void AdvanceToInteractive()
        {
            Advance(() => Game.World.Interactive);
        }

        public void Advance(Func<bool> predicate)
        {
            while (!predicate())
            {
                Tick();
            }
        }

        public void MouseClick()
        {
            Input.MouseDown();
            Tick();
            Input.MouseUp();
            Tick();
        }

        public void MouseClick(int x, int y)
        {
            Input.MouseMove(x, y);
            Tick();
            Input.MouseDown();
            Tick();
            Input.MouseUp();
            Tick();
        }

        public void MouseClick(Point p)
        {
            MouseClick(p.X, p.Y);
        }

        public void MouseClick(Vector2 v)
        {
            MouseClick(v.ToPoint());
        }

        public void MouseMove(int x, int y)
        {
            Input.MouseMove(x, y);
            Tick();
        }

        public void MouseMove(Point p)
        {
            MouseMove(p.X, p.Y);
        }

        public void KeyPress(Keys key)
        {
            Input.KeyDown(key);
            Tick();
            Input.KeyUp(key);
            Tick();
        }

        public void KeyPress(params Keys[] keys)
        {
            foreach (var Key in keys)
            {
                Input.KeyDown(Key);
            }
            Tick();

            foreach (var Key in keys)
            {
                Input.KeyUp(Key);
            }
            Tick();
        }

        public void EnterText(string text)
        {
            foreach (char Char in text)
            {
                List<string> Input = TranslateChar(Char);
                List<Keys> KeySet = new List<Keys>();
                foreach (var Identifier in Input)
                {
                    KeySet.Add((Keys)Enum.Parse(typeof(Keys), Identifier));
                }
                KeyPress(KeySet.ToArray());
            }

            KeyPress(Keys.Enter);
        }

        private List<string> Set(params string[] keys)
        {
            return new List<string>(keys);
        }

        private List<string> TranslateChar(char key)
        {
            switch ((int)key)
            {
                case 32: return Set("Space");
                case 27: return Set("Escape");
                case 46: return Set("OemPeriod");
                case 61: return Set("LeftShift", "D0");
                default: return Set(key.ToString().ToUpperInvariant());
            }
        }
    }
}
