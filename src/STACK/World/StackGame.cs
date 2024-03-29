﻿using Microsoft.Xna.Framework;
using STACK.Logging;
using System.Collections.Generic;

namespace STACK
{
	/// <summary>
	/// Class used to provide a list of scenes.
	/// </summary>
	public abstract class StackGame
	{
		public Point VirtualResolution { get; protected set; }
		public string Title { get; protected set; }
		public string SaveGameFolder { get; protected set; }
		public World World;
		public static StackEngine Engine;

		/// <summary>
		/// Scenes are added here.
		/// </summary>
		protected abstract List<Scene> GetScenes();

		public StackGame()
		{
			VirtualResolution = new Point(640, 400);
			SaveGameFolder = "STACK";
			Title = "STACK Game";
		}

		public void Start(StackEngine engine)
		{
			Engine = engine;
			OnStart();
		}

		protected virtual void OnStart() { }
		public virtual void OnExit() { }
		protected virtual void HandleInputEvent() { }
		protected virtual void OnWorldInitialized(bool restore) { }

		public void RestoreState(SaveGame state)
		{
			Log.WriteLine("Loading game " + state.Name);

			var deserializedWorld = State.Serialization.LoadState<World>(state.World);

			UnloadWorld();

			GameSettings.SetCurrentCulture(state.Culture);

			World = deserializedWorld;
			World.Setup(Engine.Services, Engine.InputProvider);
			World.LoadContent(Engine.GetWorldContent());
			World.Initialize(true);
			Engine.ApplyGameSettingsVolume();
			OnWorldInitialized(true);
		}

		public void StartWorld()
		{
			UnloadWorld();

			World = new World(Engine.Services, Engine.InputProvider, VirtualResolution, GetScenes());
			World.LoadContent(Engine.GetWorldContent());
			World.Initialize(false);
			Engine.ApplyGameSettingsVolume();
			OnWorldInitialized(false);
		}

		public void UnloadWorld()
		{
			if (World != null)
			{
				if (World.Loaded)
				{
					World.UnloadContent();
				}

				World.Unsubscribe(Engine.InputProvider);

				World = null;
			}
		}

		public static EmptyGame Empty => new EmptyGame();
	}
}
