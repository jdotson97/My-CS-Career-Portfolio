using System;
using System.Collections.Generic;
using System.Text;

namespace TextGameBase
{
    public partial class World
    {
        public class Item : TextGameObject
        {
            public float Weight { get; }
            public bool IsEquipable { get; }

            public double BaseDamage { get; }
            public double[] QualityDamageModifiers { get; }

            // Whether this item is able to be broken down into other items
            // public bool IsSalvagable { get; } // <-- May not need this, as the items command funcitons could take a breakdown command and give the player the raw materials
            // Is this item a template to generate different version. Tree branch is template, therefore = small branch, large branch, etc..
            // public readonly bool IsTemplate { get; }

            public Item(string name, string description = default, bool isSingular = true, bool isEquipable = false, float weight = 0f, Dictionary<string, TOCommand> commands = default, double baseDamage = 0f, double[] qualityDamageModifiers = default, List<string> senseDescriptions = default, Dictionary<string, TOCommand> sensing = default) :
                base(name, description, isSingular, commands, senseDescriptions, sensing)
            {
                IsEquipable = isEquipable;
                Weight = weight;
                BaseDamage = baseDamage;
                QualityDamageModifiers = qualityDamageModifiers ?? QualityDamageModDefault;
            }
            public Item(Item copy) : this(copy.Name, copy.Description, copy.IsSingular, copy.IsEquipable, copy.Weight, copy.Commands, copy.BaseDamage, copy.QualityDamageModifiers, copy.SenseDescriptions, copy.Sensing) { }

            // This constructor is specifically for world generation. It takes the seed so this can be given tiers/rarity from the static base copy from Items
            public Item(int seed, Item staticItem) : this(staticItem)
            {
                var rand = new Random(seed);
                var quality = rand.Next(0, (int)Quality.NumQualityTiers);
                BaseDamage *= QualityDamageModifiers[quality];
                // Name = $"{QualityPrefix[quality]}{Name}";
            }

            #region ** Static **
            // Enums
            public enum Quality : int { Garbage, VeryLow, Low, BelowAverage, Average, AboveAverage, High, VeryHigh, ExHigh, NumQualityTiers }
            public enum Condition : int { Broken, VeryPoor, Poor, Average, Fine, VeryFine, Perfect, NumConditionLevels }
            // Enum to string maps
            public static readonly string[] QualityToString = new string[(int)Quality.NumQualityTiers] { "garbage", "very low", "low", "below average", "average", "above average", "high", "very high", "flawless" };
            public static readonly string[] ConditionToString = new string[(int)Condition.NumConditionLevels] { "broken", "very poor", "poor", "average", "fine", "very fine", "perfect" };
            // Defaults
            public static readonly double[] QualityDamageModDefault = new double[(int)Quality.NumQualityTiers] { 0f, 0.25f, 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f };
            #endregion
        }

        public static class Items
        {
            public static readonly Item WoodenPlank = new Item
            (
                name: "wood plank",
                isEquipable: true,
                weight: 2.0f,
                baseDamage: 2f, 

                commands:
                new Dictionary<string, TextGameObject.TOCommand>
                {
                { "swing", delegate(string actionString, TextGameObject thisItem, Player player, World world)
                    {
                        return "You swing the wooden plank, but it doesn't do anything";
                    }
                },
                { "throw", delegate(string actionString, TextGameObject thisItem, Player player, World world)
                    {
                        world.Map[world.SpawnPoint[0], world.SpawnPoint[1]].Items.Add((Item)thisItem);
                        player.Inventory.Remove((Item)thisItem);
                        player.EquipedItem = default;
                        return "You threw the wood plank, and it lands with a thud out in front of you";
                    }
                },
                { "break", delegate(string actionString, TextGameObject thisItem, Player player, World world)
                    {
                        return "You broke the plank";
                    }
                }
                }
            );

            public static readonly Item TreeBranch = new Item
            (
                name: "tree branch",
                isEquipable: true,
                weight: 5.0f,
                baseDamage: 3f,

                commands:
                new Dictionary<string, TextGameObject.TOCommand>
                {
                { "swing", delegate(string actionString, TextGameObject thisItem, Player player, World world)
                    {
                        var result = "You swing the branch. It makes a whooshing sound as it cuts through the air";
                        if (actionString != TreeBranch.Name)
                        {
                            // Check to see if they wanted to swing it at something, and not just say swing it
                        }
                        return result;
                    }
                },
                { "throw", delegate(string actionString, TextGameObject thisItem, Player player, World world)
                    {
                        world.Map[world.SpawnPoint[0], world.SpawnPoint[1]].Items.Add((Item)thisItem);
                        player.Inventory.Remove((Item)thisItem);
                        player.EquipedItem = default;
                        return "You threw the tree branch, and it lands with a thud out in front of you";
                    }
                }
                }
            );
        }
    }  
}
