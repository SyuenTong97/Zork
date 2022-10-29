using System;
using System.IO;
using Newtonsoft.Json;

namespace Zork
{
    public class Game
    {
        public World World { get; private set; }

        [JsonIgnore]
        public Player Player { get; private set; }

        [JsonIgnore]
        private bool IsRunning { get; set; }

        public Game(World world, string StartingLocation)
        {
            World = world;
            Player = new Player(World, StartingLocation);
        }

        public void Run()
        {
            IsRunning = true;
            Room previousRoom = null;
            while (IsRunning)
            {
                Console.WriteLine(Player.Location);
                if (previousRoom != Player.Location)
                {
                    Console.WriteLine(Player.Location.Description);
                    foreach (Item item in Player.Location.Inventory)
                    {
                        Console.WriteLine(item.Description);
                    }
                    previousRoom = Player.Location;
                }
                Console.Write("\n> ");

                string inputString = Console.ReadLine().Trim();
                char separator = ' ';
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

                switch (command)
                {
                    case Commands.Quit:
                        IsRunning = false;
                        break;

                    case Commands.Look:
                        Console.WriteLine(Player.Location.Description);
                        foreach (Item item in Player.Location.Inventory)
                        {
                            Console.WriteLine(item.Description);
                        }
                        break;

                    case Commands.North:
                    case Commands.South:
                    case Commands.East:
                    case Commands.West:
                        Directions direction = Enum.Parse<Directions>(command.ToString(), true);
                        if (Player.Move(direction) == false)
                        {
                            Console.WriteLine("The way is shut!\n");
                        }
                        break;

                    case Commands.Take:
                        //if (commandTokens.Length == 2)
                        //{
                        //    if (string.Equals(commandTokens[1], ))
                        //    {
                        //        Console.WriteLine($"Object found. {}\n");
                        //        continue;
                        //    }
                        //    else
                        //    {
                        //        Console.WriteLine("You can't see any such thing.\n");
                        //    }
                        //}
                        //else
                        //{
                        //    Console.WriteLine("This command requires a subject.\n");
                        //}
                        break;
                    case Commands.Drop:
                        break;
                    case Commands.Inventory:
                        if (Player.Inventory.Count >= 1)
                        {
                            foreach (Item item in Player.Inventory)
                            {
                                Console.WriteLine(item.Description);
                            }
                        }
                        else
                        {
                            Console.WriteLine("You are empty handed.\n");
                        }
                        break;

                    default:
                        Console.WriteLine("Unknown command.\n");
                        break;
                }
            }
        }

        private static Commands ToCommand(string commandString) => Enum.TryParse<Commands>(commandString, true, out Commands result) ? result : Commands.Unknown;
    }
}
