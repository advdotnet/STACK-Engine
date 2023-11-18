using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
	/// <summary>
	/// Wraps an audio emitter
	/// </summary>
	[Serializable]
	public class AudioEmitter : AudioTransmission
	{
		public Microsoft.Xna.Framework.Audio.AudioEmitter Emitter = new Microsoft.Xna.Framework.Audio.AudioEmitter();

		protected override void SetPosition(Vector3 position)
		{
			Emitter.Position = position;
		}

		public static AudioEmitter Create(Entity addTo)
		{
			return addTo.Add<AudioEmitter>();
		}

		public AudioEmitter SetUpdatePositionWithTransform(bool val) { UpdatePositionWithTransform = val; return this; }
		public AudioEmitter SetScale(float val) { Scale = val; return this; }
	}
}
