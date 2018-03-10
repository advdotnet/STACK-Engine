using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STACK.Debug
{
    /// <summary>
    /// Stores a screenshot to the hard disk.
    /// </summary>
    class ScreenshotCommand : IConsoleCommand
    {
        public string Name
        {
            get
            {
                return "screenshot";
            }
        }

        public string Description
        {
            get
            {
                return "Makes a screenshot.";
            }
        }

        private readonly StackEngine Engine;

        public ScreenshotCommand(StackEngine game)
        {
            Engine = game;            
        }

        public void Execute(Console console, string[] arguments)
        {
            //Engine.StackEngine.Renderer.SaveScreenShot(Engine.Game.World);
        }
    }
}
