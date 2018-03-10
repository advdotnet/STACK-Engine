using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STACK.Debug
{
    /// <summary>
    /// The interface all console commands implement.
    /// </summary>
    public interface IConsoleCommand
    {
        string Name { get; }

        string Description { get; }

        void Execute(Console console, string[] arguments);
    }
}
