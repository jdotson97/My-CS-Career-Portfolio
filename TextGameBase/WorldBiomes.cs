using System;
using System.Collections.Generic;
using System.Text;

namespace TextGameBase
{
    public partial class World
    {
        public class Biome
        {
            public string Name { get; }
            public string Description { get; }
            // public List<TerrainFeature> BasicFeatures { get; }
            public List<TerrainFeature> Features { get; }
            public List<Item> Items { get; }
            public List<NPC> Creatures { get; }

            public int NumFeaturesMin { get; }
            public int NumFeaturesMax { get; }
            public int NumItemsMin { get; }
            public int NumItemsMax { get; }
            public int NumCreaturesMin { get; }
            public int NumCreaturesMax { get; }

            public Biome()
            {
                Name = "Impassable";
                Description = "An impassable wall of terrain. Doesn't appear to be anyway past...";
                Features = new List<TerrainFeature>();
                Items = new List<Item>();
                Creatures = new List<NPC>();
            }
            public Biome(string name, string description, List<TerrainFeature> features, List<Item> items, List<NPC> creatures, int minFeatures = 0, int maxFeatures = 2, int minItems = 0, int maxItems = 2, int minCreatures = 0, int maxCreatures = 2)
            {
                Name = name;
                Description = description;
                Features = features;
                Items = items;
                Creatures = creatures;
                NumFeaturesMin = minFeatures;
                NumFeaturesMax = maxFeatures;
                NumItemsMin = minItems;
                NumItemsMax = maxItems;
                NumCreaturesMin = minCreatures;
                NumCreaturesMax = maxCreatures;
            }
        }

        public static readonly List<Biome> Biomes = new List<Biome>
        {
            new Biome("jungle", "an extremely thick forest full of vegatation", new List<TerrainFeature>(), new List<Item>(), new List<NPC>()),

            new Biome("dense forest", "a very thick grouping of trees with dense foliage", new List<TerrainFeature>
            {
                TerrainFeatures.MapleTree,
                TerrainFeatures.OakTree
            }, new List<Item>
            {
                Items.WoodenPlank,
                Items.TreeBranch
            }, new List<NPC>
            {
                NPCs.Carter
            }, 1, 4, 2, 5, 1, 2),

            new Biome("forest", "a place rich in trees, foliage, and seems rich in other plant and animal life", new List<TerrainFeature>
            {
                TerrainFeatures.MapleTree,
                TerrainFeatures.OakTree
            }, new List<Item>
            {
                Items.WoodenPlank,
                Items.TreeBranch
            }, new List<NPC>
            {
                NPCs.Carter
            }, 1, 4, 1, 4, 1, 2),

            new Biome("light forest", "a less dense forest that affords farther visibility", new List<TerrainFeature>
            {
                TerrainFeatures.MapleTree,
                TerrainFeatures.OakTree
            }, new List<Item>
            {
                Items.WoodenPlank,
                Items.TreeBranch
            }, new List<NPC>
            {
                NPCs.Carter
            }, 1, 3, 1, 3, 1, 2),

            new Biome("meadow", "a somewhat overgrown field with little trees but lots of foliage", new List<TerrainFeature>
            {
                TerrainFeatures.MapleTree,
                TerrainFeatures.OakTree
            }, new List<Item>
            {
                Items.WoodenPlank,
                Items.TreeBranch
            }, new List<NPC>
            {
                NPCs.Carter
            }, 1, 2, 1, 2, 1, 2),

            new Biome("field", "a clearing showing waning amounts of foliage and little to no trees", new List<TerrainFeature>(), new List<Item>(), new List<NPC>()),
            new Biome("plains", "a far reaching plateau hosting little plant life", new List<TerrainFeature>(), new List<Item>(), new List<NPC>()),
            new Biome("wasteland", "a barren, desolate place with almost no foliage to be found", new List<TerrainFeature>(), new List<Item>(), new List<NPC>())
        };
    }
}
