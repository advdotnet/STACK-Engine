using STACK.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace STACK
{
    [Serializable]
    public class Verb
    {
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

            return (Text == item.Text && Preposition == item.Preposition && Ditransitive == item.Ditransitive);
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode() ^ Preposition.GetHashCode() ^ Ditransitive.GetHashCode();
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
                StringBuilder.Clear();

                StringBuilder.Append(Text);
                StringBuilder.Append(SPACE);
                StringBuilder.Append(pri);

                if (!Ditransitive || !primarySelected)
                {
                    return StringBuilder.ToString();
                }

                StringBuilder.Append(SPACE);
                StringBuilder.Append(Preposition);

                if (string.IsNullOrEmpty(sec))
                {
                    return StringBuilder.ToString();
                }

                StringBuilder.Append(SPACE);
                StringBuilder.Append(sec);

                return StringBuilder.ToString();
            }
        }

        public static Verb Create(string name, string preposition = "", bool ditransitive = false)
        {
            return new Verb(name, preposition, ditransitive);
        }
    }

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

    public delegate Script InteractionFn(InteractionContext context);

    /// <summary>
    /// Represents possible interactions with game objects.
    /// </summary>
    [Serializable]
    public class Interactions : Dictionary<object, Dictionary<Verb, InteractionFn>>
    {
        public const string DEFAULTSCRIPTNAME = "DefaultInteractionScript";

        public Interactions() : base() { }

        public Interactions(SerializationInfo info, StreamingContext context) : base(info, context) { }

        private object CurrentSender = null;

        public Interactions For(object sender)
        {
            CurrentSender = sender;
            return this;
        }

        public Interactions Add(Verb key)
        {
            return Add(key, (a) => { return null; });
        }

        /// <summary>
        /// Executes the given script on the given actor entity.
        /// </summary>
        /// <param name="key">verb</param>
        /// <param name="val">script</param>
        /// <param name="actor">entity</param>
        /// <param name="pred">predicate</param>
        /// <returns></returns>
        public Interactions Add(Verb key, IEnumerator val, Entity actor, Func<bool> pred = null)
        {
            if (pred == null || pred())
            {
                return Add(key, GetDefaultInteractionFn(val, actor), true);
            }

            return this;
        }

        /// <summary>
        /// Executes the given script on the current "for" object.
        /// </summary>
        /// <param name="key">verb</param>
        /// <param name="val">script</param>
        /// <param name="pred">predicate</param>
        /// <returns></returns>
        public Interactions Add(Verb key, IEnumerator val, Func<bool> pred = null)
        {
            if (pred == null || pred())
            {
                return Add(key, GetDefaultInteractionFn(val, (Entity)CurrentSender), true);
            }

            return this;
        }

        public Interactions Add(Verb key, InteractionFn val, Func<bool> pred = null)
        {
            if (pred == null || pred())
            {
                return Add(key, val, true);
            }

            return this;
        }

        public Interactions Add(Verb key, InteractionFn val, bool pred)
        {
            if (CurrentSender != null && pred)
            {
                if (!ContainsKey(CurrentSender))
                {
                    Add(CurrentSender, new Dictionary<Verb, InteractionFn>());
                }

                this[CurrentSender].Add(key, val);
            }

            return this;
        }

        /// <summary>
        /// Default interaction function: start a script for the given object
        /// </summary>
        /// <param name="script"></param>
        /// <param name="actor"></param>
        /// <returns></returns>
        private InteractionFn GetDefaultInteractionFn(IEnumerator script, Entity actor)
        {
            return (ctx) => { return actor.Get<Scripts>().Start(script, DEFAULTSCRIPTNAME); };
        }

        public static Interactions Create()
        {
            return new Interactions();
        }

        public Dictionary<Verb, InteractionFn> GetFor(object sender)
        {
            Dictionary<Verb, InteractionFn> Result;

            if (TryGetValue(sender, out Result))
            {
                return Result;
            }

            return new Dictionary<Verb, InteractionFn>();
        }

        public static Interactions None = new Interactions();
    }
}
