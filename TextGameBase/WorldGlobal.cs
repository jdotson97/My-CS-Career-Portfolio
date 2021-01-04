using System;
using System.Collections.Generic;
using System.Text;

namespace TextGameBase
{
    public partial class World
    {
        public int biomeDefault;
        public int climateDefault;

        public static readonly uint locationSpawnChance = 20;
        public static readonly double biomeGradientMin = 0;
        public static readonly double biomeGradientMax = 2.5f;
        public static readonly double climateGradientMin = 0;
        public static readonly double climateGradientMax = 1.5f;
        public static readonly double eerinessGradientMax = 1.5f;

        public static readonly List<string> Climate = new List<string> 
        { 
            "scorching", "very hot", "hot", "warm", "slightly warm", 
            "slightly chilly", "chilly", "cold", "very cold", "freezing"
        };
        public static readonly List<string> Eerieness = new List<string>
        {
            "very scary", "scary", "unsettling", "a little unsettling", "neutral",
            "somewhat safe", "safe", "very safe"
        };
        public static readonly List<string> Attractiveness = new List<string>
        {
            "disgusting", "hideous", "pretty ugly", "ugly", "kinda ugly", "neutral",
            "somewhat pretty", "pretty", "very pretty", "beautiful", "breathtaking"
        };

        public static readonly Dictionary<string, int[]> DirectionTo2DVector = new Dictionary<string, int[]>()
        {
            { "North", new int[2] { 0, 1 } },       { "north", new int[2] { 0, 1 } },
            { "NorthEast", new int[2] { 1, 1 } },   { "northeast", new int[2] { 1, 1 } },
            { "East", new int[2] { 1, 0 } },        { "east", new int[2] { 1, 0 } },
            { "SouthEast", new int[2] { 1, -1 } },  { "southeast", new int[2] { 1, -1 } },
            { "South", new int[2] { 0, -1 } },      { "south", new int[2] { 0, -1 } },
            { "SouthWest", new int[2] { -1, -1 } }, { "southwest", new int[2] { -1, -1 } },
            { "West", new int[2] { -1, 0 } },       { "west", new int[2] { -1, 0 } },
            { "NorthWest", new int[2] { -1, 1 } },  { "northwest", new int[2] { -1, 1 } }
        };

        public static List<int> WorldSizeMap = new List<int> { 3, 4, 5, 6, 7 };
        public static List<int> EnvironmentSizeMap = new List<int> { 2, 3, 5, 8, 13 };

        public enum Directions : int { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest }
        public enum WorldSize : int { XSmall, Small, Medium, Large, XLarge }
        public enum EnvironmentSize : int { XSmall, Small, Medium, Large, XLarge }
    }
}
