/*Anne-Lii Hansen
Spelet "Sänka skepp" eller "Battleship" med C#.NET */

//importerar nödvändiga bibliotek
using System;
using System.Collections.Generic;// Tillägg för att hantera listor
using System.Diagnostics;  // Tillägg för Stopwatch

namespace BattleshipGame
{
    class Program
    {
        //En lista för att lagra datorns mål(angränsande rutor till träffarna)
        static List<(int, int)> targetQueue = new List<(int, int)>();

        static void Main(string[] args)
        {

            // Skapar en Stopwatch-instans för att mäta tiden
            Stopwatch stopwatch = new Stopwatch();

            //skapar ett 10x10 rutnät/spelplan var
            char[,] playerGrid = new char[10, 10];
            char[,] computerGrid = new char[10, 10];

            //initierar spelplanerna med ~ symboler för vatten
            InitializeGrid(playerGrid);
            InitializeGrid(computerGrid);

            Console.WriteLine("Välkommen till SÄNKA SKEPP!\n");

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

            // Startar klockan när spelet börjar
            stopwatch.Start();
            Console.WriteLine("Klockan har startats!\n");

            //variabel för att hålla koll på om spelet pågår eller ej
            bool gameOn = true;

            //Spelloop tills spelet är slut
            while (gameOn)
            {

                // Visa datorns spelplan innan spelaren skjuter
                Console.WriteLine("\nDatorns spelplan (före ditt skott):\n");
                PrintComputerGrid(computerGrid);


                Console.WriteLine("\nDin tur att skjuta\n");
                PlayerShoot(computerGrid, computerShips);

                // Kolla om spelaren vann
                if (AllShipsSunk(computerGrid))
                {
                    // Stoppar klockan vid vinst
                    stopwatch.Stop();

                    Console.WriteLine("GRATTIS, du har sänkt alla skepp! Du vann!!!");
                    Console.WriteLine($"Spelet varade i {stopwatch.Elapsed.Minutes} minuter och {stopwatch.Elapsed.Seconds} sekunder.");
                    PrintComputerGrid(computerGrid);
                    gameOn = false;
                    break;//avslutar spelet
                }

                // Visar datorns spelplan efter att spelaren har skjutit
                Console.WriteLine("\nDatorns spelplan:\n");
                PrintComputerGrid(computerGrid);

                // Paus för att låta spelaren se resultatet
                Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
                Console.ReadKey(); // Väntar på att spelaren trycker på en tangent

                // Rensa konsollen innan datorns tur börjar
                Console.Clear();

                // Datorn skjuter på spelarens spelplan
                Console.WriteLine("\nDatorns tur att skjuta.");//debug
                ComputerShoot(playerGrid, playerShips);

                // Kolla om datorn vann
                if (AllShipsSunk(playerGrid))
                {
                    // Stoppar klockan vid förlust
                    stopwatch.Stop();

                    Console.WriteLine("Datorn sänkte alla dina skepp. Du förlorade.");
                    Console.WriteLine($"Spelet varade i {stopwatch.Elapsed.Minutes} minuter och {stopwatch.Elapsed.Seconds} sekunder.");
                    gameOn = false;
                    break;//avslutar spelet
                }

                // Skriv ut spelarens spelplan efter varje tur
                Console.WriteLine("\nDin spelplan:\n");
                PrintGrid(playerGrid);
            }

            Console.WriteLine("Spelet är över. Tack för att du spelade!");
        }

        // Datorns sjuklogik, väljer slumpmässigt rutor att skjuta på
        static void ComputerShoot(char[,] grid, Ship[] playerShips)
        {
            Random rnd = new Random();
            int row = 0, col = 0;
            bool validShot = false;

            // Kontrollera om det finns mål i targetQueue att skjuta på
            if (targetQueue.Count > 0)
            {
                (row, col) = targetQueue[0]; // Välj det första målet från kön
                targetQueue.RemoveAt(0); // Ta bort målet från kön efter att ha skjutit
                validShot = true; // Vi har redan valt en giltig ruta från targetQueue
            }
            else
            {
                // Om det inte finns några mål i kön, välj en slumpmässig ruta
                while (!validShot)
                {
                    row = rnd.Next(0, 10);
                    col = rnd.Next(0, 10);

                    // Kolla om platsen redan är skjuten
                    if (grid[row, col] == 'x' || grid[row, col] == '/' || grid[row, col] == 'o')
                    {
                        continue; // Om platsen redan har blivit skjuten, fortsätt försöka
                    }

                    validShot = true; // Hittar en giltig ruta att skjuta på
                }
            }

            if (grid[row, col] == 'S') // Träffar skepp
            {
                Console.WriteLine($"Datorn TRÄFFADE ett skepp på ({row}, {col})!");
                grid[row, col] = 'o'; // Markera träff med 'o'

                // Lägg till angränsande rutor för att skjuta på
                AddAdjacentTargets(row, col);

                // Kontrollera om skeppet som träffades har sjunkit
                foreach (var ship in playerShips)
                {
                    if (ship.Positions.Contains((row, col)))
                    {
                        // Kontrollera om hela skeppet är sänkt
                        if (ship.IsSunk(grid))
                        {
                            Console.WriteLine($"Datorn har SÄNKT ditt skepp: {ship.Name}!");

                            // Ändra alla 'o' på detta skepp till 'x' för att visa att skeppet är sänkt
                            foreach (var position in ship.Positions)
                            {
                                int shipRow = position.Item1;
                                int shipCol = position.Item2;

                                if (grid[shipRow, shipCol] == 'o') // Om det är en träff som ännu inte markerats som sänkt
                                {
                                    grid[shipRow, shipCol] = 'x'; // Ändra till 'x'
                                }
                            }
                            break; // Avsluta loopen när skeppet har sänkts
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"Datorn MISSADE på ({row}, {col}).");
                grid[row, col] = '/'; // Markera miss
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
        static void PlayerShoot(char[,] grid, Ship[] computerShips)
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

                    Console.Clear();//rensar konsollen
                    if (grid[row, col] == 'S')
                    {
                        Console.WriteLine("Du TRÄFFADE ett skepp!");
                        grid[row, col] = 'o'; // Markera träff

                        // Kontrollera om skeppet som träffades har sänkts
                        foreach (var ship in computerShips)
                        {
                            if (ship.Positions.Contains((row, col)))
                            {
                                if (ship.IsSunk(grid))
                                {
                                    Console.WriteLine($"{ship.Name} har sänkts!");
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Du MISSADE.");
                        grid[row, col] = '/'; // Markera miss
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

        //Metod för att skriva ut spelplanen till konsollen
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

        // Metod för att skriva ut datorns spelplan, utan att avslöja skeppen
        static void PrintComputerGrid(char[,] grid)
        {
            Console.WriteLine("  0 1 2 3 4 5 6 7 8 9 "); // rubriker till kolumnerna
            for (int row = 0; row < 10; row++)
            {
                Console.Write(row + " "); // radnumrering
                for (int col = 0; col < 10; col++)
                {
                    if (grid[row, col] == 'S') // Om det är ett skepp, visa bara vatten
                    {
                        Console.Write("~ ");
                    }
                    else
                    {
                        Console.Write(grid[row, col] + " "); // Visa träff, miss eller vatten
                    }
                }
                Console.WriteLine();// tom rad
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

