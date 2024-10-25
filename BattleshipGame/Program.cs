/*Anne-Lii Hansen
Spelet "Sänka skepp" eller "Battleship" med C#.NET */

using System;
using System.Collections.Generic;

namespace BattleshipGame
{
    class Program
    {

        static List<(int, int)> targetQueue = new List<(int, int)>();

        static void Main(string[] args)
        {
            //skapar ett 10x10 rutnät
            char[,] playerGrid = new char[10, 10];
            char[,] computerGrid = new char[10, 10];        

            InitializeGrid(playerGrid);//Anropar
            InitializeGrid(computerGrid);//Anropar


            // Skapa och placera datorns skepp som är dolda för spelaren
            Ship[] computerShips = {
                new Ship.Carrier(),
                new Ship.Battleship(),
                new Ship.Submarine(),
                new Ship.Destroyer()
            };

            foreach (var ship in computerShips)
            {
                ship.PlaceShip(computerGrid); // Placeras av datorn, spelaren ser inte detta
            }


            // Skapa och placera spelarens skepp
            Ship[] playerShips = {
                new Ship.Carrier(),
                new Ship.Battleship(),
                new Ship.Submarine(),
                new Ship.Destroyer()
            };

            // Placera alla skepp
            foreach (var ship in playerShips)
            {
                ship.PlaceShip(playerGrid);
            }


            //starta spelloopen
            bool gameOn = true;

            while (gameOn)
            {
                Console.WriteLine("Din tur att skjuta");
                PlayerShoot(computerGrid);

                // Kolla om spelaren vann
                if (AllShipsSunk(computerGrid))
                {
                    Console.WriteLine("Grattis, du har sänkt alla skepp! Du vann!");
                    gameOn = false;
                    continue;
                }

                // Datorn skjuter på spelarens grid
                Console.WriteLine("\nDatorns tur...");
                ComputerShoot(playerGrid);

                // Kolla om datorn vann
                if (AllShipsSunk(playerGrid))
                {
                    Console.WriteLine("Datorn sänkte alla dina skepp. Du förlorade.");
                    gameOn = false;
                }

                // Skriv ut spelarens grid efter varje tur
                Console.WriteLine("\nDitt rutnät:");
                PrintGrid(playerGrid);
            }
        }

        // Datorns skjutlogik
        static void ComputerShoot(char[,] grid)
        {
            Random rnd = new Random();
            int row, col;

            // Om det finns mål i kön, använd dem först
            if (targetQueue.Count > 0)
            {
                (row, col) = targetQueue[0];
                targetQueue.RemoveAt(0);
            }
            else
            {
                row = rnd.Next(0, 10);
                col = rnd.Next(0, 10);
            }

            if (grid[row, col] == '~') // Om det är vatten
            {
                if (grid[row, col] == 'S') // Träffar skepp
                {
                    Console.WriteLine($"Datorn träffade ett skepp på ({row}, {col})!");
                    grid[row, col] = 'X'; // Markera träff
                    AddAdjacentTargets(row, col); // Lägg till angränsande mål i kön
                }
                else
                {
                    Console.WriteLine($"Datorn missade på ({row}, {col}).");
                    grid[row, col] = 'O'; // Markera miss
                }
            }
        }

         // Lägg till angränsande mål i targetQueue
        static void AddAdjacentTargets(int row, int col)
        {
            if (row > 0) targetQueue.Add((row - 1, col)); // Upp
            if (row < 9) targetQueue.Add((row + 1, col)); // Ner
            if (col > 0) targetQueue.Add((row, col - 1)); // Vänster
            if (col < 9) targetQueue.Add((row, col + 1)); // Höger
        }

        // Metod för Spelarens skjutlogik
        static void PlayerShoot(char[,] grid)
        {
            int row = Ship.GetValidCoordinate("rad", 0, 9);
            int col = Ship.GetValidCoordinate("kolumn", 0, 9);

            if (grid[row, col] == 'S')
            {
                Console.WriteLine("Du träffade ett skepp!");
                grid[row, col] = 'X'; // Markera träff
            }
            else
            {
                Console.WriteLine("Du missade.");
                grid[row, col] = 'O'; // Markera miss
            }
        }

        // Kollar om alla skepp är sänkta
        static bool AllShipsSunk(char[,] grid)
        {
            foreach (char cell in grid)
            {
                if (cell == 'S') // Om det finns skepp kvar
                {
                    return false;
                }
            }
            return true;
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

        //Metod för att initialisera spelplanen med vatten
        static void InitializeGrid(char[,] grid)
        {

            //fyller spelplanen med symboler som representerar vatten
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    grid[row, col] = '~'; //representerar vågor
                }
            }
        }
    }
}

