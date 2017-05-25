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
                int TextLength = (text ?? string.Empty).Length;
                duration = 1 + TextLength * 0.175f;
            }
            return duration;
        }
    }    
}
