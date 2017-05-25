using System;

namespace STACK
{
    /// <summary>
    /// Interface to control skipping of text lines or whole cutscenes
    /// </summary>
    public interface ISkipContent
    {
        SkipCutscene SkipCutscene { get; }
        SkipText SkipText { get; }
    }

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

    public class SkipText : BaseSkip { }

    public class SkipCutscene : BaseSkip
    {
        Action<GameSpeed> SetSpeedFn;

        public SkipCutscene(Action<GameSpeed> setSpeedFn)
        {
            SetSpeedFn = setSpeedFn;
        }

        protected override void StartAction()
        {
            SetSpeedFn?.Invoke(GameSpeed.Infinity);
        }

        protected override void StopAction()
        {
            SetSpeedFn?.Invoke(GameSpeed.Default);
        }
    }
}
