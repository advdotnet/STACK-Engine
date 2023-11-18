using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
	/// <summary>
	/// Base class for AudioEmitter and AudioListener
	/// </summary>
	[Serializable]
	public abstract class AudioTransmission : Component, IInitialize, INotify
	{
		[NonSerialized]
		private Transform _transform;
		public float Scale { get; set; }
		public bool UpdatePositionWithTransform { get; set; }

		public AudioTransmission()
		{
			Scale = 200;
			UpdatePositionWithTransform = true;
		}

		protected abstract void SetPosition(Vector3 position);

		public void Notify<T>(string message, T data)
		{
			if (Messages.PositionChanged == message)
			{
				UpdatePositionFromTransform();
			}
		}

		protected void UpdatePositionFromTransform()
		{
			if (UpdatePositionWithTransform && null != _transform)
			{
				var newPosition = new Vector3(_transform.Position.X / Scale, _transform.Position.Y / Scale, 0);
				SetPosition(newPosition);
			}
		}

		public void Initialize(bool restore)
		{
			CacheTransients();
			UpdatePositionFromTransform();
		}

		private void CacheTransients()
		{
			_transform = Get<Transform>();
		}
	}
}
