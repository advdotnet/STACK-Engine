using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using STACK;
using STACK.Components;
using STACK.Graphics;
using StarFinder;
using System;
using System.Collections.Generic;

namespace PathFinding
{

    /// <summary>
    /// Sample scene containing a ScenePath.
    /// </summary>
    [Serializable]
    public class Scene : STACK.Scene
    {
        private List<Vector2> Waypoints = new List<Vector2>(5);

        public Scene()
        {
            Enabled = true;
            Visible = true;

            InputDispatcher
                .Create(this)
                .SetOnMouseUpFn(OnMouseUp)
                .SetOnMouseMoveFn(OnMouseMove)
                .SetOnKeyDownFn(OnKeyDown);

            ScenePath
                .Create(this)
                .SetPath(CreatePath());

            Push(new Mouse());
        }

        /// <summary>
        /// Creates a sample path.
        /// </summary>
        /// <returns></returns>
        private Path CreatePath()
        {
            var Points = new PathVertex[9 + 5 + 1];
            Points[0] = new PathVertex(10, 10);
            Points[1] = new PathVertex(50, 10);
            Points[2] = new PathVertex(10, 50);
            Points[3] = new PathVertex(50, 55);
            Points[4] = new PathVertex(25, 75);
            Points[5] = new PathVertex(125, 125);
            Points[6] = new PathVertex(250, 250);
            Points[7] = new PathVertex(200, 35);
            Points[8] = new PathVertex(250, 10);
            Points[9] = new PathVertex(300, 50);
            Points[10] = new PathVertex(350, 10);
            Points[11] = new PathVertex(450, 200);
            Points[12] = new PathVertex(301, 200);
            Points[13] = new PathVertex(350, 350);
            Points[14] = new PathVertex(250, 350);


            int[] Indices = new int[45];
            Indices[0] = 0; Indices[1] = 1; Indices[2] = 2;
            Indices[3] = 1; Indices[4] = 2; Indices[5] = 3;
            Indices[6] = 2; Indices[7] = 3; Indices[8] = 4;
            Indices[9] = 3; Indices[10] = 4; Indices[11] = 5;
            Indices[12] = 4; Indices[13] = 5; Indices[14] = 6;
            Indices[15] = 5; Indices[16] = 6; Indices[17] = 7;
            Indices[18] = 6; Indices[19] = 7; Indices[20] = 8;
            Indices[21] = 6; Indices[22] = 8; Indices[23] = 9;
            Indices[24] = 8; Indices[25] = 9; Indices[26] = 10;
            Indices[27] = 9; Indices[28] = 10; Indices[29] = 11;
            Indices[30] = 9; Indices[31] = 12; Indices[32] = 11;
            Indices[33] = 11; Indices[34] = 12; Indices[35] = 13;
            Indices[36] = 6; Indices[37] = 14; Indices[38] = 4;
            Indices[39] = 14; Indices[40] = 6; Indices[41] = 13;
            Indices[42] = 12; Indices[43] = 6; Indices[44] = 13;

            var Collection = new Mesh<TriangleVertexData>(Points, Indices);

            return new Path(Points, Indices);
        }

        public override void OnDraw(Renderer renderer)
        {
            renderer.SpriteBatch.DrawString(renderer.DefaultFont, "Press mouse button to set source.", new Vector2(135, 370), Color.White);

            if (renderer.Stage != RenderStage.PostBloom)
            {
                renderer.PrimitivesRenderer.DrawRectangle(new Rectangle(0, 0, PathFindingGame.VIRTUAL_WIDTH, PathFindingGame.VIRTUAL_HEIGHT), Color.Black);
                renderer.PrimitivesRenderer.DrawRectangle(new Rectangle((int)LastClickPosition.X - 1, (int)LastClickPosition.Y - 1, 2, 2), Color.Red);
                for (int i = 0; i < Waypoints.Count - 1; i++)
                {
                    renderer.PrimitivesRenderer.DrawLine(Waypoints[i], Waypoints[i + 1], Color.Green);
                }
            }

            base.OnDraw(renderer);

        }

        private void OnKeyDown(Keys key)
        {
            if (key == Keys.Escape)
            {
                StackGame.Engine.Exit();
            }
        }

        private Vector2 LastClickPosition = new Vector2(20, 20);

        private void OnMouseUp(Vector2 position, MouseButton button)
        {
            if (Get<ScenePath>().Path.Contains(position))
            {
                LastClickPosition = position;
            }
        }

        private void OnMouseMove(Vector2 position)
        {
            Get<ScenePath>().Path.FindPath(LastClickPosition, position, ref Waypoints);
        }
    }
}
