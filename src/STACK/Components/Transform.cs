using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
    [Serializable]
    public class Transform : Component
    {
        bool _UpdateZWithPosition;
        bool _Absolute;
        float _Z = 0;
        State _State = State.Idle;
        Vector2 _Position = Vector2.Zero;
        float _Scale = 1f;
        float _Speed = 150.0f;
        Vector2 _Orientation = Vector2.Zero;
        Func<float> _CalculateEffectiveSpeedFn;

        public bool UpdateZWithPosition { get { return _UpdateZWithPosition; } set { _UpdateZWithPosition = value; } }
        public bool Absolute { get { return _Absolute; } }


        public float Scale { get { return _Scale; } set { _Scale = value; } }
        public float Speed { get { return _Speed; } set { _Speed = value; } }

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
                    Entity.DrawOrder = value;
                    _Z = value;
                }
            }
        }

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

                Parent?.Notify(Messages.AnimationStateChanged, value);
                _State = value;
            }
        }

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
                Parent?.Notify(Messages.PositionChanged, value);
            }
        }

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
                return _CalculateEffectiveSpeedFn();
            }
        }

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
                Parent.Notify(Messages.OrientationChanged, value);
            }
        }

        public Transform()
        {
            _CalculateEffectiveSpeedFn = CalculateEffectiveSpeedDefault;
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
        public Transform SetAbsolute(bool value) { _Absolute = value; return this; }
        public Transform AddState(State state) { State = State.Add(state); return this; }
        public Transform RemoveState(State state) { State = State.Remove(state); return this; }
        public Transform SetCalculateEffectiveSpeedFn(Func<float> calculateEffectiveSpeedFn) { _CalculateEffectiveSpeedFn = calculateEffectiveSpeedFn; return this; }
    }
}
