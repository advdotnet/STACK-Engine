using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace STACK.Debug
{
    /// <summary>
    /// Resumes the game.
    /// </summary>
    class ResumeCommand : IConsoleCommand
    {
        public string Name
        {
            get
            {
                return "resume";
            }
        }

        public string Description
        {
            get
            {
                return "Resumes the game.";
            }
        }

        private readonly StackEngine Engine;

        public ResumeCommand(StackEngine game)
        {
            Engine = game;
        }

        public void Execute(Console console, string[] arguments)
        {
            if (Engine.Game != null && Engine.Game.World != null)
            {
                Engine.Game.World.Interactive = true;
            }
        }
    }
}
