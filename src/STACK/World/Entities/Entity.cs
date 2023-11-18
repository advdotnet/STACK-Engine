using STACK.Components;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace STACK
{
	[Serializable]
	[DebuggerDisplay("ID = {ID}")]
	public class Entity : BaseEntityCollection
	{
		public Scene _DrawScene;
		[NonSerialized]
		private Scene _updateScene;

		public Scene DrawScene
		{
			get => _DrawScene;
			set
			{
				if (value != _DrawScene)
				{
					var previous = _DrawScene;
					_DrawScene = value;

					if (_DrawScene != null && _DrawScene.Initialized)
					{
						_DrawScene.GameObjectCache.CacheVisibleObjects();
						_DrawScene.GameObjectCache.CacheObjectsToDraw();
					}

					if (previous != null && previous.Initialized)
					{
						previous.GameObjectCache.CacheVisibleObjects();
						previous.GameObjectCache.CacheObjectsToDraw();
					}
				}
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext c)
		{
			_updateScene = (Scene)Parent;
		}

		public World World => _updateScene.World;

		public Scene UpdateScene
		{
			get => _updateScene;
			set
			{
				if (value != _updateScene)
				{
					var previous = _updateScene;
					Parent = value;
					_updateScene = value;

					previous?.Pop(this);
					if (null != _updateScene && !_updateScene.Items.Contains(this))
					{
						_updateScene.Push(this);
					}
				}
			}
		}

		public Entity()
		{

		}

		public Entity(string id) : this()
		{
			if (!string.IsNullOrEmpty(id))
			{
				_ID = id;
			}
		}

		public void EnterScene(string name)
		{
			EnterScene(World[name]);
		}

		public virtual void EnterScene(Scene scene)
		{
			DrawScene.Notify(Messages.EntityLeavesScene, this);
			Notify(Messages.SceneEnter, scene);
			DrawScene = scene;
			Notify(Messages.SceneEntered, scene);
			DrawScene.Notify(Messages.EntityEntersScene, this);
		}

		public override void OnPropertyChanged(string property)
		{
			if (Properties.DrawOrder == property || Properties.Visible == property)
			{
				if (UpdateScene != DrawScene)
				{
					if (DrawScene != null && DrawScene.Initialized)
					{
						DrawScene.GameObjectCache.CacheVisibleObjects();
						DrawScene.GameObjectCache.CacheObjectsToDraw(Properties.DrawOrder == property);
					}
				}

				if (UpdateScene != null && UpdateScene.Initialized)
				{
					UpdateScene.GameObjectCache.CacheVisibleObjects();
					UpdateScene.GameObjectCache.CacheObjectsToDraw(Properties.DrawOrder == property);
				}
			}

			base.OnPropertyChanged(property);
		}
	}
}
