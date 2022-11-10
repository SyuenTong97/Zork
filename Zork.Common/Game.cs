using System;

namespace Zork.Common
{
    public class Game
    {
        public World World { get; }

        public Player Player { get; }

        public IInputService Input { get; private set; }

        public IOutputService Output { get; private set; }

        public bool IsRunning { get; private set; }

        public Game(World world, string startingLocation)
        {
            World = world;
            Player = new Player(World, startingLocation);
        }

        public void Run(IInputService input, IOutputService output)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Output = output ?? throw new ArgumentNullException(nameof(output));

            Input.InputReceived += OnInputReceived;
            IsRunning = true;
            Output.WriteLine(Player.CurrentRoom);
            Output.WriteLine(Player.CurrentRoom.Description);

        }

        private void OnInputReceived(object sender, string inputString)
        {
            Commands command = ToCommand(inputString);

            Room previousRoom = Player.CurrentRoom;
            switch (command)
            {
                case Commands.Quit:
                    IsRunning = false;
                    Output.WriteLine("Thank you for playing!");
                    break;

                case Commands.Look:
                    Output.WriteLine(Player.CurrentRoom.Description);
                    foreach (Item item in Player.CurrentRoom.Inventory)
                    {
                        Output.WriteLine(item.Description);
                    }
                    break;

                case Commands.North:
                case Commands.South:
                case Commands.East:
                case Commands.West:
                    Directions direction = (Directions)command;
                    if (Player.Move(direction))
                    {
                        Output.WriteLine($"You moved {direction}.");
                    }
                    else
                    {
                        Output.WriteLine("The way is shut!");
                    }
                    break;

                case Commands.Take:
                    if (commandTokens.Length == 2)
                    {
                        Output.WriteLine(Player.Take(subject));
                    }
                    else
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    break;

                case Commands.Drop:
                    if (commandTokens.Length == 2)
                    {
                        Output.WriteLine(Player.Drop(subject));
                    }
                    else
                    {
                        Output.WriteLine("This command requires a subject.");
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
                    }
                    else
                    {
                        Output.WriteLine("You are empty handed.");
                    }
                    break;

                default:
                    Output.WriteLine("Unknown command.");
                    break;
            }

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

            //while (isRunning)
            //{
            //    Output.WriteLine($"\n{Player.CurrentRoom}");
            //    if (previousRoom != Player.CurrentRoom)
            //    {
            //        Output.WriteLine(Player.CurrentRoom.Description);
            //        foreach (Item item in Player.CurrentRoom.Inventory)
            //        {
            //            Output.WriteLine(item.Description);
            //        }
            //        Output.Write("\n");
            //        previousRoom = Player.CurrentRoom;
            //    }

            //    Output.Write("> ");

            //    string inputString = Console.ReadLine().Trim();
            //    char separator = ' ';
            //    string[] commandTokens = inputString.Split(separator);

            //    string verb = null;
            //    string subject = null;
            //    if (commandTokens.Length == 0)
            //    {
            //        continue;
            //    }
            //    else if (commandTokens.Length == 1)
            //    {
            //        verb = commandTokens[0];

            //    }
            //    else
            //    {
            //        verb = commandTokens[0];
            //        subject = commandTokens[1];
            //    }

            //    Commands command = ToCommand(verb);
            //    string outputString;
            //    switch (command)
            //    {
            //        case Commands.Quit:
            //            isRunning = false;
            //            outputString = "Thank you for playing!";
            //            break;

            //        case Commands.Look:
            //            output.WriteLine(Player.CurrentRoom.Description);
            //            foreach (Item item in Player.CurrentRoom.Inventory)
            //            {
            //                output.WriteLine(item.Description);
            //            }
            //            outputString = null;
            //            break;

            //        case Commands.North:
            //        case Commands.South:
            //        case Commands.East:
            //        case Commands.West:
            //            Directions direction = (Directions)command;
            //            if (Player.Move(direction))
            //            {
            //                outputString = $"You moved {direction}.";
            //            }
            //            else
            //            {
            //                outputString = "The way is shut!";
            //            }
            //            break;

            //        case Commands.Take:
            //            if (commandTokens.Length == 2)
            //            {
            //                outputString = Player.Take(subject);
            //            }
            //            else
            //            {
            //                outputString = "This command requires a subject.";
            //            }
            //            break;

            //        case Commands.Drop:
            //            if (commandTokens.Length == 2)
            //            {
            //                outputString = Player.Drop(subject);
            //            }
            //            else
            //            {
            //                outputString = "This command requires a subject.";
            //            }
            //            break;

            //        case Commands.Inventory:
            //            if (Player.Inventory.Count >= 1)
            //            {
            //                Output.WriteLine("You are carrying:");
            //                foreach (Item item in Player.Inventory)
            //                {
            //                    Output.WriteLine(item.Description);
            //                }
            //                outputString = null;
            //            }
            //            else
            //            {
            //                outputString = "You are empty handed.";
            //            }
            //            break;

            //        default:
            //            outputString = "Unknown command.";
            //            break;
            //    }

            //    if (outputString != null)
            //    {
            //        Output.WriteLine(outputString);
            //    }
            //}
        }

        private static (Commands command, string subject) ItemCheck(string inputString)
        {
            (Commands command, string subject) result = (Commands.Unknown, string.Empty);
            char separator = ' ';
            string[] commandTokens = inputString.Split(separator);
            switch (commandTokens.Length)
            {
                case 1:
                    result.command = ToCommand(commandTokens[0]);
                    break;
                case 2:
                    result = (ToCommand(commandTokens[0], commandTokens[1]));
                    break;
                default:
                    break;
            }
            return result;
        }

        private static Commands ToCommand(string commandString) => Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;
    }
}
