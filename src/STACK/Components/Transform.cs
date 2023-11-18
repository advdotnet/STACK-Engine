using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
	[Serializable]
	public class Transform : Component
	{
		private bool _updateZWithPosition;
		private bool _absolute;
		private float _z = 0;
		private State _state = State.Idle;
		private Vector2 _position = Vector2.Zero;
		private float _scale = 1f;
		private float _speed = 150.0f;
		private Vector2 _orientation = Vector2.Zero;
		private Func<float> _calculateEffectiveSpeedFn;

		public bool UpdateZWithPosition { get => _updateZWithPosition; set => _updateZWithPosition = value; }
		public bool Absolute => _absolute;

		public float Scale { get => _scale; set => _scale = value; }
		public float Speed { get => _speed; set => _speed = value; }

		public float Z
		{
			get => _z;
			set
			{
				if (_z != value)
				{
					Entity.DrawOrder = value;
					_z = value;
				}
			}
		}

		public State State
		{
			get => _state;
			set
			{
				if (_state == value)
				{
					return;
				}

				Parent?.Notify(Messages.AnimationStateChanged, value);
				_state = value;
			}
		}

		public Vector2 Position
		{
			get => _position;
			set
			{
				if (_position == value)
				{
					return;
				}

				if (UpdateZWithPosition)
				{
					Z = value.Y;
				}
				_position = value;
				Parent?.Notify(Messages.PositionChanged, value);
			}
		}

		private float CalculateEffectiveSpeedDefault()
		{
			var result = Scale;
			var spriteData = Get<SpriteData>();

			if (spriteData != null)
			{
				result *= spriteData.Scale.Y;
			}

			return result * Speed;
		}

		public float EffectiveSpeed => _calculateEffectiveSpeedFn();

		public Vector2 Orientation
		{
			get => _orientation;
			set
			{
				value.Normalize();

				if (_orientation == value)
				{
					return;
				}

				_orientation = value;
				Parent.Notify(Messages.OrientationChanged, value);
			}
		}

		public Transform()
		{
			_calculateEffectiveSpeedFn = CalculateEffectiveSpeedDefault;
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
			set => Orientation = value.ToVector2();
			get => Orientation.ToDirection8();
		}

		public Directions4 Direction4
		{
			set => Orientation = value.ToVector2();
			get => Orientation.ToDirection4();
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
		public Transform SetAbsolute(bool value) { _absolute = value; return this; }
		public Transform AddState(State state) { State = State.Add(state); return this; }
		public Transform RemoveState(State state) { State = State.Remove(state); return this; }
		public Transform SetCalculateEffectiveSpeedFn(Func<float> calculateEffectiveSpeedFn) { _calculateEffectiveSpeedFn = calculateEffectiveSpeedFn; return this; }
	}
}
