using System;

namespace STACK
{
    public struct GameSpeed
    {
        /// <summary>
        /// The duration of one game tick in seconds.
        /// </summary>
        public const float TickDuration = (float)DefaultTargetElapsedTime / 1000f;

        public TimeSpan TargetElapsedTime { get; private set; }
        public TimeSpan MaxElapsedTime { get; private set; }
        public string Text { get; private set; }

        private const double DefaultTargetElapsedTime = 1000d / 60d;

        private GameSpeed(TimeSpan target, TimeSpan max, string text)
        {
            TargetElapsedTime = target;
            MaxElapsedTime = max;
            Text = text;
        }

        public override bool Equals(Object obj)
        {
            return (obj is GameSpeed) && (this == (GameSpeed)obj);
        }
        public override int GetHashCode()
        {
            return TargetElapsedTime.GetHashCode() ^ MaxElapsedTime.GetHashCode();
        }
        public static bool operator ==(GameSpeed x, GameSpeed y)
        {
            return x.TargetElapsedTime == y.TargetElapsedTime && x.MaxElapsedTime == y.MaxElapsedTime;
        }
        public static bool operator !=(GameSpeed x, GameSpeed y)
        {
            return !(x == y);
        }

        public static GameSpeed Default = new GameSpeed(TimeSpan.FromTicks((long)10000000 / (long)60), TimeSpan.FromMilliseconds(500), "1x");
        public static GameSpeed Half = new GameSpeed(TimeSpan.FromTicks((long)10000000 / (long)120), TimeSpan.FromMilliseconds(500), "0.5x");
        public static GameSpeed Double = new GameSpeed(TimeSpan.FromTicks((long)10000000 / (long)30), TimeSpan.FromMilliseconds(500), "2x");
        public static GameSpeed Infinity = new GameSpeed(TimeSpan.FromTicks(400), TimeSpan.FromMilliseconds(500), "inf");
    }
}
