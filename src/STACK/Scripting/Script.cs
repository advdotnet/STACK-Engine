using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections;
using System.Diagnostics;

namespace STACK
{
	/// <summary>
	/// Object wrapping an IEnumerator.
	/// </summary>
	[DebuggerDisplay("Script {ID}")]
	[Serializable]
	public class Script
	{
		private readonly IEnumerator _enumerator;
		public bool Done { get; private set; }
		public bool Enabled { get; set; }
		public string ID { get; private set; }

		private Script() { }

		public Script(IEnumerator script, string id = "")
		{
			_enumerator = script;
			Done = false;
			ID = id;
		}

		public void Clear()
		{
			Done = true;
		}

		/// <summary>
		/// Advances the script one iteration.
		/// </summary>        
		public bool MoveNext()
		{
			if (Done || Enabled || NestedMoveNext(_enumerator))
			{
				return !Done;
			}

			Done = !_enumerator.MoveNext();

			return !Done;
		}

		/// <summary>
		/// Returns if the enumerator currently holds a running script.
		/// </summary>        
		private bool NestedRunningScript()
		{
			return _enumerator.Current is Script script && !script.Done;
		}

		public Script GetNestedScript() => _enumerator.Current as Script;

		private bool NestedMoveNext(IEnumerator enumerator)
		{
			return NestedRunningScript() ||
				(enumerator.Current is IEnumerator nested && MoveNext(nested));
		}

		private bool MoveNext(IEnumerator enumerator)
		{
			if (NestedMoveNext(enumerator))
			{
				return true;
			}

			return enumerator.MoveNext();
		}

		public static Script None => new Script() { Done = true };

		/// <summary>
		/// Yields as long as the given script is not done executing.
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		public static IEnumerator WaitFor(params Script[] scripts)
		{
			while (!AllScriptsDone(scripts))
			{
				yield return 0;
			}
		}

		/// <summary>
		/// Yields as long as the given soundeffect is playing.
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		public static IEnumerator WaitFor(SoundEffectInstance soundEffect)
		{
			while (soundEffect.IsPlaying())
			{
				yield return 0;
			}
		}

		private static bool AllScriptsDone(Script[] scripts)
		{
			for (var i = 0; i < scripts.Length; i++)
			{
				if (scripts[i].Done == false)
				{
					return false;
				}
			}

			return true;
		}
	}
}
