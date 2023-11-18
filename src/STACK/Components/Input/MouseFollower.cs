using System;

namespace STACK.Components
{
	/// <summary>
	/// Synchronizes the Position property of a transform component with the mouse coordinates.
	/// </summary>
	[Serializable]
	public class MouseFollower : Component, IUpdate
	{
		private bool _enabled;
		private float _updateOrder;

		public bool Enabled { get => _enabled; set => _enabled = value; }
		public float UpdateOrder { get => _updateOrder; set => _updateOrder = value; }

		public MouseFollower()
		{
			Enabled = true;
		}

		public void Update()
		{
			var position = Entity.UpdateScene.World.Get<Mouse>().Position;
			Get<Transform>().Position = position;
		}

		public static MouseFollower Create(Entity addTo)
		{
			return addTo.Add<MouseFollower>();
		}
	}
}
