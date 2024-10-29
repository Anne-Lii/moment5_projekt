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

         // Konstruktor för att skapa ett skepp med specifik längd och namn
        public Ship(int length, string name)
        {
            Length = length;
            Name = name;
        }

        //Metod för att placera spelarens skepp på spelplanen
        public void PlaceShip(char[,] grid)
        {

            int row, col;
            char direction;
            bool validPlacement = false;

            Console.WriteLine($"Placera {Name} (längd: {Length})");

            //loopar tills en giltid plats hittas
            while (!validPlacement)

            {
                // Ber spelaren om rad och kolumn för att placera ut skeppen
                row = GetValidCoordinate("rad", 0, 9);
                col = GetValidCoordinate("kolumn", 0, 9);
                direction = GetValidDirection();

                //kontrollerar om skeppen kan placeras där
                if (CanPlaceShip(grid, row, col, direction))
                {
                    //Placerar skeppen på spelplanen
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

            // Loopar tills giltig placering hittas
            while (!validPlacement)
            {
                //skapar slumpmässiga koordinater samt riktning v eller h
                row = rnd.Next(0, 10);
                col = rnd.Next(0, 10);
                direction = rnd.Next(0, 2) == 0 ? 'h' : 'v'; // Slumpmässig riktning

                // Kontrollerar om skeppet kan placeras på utvald slumpmässig plats
                if (CanPlaceShip(grid, row, col, direction))
                {
                    // Placerar skeppet på spelplanen
                    PlaceShipOnGrid(grid, row, col, direction);
                    validPlacement = true;
                }
            }
        }


        //metoden för att kontrollera om skeppet får plats pga av andra placerade skepp
        private bool CanPlaceShip(char[,] grid, int row, int col, char direction)
        {
            if (direction == 'h')
            {
                if (col + Length > 9) return false; // om skeppet går utanför gränsen returnera false

                for (int i = 0; i < Length; i++)
                {
                    if (grid[row, col + i] == 'S') return false;// kontroll om en ruta redan är upptagen
                }
            }
            else if (direction == 'v')
            {
                if (row + Length > 9) return false;// om skeppet går utanför gränsen returnera false

                for (int i = 0; i < Length; i++)
                {
                    if (grid[row + i, col] == 'S') return false;// kontroll om en ruta redan är upptagen
                }
            }
            return true;
        }

        //Metod för att placera skeppet på spelplanen och spara positionen
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

        // Metod för att kontrollera om ett skepp har sjunkit
        public bool IsSunk(char[,] grid)
        {

            // Kontrollera om alla delar av skeppet är träffade (markeras med 'o')
            foreach (var (row, col) in Positions)
            {

                if (grid[row, col] != 'o') // Om en del inte är träffad, returnera falskt
                {
                    return false;
                }
            }

            // Om alla delar är träffade, ändra alla 'o' till 'X' för att signalera att skeppet är sänkt
            foreach (var (row, col) in Positions)
            {
                grid[row, col] = 'X'; // Ändra alla träffar till 'X'
            }

            return true; // Returnera att skeppet är sänkt
        }

        // metod för att ta emot och validera inmatning av koordinater från spelaren
        public static int GetValidCoordinate(string prompt, int min, int max)
        {
            int coord;
            while (true)
            {
                Console.WriteLine($"Ange {prompt} ({min}-{max}):");
                string? input = Console.ReadLine();
                if (int.TryParse(input, out coord) && coord >= min && coord <= max)
                {
                    return coord; // returnera giltig koordinat
                }
                Console.WriteLine("Ogiltig inmatning. Försök igen.");
            }
        }

        //metod för att välja riktning på placering av skepp
        private char GetValidDirection()
        {
            while (true)
            {
                Console.WriteLine("Välj riktning (h för horisontellt, v för vertikalt):");
                string? input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input) && (input == "h" || input == "v"))
                {
                    return char.Parse(input);// Returnera giltig riktning
                }
                Console.WriteLine("Ogiltig inmatning. Försök igen.");
            }
        }

        // klasser för de olika skeppen med namn och längd
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