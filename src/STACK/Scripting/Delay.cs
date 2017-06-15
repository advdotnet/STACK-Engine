using System.Collections;

namespace STACK
{    
    public static class Delay 
    {
        /// <summary>
        /// Delay for a duration in seconds.
        /// </summary>  
        public static IEnumerator Seconds(float seconds) 
        {
            float Elapsed = 0;

            while (Elapsed < seconds) 
            {
                Elapsed += GameSpeed.TickDuration;
                yield return 0;
            }
        }

        /// <summary>
        /// Delay for a number of updates.
        /// </summary>
        public static IEnumerator Updates(int count) 
        {
            return Seconds(count * GameSpeed.TickDuration);
        }
    }        
}
