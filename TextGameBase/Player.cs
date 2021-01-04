using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace TextGameBase
{
    public class Player
    {
        public string Name { get; }
        public World.Item EquipedItem { get; set; }
        public List<World.Item> Inventory { get; set; }

        public double CurrentInventoryWeight { get; set; }
        public double MaxInventoryWeight { get; }

        public double CurrentHealth { get; set; }
        public double MaxHealth { get; set; }

        public Player()
        {
            EquipedItem = default;
            Inventory = new List<World.Item>();
            CurrentInventoryWeight = 0f;
            MaxInventoryWeight = 100.0f;
        }
        public Player(string name) : this()
        {

        }

        public string EquipItem(string name)
        {
            var item = Inventory.Find((i) => i.Name == name);

            if (item == default)
                return $"Sorry, I don't have a {name} to equip.";
            else
            {
                if (item.IsEquipable)
                {
                    EquipedItem = item;
                    return $"Your {name} is now equipped";
                }
                else
                    return $"Sorry, but I cannot equip a {name}";
            }
        }

        public string GetInventory()
        {
            if (Inventory.Count == 0)
                return "You are not carrying anything";
            else
            {
                var inventoryString = "";
                var totalWeight = 0.0f;

                var temp = new List<World.Item>(Inventory);
                while (temp.Count != 0)
                {
                    var itemCount = 0;
                    var itemName = temp[0].Name;
                    var itemWeight = temp[0].Weight;

                    temp.ForEach((i) =>
                    {
                        if (i.Name == temp[0].Name)
                        {
                            itemCount++;
                            totalWeight += i.Weight;
                        }      
                    });
                    temp.RemoveAll((i) => i.Name == itemName);

                    inventoryString += $"|> {itemCount}x {itemName} ({itemWeight * itemCount} lbs)\n\t";  
                }

                return  $"This is what I am carrying:\n\t" +
                        $"----------------------------------------------\n\t" +
                        $"{inventoryString}\n\t" +
                        $"----------------------------------------------\n\n\t" +
                        $"Weight: {totalWeight} / {MaxInventoryWeight} lbs";
            }
        }
    }
}
