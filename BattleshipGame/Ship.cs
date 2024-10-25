using System;

namespace BattleshipGame
{
    public class Ship
    {
        public int Length { get; private set; }
        public string Name { get; private set; }

        public Ship(int length, string name)
        {
            Length = length;
            Name = name;
        }

        //Metod för att placera skepp
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
                }
                else
                {
                    Console.WriteLine("Ogiltig placering. Försök igen.");
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
            if (direction == 'h')
            {
                for (int i = 0; i < Length; i++)
                {
                    grid[row, col + i] = 'S';
                }
            }
            else if (direction == 'v')
            {
                for (int i = 0; i < Length; i++)
                {
                    grid[row + i, col] = 'S';
                }
            }
        }

        // Hjälpmetoder för inmatning av koordinater och riktning
        private int GetValidCoordinate(string prompt, int min, int max)
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