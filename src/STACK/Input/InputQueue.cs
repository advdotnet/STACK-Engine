using System.Collections.Generic;

namespace STACK.Input
{
    /// <summary>
    /// Queue of InputEvents.
    /// </summary>
    public class InputQueue : Queue<InputEvent>
    {
        private InputEventFileLogger Logger;

        public InputQueue(bool record = false) : base(5)
        {
            if (record)
            {
                Logger = new InputEventFileLogger("demo.dat");
            }
        }

        public new void Enqueue(InputEvent item)
        {
            if (Logger != null)
            {
                Logger.Log(item);
            }

            base.Enqueue(item);
        }

        public void OnUnload()
        {
            if (Logger != null)
            {
                Logger.Flush();
            }
        }
    }
}
