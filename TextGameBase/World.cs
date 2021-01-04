using System;
using System.Collections.Generic;
using System.Text;

namespace TextGameBase
{
    public partial class World
    {        
        // An array of references to all the scenes present within this world
        public Environment[,] Map { get; }
        public int[] SpawnPoint { get; set; }

        private World() { }
        public World(int size, int seed)
        {
            // Define a new random object and seed it for all calculations here
            Random random = new Random(seed);

            // Create the default states for the biomes, climates...
            biomeDefault = (int)Math.Round((double)Biomes.Count / 2) + (int)Math.Round((biomeGradientMax - biomeGradientMin) * (0.50f - random.NextDouble()) * 2);
            climateDefault = (int)Math.Round((double)Climate.Count / 2) + (int)Math.Round((climateGradientMax - climateGradientMin) * (0.50f - random.NextDouble()) * 2);     

            // To account for the bounding borders around the world
            var fullSize = size + 2;

            // Make a new map of the specified size
            Map = new Environment[fullSize, fullSize];

            var mapCenter = (int)Math.Round((float)fullSize / 2);
            SpawnPoint = new int[2] { mapCenter, mapCenter };

            // Now go over it again adding the flucuations
            var currentRow = 1;
            for (int j = 1; j < fullSize - 1; ++j)
            {
                for (int i = 1; i < fullSize - 1; ++i)
                {
                    Map[i, j] = GenerateEnvironment(random.Next(), new List<Environment>
                    {
                        Map[i - 1, j - 1],  Map[i, j - 1],  Map[i + 1, j - 1],
                        Map[i - 1, j    ],  Map[i, j    ],  Map[i + 1, j    ],
                        Map[i - 1, j + 1],  Map[i, j + 1],  Map[i + 1, j + 1]
                    });

                    // For debugging purposes and visualization
                    /* if (j != currentRow)
                    {
                        currentRow = j;
                        Console.Write("\n");
                    }

                    Console.Write($"{Map[i, j].BiomeID}"); */
                }          
            }
        }

        // Takes an environment Matrix and generates an environment square based on the surroundings
        private Environment GenerateEnvironment(int seed, List<Environment> matrix)
        {
            float climateSum = 0f, biomeSum = 0f;
            foreach (var e in matrix)
            {       
                climateSum += (e == null || e.ClimateID == -1) ? climateDefault : e.ClimateID;
                biomeSum += (e == null || e.BiomeID == -1) ? biomeDefault : e.BiomeID;
            }

            // Calculate the average using the surrounding environments to this one
            var biomeAverage = biomeSum / matrix.Count;
            var climateAverage = climateSum / matrix.Count;

            // Define a new random object and seed it for all calculations here
            Random random = new Random(seed);

            // Calculate the modifiers to add more flavor to the environment
            var biomeModifier = biomeGradientMax * (0.50f - random.NextDouble()) * 2;
            var climateModifier = climateGradientMax * (0.50f - random.NextDouble()) * 2;

            // Now get our environment object
            return new Environment
            (
                seed,
                (int)Math.Min(Math.Max(Math.Round(biomeAverage + biomeModifier), 0), Biomes.Count - 1),
                (int)Math.Min(Math.Max(Math.Round(climateAverage + climateModifier), 0), Climate.Count - 1)
            );
        }
    }
}
