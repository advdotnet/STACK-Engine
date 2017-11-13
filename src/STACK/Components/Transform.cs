using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
    [Flags]
    public enum State
    {
        Idle = 0,
        Walking = 1,
        Talking = 2,
        Custom = 4
    }

    /// <summary>
    /// Discretizes an orientation vector into eight distinct directions.
    /// </summary>
    public enum Directions8
    {
        None, LeftUp, Up, RightUp, Right, RightDown, Down, LeftDown, Left
    }

    /// <summary>
    /// Discretizes an orientation vector into four distinct directions.
    /// </summary>
    public enum Directions4
    {
        None, Left, Up, Right, Down
    }

    public static class Direction
    {
        public static Vector2 ToVector2(this Directions4 direction)
        {
            switch (direction)
            {
                case Directions4.Up: return new Vector2(0, -1);
                case Directions4.Right: return new Vector2(1, 0);
                case Directions4.Down: return new Vector2(0, 1);
                case Directions4.Left: return new Vector2(-1, 0);
            }
            return Vector2.Zero;
        }

        public static Vector2 ToVector2(this Directions8 direction)
        {
            switch (direction)
            {
                case Directions8.LeftUp: return new Vector2(-1, -1);
                case Directions8.Up: return new Vector2(0, -1);
                case Directions8.RightUp: return new Vector2(1, -1);
                case Directions8.Right: return new Vector2(1, 0);
                case Directions8.RightDown: return new Vector2(1, 1);
                case Directions8.Down: return new Vector2(0, 1);
                case Directions8.LeftDown: return new Vector2(-1, 1);
                case Directions8.Left: return new Vector2(-1, 0);
            }
            return Vector2.Zero;
        }

        public static Directions4 ToDirection4(this Vector2 value)
        {
            if (value.Equals(Vector2.Zero))
            {
                return Directions4.None;
            }

            if (Math.Abs(value.X) > Math.Abs(value.Y))
            {
                if (value.X > 0)
                {
                    return Directions4.Right;
                }
                else
                {
                    return Directions4.Left;
                }
            }
            else
            {
                if (value.Y > 0)
                {
                    return Directions4.Down;
                }
                else
                {
                    return Directions4.Up;
                }
            }
        }

        public static Directions8 ToDirection8(this Vector2 value)
        {
            var AbsX = Math.Abs(value.X);
            var AbsY = Math.Abs(value.Y);

            if (AbsX == 0 && AbsY == 0)
            {
                return Directions8.None;
            }

            if (AbsX > AbsY)
            {
                // vertical side
                double Half = AbsX * 0.4142;
                if (value.X > 0)
                {
                    // left side
                    if (value.Y > Half) return Directions8.LeftDown;
                    if (value.Y < -Half) return Directions8.LeftUp;
                    return Directions8.Left;
                }
                else
                {
                    // right side
                    if (value.Y > Half) return Directions8.RightDown;
                    if (value.Y < -Half) return Directions8.RightUp;
                    return Directions8.Right;
                }
            }
            else
            {
                // horizontal side
                var Half = AbsY * 0.4142;
                if (value.Y > 0)
                {
                    // bottom
                    if (value.X > Half) return Directions8.RightDown;
                    if (value.X < -Half) return Directions8.LeftDown;
                    return Directions8.Down;
                }
                else
                {
                    // top
                    if (value.X > Half) return Directions8.RightUp;
                    if (value.X < -Half) return Directions8.LeftUp;
                    return Directions8.Up;
                }
            }
        }
    }

    public static class StateExtensions
    {
        public static string ToAnimationName(this State state)
        {
            switch (state)
            {
                case State.Idle: return "idle";
                case State.Talking: return "talk";
                case State.Walking: return "walk";
                case State.Custom: return "custom";
                case State.Talking | State.Walking: return "walktalk";
            }

            return string.Empty;
        }

        public static bool Has(this State state, State value)
        {
            return state.HasFlag(value);
        }

        public static bool Is(this State state, State value)
        {
            return state == value;
        }

        public static State Add(this State state, State value)
        {
            state |= value;
            return state;
        }

        public static State Remove(this State state, State value)
        {
            state &= ~value;
            return state;
        }
    }

    [Serializable]
    public class Transform : Component
    {
        public bool UpdateZWithPosition { get; set; }
        public bool Absolute { get; private set; }

        float _Z = 0;
        public float Z
        {
            get
            {
                return _Z;
            }
            set
            {
                if (_Z != value)
                {
                    Entity.Priority = value;
                }
                _Z = value;
            }
        }

        private State _State = State.Idle;
        public State State
        {
            get
            {
                return _State;
            }
            set
            {
                if (_State == value)
                {
                    return;
                }

                NotifyParent(Messages.AnimationStateChanged, value);
                _State = value;
            }
        }

        private Vector2 _Position = Vector2.Zero;
        public Vector2 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                if (_Position == value)
                {
                    return;
                }

                if (UpdateZWithPosition)
                {
                    Z = value.Y;
                }
                _Position = value;
                NotifyParent(Messages.PositionChanged, value);
            }
        }

        public float Scale = 1f;
        public float Speed = 150.0f;
        private Func<float> CalculateEffectiveSpeedFn;

        private float CalculateEffectiveSpeedDefault()
        {
            var Result = Scale;
            var SpriteData = Get<SpriteData>();

            if (SpriteData != null)
            {
                Result *= SpriteData.Scale.Y;
            }

            return Result * Speed;
        }

        public float EffectiveSpeed
        {
            get
            {
                return CalculateEffectiveSpeedFn();
            }
        }


        private Vector2 _Orientation = Vector2.Zero;
        public Vector2 Orientation
        {
            get
            {
                return _Orientation;
            }
            set
            {
                value.Normalize();

                if (_Orientation == value)
                {
                    return;

                }
                _Orientation = value;
                NotifyParent(Messages.OrientationChanged, value);
            }
        }

        public Transform()
        {
            Visible = false;
            CalculateEffectiveSpeedFn = CalculateEffectiveSpeedDefault;
        }

        public void Turn(Directions8 direction)
        {
            Direction8 = direction;
        }

        public void Turn(Directions4 direction)
        {
            Direction4 = direction;
        }

        public Directions8 Direction8
        {
            set
            {
                Orientation = value.ToVector2();
            }
            get
            {
                return Orientation.ToDirection8();
            }
        }

        public Directions4 Direction4
        {
            set
            {
                Orientation = value.ToVector2();
            }
            get
            {
                return Orientation.ToDirection4();
            }
        }

        public static Transform Create(Entity addTo)
        {
            return addTo.Add<Transform>();
        }

        public Transform SetPosition(float x, float y) { Position = new Vector2(x, y); return this; }
        public Transform SetPosition(Vector2 value) { Position = value; return this; }
        public Transform SetState(State value) { State = value; return this; }
        public Transform SetZ(float value) { Z = value; return this; }
        public Transform SetScale(float value) { Scale = value; return this; }
        public Transform SetSpeed(float value) { Speed = value; return this; }
        public Transform SetOrientation(float x, float y) { Orientation = new Vector2(x, y); return this; }
        public Transform SetOrientation(Vector2 value) { Orientation = value; return this; }
        public Transform SetDirection(Directions4 value) { Orientation = value.ToVector2(); return this; }
        public Transform SetDirection(Directions8 value) { Orientation = value.ToVector2(); return this; }
        public Transform SetUpdateZWithPosition(bool value) { UpdateZWithPosition = value; return this; }
        public Transform SetAbsolute(bool value) { Absolute = value; return this; }
        public Transform AddState(State state) { State = State.Add(state); return this; }
        public Transform RemoveState(State state) { State = State.Remove(state); return this; }
        public Transform SetCalculateEffectiveSpeedFn(Func<float> calculateEffectiveSpeedFn) { CalculateEffectiveSpeedFn = calculateEffectiveSpeedFn; return this; }
    }
}
