using System;
using System.Collections.Generic;
using System.Text;

namespace TextGameBase
{
    public partial class World
    {
        #region ** Commands **
        public string Pickup(string name, Player player)
        {
            var item = Map[SpawnPoint[0], SpawnPoint[1]].Items.Find((i) => i.Name == name);

            if (item == default)
                return $"I couldn't find a {name} to pickup.";
            else
            {
                if ((player.CurrentInventoryWeight + item.Weight) <= player.MaxInventoryWeight)
                {
                    // Add item to player inventory
                    player.Inventory.Add(item);
                    player.CurrentInventoryWeight += item.Weight;

                    // Despawn item from the environment
                    Map[SpawnPoint[0], SpawnPoint[1]].Items.Remove(item);

                    return $"You picked up a {name}";
                }
                else
                    return $"I couldn't pickup the {name} because my inventory is full.";
            }
        }

        public string Inspect(string command, Player player)
        {
            // Make sure that the user entered an item that they want to sense and not just the word
            if (!command.Contains(' '))
                return $"What would you like to {command}?";
            
            // Get both the actual word for the sense and the rest of the string to describe what to sense
            var sense = command.Substring(0, command.IndexOf(' '));
            var featureDescription = command.Substring(command.IndexOf(' ') + 1);

            // var textObject = (player.EquipedItem != default && player.EquipedItem.Name == featureDescription) ? player.EquipedItem : null ??
            //   Map[SpawnPoint[0], SpawnPoint[1]].Items.Find((i) => i.Name == featureDescription) ??

            // Look for the item in the players hands
            if (player.EquipedItem != default && player.EquipedItem.Name == featureDescription)
            {
                return $"The {player.EquipedItem.Name} {player.EquipedItem.Sensing[sense](command, player.EquipedItem, player, default)}";
            }
            // Look for the item in the environment
            else if (Map[SpawnPoint[0], SpawnPoint[1]].Items.Count > 0 && Map[SpawnPoint[0], SpawnPoint[1]].Items.Exists((i) => i.Name == featureDescription))
            {
                var item = Map[SpawnPoint[0], SpawnPoint[1]].Items.Find((i) => i.Name == featureDescription);
                return $"The {item.Name} {item.Sensing[sense](command, item, player, default)}";
            }
            // Look for a feature in the environment
            else if (Map[SpawnPoint[0], SpawnPoint[1]].Features.Count > 0 && Map[SpawnPoint[0], SpawnPoint[1]].Features.Exists((i) => i.Name == featureDescription))
            {
                var feature = Map[SpawnPoint[0], SpawnPoint[1]].Features.Find((i) => i.Name == featureDescription);
                return $"The {feature.Name} {feature.Sensing[sense](command, feature, player, default)}";
            }
            // Look for a feature in the environment
            else if (Map[SpawnPoint[0], SpawnPoint[1]].Beings.Count > 0 && Map[SpawnPoint[0], SpawnPoint[1]].Beings.Exists((b) => b.Name == featureDescription))
            {
                var being = Map[SpawnPoint[0], SpawnPoint[1]].Beings.Find((b) => b.Name == featureDescription);
                return $"{(being.IsUnique ? "" : "The ")}{being.Name} {being.Sensing[sense](command, being, player, default)}.";
            }
            // Give up and return a fail
            else
                return $"Sorry, I can't find any '{featureDescription}(s)' nearby.";
        }

        public string Look(string direction = "")
        {
            return $"Coordinates: ({SpawnPoint[0]}, {SpawnPoint[1]})\n\n\t{Map[SpawnPoint[0], SpawnPoint[1]].GetDescription()}";
        }

        public string Move(string direction)
        {
            try
            {
                SpawnPoint[0] += DirectionTo2DVector[direction][0];
                SpawnPoint[1] += DirectionTo2DVector[direction][1];
            }
            catch
            {
                return $"Sorry '{direction}' is not a direction. Try again!";
            }

            return Look();
        }
        #endregion

        // TAke a player as a parameter in the future
        public int StartGame(Player player)
        {
            Console.Clear();
            Console.Write("A. B. CharCraft 0.0.1 by Josh Dotson\n");

            var quit = false;
            var skipNPC = true;
            while (!quit)
            {
                if (!skipNPC)
                {                 
                    var beings = Map[SpawnPoint[0], SpawnPoint[1]].Beings;
                    if (beings.Count > 0)
                    {
                        Console.WriteLine("\n\tCreature Events:");
                        for (int i = 0; i < beings.Count; ++i)
                        {
                            Console.WriteLine($"\tCreature #{i + 1}> {beings[i].GoAI("", beings[i], player, this)}.");
                        }
                    }   
                }
                else
                    skipNPC = false;

                Console.Write("\n\tc={==> ");
                var command = Console.ReadLine();

                Console.Clear();
                Console.Write("A. B. CharCraft 0.0.1 by Josh Dotson\n");

                // Simply initial way of extracting out the first word
                var words = command.Split(' ');

                // Figure out the command
                switch (words[0])
                {
                    // Movement
                    case "look": { Console.WriteLine($"\n\t{Look()}"); break; }
                    case "move": case "travel": case "go":
                        {
                            if (words.Length < 2)
                                Console.WriteLine($"\n\tWhich direction would you like to travel?");
                            else
                                Console.WriteLine($"\n\t{Move(words[1])}");

                            skipNPC = true;
                            break;
                        }

                    // Inventory Management
                    case "pickup": case "grab": { Console.WriteLine($"\n\t{Pickup(string.Join(" ", words[1..^0]), player)}"); break; } // ADD PICKUP COMMAND TO ALL ITEMS INSTEAD
                    case "inventory": { Console.WriteLine($"\n\t{player.GetInventory()}"); break; }
                    case "equip": { Console.WriteLine($"\n\t{player.EquipItem(string.Join(" ", words[1..^0]))}"); break; }
                    case "equipped": 
                        { 
                            var equippedString = player.EquipedItem == default ? "You currently have no item equipped" : $"You have a {player.EquipedItem.Name} equipped";
                            Console.WriteLine($"\n\t{equippedString}"); 
                            break; 
                        }

                    // Senses
                    case "view": case "listen": case "touch": case "smell": case "taste":
                        {
                            Console.WriteLine($"\n\t{Inspect(command, player)}"); 
                            break;
                        }
                        
                    case "quit": quit = true; break;
                    default: 
                        {
                            // See if the equipped item that the player is holding has any actions with this word
                            if (player.EquipedItem != default && player.EquipedItem.Commands.ContainsKey(words[0]))
                                Console.WriteLine($"\n\t{player.EquipedItem.ExecuteCommand(words[0], string.Join(" ", words[1..^0]), player, this)}"); 
                            else
                            {
                                var name = string.Join(" ", words[1..^0]);
                                var being = Map[SpawnPoint[0], SpawnPoint[1]].Beings.Find((b) => b.Name == name);
                                if (being != default && being.Commands.ContainsKey(words[0]))
                                    Console.WriteLine($"\n\t{being.Commands[words[0]](string.Empty, being, player, this)}.");
                                else
                                    Console.WriteLine($"\n\tSorry, I don't know what '{words[0]}' means. Try again!");
                            }          

                            break; 
                        }
                }
            }

            return 0;
        }
    }
}
