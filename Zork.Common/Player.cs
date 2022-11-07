using System;
using System.Collections.Generic;

namespace Zork.Common
{
    public class Player
    {
        public Room CurrentRoom
        {
            get => _currentRoom;
            set => _currentRoom = value;
        }

        public List<Item> Inventory { get; }

        public Player(World world, string startingLocation)
        {
            _world = world;

            if (_world.RoomsByName.TryGetValue(startingLocation, out _currentRoom) == false)
            {
                throw new Exception($"Invalid starting location: {startingLocation}");
            }

            Inventory = new List<Item>();
        }

        public bool Move(Directions direction)
        {
            bool didMove = _currentRoom.Neighbors.TryGetValue(direction, out Room neighbor);
            if (didMove)
            {
                CurrentRoom = neighbor;
            }

            return didMove;
        }

        public string Take(string itemName)
        {
            string outputString = null;
            Item itemToTake = null;
            foreach (Item item in _world.Items)
            {
                if (string.Compare(item.Name, itemName, ignoreCase: true) == 0)
                {
                    itemToTake = item;
                    break;
                }
            }

            if (itemToTake != null)
            {
                bool itemInRoom = false;
                foreach (Item item in _currentRoom.Inventory)
                {
                    if (item == itemToTake)
                    {
                        itemInRoom = true;
                        break;
                    }
                }

                if (itemInRoom == false)
                {
                    outputString = "I see no such thing.";
                }
                else
                {
                    AddToPlayerInventory(itemToTake);
                    _currentRoom.RemoveFromRoomInventory(itemToTake);
                    outputString = $"Taken {itemName}.";
                }
            }
            else
            {
                outputString = "That item does not exist.";
            }
            return outputString;
        }

        public string Drop(string itemName)
        {
            string outputString = null;
            Item itemToDrop = null;
            foreach (Item item in _world.Items)
            {
                if (string.Compare(item.Name, itemName, ignoreCase: true) == 0)
                {
                    itemToDrop = item;
                    break;
                }
            }

            if (itemToDrop != null)
            {
                bool itemInRoom = false;
                foreach (Item item in Inventory)
                {
                    if (item == itemToDrop)
                    {
                        itemInRoom = true;
                        break;
                    }
                }

                if (itemInRoom == false)
                {
                    outputString = "I see no such thing.";
                }
                else
                {
                    RemoveFromPlayerInventory(itemToDrop);
                    _currentRoom.AddToRoomInventory(itemToDrop);
                    outputString = $"Dropped {itemName}.";
                }
            }
            else
            {
                outputString = "That item does not exist.";
            }
            return outputString;
        }

        void AddToPlayerInventory(Item itemToAdd)
        {
            Inventory.Add(itemToAdd);
        }

        void RemoveFromPlayerInventory(Item itemToRemove)
        {
            Inventory.Remove(itemToRemove);
        }

        private World _world;
        private Room _currentRoom;
    }
}
