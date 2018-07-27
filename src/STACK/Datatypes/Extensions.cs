using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using STACK.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace STACK
{
    public static class ExtensionMethods
    {
        public static Vector3 ToInt(this Vector3 value)
        {
            return new Vector3((int)value.X, (int)value.Y, (int)value.Z);
        }

        public static Vector2 XYToVector2(this Vector3 value)
        {
            return new Vector2(value.X, value.Y);
        }

        public static Vector2 ToInt(this Vector2 value)
        {
            return new Vector2((int)value.X, (int)value.Y);
        }

        public static Vector2 ToVector2(this Point value)
        {
            return new Vector2(value.X, value.Y);
        }

        public static Point ToPoint(this Vector2 value)
        {
            return new Point((int)value.X, (int)value.Y);
        }

        /// <summary>
        /// Required for FNA.
        /// </summary>
        public static bool Contains(this Rectangle rect, Vector2 point)
        {
            return rect.Contains(new Point((int)point.X, (int)point.Y));
        }

        public static MouseState UpdatePosition(this MouseState state, int x, int y)
        {
            return new MouseState(x, y, state.ScrollWheelValue, state.LeftButton,
                state.MiddleButton, state.RightButton, state.XButton1, state.XButton2);
        }

        public static bool IsPlaying(this SoundEffectInstance sound)
        {
            if (null == sound)
            {
                return false;
            }

            return (SoundState.Playing == sound.State);
        }

        public static bool Has(this Alignment alignment, Alignment value)
        {
            return (alignment & value) == value;
        }

        public static bool Has(this SpriteEffects spriteEffects, SpriteEffects value)
        {
            return (spriteEffects & value) == value;
        }

        /// <summary>
        /// Searches the current AppDomain for components implementing IWorldAutoAdd and
        /// adds them to the World.
        /// </summary>
        /// <param name="world"></param>
        public static void AutoAddWorldComponents(World world)
        {
            var Assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var Types = Assemblies.SelectMany(a => GetLoadableTypes(a));

            var q = from t in Types
                    where t.IsClass && typeof(IWorldAutoAdd).IsAssignableFrom(t) &&
                        !t.IsAbstract && typeof(Component).IsAssignableFrom(t)
                    select t;

            foreach (var ComponentType in q)
            {
                Log.WriteLine("Adding World Component: " + ComponentType.Name);
                var method = typeof(World).GetMethod("Add", new Type[] { });
                method.MakeGenericMethod(ComponentType).Invoke(world, null);
            }
        }

        /// <summary>
        /// Circumvent ReflectionTypeLoadException in mono.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}
