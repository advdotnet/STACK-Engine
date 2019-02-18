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
        StringBuilder StringBuilder = new StringBuilder();

        [OnDeserializing]
        void OnDeserializing(StreamingContext c)
        {
            StringBuilder = new StringBuilder();
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

            if (ReferenceEquals(obj1, null) || ReferenceEquals(obj2, null))
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

        const string SPACE = " ";

        [NonSerialized]
        string LastPrimaryCaption;
        [NonSerialized]
        bool LastPrimarySelected;
        [NonSerialized]
        string LastSecondaryCaption;
        [NonSerialized]
        string LastResult;

        public string CreateActionString(Entity primary = null, bool primarySelected = false, Entity secondary = null)
        {
            var pri = string.Empty;
            pri = primary?.Get<Hotspot>()?.Caption;
            var sec = string.Empty;
            sec = secondary?.Get<Hotspot>()?.Caption;

            if (string.IsNullOrEmpty(pri))
            {
                return Text;
            }
            else
            {
                if (LastPrimaryCaption == pri && LastPrimarySelected == primarySelected && LastSecondaryCaption == sec && null != LastResult)
                {
                    return LastResult;
                }

                LastPrimaryCaption = pri;
                LastSecondaryCaption = sec;
                LastPrimarySelected = primarySelected;

                StringBuilder.Clear();

                StringBuilder.Append(Text);
                StringBuilder.Append(SPACE);
                StringBuilder.Append(pri);

                if (!Ditransitive || !primarySelected)
                {
                    return LastResult = StringBuilder.ToString();
                }

                StringBuilder.Append(SPACE);
                StringBuilder.Append(Preposition);

                if (string.IsNullOrEmpty(sec))
                {
                    return LastResult = StringBuilder.ToString();
                }

                StringBuilder.Append(SPACE);
                StringBuilder.Append(sec);

                return LastResult = StringBuilder.ToString();
            }
        }

        public static Verb Create(string name, string preposition = "", bool ditransitive = false)
        {
            return new Verb(name, preposition, ditransitive);
        }
    }
}
