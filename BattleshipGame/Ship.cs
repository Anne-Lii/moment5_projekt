using System;

namespace BattleshipGame
{
    public class Ship
    {
        public int Length {get; private set;}
        public Ship(int length)
        {
            Length = length;
        }

        //Metod för att placera skepp
        public void PlaceShip(char[,] grid)
        {
            Console.WriteLine("Placera ditt skepp:");
            Console.WriteLine("Ange rad (0-9):");
            int row = int.Parse(Console.ReadLine());
            Console.WriteLine("Ange kolumn (0-9):");
            int col = int.Parse(Console.ReadLine());

            Console.WriteLine("Välj riktning (h för horisontellt, v för vertikalt):");
            char direction = char.Parse(Console.ReadLine());

            //Placera skeppet på spelplanen baserat på spelarens val
            if (direction == 'h')
            {
                for (int i = 0; i < Length; i++)
                {
                    grid[row,col + i] = 'S'; //S representerar ett skepp
                }
            } 
            else if (direction == 'v')
            {
                for (int i = 0; i < Length; i++)
                {
                    grid[row + i,col] = 'S'; //S representerar ett skepp
                }
            } 
            else
            {
                Console.WriteLine("Ogiltig riktning!");
            }
        }
    }
}