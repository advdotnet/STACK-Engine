using System;

namespace STACK
{
	[Serializable]
	public static class TextDuration
	{
		public const float Auto = 0;
		public const float Persistent = -1;

		public static float Default(string text, float duration)
		{
			if (duration == Auto)
			{
				var textLength = (text ?? string.Empty).Length;
				duration = 1 + textLength * 0.1f;
			}

			return duration;
		}
	}
}
