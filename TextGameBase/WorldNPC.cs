using System;
using System.Collections.Generic;
using System.Text;

namespace TextGameBase
{
    public partial class World
    {
        // This class will represent both hostile and non hostile creatures including other humans
        public class NPC : TextGameObject
        {
            // Is this NPC a properly named one, or is it generic like a dog
            public bool IsUnique { get; }

            // These hold a unique set of parameters specific to each instance of this class
            public Dictionary<string, dynamic> Properties { get; }

            // Ways this NPC can interract with its environment on its turn
            public Dictionary<string, TOCommand> AICommands { get; }

            public NPC(string name, string description, bool isUnique = false, bool isSingular = true, Dictionary<string, dynamic> properties = default, Dictionary<string, TOCommand> aiCommands = default, TOCommand goAI = default, Dictionary<string, TOCommand> commands = default, List<string> senseDescriptions = default, Dictionary<string, TOCommand> sensing = default) : 
                base(name, description, isSingular, commands, senseDescriptions, sensing)
            {
                IsUnique = isUnique;
                Properties = properties ?? new Dictionary<string, dynamic>();
                AICommands = aiCommands ?? new Dictionary<string, TOCommand>();

                // What any NPC does by default
                GoAI = goAI ?? new TOCommand(delegate(string parameters, TextGameObject @this, Player player, World world)
                {
                    return "Doesn't do anything";
                });
            }
            public NPC(NPC copy) : this(copy.Name, copy.Description, copy.IsUnique, copy.IsSingular, copy.Properties, copy.AICommands, copy.GoAI, copy.Commands, copy.SenseDescriptions, copy.Sensing) { }

            // This constructor is specifically for world generation. It takes the seed so this can be given tiers/rarity from the static base copy from NPCS
            public NPC(int seed, NPC staticNPC) : this(staticNPC)
            {
                var rand = new Random(seed);
            }

            // Executes an AICommand using this NPCs AI
            public TOCommand GoAI { get; }
            // Executes an AICommand for this NPC manually
            // public TOCommand Go { get; }
        }

        public static class NPCs
        {
            public static NPC Carter = new NPC
            (
                name: "Carter",
                description: "Carter is an ugly black cat with long fur, and a very annoying way of going about life as a cat. Unfortunatly he belongs to you...",
                isUnique: true,

                senseDescriptions: new List<string>
                {
                    /*Sight*/ "looks like a ugly black cat sporting a hideously ugly face, and a stupid tail",
                    /*Sound*/ "seems to be making this tiny little annoying ass meowing sounds almost like he wants you to feed him",
                    /*Touch*/ "feels very soft and makes an ugly trill sound when touched",
                    /*Smell*/ "smells like he taken a fair share of farts to the face",
                    /*Taste*/ "you'd rather not..."
                },

                // Carter's properties specific to him
                properties: new Dictionary<string, dynamic> { { "hasBeenFed", false } },

                // Carter's AI
                goAI: delegate (string actionString, TextGameObject thisNPC, Player player, World world)
                {
                    var carter = (NPC)thisNPC;
                    if (carter.Properties["hasBeenFed"])
                        return "Carter is currently sleeping off the food";
                    else
                        return $"Carter beings petting himself around your legs. He then meows at you like he wants something";
                },

                // Carter's supported commands
                aiCommands: new Dictionary<string, TextGameObject.TOCommand>
                {

                },
                // Player's commands to interact with Carter
                commands: new Dictionary<string, TextGameObject.TOCommand>
                {
                    { "feed", delegate(string actionString, TextGameObject thisNPC, Player player, World world)
                        {
                            ((NPC)thisNPC).Properties["hasBeenFed"] = true;
                            return $"A can of cat food appears, and he starts meowing frantically. You give it to him, he devours it in 2 bites, finally stops meowing, and goes to sleep";
                        }
                    }
                }        
            );
        }
    }
}
