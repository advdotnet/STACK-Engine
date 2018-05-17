using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace STACK.Components
{
    /// <summary>
    /// Scripts like walk, say and playanimation
    /// </summary>
    [Serializable]
    public static class ActorScripts
    {
        public const string GOTOSCRIPTID = "goto";

        public static Script GoTo(this Scripts scripts, float x, float y, Directions8 direction = Directions8.None, Action cb = null)
        {
            return GoTo(scripts, new Vector2(x, y), direction, cb);
        }

        public static Script GoTo(this Scripts scripts, Vector2 target, Directions8 direction = Directions8.None, Action cb = null)
        {
            if (scripts.HasScript(GOTOSCRIPTID))
            {
                scripts.Remove(GOTOSCRIPTID);
                //throw new Exception();
            }
            return scripts.Enqueue(GoToScript(scripts, target, direction, cb), GOTOSCRIPTID);
        }

        public static Script Say(this Scripts scripts, string text, float duration = 0)
        {
            return scripts.Start(SayScript(scripts, text, duration), "say");
        }

        public static Script PlayAnimation(this Scripts scripts, string animation, bool looped = false)
        {
            return scripts.Start(PlayAnimationScript(scripts, animation, looped), "animation");
        }

        private static IEnumerator GoToScript(Scripts scripts, Vector2 target, Directions8 direction = Directions8.None, Action cb = null)
        {
            var Navigation = scripts.Get<Navigation>();
            var Transform = scripts.Get<Transform>();

            if (Transform == null)
            {
                yield break;
            }

            List<Vector2> Waypoints;

            if (Navigation == null)
            {
                Waypoints = new List<Vector2>() { target };
            }
            else
            {
                Navigation.FindPath(target);
                Waypoints = Navigation.WayPoints;
            }

            foreach (var WayPoint in Waypoints)
            {
                Vector2 Target;
                while ((Target = WayPoint - Transform.Position).Length() > 1)
                {
                    Transform.AddState(State.Walking);
                    Transform.Orientation = Target;

                    // prevents loops for high walking speeds
                    var Increment = Transform.Orientation * GameSpeed.TickDuration * Transform.EffectiveSpeed;
                    var Speeding = Increment.LengthSquared() > Target.LengthSquared();

                    Transform.Position = (Transform.Position + (Speeding ? Target : Increment));

                    yield return 0;
                }
            }

            if (direction != Directions8.None)
            {
                Transform.Direction8 = direction;
                yield return 0;
            }

            Transform.RemoveState(State.Walking);

            cb?.Invoke();
        }

        private static IEnumerator PlayAnimationScript(Scripts scripts, string animation, bool looped = false)
        {
            var PlayAnimation = scripts.GetInterface<IPlayAnimation>();

            if (PlayAnimation != null)
            {
                PlayAnimation.PlayAnimation(animation, looped);

                var Transform = scripts.Get<Transform>();

                if (null != Transform)
                {
                    Transform.State = State.Custom;
                }

                while (PlayAnimation.Playing)
                {
                    yield return 1;
                }

                if (null != Transform)
                {
                    Transform.State = State.Idle;
                }

                yield break;
            }
        }

        private static IEnumerator SayScript(Scripts scripts, string text, float duration = TextDuration.Auto)
        {
            var Speaker = scripts.Get<Text>();
            var SkipText = ((Entity)scripts.Parent).World.Get<SkipContent>().SkipText;

            if (Speaker == null)
            {
                yield break;
            }

            // only allow skipping when a text is already presented
            if (null != SkipText && SkipText.Enabled)
            {
                SkipText.Stop();
            }

            var Transform = scripts.Get<Transform>();
            Vector2 Position;

            if (Transform != null && !Transform.State.Has(State.Talking))
            {
                Transform.AddState(State.Talking);
                Position = Transform.Position - Speaker.Offset;
            }
            else
            {
                Position = -Speaker.Offset;
            }

            Speaker.Set(text, duration, Position);

            float Elapsed = 0;
            float Duration = TextDuration.Default(text, duration);

            while (Elapsed < Duration + 2 * Speaker.FadeDuration && (null == SkipText || !SkipText.Enabled))
            {
                Elapsed += GameSpeed.TickDuration;
                yield return 1;
            }

            Transform?.RemoveState(State.Talking);

            // needed for skipping
            Speaker.Clear();
            SkipText?.Stop();
        }
    }
}
