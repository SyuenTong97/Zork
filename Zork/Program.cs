using System;
using System.Collections.Generic;
using System.IO;

namespace Zork
{
    class Program
    {
        private static Room CurrentRoom
        {
            get
            {
                return _rooms[_location.Vertical, _location.Horizontal];
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Zork!");
            const string defaultRoomsFilename = "Rooms.txt";
            string roomsFilename = (args.Length > 0 ? args[(int)CommandLineArguments.RoomsFilename] : defaultRoomsFilename);
            InitalizeRoomDescriptions(roomsFilename);

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

        static Program()
        {
            RoomMap = new Dictionary<string, Room>();
            foreach (Room room in _rooms)
            {
                RoomMap[room.Name] = room;
            }
        }

        private static bool Move(Commands command)
        {
            bool didMove = false;
            switch (command)
            {
                case Commands.North when _location.Vertical < _rooms.GetLength(1) - 1:
                    _location.Vertical++;
                    didMove = true;
                    break;
                case Commands.South when _location.Vertical > 0:
                    _location.Vertical--;
                    didMove = true;
                    break;

                case Commands.East when _location.Horizontal < _rooms.GetLength(0) - 1:
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

        private static void InitalizeRoomDescriptions(string roomsFilename)
        {
            const string fieldDelimiter = "##";
            const int expectedFieldCount = 2;

            string[] lines = File.ReadAllLines(roomsFilename);
            foreach (string line in lines)
            {
                string[] fields = line.Split(fieldDelimiter);
                if (fields.Length != expectedFieldCount)
                {
                    throw new InvalidDataException("Invalid record.");
                }

                string name = fields[(int)Fields.Name];
                string description = fields[(int)Fields.Description];

                RoomMap[name].Description = description;
            }
        }

        private enum Fields
        {
            Name = 0,
            Description
        }

        private enum CommandLineArguments
        {
            RoomsFilename = 0
        }

        private static readonly Room[,] _rooms =
        {
            { new Room("Rocky Trail"), new Room("South of House"), new Room("Canyon View") },
            { new Room("Forest"), new Room("West of House"), new Room("Behind House") },
            { new Room("Dense Woods"), new Room("North of House"), new Room("Clearing") }
        };

        private static (int Vertical, int Horizontal) _location = (1, 1);
        private static readonly Dictionary<string, Room> RoomMap;

    }
}