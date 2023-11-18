using System.Collections.Generic;

namespace STACK
{
	public class EmptyGame : StackGame
	{
		protected override List<Scene> GetScenes()
		{
			return new List<Scene> { new Scene("1") };
		}

		protected override void OnStart()
		{
			StartWorld();
		}
	}
}
