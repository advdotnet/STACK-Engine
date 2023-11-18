using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace STACK.Components
{
	/// <summary>
	/// Container for a set of scripts (IEnumerables) used to describe game logic spanning several updates.
	/// </summary>
	[Serializable]
	[DebuggerDisplay("{ScriptCollection}")]
	public class Scripts : Component, IUpdate
	{
		public readonly List<Script> ScriptCollection = new List<Script>(3);
		public bool Enabled { get; set; }
		public float UpdateOrder { get; set; }

		public Scripts()
		{
			Enabled = true;
		}

		/// <summary>
		/// Updates all scripts.
		/// </summary>        				
		public void Update()
		{
			for (var i = ScriptCollection.Count - 1; i >= 0; i--)
			{
				Advance(ScriptCollection[i]);
			}
		}

		/// <summary>
		/// Adds a script with the given id and executes it immediately.
		/// </summary>
		public Script Start(IEnumerator enumerator, string id = "")
		{
			var enqueuedScript = Enqueue(enumerator, id);
			Advance(enqueuedScript);
			return enqueuedScript;
		}

		public Script Enqueue(IEnumerator enumerator, string id = "")
		{
			var script = new Script(enumerator, id);
			ScriptCollection.Add(script);
			return script;
		}

		private void Advance(Script script)
		{
			if (!script.MoveNext())
			{
				ScriptCollection.Remove(script);
			}
		}

		/// <summary>
		/// Removes all scripts.
		/// </summary>
		public void Clear()
		{
			foreach (var script in ScriptCollection)
			{
				script.Clear();
			}

			ScriptCollection.Clear();
		}

		/// <summary>
		/// Removes a script with a given id.
		/// </summary>        
		public void Remove(string id)
		{
			for (var i = ScriptCollection.Count - 1; i >= 0; i--)
			{
				if (id == ScriptCollection[i].ID)
				{
					ScriptCollection[i].Clear();
					ScriptCollection.RemoveAt(i);
				}
			}
		}

		public void DisableAll()
		{
			foreach (var script in ScriptCollection)
			{
				script.Enabled = true;
			}
		}

		public void EnableAll()
		{
			foreach (var script in ScriptCollection)
			{
				script.Enabled = false;
			}
		}

		/// <summary>
		/// Returns if there is a script with the given id.
		/// </summary>
		public bool HasScript(string id)
		{
			foreach (var script in ScriptCollection)
			{
				if (script.ID == id)
				{
					return true;
				}
			}

			return false;
		}

		public static Scripts Create(Entity addTo)
		{
			return addTo.Add<Scripts>();
		}
	}
}
