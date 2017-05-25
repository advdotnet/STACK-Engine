using System;
using STACK.Components;

namespace STACK
{
    [Serializable]
    public class Location : Scene
    {
        const string BACKGROUND_ID = "background";

        public Location(string image, int columns = 1, int rows = 1)
        {
            var Background = new Entity(BACKGROUND_ID);
            Background.SetPriority(-1);

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
                return this[BACKGROUND_ID];
            }
        }
    }
}
