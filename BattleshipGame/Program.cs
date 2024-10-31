/*Anne-Lii Hansen
Spelet "Sänka skepp" eller "Battleship" med C#.NET */

//importerar nödvändiga bibliotek
using System;

namespace BattleshipGame
{
    class Program
    {

        static void Main(string[] args)
        {
            //skapar en ny instans av Game och anropar game.start()
            var game = new Game();
            game.Start();
        }
    }
}

