using STACK.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace STACK
{
    public delegate Script InteractionFn(InteractionContext context);

    /// <summary>
    /// Represents possible interactions with game objects.
    /// </summary>
    [Serializable]
    public class Interactions : Dictionary<object, Dictionary<Verb, InteractionFn>>
    {
        public const string DEFAULTSCRIPTNAME = "DefaultInteractionScript";

        private Interactions() : base() { }

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
        /// <param name="actor">entity to execute the script on</param>
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
            return (ctx) => actor.Get<Scripts>().Start(script, DEFAULTSCRIPTNAME);
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
