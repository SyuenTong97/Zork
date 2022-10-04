using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Zork
{
    class Program
    {
        private static Room CurrentRoom
        {
            get
            {
                return Rooms[_location.Vertical, _location.Horizontal];
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Zork!");
            const string defaultRoomsFilename = "Rooms.json";
            string roomsFilename = (args.Length > 0 ? args[(int)CommandLineArguments.RoomsFilename] : defaultRoomsFilename);
            InitalizeRooms(roomsFilename);

            Room previousRoom = null;
            bool isRunning = true;
            while (isRunning)
            {
                if (previousRoom != CurrentRoom && CurrentRoom.Visited == false)
                {
                    Console.WriteLine(CurrentRoom.Description);
                    previousRoom = CurrentRoom;
                    CurrentRoom.Visited = true;
                }

                Console.Write($"{CurrentRoom}\n> ");

                string inputString = Console.ReadLine().Trim();
                Commands command = ToCommand(inputString);

                string outputString;
                switch (command)
                {
                    case Commands.Quit:
                        isRunning = false;
                        outputString = "Thank you for playing!";
                        break;
                    case Commands.Look:
                        outputString = CurrentRoom.Description;
                        break;
                    case Commands.North:
                    case Commands.South:
                    case Commands.East:
                    case Commands.West:
                        if (Move(command))
                        {
                            outputString = $"You moved {command}.";
                        }
                        else
                        {
                            outputString = "The way is shut!";
                        }
                        break;

                    default:
                        outputString = "Unknown command.";
                        break;
                }
                Console.WriteLine(outputString);

            }
        }

        static Commands ToCommand(string commandString)
        {
            if (Enum.TryParse<Commands>(commandString, true, out Commands command))
            {
                return command;
            }
            else
            {
                return Commands.Unknown;
            }
        }

        private static bool Move(Commands command)
        {
            bool didMove = false;
            switch (command)
            {
                case Commands.North when _location.Vertical < Rooms.GetLength(1) - 1:
                    _location.Vertical++;
                    didMove = true;
                    break;
                case Commands.South when _location.Vertical > 0:
                    _location.Vertical--;
                    didMove = true;
                    break;

                case Commands.East when _location.Horizontal < Rooms.GetLength(0) - 1:
                    _location.Horizontal++;
                    didMove = true;
                    break;
                case Commands.West when _location.Horizontal > 0:
                    _location.Horizontal--;
                    didMove = true;
                    break;
                default:
                    break;
            }
            return didMove;
        }

        private static void InitalizeRooms(string roomsFilename) =>
            Rooms = JsonConvert.DeserializeObject<Room[,]>(File.ReadAllText(roomsFilename));

        private enum Fields
        {
            Name = 0,
            Description
        }

        private enum CommandLineArguments
        {
            RoomsFilename = 0
        }

        private static Room[,] Rooms;
        private static (int Vertical, int Horizontal) _location = (1, 1);
    }
}