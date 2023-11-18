using System;

namespace STACK.Components
{
	/// <summary>
	/// Synchronizes a game object's visible property to the game world's interactive property.
	/// </summary>
	[Serializable]
	public class InteractiveVisibility : Component, IUpdate
	{
		public bool Enabled { get; set; }
		public float UpdateOrder { get; set; }

		public InteractiveVisibility()
		{
			Enabled = true;
		}

		public void Update()
		{
			Entity.Visible = Entity.World.Interactive;
		}

		public static InteractiveVisibility Create(Entity addTo)
		{
			return addTo.Add<InteractiveVisibility>();
		}
	}
}
