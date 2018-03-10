using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STACK.Debug
{
    /// <summary>
    /// Gets the value of a variable.
    /// </summary>
    class GetCommand : IConsoleCommand
    {
        public string Name
        {
            get
            {
                return "get";
            }
        }

        public string Description
        {
            get
            {
                return "Gets a variable.";
            }
        }

        private readonly StackEngine Engine;

        public GetCommand(StackEngine game)
        {
            Engine = game;
        }

        public void Execute(Console console, string[] arguments)
        {
            if (arguments.Length == 1)
            {
                var props = typeof(EngineVariables).GetFields();

                if (arguments[0].ToUpperInvariant() == "STACK")
                {                    
                    foreach (var prop in props)
                    {
                        console.WriteLine(" " + prop.Name + " = " + prop.GetValue(null), STACK.Debug.Console.Channel.System);
                    }
                }
                else if (arguments[0].ToUpperInvariant().StartsWith("STACK."))
                {
                    string Variable = arguments[0].ToUpperInvariant().Replace("STACK.", "");

                    string Value = "";

                    foreach (var prop in props)
                    {
                        if (prop.Name.ToUpperInvariant() == Variable.Trim())
                        {
                            Value = prop.GetValue(null).ToString();
                        }
                    }

                    console.WriteLine(Value, STACK.Debug.Console.Channel.System);
                }
            }
        }
    }
}
