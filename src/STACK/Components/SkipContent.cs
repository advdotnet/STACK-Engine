using System;
using System.Runtime.Serialization;

namespace STACK.Components
{
	[Serializable]
	public class SkipContent : Component, ISkipContent
	{
		[NonSerialized]
		private ISkipContent _interface;

		/// <summary>
		/// Serialize possible states in savegames.
		/// </summary>
		private bool _skipTextPossible;
		private bool _skipCutscenePossible;

		[OnSerializing]
		private void OnSerializingMethod(StreamingContext context)
		{
			if (null != SkipText)
			{
				_skipTextPossible = SkipText.Possible;
			}
			else
			{
				_skipTextPossible = true;
			}

			if (null != SkipCutscene)
			{
				_skipCutscenePossible = SkipCutscene.Possible;
			}
		}

		private void PropagatePossibleStates()
		{
			if (null != SkipText)
			{
				SkipText.Possible = _skipTextPossible;
			}

			if (null != SkipCutscene)
			{
				SkipCutscene.Possible = _skipCutscenePossible;
			}
		}

		public SkipContent()
		{

		}

		public SkipCutscene SkipCutscene => _interface?.SkipCutscene;

		public SkipText SkipText => _interface?.SkipText;

		public static SkipContent Create(World addTo)
		{
			return addTo.Add<SkipContent>();
		}

		public SkipContent SetInterface(ISkipContent value) { _interface = value; PropagatePossibleStates(); return this; }
		public SkipContent SetInterfaceFromServiceProvider(IServiceProvider value) { if (null != value) { _interface = value.GetService(typeof(ISkipContent)) as ISkipContent; } PropagatePossibleStates(); return this; }
	}
}
