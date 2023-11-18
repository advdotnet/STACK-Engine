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
			var navigation = scripts.Get<Navigation>();
			var transform = scripts.Get<Transform>();

			if (transform == null)
			{
				yield break;
			}

			List<Vector2> waypoints;

			if (navigation == null)
			{
				waypoints = new List<Vector2>() { target };
			}
			else
			{
				navigation.FindPath(target);
				waypoints = navigation.WayPoints;
			}

			foreach (var wayPoint in waypoints)
			{
				Vector2 actualTarget;
				while ((actualTarget = wayPoint - transform.Position).Length() > 1)
				{
					transform.AddState(State.Walking);
					transform.Orientation = actualTarget;

					// prevents loops for high walking speeds
					var increment = transform.Orientation * GameSpeed.TickDuration * transform.EffectiveSpeed;
					var speeding = increment.LengthSquared() > actualTarget.LengthSquared();

					transform.Position += (speeding ? actualTarget : increment);

					yield return 0;
				}
			}

			if (direction != Directions8.None)
			{
				transform.Direction8 = direction;
				yield return 0;
			}

			transform.RemoveState(State.Walking);

			cb?.Invoke();
		}

		private static IEnumerator PlayAnimationScript(Scripts scripts, string animation, bool looped = false)
		{
			var playAnimation = scripts.GetInterface<IPlayAnimation>();

			if (playAnimation != null)
			{
				playAnimation.PlayAnimation(animation, looped);

				var transform = scripts.Get<Transform>();

				if (null != transform)
				{
					transform.State = State.Custom;
				}

				while (playAnimation.Playing)
				{
					yield return 1;
				}

				if (null != transform)
				{
					transform.State = State.Idle;
				}

				yield break;
			}
		}

		private static IEnumerator SayScript(Scripts scripts, string text, float duration = TextDuration.Auto)
		{
			var speaker = scripts.Get<Text>();
			var skipText = ((Entity)scripts.Parent).World.Get<SkipContent>().SkipText;

			if (speaker == null)
			{
				yield break;
			}

			// only allow skipping when a text is already presented
			if (null != skipText && skipText.Enabled)
			{
				skipText.Stop();
			}

			var transform = scripts.Get<Transform>();
			Vector2 position;

			if (transform != null && !transform.State.Has(State.Talking))
			{
				transform.AddState(State.Talking);
				position = transform.Position - speaker.Offset;
			}
			else
			{
				position = -speaker.Offset;
			}

			speaker.Set(text, duration, position);

			float elapsed = 0;
			var normalizedDuration = TextDuration.Default(text, duration);

			while (elapsed < normalizedDuration + 2 * speaker.FadeDuration && (null == skipText || !skipText.Enabled))
			{
				elapsed += GameSpeed.TickDuration;
				yield return 1;
			}

			transform?.RemoveState(State.Talking);

			// needed for skipping
			speaker.Clear();
			skipText?.Stop();
		}
	}
}
