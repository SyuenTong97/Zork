﻿using System;
using System.Linq;
using Newtonsoft.Json;

namespace Zork.Common
{
    public class Game
    {
        public World World { get; }

        [JsonIgnore]
        public Player Player { get; }

        [JsonIgnore]
        public IInputService Input { get; private set; }

        [JsonIgnore]
        public IOutputService Output { get; private set; }

        [JsonIgnore]
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

            IsRunning = true;
            Input.InputReceived += OnInputReceived;
            Output.WriteLine("Welcome to Zork!");
            Look();
            Output.WriteLine(Player.CurrentRoom);
        }

        public void OnInputReceived(object sender, string inputString)
        {
            char separator = ' ';
            string[] commandTokens = inputString.Split(separator);

            string verb;
            string subject = null;
            string preposition = null;
            string thisItem = null;
            if (commandTokens.Length == 0)
            {
                return;
            }
            else if (commandTokens.Length == 1)
            {
                verb = commandTokens[0];
            }
            else if (commandTokens.Length == 2)
            {
                verb = commandTokens[0];
                subject = commandTokens[1];
            }
            else if (commandTokens.Length == 3)
            {
                verb = commandTokens[0];
                subject = commandTokens[1];
                preposition = commandTokens[2];
            }
            else
            {
                verb = commandTokens[0];
                subject = commandTokens[1];
                preposition = commandTokens[2];
                thisItem = commandTokens[3];
            }

            Room previousRoom = Player.CurrentRoom;
            Commands command = ToCommand(verb);
            switch (command)
            {
                case Commands.Quit:
                    IsRunning = false;
                    Output.WriteLine("Thank you for playing!");
                    break;

                case Commands.Look:
                    Look();
                    break;

                case Commands.North:
                case Commands.South:
                case Commands.East:
                case Commands.West:
                    Directions direction = (Directions)command;
                    Output.WriteLine("");
                    Output.WriteLine(Player.Move(direction) ? $"You moved {direction}." : "The way is shut!");
                    break;

                case Commands.Take:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else
                    {
                        Take(subject);
                    }
                    break;

                case Commands.Drop:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else
                    {
                        Drop(subject);
                    }
                    break;

                case Commands.Inventory:
                    if (Player.Inventory.Count() == 0)
                    {
                        Output.WriteLine("You are empty handed.");
                    }
                    else
                    {
                        Output.WriteLine("You are carrying:");
                        foreach (Item item in Player.Inventory)
                        {
                            Output.WriteLine(item.InventoryDescription);
                        }
                    }
                    break;
                case Commands.Reward:
                    Player.Rewards++;
                    break;

                case Commands.Attack:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else if (string.IsNullOrEmpty(preposition))
                    {
                        Output.WriteLine("With what?");
                    }
                    else if (string.IsNullOrEmpty(preposition))
                    {
                        Output.WriteLine("With what?");
                    }
                    else
                    {
                        Attack(subject, preposition, thisItem);
                    }
                    break;

                default:
                    Output.WriteLine("Unknown command.");
                    break;
            }

            if (command != Commands.Unknown)
            {
                Player.Moves++;
            }

            if (ReferenceEquals(previousRoom, Player.CurrentRoom) == false)
            {
                Look();
            }
            Output.WriteLine(Player.CurrentRoom);
            Output.WriteLine("");
        }
        
        private void Look()
        {
            Output.WriteLine(Player.CurrentRoom.Description);
            foreach (Item item in Player.CurrentRoom.Inventory)
            {
                Output.WriteLine(item.LookDescription);
            }
        }

        private void Take(string itemName)
        {
            Item itemToTake = Player.CurrentRoom.Inventory.FirstOrDefault(item => string.Compare(item.Name, itemName, ignoreCase: true) == 0);
            if (itemToTake == null)
            {
                Output.WriteLine("You can't see any such thing.");                
            }
            else
            {
                Player.AddItemToInventory(itemToTake);
                Player.CurrentRoom.RemoveItemFromInventory(itemToTake);
                Output.WriteLine("Taken.");
            }
        }

        private void Drop(string itemName)
        {
            Item itemToDrop = Player.Inventory.FirstOrDefault(item => string.Compare(item.Name, itemName, ignoreCase: true) == 0);
            if (itemToDrop == null)
            {
                Output.WriteLine("You can't see any such thing.");                
            }
            else
            {
                Player.CurrentRoom.AddItemToInventory(itemToDrop);
                Player.RemoveItemFromInventory(itemToDrop);
                Output.WriteLine("Dropped.");
            }
        }

        private void Attack(string subject, string preprosition, string thisItem)
        {
            string correctPreposition = "with";
            Item itemToAttackWith = Player.Inventory.FirstOrDefault(item => string.Compare(item.Name, thisItem, ignoreCase: true) == 0);
            bool weapon = itemToAttackWith.IsWeapon;
            if (string.Compare(preprosition, correctPreposition, ignoreCase: true) == 0)
            {
                Output.WriteLine("With what?");
            }
            else if (itemToAttackWith == null)
            {
                Output.WriteLine("You can't see any such thing.");
            }
            else if (weapon == false)
            {
                Output.WriteLine($"Can't attack with a {thisItem}.");
            }
        }

        private static Commands ToCommand(string commandString) => Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;
    }
}