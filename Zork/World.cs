﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Zork
{
    public class World
    {
        public HashSet<Room> Rooms { get; set; }

        public Item[] Items { get; }

        public List<Item> Inventory { get; }

        [JsonIgnore]
        public IReadOnlyDictionary<string, Room> RoomsByName => mRoomsByName;

        [JsonIgnore]
        public Dictionary<string, Item> ItemsByName { get; }

        public World(Item[] items)
        {
            Items = items;
            ItemsByName = new Dictionary<string, Item>();
            foreach (Item item in Items)
            {
                ItemsByName.Add(item.Name, item);
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            mRoomsByName = Rooms.ToDictionary(room => room.Name, room => room);

            foreach (Room room in Rooms)
            {
                room.UpdateNeighbors(this);
                room.UpdateInventory(this);
            }
        }

        private Dictionary<string, Room> mRoomsByName;
    }
}
