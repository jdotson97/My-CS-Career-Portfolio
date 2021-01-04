using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace TextGameBase
{
    public partial class World
    {
        public class Environment : TextGameObject
        {
            public int BiomeID;   // <--- MAKE BIOME WORK MORE LIKE ITEM. MAKE IT SO THE STATIC BIOMES ARE JUST FORMULAS, AND GIVE ENVIRONMENT A SEEDED COPY CONSTRUCTOR
            public int ClimateID;

            public List<TerrainFeature> Features { get; }
            public List<Item> Items { get; }
            public List<NPC> Beings { get; }

            private List<TerrainFeature> FeatureSpawns { get; }
            private List<Item> ItemSpawns { get; }
            private List<NPC> CreatureSpawns { get; }

            public int NumFeaturesMin { get; }
            public int NumFeaturesMax { get; }
            public int NumItemsMin { get; }
            public int NumItemsMax { get; }
            public int NumCreaturesMin { get; }
            public int NumCreaturesMax { get; }

            #region ** Old Constructors
            public Environment()
            {
                BiomeID = -1;
                Features = new List<TerrainFeature>();
                Items = new List<Item>();
                Beings = new List<NPC>();
                ClimateID = -1;
            }
            public Environment(int seed, int biome, int climate) : this()
            {
                BiomeID = biome;
                ClimateID = climate;

                // Random number generator for below
                Random rand = new Random(seed);

                #region ** Spawn Terrain Features **
                var numFeatures = rand.Next(Biomes[BiomeID].NumFeaturesMin, Biomes[BiomeID].NumFeaturesMax);
                numFeatures = Math.Min(Biomes[BiomeID].Features.Count, numFeatures);

                // Use this biomes ID to get a randomized list of features for this envrionment based on the static list of possible features in the static Biome array
                var possibleFeatures = new List<TerrainFeature>(Biomes[BiomeID].Features); // Using lists with just = returns a reference to the object your assigning it to!!
                for (int i = 0; i < numFeatures; ++i)
                {
                    // Genreate a random index for a random item in the possible items list
                    var rindex = rand.Next(0, possibleFeatures.Count);

                    // Add that item to this environments terrain features 
                    Features.Add(new TerrainFeature(possibleFeatures[rindex]));

                    // Then remove that feature from the list of possible features since the environment already has them in it
                    possibleFeatures.RemoveAt(rindex);
                }
                #endregion

                #region ** Spawn Items **
                var numItems = rand.Next(Biomes[BiomeID].NumItemsMin, Biomes[BiomeID].NumItemsMax);

                var possibleItems = new List<Item>(Biomes[BiomeID].Items);
                if (possibleItems.Count > 0)
                {
                    for (int i = 0; i < numItems; ++i)
                    {
                        // Genreate a random index for a random item in the possible items list
                        var rindex = rand.Next(0, possibleItems.Count);
                        Items.Add(new Item(seed, possibleItems[rindex]));
                    }
                }
                #endregion

                #region ** Spawn Beings **
                var numBeings = rand.Next(Biomes[BiomeID].NumItemsMin, Biomes[BiomeID].NumItemsMax);

                var possibleBeings = new List<NPC>(Biomes[BiomeID].Creatures);
                if (possibleBeings.Count > 0)
                {
                    for (int i = 0; i < numBeings; ++i)
                    {
                        // Genreate a random index for a random item in the possible items list
                        var rindex = rand.Next(0, possibleBeings.Count);
                        Beings.Add(new NPC(seed, possibleBeings[rindex]));
                    }
                }
                #endregion
            }
            #endregion

            public Environment(string name, string description, List<TerrainFeature> featureSpawns = default, List<Item> itemSpawns = default, List<NPC> creatureSpawns  = default, int featureMin = 0, int featureMax = 0, int itemMin = 0, int itemMax = 0, int creatureMin = 0, int creatureMax = 0, Dictionary<string, TOCommand> commands = default, List<string> senseDescriptions = default, Dictionary<string, TOCommand> sensing = default) : 
                base(name, description, true, commands, senseDescriptions, sensing)
            {
                FeatureSpawns = featureSpawns;
                ItemSpawns = itemSpawns;
                CreatureSpawns = creatureSpawns;
                NumFeaturesMin = featureMin;
                NumFeaturesMax = featureMax;
                NumItemsMin = itemMin;
                NumItemsMax = itemMax;
                NumCreaturesMin = creatureMin;
                NumCreaturesMax = creatureMax;
            }
            public Environment(Environment copy) : this(copy.Name, copy.Description, copy.FeatureSpawns, copy.ItemSpawns, copy.CreatureSpawns, copy.NumFeaturesMin, copy.NumFeaturesMax, copy.NumItemsMin, copy.NumItemsMax, copy.NumCreaturesMin, copy.NumCreaturesMax, copy.Commands, copy.SenseDescriptions, copy.Sensing) { }

            // This constructor is specifically for world generation. It takes the seed so this can be given unique attributes from the static base copy from Environments
            public Environment(int seed, Environment staticEnvironment) : this(staticEnvironment)
            {

            }

            // Generate a desciprtion of this object and return it as a string
            public override string GetDescription()
            {
                if (BiomeID != -1 && ClimateID != -1)
                {
                    var description = $"You are now standing in a {Biomes[BiomeID].Name}, {Biomes[BiomeID].Description}. ";

                    #region ** Terrain Features **
                    // See if there are terrain features to describe
                    if (Features.Count > 0)
                    {
                        description += $"This {Biomes[BiomeID].Name} seems to be populated by ";

                        var allFeatures = Features.ToList();
                        for (int i = 0; i < allFeatures.Count; ++i)
                        {
                            var isLast = (i == allFeatures.Count - 1);

                            // Build the list of items using the correct grammer
                            description += (isLast && i > 0) ? "and " : ""; // Prefix
                            description += allFeatures[i].Name; // Body
                            description += isLast ? ". " : ", "; // Suffix
                        }
                    }
                    #endregion

                    #region ** Climate **
                    // Add in the climate descirption
                    description += $"It is {Climate[ClimateID]} here. ";
                    #endregion

                    #region ** Items **
                    // See if there are any items laying around
                    if (Items.Count > 0)
                    {
                        description += "Looking around, you see ";

                        var allItems = new List<Item>(Items);
                        var isFirstIteration = true;
                        while (allItems.Count != 0)
                        {
                            var itemCount = 0;
                            var itemName = allItems[0].Name;

                            // Check to see if this is the last item so we know if we should be putting an and in the sentence
                            var lastItem = allItems.TrueForAll((i) => i.Name == itemName);

                            // Count of the number same items in the environment, and remove them
                            allItems.ForEach((i) =>
                            {
                                if (i.Name == allItems[0].Name)
                                    itemCount++;
                            });
                            allItems.RemoveAll((i) => i.Name == itemName);

                            string itemDescPrefix = "a", itemDescSuffix = "";
                            if (itemCount > 1)
                            {
                                itemDescPrefix = itemCount.ToString();
                                itemDescSuffix = "s";
                            }

                            // If this is the last item, the grammer needs to be adjusted a little
                            if (lastItem)
                            {
                                if (!isFirstIteration)
                                    itemDescPrefix = $"and {itemDescPrefix}";
                                itemDescSuffix += ". ";
                            }
                            else
                                itemDescSuffix += ", ";
                                

                            description += $"{itemDescPrefix} {itemName}{itemDescSuffix}";
                            isFirstIteration = false;
                        }
                    }
                    #endregion

                    #region ** NPCs **
                    if (Beings.Count > 0)
                    {
                        description += $"There {((Beings.Count > 1)?"are":"is")} also {Beings.Count} creature{((Beings.Count > 1) ? "s" : "")} here.\n";
                        
                        for (int i = 0; i < Beings.Count; ++i)
                        {
                            description += $"\n\tCreature #{i + 1}> It's {(Beings[i].IsUnique? "" : "a")} {Beings[i].Name}: {Beings[i].GetDescription()}.";
                        }
                    }
                    #endregion

                    // And return!
                    return description;
                }
                else
                    return "This area is out of bounds"; 
                    
            }
        }

        // These act as templates to give to Environment's seeded copy constructor
        public static class Environments
        {
            // So that biomes can be generated smoothly still across the map
            public static List<Environment> BiomeSpectrum = new List<Environment>
            {
                
            };
        }
    }
}
