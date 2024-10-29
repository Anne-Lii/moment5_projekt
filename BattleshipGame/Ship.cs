using System;
using System.Collections.Generic;

namespace BattleshipGame
{
    public class Ship
    {
        public int Length { get; private set; }
        public string Name { get; private set; }

         // Lista för att hålla reda på alla skeppspositioner
        public List<(int row, int col)> Positions { get; private set; } = new List<(int row, int col)>();

        public Ship(int length, string name)
        {
            Length = length;
            Name = name;
        }

        //Metod för att placera spelarens skepp
        public void PlaceShip(char[,] grid)
        {

            int row, col;
            char direction;
            bool validPlacement = false;

            Console.WriteLine($"Placera {Name} (längd: {Length})");

            while (!validPlacement)
            {
                row = GetValidCoordinate("rad", 0, 9);
                col = GetValidCoordinate("kolumn", 0, 9);
                direction = GetValidDirection();

                if (CanPlaceShip(grid, row, col, direction))
                {
                    PlaceShipOnGrid(grid, row, col, direction);
                    validPlacement = true;

                    Console.Clear();//Rensar konsollen
                }
                else
                {
                    Console.WriteLine("Ogiltig placering. Försök igen.");
                }
            }
        }

        //Metod för att slumpmässigt placera datorns skepp
        public void PlaceShipRandom(char[,] grid)
        {
            Random rnd = new Random();
            int row, col;
            char direction;
            bool validPlacement = false;

            while (!validPlacement)
            {
                row = rnd.Next(0, 10);
                col = rnd.Next(0, 10);
                direction = rnd.Next(0, 2) == 0 ? 'h' : 'v'; // Slumpmässig riktning

                if (CanPlaceShip(grid, row, col, direction))
                {
                    PlaceShipOnGrid(grid, row, col, direction);
                    validPlacement = true;
                }
            }
        }


        //kontrollera om skeppet får plats
        private bool CanPlaceShip(char[,] grid, int row, int col, char direction)
        {
            if (direction == 'h')
            {
                if (col + Length > 9) return false;

                for (int i = 0; i < Length; i++)
                {
                    if (grid[row, col + i] == 'S') return false;
                }
            }
            else if (direction == 'v')
            {
                if (row + Length > 9) return false;

                for (int i = 0; i < Length; i++)
                {
                    if (grid[row + i, col] == 'S') return false;
                }
            }
            return true;
        }

        private void PlaceShipOnGrid(char[,] grid, int row, int col, char direction)
        {
            Positions.Clear(); // Rensa gamla positioner

            if (direction == 'h')
            {
                for (int i = 0; i < Length; i++)
                {
                    grid[row, col + i] = 'S';
                    Positions.Add((row, col + i)); // Sparar positionen
                }
            }
            else if (direction == 'v')
            {
                for (int i = 0; i < Length; i++)
                {
                    grid[row + i, col] = 'S';
                    Positions.Add((row + i, col)); // Sparar positionen
                }
            }
        }

        // Kontrollera om skeppet har sjunkit
        public bool IsSunk(char[,] grid)
        {
            foreach (var (row, col) in Positions)
            {
                if (grid[row, col] != 'X') // Om någon ruta fortfarande inte är träffad
                {
                    return false;
                }
            }
            return true; // Alla rutor har blivit träffade
        }

        // Hjälpmetoder för inmatning av koordinater och riktning
        public static int GetValidCoordinate(string prompt, int min, int max)
        {
            int coord;
            while (true)
            {
                Console.WriteLine($"Ange {prompt} ({min}-{max}):");
                string? input = Console.ReadLine();
                if (int.TryParse(input, out coord) && coord >= min && coord <= max)
                {
                    return coord;
                }
                Console.WriteLine("Ogiltig inmatning. Försök igen.");
            }
        }

        private char GetValidDirection()
        {
            while (true)
            {
                Console.WriteLine("Välj riktning (h för horisontellt, v för vertikalt):");
                string? input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input) && (input == "h" || input == "v"))
                {
                    return char.Parse(input);
                }
                Console.WriteLine("Ogiltig inmatning. Försök igen.");
            }
        }

        // Olika skepp
        public class Destroyer : Ship
        {
            public Destroyer() : base(2, "Destroyer") { }
        }

        public class Submarine : Ship
        {
            public Submarine() : base(3, "Submarine") { }
        }

        public class Battleship : Ship
        {
            public Battleship() : base(4, "Battleship") { }
        }

        public class Carrier : Ship
        {
            public Carrier() : base(5, "Carrier") { }
        }

    }
}