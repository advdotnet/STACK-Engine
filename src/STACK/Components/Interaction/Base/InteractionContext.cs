using System;

namespace STACK
{
	/// <summary>
	/// Context of a given interaction. Which object has been used by whom on what object using what verb.
	/// </summary>
	[Serializable]
	public struct InteractionContext
	{
		public InteractionContext(Entity sender, Entity primary = null, Entity secondary = null, Verb verb = null, object parameter = null)
		{
			Sender = sender;
			Primary = primary;
			Secondary = secondary;
			Verb = verb;
			Parameter = parameter;
		}

		public Entity Sender { get; private set; }
		public Entity Primary { get; private set; }
		public Entity Secondary { get; private set; }
		public Verb Verb { get; private set; }
		public object Parameter { get; private set; }
	}
}
