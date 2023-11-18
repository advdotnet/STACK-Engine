using System;

namespace STACK
{
	public class SkipCutscene : BaseSkip
	{
		private readonly Action<GameSpeed> _setSpeedFn;

		public SkipCutscene(Action<GameSpeed> setSpeedFn)
		{
			_setSpeedFn = setSpeedFn;
		}

		protected override void StartAction()
		{
			_setSpeedFn?.Invoke(GameSpeed.Infinity);
		}

		protected override void StopAction()
		{
			_setSpeedFn?.Invoke(GameSpeed.Default);
		}
	}
}
