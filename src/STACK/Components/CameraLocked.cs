using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
    /// <summary>
    /// When an entity with this component changes its DrawScene it is made sure that 
    /// the new Scene is visible and the old Scene set to invisible.
    /// 
    /// Also the DrawScene's camera will scroll to ensure the entity is in the visible region.
    /// </summary>
    [Serializable]
    public class CameraLocked : Component
    {
        public float Acceleration { get; set; }
        public float Damping { get; set; }
        public bool Scroll { get; set; }
        public bool CenterCharacter { get; set; }

        private bool NewSceneEntered = false;

        public CameraLocked()
        {
            Acceleration = 8f;
            Damping = 3.0f;
            Scroll = true;
            CenterCharacter = true;
        }

        public override void OnNotify<T>(string message, T data)
        {
            if (!Enabled)
            {
                return;
            }

            if (message == Messages.SceneEnter)
            {
                Scene Scene = (Scene)(object)data;

                Entity.DrawScene.Visible = false;
                Entity.DrawScene.Enabled = false;

                Scene.Visible = true;
                Scene.Enabled = true;

                NewSceneEntered = true;
            }
        }

        public override void OnUpdate()
        {
            if (!Scroll)
            {
                return;
            }

            var Position = Get<Transform>().Position;
            var BackgroundObject = Entity.DrawScene.GetObject(Location.BACKGROUND_ENTITY_ID);

            if (null == BackgroundObject)
            {
                return;
            }

            var BackgroundSprite = BackgroundObject.Get<Sprite>();
            var BackgroundWidth = BackgroundSprite.Texture.Width / BackgroundSprite.Columns;
            var BackgroundHeight = BackgroundSprite.Texture.Height / BackgroundSprite.Rows;
            var Camera = Entity.DrawScene.Get<Camera>();
            var TransformedPosition = Camera.Transform(Position);
            var Resolution = Entity.World.Get<RenderSettings>().VirtualResolution;
            var Delta = Vector2.Zero;

            if (NewSceneEntered && CenterCharacter)
            {
                var NewX = Position.X - Resolution.X / 2;
                NewX = Math.Max(0, NewX);
                NewX = Math.Min(BackgroundWidth - Resolution.X, NewX);

                var NewY = Position.Y - Resolution.Y / 2;
                NewY = Math.Max(0, NewY);
                NewY = Math.Min(BackgroundHeight - Resolution.Y, NewY);

                Camera.Position = new Vector2(NewX, NewY);

                NewSceneEntered = false;
            }

            var ShouldScrollLeft = Camera.Position.X > 0 && TransformedPosition.X < Resolution.X / 2f - (Resolution.X / 15f);
            var ShouldScrollRight = Camera.Position.X < BackgroundWidth - Resolution.X && TransformedPosition.X > Resolution.X - Resolution.X / 2f + (Resolution.X / 15f);

            if (ShouldScrollLeft || ShouldScrollRight)
            {
                Delta = new Vector2((TransformedPosition.X / (Resolution.X / 2f) - 1) * Acceleration, 0);
                float Damp = (ShouldScrollLeft ? Camera.Position.X : BackgroundWidth - Resolution.X - Camera.Position.X) / (Resolution.X / 2f);
                Delta *= Math.Min(1, Damp * Damping);
            }
            else
            {
                Camera.Position = Camera.Position.ToInt();
            }

            Camera.Move(Delta);
        }

        public static CameraLocked Create(Entity addTo)
        {
            return addTo.Add<CameraLocked>();
        }

        public CameraLocked SetAcceleration(float value) { Acceleration = value; return this; }
        public CameraLocked SetDamping(float value) { Damping = value; return this; }
        public CameraLocked SetScroll(bool value) { Scroll = value; return this; }
        public CameraLocked SetCenterCharacter(bool value) { CenterCharacter = value; return this; }
    }
}
