using System;
using System.Threading.Tasks;

namespace TextGameBase
{
    class Program
    {
        public static void Main(string[] args)
        {
            // Initialize world from save file if the user wants
            var World = new World(10, 0);
            var Player = new Player();

            World.StartGame(Player);

            // Ask the user if they would like to save their game
            // Console.ReadLine();
        }
    }
}
