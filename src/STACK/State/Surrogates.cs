using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace STACK.Surrogates
{
    class StackSurrogateSelector : ISurrogateSelector
    {
        ISurrogateSelector _next;

        List<Type> SerializedTypes = new List<Type>()
        {
            typeof(System.Collections.IEnumerator),
            typeof(Microsoft.Xna.Framework.GameTime),
            typeof(Microsoft.Xna.Framework.Point),
            typeof(Microsoft.Xna.Framework.Vector2),
            typeof(Microsoft.Xna.Framework.Vector3),
            typeof(Microsoft.Xna.Framework.Rectangle),
            typeof(Microsoft.Xna.Framework.Matrix),
            typeof(Microsoft.Xna.Framework.Color),
            typeof(Microsoft.Xna.Framework.Audio.AudioListener),
            typeof(Microsoft.Xna.Framework.Audio.AudioEmitter)
        };

        public void ChainSelector(ISurrogateSelector selector)
        {
            _next = selector;
        }

        public ISurrogateSelector GetNextSelector()
        {
            return _next;
        }

        public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            if (SerializedTypes.Any(t => t.IsAssignableFrom(type)))
            {
                selector = this;
                return new FieldSerializationSurrogate();
            }
            else
            {
                if (_next == null)
                {
                    selector = null;
                    return null;
                }
                else
                {
                    return _next.GetSurrogate(type, context, out selector);
                }
            }
        }
    }

    class FieldSerializationSurrogate : ISerializationSurrogate
    {
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            foreach (var FieldInfo in obj.GetType().GetFields(Flags))
            {
                info.AddValue(FieldInfo.Name, FieldInfo.GetValue(obj));
            }
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            foreach (var FieldInfo in obj.GetType().GetFields(Flags))
            {
                FieldInfo.SetValue(obj, info.GetValue(FieldInfo.Name, FieldInfo.FieldType));
            }

            return obj;
        }
    }
}
