using System;
using Zork.Common;

namespace Zork.Cli
{
    internal class ConsoleInputService : IInputService
    {
        public EventHandler<string> InputReceived;
        public void ProcessInput()
        {
            string inputString = Console.ReadLine().Trim();
            InputReceived?.Invoke(this, inputString);
        }
    }

}
