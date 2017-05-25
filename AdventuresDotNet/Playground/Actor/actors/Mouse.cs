using System;
using STACK;
using STACK.Components;

namespace Actor
{
    [Serializable]
    public class Mouse : Entity
    {
        public Mouse()
        {
            Transform
                .Create(this);

            Sprite
                .Create(this)
                .SetImage("rufzeichen", 2)
                .SetFrame(1)
                .SetRenderStage(RenderStage.PostBloom);

            SpriteData
                .Create(this)
                .SetOffset(-20, -20);

            MouseFollower
                .Create(this);

            InteractiveVisibility
                .Create(this);
        }

        public void ShowNormal()
        {
            Get<Sprite>().CurrentFrame = 1;
        }

        public void ShowInvert()
        {
            Get<Sprite>().CurrentFrame = 2;
        }
    }
}
