using System.Reflection;
using System.Runtime.Serialization;

namespace STACK.Surrogates
{
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
