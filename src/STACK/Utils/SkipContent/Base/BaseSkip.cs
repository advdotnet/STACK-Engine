namespace STACK
{
    public abstract class BaseSkip
    {
        public bool Enabled { get; protected set; }
        public bool Possible { get; set; }

        public void Start()
        {
            if (!Possible)
            {
                return;
            }

            StartAction();
            Enabled = true;
        }

        public void Stop()
        {
            StopAction();
            Enabled = false;
        }

        protected virtual void StartAction() { }
        protected virtual void StopAction() { }
    }
}
