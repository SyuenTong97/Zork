using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Zork
{
    public class Player
    {
        public World World { get; }

        public List<Item> Inventory { get; }

        public string StartingLocation { get; }

        [JsonIgnore]
        public Room Location { get; private set; }

        [JsonIgnore]
        public string LocationName
        {
            get
            {
                return Location?.Name;
            }
            set
            {
                Location = World?.RoomsByName.GetValueOrDefault(value);
            }
        }

        public Player(World world, string startingLocation)
        {
            World = world;
            StartingLocation = startingLocation;
            LocationName = startingLocation;
            Inventory = new List<Item>();
        }

        public bool Move(Directions direction)
        {
            bool isValidMove = Location.Neighbors.TryGetValue(direction, out Room destination);
            if (isValidMove)
            {
                Location = destination;
            }
            return isValidMove;
        }

        public void Take(string itemName)
        {
            Item itemToTake = null;
            foreach (Item item in World.Items)
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
                foreach (Item item in Location.Inventory)
                {
                    if (item == itemToTake)
                    {
                        itemInRoom = true;
                        break;
                    }
                }

                if (itemInRoom == false)
                {
                    Console.WriteLine("I see no such thing.");
                }
                else
                {
                    AddToPlayerInventory(itemToTake);
                    Location.RemoveFromRoomInventory(itemToTake);
                    Console.WriteLine($"Taken {itemName}.");
                }
            }
            else
            {
                Console.WriteLine("That item does not exist.");
            }
        }

        public void Drop(string itemName)
        {
            Item itemToDrop = null;
            foreach (Item item in World.Items)
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
                    Console.WriteLine("I see no such thing.");
                }
                else
                {
                    RemoveFromPlayerInventory(itemToDrop);
                    Location.AddToRoomInventory(itemToDrop);
                    Console.WriteLine($"Dropped {itemName}.");
                }
            }
            else
            {
                Console.WriteLine("That item does not exist.");
            }
        }

        void AddToPlayerInventory(Item itemToAdd)
        {
            Inventory.Add(itemToAdd);
        }
        void RemoveFromPlayerInventory(Item itemToRemove)
        {
            Inventory.Remove(itemToRemove);
        }
    }
}
