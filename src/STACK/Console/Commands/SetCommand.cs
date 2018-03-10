using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace STACK.Debug
{
    /// <summary>
    /// Sets a variable.
    /// </summary>
    class SetCommand : IConsoleCommand
    {
        public string Name
        {
            get
            {
                return "set";
            }
        }

        public string Description
        {
            get
            {
                return "Sets a variable.";
            }
        }

        private readonly StackEngine Engine;

        public SetCommand(StackEngine game)
        {
            Engine = game;            
        }

        public static T Parse<T>(string value)
        {
            return (T)System.ComponentModel.TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
        }

        public void Execute(Console console, string[] arguments)
        {
            if (arguments.Length == 3 && arguments[1] == "=" && arguments[0].Contains("."))
            {

                var VariableName = arguments[0].Split('.')[1].ToUpperInvariant();
                var Properties = typeof(EngineVariables).GetFields();                
                string Value = arguments[2].Trim();                

                foreach (var prop in Properties)
                {
                    if (prop.Name.ToUpperInvariant() == VariableName)
                    {
                        try
                        {
                            Type Test = prop.FieldType;

                            MethodInfo method = typeof(SetCommand).GetMethod("Parse").MakeGenericMethod(new Type[] { Test });

                            object Result = method.Invoke(this, new object[] { Value });

                            prop.SetValue(null, Result);
                            Value = prop.GetValue(null).ToString();

                            console.WriteLine(Value, Console.Channel.System);
                        }
                        catch
                        {
                            console.WriteLine("Could not set value.", Console.Channel.Error);
                        }

                        return;
                    }
                }

                console.WriteLine("Variable not found.", Console.Channel.Error);
            }
            else
            {
                console.WriteLine("Syntax is SET <namespace>.<variable> = <value>.", Console.Channel.Error);
            }
        }
    }
}
