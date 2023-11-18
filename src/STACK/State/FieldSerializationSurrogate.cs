using System.Reflection;
using System.Runtime.Serialization;

namespace STACK.Surrogates
{
	internal class FieldSerializationSurrogate : ISerializationSurrogate
	{
		private const BindingFlags _flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			foreach (var fieldInfo in obj.GetType().GetFields(_flags))
			{
				info.AddValue(fieldInfo.Name, fieldInfo.GetValue(obj));
			}
		}

		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			foreach (var fieldInfo in obj.GetType().GetFields(_flags))
			{
				fieldInfo.SetValue(obj, info.GetValue(fieldInfo.Name, fieldInfo.FieldType));
			}

			return obj;
		}
	}
}
