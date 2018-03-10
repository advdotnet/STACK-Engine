using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace STACK.Debug
{
    /// <summary>
    /// Exits the game.
    /// </summary>
    class ExitCommand : IConsoleCommand
    {
        public string Name
        {
            get
            {
                return "exit";
            }
        }

        public string Description
        {
            get
            {
                return "Exits the game.";
            }
        }

        private readonly StackEngine Engine;

        public ExitCommand(StackEngine game)
        {
            Engine = game;
        }

        public void Execute(Console console, string[] arguments)
        {
            Engine.Exit();            
        }
    }
}
