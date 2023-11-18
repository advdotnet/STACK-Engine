using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
	/// <summary>
	/// Wraps an audio listener
	/// </summary>
	[Serializable]
	public class AudioListener : AudioTransmission
	{
		public Microsoft.Xna.Framework.Audio.AudioListener Listener = new Microsoft.Xna.Framework.Audio.AudioListener();

		protected override void SetPosition(Vector3 position)
		{
			Listener.Position = position;
		}

		public static AudioListener Create(Entity addTo)
		{
			return addTo.Add<AudioListener>();
		}

		public AudioListener SetUpdatePositionWithTransform(bool val) { UpdatePositionWithTransform = val; return this; }
		public AudioListener SetScale(float val) { Scale = val; return this; }
	}
}
