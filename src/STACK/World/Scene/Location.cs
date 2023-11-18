using STACK.Components;
using System;

namespace STACK
{
	[Serializable]
	public class Location : Scene
	{
		public const string BACKGROUND_ENTITY_ID = "SCENE_BACKGROUND_ENTITY_ID";

		public Location(string image, int columns = 1, int rows = 1)
		{
			var background = new Entity(BACKGROUND_ENTITY_ID);
			background.SetDrawOrder(-2);

			Sprite
				.Create(background)
				.SetImage(image, columns, rows);

			SpriteData
				.Create(background);

			Push(background);
		}

		public Entity Background => this[BACKGROUND_ENTITY_ID];
	}
}
