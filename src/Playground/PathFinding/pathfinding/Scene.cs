using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using STACK;
using STACK.Components;
using STACK.Graphics;
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
		private List<Vector2> _waypoints = new List<Vector2>(5);
		private Vector2 _lastClickPosition = new Vector2(20, 20);

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
			var points = new PathVertex[9 + 5 + 1];
			points[0] = new PathVertex(10, 10);
			points[1] = new PathVertex(50, 10);
			points[2] = new PathVertex(10, 50);
			points[3] = new PathVertex(50, 55);
			points[4] = new PathVertex(25, 75);
			points[5] = new PathVertex(125, 125);
			points[6] = new PathVertex(250, 250);
			points[7] = new PathVertex(200, 35);
			points[8] = new PathVertex(250, 10);
			points[9] = new PathVertex(300, 50);
			points[10] = new PathVertex(350, 10);
			points[11] = new PathVertex(450, 200);
			points[12] = new PathVertex(301, 200);
			points[13] = new PathVertex(350, 350);
			points[14] = new PathVertex(250, 350);


			var indices = new int[45];
			indices[0] = 0; indices[1] = 1; indices[2] = 2;
			indices[3] = 1; indices[4] = 2; indices[5] = 3;
			indices[6] = 2; indices[7] = 3; indices[8] = 4;
			indices[9] = 3; indices[10] = 4; indices[11] = 5;
			indices[12] = 4; indices[13] = 5; indices[14] = 6;
			indices[15] = 5; indices[16] = 6; indices[17] = 7;
			indices[18] = 6; indices[19] = 7; indices[20] = 8;
			indices[21] = 6; indices[22] = 8; indices[23] = 9;
			indices[24] = 8; indices[25] = 9; indices[26] = 10;
			indices[27] = 9; indices[28] = 10; indices[29] = 11;
			indices[30] = 9; indices[31] = 12; indices[32] = 11;
			indices[33] = 11; indices[34] = 12; indices[35] = 13;
			indices[36] = 6; indices[37] = 14; indices[38] = 4;
			indices[39] = 14; indices[40] = 6; indices[41] = 13;
			indices[42] = 12; indices[43] = 6; indices[44] = 13;

			return new Path(points, indices);
		}

		public override void OnDraw(Renderer renderer)
		{
			renderer.SpriteBatch.DrawString(renderer.DefaultFont, "Press mouse button to set source.", new Vector2(135, 370), Color.White);

			if (renderer.Stage != RenderStage.PostBloom)
			{
				renderer.PrimitivesRenderer.DrawRectangle(new Rectangle(0, 0, PathFindingGame.VIRTUAL_WIDTH, PathFindingGame.VIRTUAL_HEIGHT), Color.Black);
				renderer.PrimitivesRenderer.DrawRectangle(new Rectangle((int)_lastClickPosition.X - 1, (int)_lastClickPosition.Y - 1, 2, 2), Color.Red);
				for (var i = 0; i < _waypoints.Count - 1; i++)
				{
					renderer.PrimitivesRenderer.DrawLine(_waypoints[i], _waypoints[i + 1], Color.Green);
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


		private void OnMouseUp(Vector2 position, MouseButton button)
		{
			if (Get<ScenePath>().Path.Contains(position))
			{
				_lastClickPosition = position;
			}
		}

		private void OnMouseMove(Vector2 position)
		{
			Get<ScenePath>().Path.FindPath(_lastClickPosition, position, ref _waypoints);
		}
	}
}
