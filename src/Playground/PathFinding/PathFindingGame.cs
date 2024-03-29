﻿using Microsoft.Xna.Framework;
using STACK;
using STACK.Components;
using System.Collections.Generic;

namespace PathFinding
{
	/// <summary>
	/// The game class
	/// </summary>
	public class PathFindingGame : StackGame
	{
		public const int VIRTUAL_WIDTH = 640;
		public const int VIRTUAL_HEIGHT = 400;

		public PathFindingGame()
		{
			VirtualResolution = new Point(VIRTUAL_WIDTH, VIRTUAL_HEIGHT);
			Title = "Playground: PathFinding";
			EngineVariables.DebugPath = true;
		}

		protected override List<STACK.Scene> GetScenes() => new List<STACK.Scene>() { new Scene() };

		protected override void OnStart()
		{
			StartWorld();
			World.Get<RenderSettings>().BloomEnabled = false;
			Engine.Resume();
		}
	}
}
