using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace STACK.Debug
{
    /// <summary>
    /// Prints information about avaiable console commands.
    /// </summary>
    class HelpCommand : IConsoleCommand
    {
        public string Name
        {
            get
            {
                return "help";
            }
        }

        public string Description
        {
            get
            {
                return "Lists all avaiable commands. If a command name is given as a parameter, it shows help about that command.";
            }
        }

        private readonly StackEngine Engine;

        public HelpCommand(StackEngine game)
        {
            Engine = game;            
        }

        public void Execute(Console console, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                console.WriteLine("Avaiable Commands:");

                foreach (var Command in console.Commands)
                {
                    console.WriteLine(" " + Command.Name);
                }
            }            
            else if (arguments.Length == 1)
            {
                var Command = console.Commands.Where(c => c.Name.ToUpperInvariant() == arguments[0].ToUpperInvariant()).FirstOrDefault();

                if (Command == null) 
                {
                    console.WriteLine("Command not found.", Console.Channel.Error);
                } 
                else 
                {
                    console.WriteLine(Command.Description);
                }                
            }
        }
    }
}
