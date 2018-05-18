using System;

namespace STACK
{
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
