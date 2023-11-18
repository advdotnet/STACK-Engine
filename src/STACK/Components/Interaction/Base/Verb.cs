using STACK.Components;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace STACK
{
	[Serializable]
	public class Verb
	{
		public string Id { get; protected set; }
		public string Text { get; protected set; }
		public string Preposition { get; protected set; }
		public bool Ditransitive { get; protected set; }
		[NonSerialized]
		private StringBuilder _stringBuilder = new StringBuilder();

		[OnDeserializing]
		private void OnDeserializing(StreamingContext c)
		{
			_stringBuilder = new StringBuilder();
		}

		public override bool Equals(object obj)
		{
			var item = obj as Verb;

			if (item == null)
			{
				return false;
			}

			return (Id == item.Id);
		}

		public static bool operator ==(Verb obj1, Verb obj2)
		{
			if (ReferenceEquals(obj1, obj2))
			{
				return true;
			}

			if (obj1 is null || obj2 is null)
			{
				return false;
			}

			return obj1.Equals(obj2);
		}

		public static bool operator !=(Verb obj1, Verb obj2)
		{
			return !(obj1 == obj2);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Text.GetHashCode();

				hashCode = (hashCode * 397) ^ Preposition.GetHashCode();
				hashCode = (hashCode * 397) ^ Ditransitive.GetHashCode();

				return hashCode;
			}
		}

		protected Verb(string text, string preposition, bool ditransitive)
		{
			Text = text;
			Preposition = preposition;
			Ditransitive = ditransitive;
		}

		public override string ToString()
		{
			return Text;
		}

		private const string _space = " ";

		[NonSerialized]
		private string _lastPrimaryCaption;
		[NonSerialized]
		private bool _lastPrimarySelected;
		[NonSerialized]
		private string _lastSecondaryCaption;
		[NonSerialized]
		private string _lastResult;

		public string CreateActionString(Entity primary = null, bool primarySelected = false, Entity secondary = null)
		{
			var pri = primary?.Get<Hotspot>()?.Caption;
			var sec = secondary?.Get<Hotspot>()?.Caption;

			if (string.IsNullOrEmpty(pri))
			{
				return Text;
			}
			else
			{
				if (_lastPrimaryCaption == pri && _lastPrimarySelected == primarySelected && _lastSecondaryCaption == sec && null != _lastResult)
				{
					return _lastResult;
				}

				_lastPrimaryCaption = pri;
				_lastSecondaryCaption = sec;
				_lastPrimarySelected = primarySelected;

				_stringBuilder.Clear();

				_stringBuilder.Append(Text);
				_stringBuilder.Append(_space);
				_stringBuilder.Append(pri);

				if (!Ditransitive || !primarySelected)
				{
					return _lastResult = _stringBuilder.ToString();
				}

				_stringBuilder.Append(_space);
				_stringBuilder.Append(Preposition);

				if (string.IsNullOrEmpty(sec))
				{
					return _lastResult = _stringBuilder.ToString();
				}

				_stringBuilder.Append(_space);
				_stringBuilder.Append(sec);

				return _lastResult = _stringBuilder.ToString();
			}
		}

		public static Verb Create(string name, string preposition = "", bool ditransitive = false)
		{
			return new Verb(name, preposition, ditransitive);
		}
	}
}
