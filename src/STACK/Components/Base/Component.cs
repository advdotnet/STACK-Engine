using System;
using System.Runtime.Serialization;

namespace STACK
{
	[Serializable]
	public abstract class Component
	{
		private BaseEntityCollection _parent;
		[NonSerialized]
		private Entity _castedEntity = null;
		[NonSerialized]
		private Scene _castedScene = null;
		[NonSerialized]
		private World _parentWorld = null;

		public BaseEntityCollection Parent
		{
			get => _parent;
			internal set
			{
				_parent = value;
				CacheTransients();
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext c)
		{
			CacheTransients();
		}

		private void CacheTransients()
		{
			_castedEntity = _parent as Entity;
			_castedScene = _parent as Scene;
			_parentWorld = _parent as World;
		}

		public Entity Entity => _castedEntity;

		public Scene Scene => _castedScene;

		public World World => _parentWorld;

		public T Get<T>() where T : Component
		{
			return Entity.Get<T>();
		}

		public T GetInterface<T>()
		{
			return Entity.GetInterface<T>();
		}
	}
}
