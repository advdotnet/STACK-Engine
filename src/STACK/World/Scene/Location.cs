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
            var Background = new Entity(BACKGROUND_ENTITY_ID);
            Background.SetPriority(-2);

            Sprite
                .Create(Background)
                .SetImage(image, columns, rows);

            SpriteData
                .Create(Background);

            Push(Background);
        }

        public Entity Background
        {
            get
            {
                return this[BACKGROUND_ENTITY_ID];
            }
        }
    }
}
