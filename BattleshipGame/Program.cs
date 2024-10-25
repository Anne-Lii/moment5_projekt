/*Anne-Lii Hansen
Spelet "Sänka skepp" eller "Battleship" med C#.NET */

using System;

namespace Battleship
{
    class Program
    {
        static void Main(string[] args)
        {
            //skapar ett 10x10 rutnät
            char[,] grid = new char[10, 10];

            //fyller spelplanen med symboler som representerar vatten
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    grid[row,col] = '~'; //representerar vågor
                }
            }

            //Anropar metoden PrintGrid för att visa rutnätet i konsollen
            PrintGrid(grid);          

        }

        //Metod för att skriva ut rutnätet till konsollen
        static void PrintGrid(char[,] grid)
        {
            Console.WriteLine("  0 1 2 3 4 5 6 7 8 9 "); //rubriker till kolumnerna
            for (int row = 0; row < 10; row++)
            {
                Console.Write(row + " "); //radnumrering
                for (int col = 0; col < 10; col++)
                {
                    Console.Write(grid[row, col] + " ");
                }
                Console.WriteLine();//tom rad
            }
        }
    }
}

