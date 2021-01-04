using System;
using System.Collections.Generic;
using System.Text;

namespace TextGameBase
{
    public partial class World
    {
        public class TextGameObject : ICommandable, IDescribable
        {
            public string Name { get; set; }
            public string Description { get; set; }

            public bool IsSingular { get; }

            public List<string> SenseDescriptions { get; set; }
            public Dictionary<string, TOCommand> Sensing { get; } // Combine sensing with commands in the future

            // All potentially unique commands that the player can use to interact with this object
            public Dictionary<string, TOCommand> Commands { get; }

            public virtual double CurrentHP { get; set; }
            public virtual double MaxHP { get; set; }

            // Whether or not you can put Items inside this TextGameObject
            // public bool IsContainer { get; }
            // public List<TextGameObject> Contents { get; set; }
            // public readonly List<TextGameObject> PossibleContents { get; set; }

            protected TextGameObject()
            {
                Name = "Default Game Object";
                Commands = new Dictionary<string, TOCommand>();
                SenseDescriptions = new List<string>
                {
                    $"is invisible",
                    $"makes no sound",
                    $"has no texture",
                    $"has no smell",
                    $"has no taste"
                };
                Sensing = new Dictionary<string, TOCommand>
                {
                    { "view", delegate(string actionString, TextGameObject thisTO, Player player, World world) { return SenseDescriptions[(int)Sense.Sight]; } },
                    { "listen", delegate(string actionString, TextGameObject thisTO, Player player, World world) { return SenseDescriptions[(int)Sense.Hearing]; } },
                    { "touch", delegate(string actionString, TextGameObject thisTO, Player player, World world) { return SenseDescriptions[(int)Sense.Touch]; } },
                    { "smell", delegate(string actionString, TextGameObject thisTO, Player player, World world) { return SenseDescriptions[(int)Sense.Smell]; } },
                    { "taste", delegate(string actionString, TextGameObject thisTO, Player player, World world) { return SenseDescriptions[(int)Sense.Taste]; } }
                };
            }
            public TextGameObject(string name, string description = default, bool isSingular = true, Dictionary<string, TOCommand> commands = default, List<string> senseDescriptions = default, Dictionary<string, TOCommand> sensing = default, double maxHP = -1) : this()
            {
                Name = name;
                Description = description ?? "is totally indescribable";
                IsSingular = isSingular;
                Commands = commands;
                SenseDescriptions = senseDescriptions ?? SenseDescriptions;
                Sensing = sensing ?? Sensing;
            }
            public TextGameObject(TextGameObject copy) : this(copy.Name, copy.Description, copy.IsSingular, copy.Commands, copy.SenseDescriptions, copy.Sensing) { }

            // Get the description for this text game object. This is mean't to be the look a little closer function. It describes the lore and knowledge of what you are looking it
            public virtual string GetDescription()
            {
                return Description;
            }
            public virtual string ExecuteCommand(string command, string commandArgs, Player player, World world)
            {
                try { return Commands[command](commandArgs, this, player, world); }
                catch { return $"Sorry, I had trouble executing command \"{command}\""; }
            }

            public enum Sense : int { Sight, Hearing, Touch, Smell, Taste, NumSenses }
            public static readonly Dictionary<string, int> CommandToSenseMap = new Dictionary<string, int>
            {
                { "view", (int)Sense.Sight },
                { "listen", (int)Sense.Hearing },
                { "touch", (int)Sense.Touch },
                { "smell", (int)Sense.Smell },
                { "taste", (int)Sense.Taste }
            };

            public delegate string TOCommand(string commandString, TextGameObject @this, Player player, World world);

            // A repo of reusable command handlers
            public static class BasicTOCommands
            {
                public static TOCommand BreakTO = delegate (string commandString, TextGameObject thisTO, Player player, World world)
                {
                    return $"You broke the {thisTO.Name}";
                };
            }
        }

        public interface ICommandable { public string ExecuteCommand(string command, string commandArgs, Player player, World world); }
        public interface IDescribable { public string GetDescription(); }
    }
}
