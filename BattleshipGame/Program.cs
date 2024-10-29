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
            //skapar ett 10x10 rutnät var
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
                ship.PlaceShipRandom(computerGrid); // Placeras random av datorn, spelaren ser inte detta
            }

            Console.WriteLine("Datorn har placerat sina skepp.\n");


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

            Console.WriteLine("Alla skepp har placerats. Spelet börjar!\n");

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
                    break;
                }

                // Paus för att låta spelaren se resultatet
                Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
                Console.ReadKey(); // Väntar på att spelaren trycker på en tangent

                // Rensa konsollen innan datorns tur börjar
                Console.Clear();

                // Datorn skjuter på spelarens grid
                Console.WriteLine("\nDatorns tur att skjuta.");//debug
                ComputerShoot(playerGrid, playerShips);
                Console.WriteLine("\nDatorn har skjutit.");//debug

                // Kolla om datorn vann
                if (AllShipsSunk(playerGrid))
                {
                    Console.WriteLine("Datorn sänkte alla dina skepp. Du förlorade.");
                    gameOn = false;
                    break;
                }

                // Skriv ut spelarens grid efter varje tur
                Console.WriteLine("\nDitt rutnät:");
                PrintGrid(playerGrid);
            }

            Console.WriteLine("Spelet är över. Tack för att du spelade!");
        }

        static void ComputerShoot(char[,] grid, Ship[] playerShips)
        {
            Random rnd = new Random();
            int row, col;
            bool validShot = false;//variabel för kontroll om rutan blivit skjuten tidigare

            // Försök att hitta en ruta som inte har blivit skjuten på
            while (!validShot)
            {
                row = rnd.Next(0, 10);
                col = rnd.Next(0, 10);

                // Kolla om platsen redan är skjuten
                if (grid[row, col] == 'X' || grid[row, col] == 'O')
                {
                    // Om platsen redan har blivit skjuten, fortsätt försöka
                    continue;
                }

                // Om vi når hit, betyder det att platsen inte är skjuten på
                validShot = true;

                // Debug: Visa vilken rad och kolumn datorn försöker skjuta på
                Console.WriteLine($"Datorn skjuter på rad {row} och kolumn {col}...");

                if (grid[row, col] == 'S') // Träffar skepp
                {
                    Console.WriteLine($"Datorn träffade ett skepp på ({row}, {col})!");
                    grid[row, col] = 'X'; // Markera träff

                    // Lägg till angränsande rutor för att skjuta på
                    AddAdjacentTargets(row, col); // Lägg till angränsande mål i kön

                    // Kontrollera om skeppet som träffades har sjunkit
                    foreach (var ship in playerShips)
                    {
                        if (ship.Positions.Contains((row, col)) && ship.IsSunk(grid))
                        {
                            Console.WriteLine($"Datorn har sänkt ditt skepp: {ship.Name}!");
                            break;
                        }
                    }
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
            Console.WriteLine("Ange rad (0-9) eller 'q' för att avsluta spelet:");
            string? input = Console.ReadLine();

            // Om spelaren skriver "q", avsluta spelet
            if (input?.ToLower() == "q")
            {
                Console.WriteLine("Du har valt att avsluta spelet.");
                Environment.Exit(0); // Avslutar programmet direkt
            }

            // Fortsätt med att fråga efter rad om spelaren inte vill avsluta
            if (int.TryParse(input, out int row) && row >= 0 && row <= 9)
            {
                Console.WriteLine("Ange kolumn (0-9):");
                input = Console.ReadLine();

                if (int.TryParse(input, out int col) && col >= 0 && col <= 9)
                {
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
                else
                {
                    Console.WriteLine("Ogiltig kolumn. Försök igen.");
                }
            }
            else
            {
                Console.WriteLine("Ogiltig rad. Försök igen.");
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

