using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace STACK.Surrogates
{
	internal class StackSurrogateSelector : ISurrogateSelector
	{
		private ISurrogateSelector _next;
		private readonly List<Type> _serializedTypes = new List<Type>()
		{
			typeof(System.Collections.IEnumerator),
			typeof(Microsoft.Xna.Framework.GameTime),
			typeof(Microsoft.Xna.Framework.Point),
			typeof(Microsoft.Xna.Framework.Vector2),
			typeof(Microsoft.Xna.Framework.Vector3),
			typeof(Microsoft.Xna.Framework.Rectangle),
			typeof(Microsoft.Xna.Framework.Matrix),
			typeof(Microsoft.Xna.Framework.Color),
			typeof(Microsoft.Xna.Framework.Audio.AudioListener),
			typeof(Microsoft.Xna.Framework.Audio.AudioEmitter),
			typeof(FAudio.F3DAUDIO_LISTENER),
			typeof(FAudio.F3DAUDIO_VECTOR),
			typeof(FAudio.F3DAUDIO_EMITTER)
		};

		public void ChainSelector(ISurrogateSelector selector)
		{
			_next = selector;
		}

		public ISurrogateSelector GetNextSelector()
		{
			return _next;
		}

		public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
		{
			if (_serializedTypes.Any(t => t.IsAssignableFrom(type)))
			{
				selector = this;
				return new FieldSerializationSurrogate();
			}
			else
			{
				if (_next == null)
				{
					selector = null;
					return null;
				}
				else
				{
					return _next.GetSurrogate(type, context, out selector);
				}
			}
		}
	}
}
