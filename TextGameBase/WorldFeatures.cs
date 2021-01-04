using System;
using System.Collections.Generic;
using System.Text;

namespace TextGameBase
{
    public partial class World
    {
        public class TerrainFeature : TextGameObject
        {
            public TerrainFeature() : base() { }
            public TerrainFeature(string name, string description = default, bool singular = true, Dictionary<string, TOCommand> commands = default, List<string> senseDescriptions = default, Dictionary<string, TOCommand> sensing = default) : 
                base(name, description, singular, commands, senseDescriptions, sensing) 
            {
                
            }
            public TerrainFeature(TerrainFeature copy) : base(copy) { }

            // This is specifically for generating more unique features using the world seed from the base in TerrainFeatures
            public TerrainFeature(int seed, TerrainFeature staticFeature) : base(staticFeature)
            {

            }
        }

        public static class TerrainFeatures
        {
            public static TerrainFeature MapleTree = new TerrainFeature
            (
                name: "maple trees",
                singular: false,

                senseDescriptions: new List<string>
                {
                    "look short, wide, with brown bark",
                    "make a slight cracking noises in the wind",
                    "have a scaly tough feeling to them",
                    "smell earthy",
                    "taste pretty awful"
                }
            );
                
            public static TerrainFeature OakTree = new TerrainFeature
            (
                name: "oak trees",
                singular: false,

                senseDescriptions: new List<string>
                {
                    "look tall, thin, with brown bark",
                    "make a slight cracking noises in the wind",
                    "have a scaly tough feeling to them",
                    "smell earthy",
                    "taste pretty awful"
                }
            );
        }
    }
}
