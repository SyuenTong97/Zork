using System;

namespace Zork.Common
{
    public class Game
    {
        public World World { get; }

        public Player Player { get; }

        public IInputService Input { get; private set; }

        public IOutputService Output { get; private set; }

        public Game(World world, string startingLocation)
        {
            World = world;
            Player = new Player(World, startingLocation);
        }

        public void Run(IInputService input, IOutputService output)
        {
            Output = output;

            Room previousRoom = null;
            bool isRunning = true;
            while (isRunning)
            {
                Output.WriteLine($"\n{Player.CurrentRoom}");
                if (previousRoom != Player.CurrentRoom)
                {
                    Output.WriteLine(Player.CurrentRoom.Description);
                    foreach (Item item in Player.CurrentRoom.Inventory)
                    {
                        Output.WriteLine(item.Description);
                    }
                    Output.Write("\n");
                    previousRoom = Player.CurrentRoom;
                }

                Output.Write("> ");

                string inputString = Console.ReadLine().Trim();
                char  separator = ' ';
                string[] commandTokens = inputString.Split(separator);
                
                string verb = null;
                string subject = null;
                if (commandTokens.Length == 0)
                {
                    continue;
                }
                else if (commandTokens.Length == 1)
                {
                    verb = commandTokens[0];

                }
                else
                {
                    verb = commandTokens[0];
                    subject = commandTokens[1];
                }

                Commands command = ToCommand(verb);
                string outputString;
                switch (command)
                {
                    case Commands.Quit:
                        isRunning = false;
                        outputString = "Thank you for playing!";
                        break;

                    case Commands.Look:
                        output.WriteLine(Player.CurrentRoom.Description);
                        foreach (Item item in Player.CurrentRoom.Inventory)
                        {
                            output.WriteLine(item.Description);
                        }
                        outputString = null;
                        break;

                    case Commands.North:
                    case Commands.South:
                    case Commands.East:
                    case Commands.West:
                        Directions direction = (Directions)command;
                        if (Player.Move(direction))
                        {
                            outputString = $"You moved {direction}.";
                        }
                        else
                        {
                            outputString = "The way is shut!";
                        }
                        break;

                    case Commands.Take:
                        if (commandTokens.Length == 2)
                        {
                            outputString = Player.Take(subject);
                        }
                        else
                        {
                            outputString = "This command requires a subject.";
                        }
                        break;

                    case Commands.Drop:
                        if (commandTokens.Length == 2)
                        {
                            outputString = Player.Drop(subject);
                        }
                        else
                        {
                            outputString = "This command requires a subject.";
                        }
                        break;

                    case Commands.Inventory:
                        if (Player.Inventory.Count >= 1)
                        {
                            Output.WriteLine("You are carrying:");
                            foreach (Item item in Player.Inventory)
                            {
                                Output.WriteLine(item.Description);
                            }
                            outputString = null;
                        }
                        else
                        {
                            outputString = "You are empty handed.";
                        }
                        break;

                    default:
                        outputString = "Unknown command.";
                        break;
                }

                if (outputString != null)
                {
                    Output.WriteLine(outputString);
                }
            }
        }

        private static Commands ToCommand(string commandString) => Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;
    }
}
