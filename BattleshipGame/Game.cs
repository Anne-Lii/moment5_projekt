/*Anne-Lii Hansen
Spelet "Sänka skepp" eller "Battleship" med C#.NET */

using System;//importerar grundläggande funktioner från bibliotek
using System.Collections.Generic;//för att kunna använda list
using System.Diagnostics;//för att kunna använda stopwatch

namespace BattleshipGame
{
    public class Game
    {
        private List<(int, int)> targetQueue = new List<(int, int)>(); //lista att lagra koordinater som datorn ska skjuta på efter en träff
        private Stopwatch stopwatch = new Stopwatch();//timer för tidtagning
        private char[,] playerGrid;//spelplan för spelaren
        private char[,] computerGrid;//spelplan för datorn
        private Ship[] playerShips;//array för spelarens skepp
        private Ship[] computerShips;//array för datorns skepp


        //konstruktor för att skapa ett nytt spelobjekt
        public Game()
        {
            //skapar spelplaner
            playerGrid = new char[10, 10];
            computerGrid = new char[10, 10];
            //initierar skeppen
            playerShips = new Ship[] { new Ship.Carrier(), new Ship.Battleship(), new Ship.Submarine(), new Ship.Destroyer() };
            computerShips = new Ship[] { new Ship.Carrier(), new Ship.Battleship(), new Ship.Submarine(), new Ship.Destroyer() };

        }

        //metod för att initiera spelplaner och placera skepp för spelare och dator
        private void InitializeGame()
        {
            //initierar spelplanerna med vatten
            InitializeGrid(playerGrid);
            InitializeGrid(computerGrid);
            foreach (var ship in computerShips) ship.PlaceShipRandom(computerGrid);//placerar datorns skepp random
            foreach (var ship in playerShips) ship.PlaceShip(playerGrid);//spelaren placerar sina egna skepp
        }

        //metod för att starta spelet
        public void Start()
        {
            Console.Clear(); // Rensar konsollen
            Console.WriteLine("\nVälkommen till spelet Sänka Skepp!\n");
            Console.WriteLine("Spelet börjar nu. Placera dina skepp och försök sänka datorns skepp!\n");

            Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
            Console.ReadKey(); // Paus för att låta användaren läsa välkomsttexten

            InitializeGame();//initialiserar spelet

            stopwatch.Start();//startar tidtagning
            bool gameOn = true;

            while (gameOn)
            {
                Console.WriteLine("\nDatorns spelplan (före ditt skott):\n");
                PrintComputerGrid(computerGrid);//datorns spelplan

                PlayerShoot(computerGrid, computerShips);//spelarens tur att skjuta

                Console.WriteLine("\nDatorns spelplan:\n");
                PrintComputerGrid(computerGrid);//datorns spelplan
                Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
                Console.ReadKey();
                Console.Clear();//rensar konsollen

                ComputerShoot(playerGrid, playerShips);//datorns tur att skjuta

                //avslutar spelet när du sänkt alla datorns skepp
                if (AllShipsSunk(computerGrid))
                {
                    Console.Clear();//rensar konsollen
                    EndGame("GRATTIS, du har sänkt alla skepp! Du vann!!!");
                    PrintComputerGrid(computerGrid);//datorns spelplan
                    break;
                }

                //avslutar spelet om datorn sänkt alla dina skepp
                if (AllShipsSunk(playerGrid))
                {
                    EndGame("\nDatorn sänkte alla dina skepp. Du förlorade.");
                    break;
                }

                Console.WriteLine("\nDin spelplan:\n");
                PrintGrid(playerGrid);
            }

            Console.WriteLine("\nSpelet är över. Tack för att du spelade!\n");
        }

        private void EndGame(string message)
        {
            stopwatch.Stop();
            Console.WriteLine(message);
            Console.WriteLine($"\nSpelet varade i {stopwatch.Elapsed.Minutes} minuter och {stopwatch.Elapsed.Seconds} sekunder.\n");
        }

        //datorns skjutlogik
        private void ComputerShoot(char[,] grid, Ship[] playerShips)
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
                Console.WriteLine($"\nDatorn TRÄFFADE ett skepp på ({row}, {col})!");
                grid[row, col] = 'o'; // Markera träff med 'o'

                // Lägg till angränsande rutor för att skjuta på
                AddAdjacentTargets(row, col);

                // Kontrollera om skeppet som träffades har sjunkit
                foreach (var ship in playerShips)
                {
                    if (ship.Positions.Contains((row, col)))
                    {
                        // Kontrollera om hela skeppet är träffat / sänkt
                        if (ship.IsSunk(grid))
                        {
                            Console.WriteLine($"\nDatorn har SÄNKT ditt skepp: {ship.Name}!");

                            // Ändra alla 'o' på detta skepp till 'x' för att visa att skeppet är sänkt
                            foreach (var position in ship.Positions)
                            {
                                int shipRow = position.Item1;
                                int shipCol = position.Item2;

                                if (grid[shipRow, shipCol] == 'o') // Om det är en träff som ännu inte markerats som helt sänkt
                                {
                                    grid[shipRow, shipCol] = 'x'; // Ändrar till 'x'
                                }
                            }
                            break; // Avsluta loopen när skeppet har sänkts
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"\nDatorn MISSADE på ({row}, {col}).");
                grid[row, col] = '/'; // Markera miss
            }
        }


        private void PlayerShoot(char[,] grid, Ship[] computerShips)
        {
            bool validShot;

            do
            {
                Console.WriteLine("\nDin tur att skjuta\n");
                Console.WriteLine("Ange rad (0-9) eller 'q' för att avsluta spelet:");
                string? input = Console.ReadLine();

                // Om spelaren skriver "q", avsluta spelet
                if (input?.ToLower() == "q")
                {
                    Console.WriteLine("\nDu har valt att avsluta spelet.");
                    Environment.Exit(0); // Avslutar programmet direkt
                }

                // Fortsätter att fråga efter rad om spelaren inte vill avsluta
                validShot = int.TryParse(input, out int row) && row >= 0 && row <= 9;
                if (validShot)
                {
                    Console.WriteLine("Ange kolumn (0-9):");
                    input = Console.ReadLine();

                    validShot = int.TryParse(input, out int col) && col >= 0 && col <= 9;
                    if (validShot)
                    {
                        Console.WriteLine($"DEBUG: Skjuter på grid[{row}, {col}] = {grid[row, col]}"); // Kontroll av aktuellt innehåll

                        // Kontrollera om spelaren redan skjutit på denna ruta
                        if (grid[row, col] == 'o' || grid[row, col] == '/' || grid[row, col] == 'x' || grid[row, col] == 'X')
                        {
                            Console.WriteLine("Du har redan skjutit på denna ruta. Välj en annan.");
                            validShot = false; // Sätter till false för att stanna i loopen och få nya koordinater
                        }
                        else
                        {
                            // Om rutan är ledig, markera skottet som giltigt
                            validShot = true;

                            // Om spelaren träffar ett skepp
                            if (grid[row, col] == 'S')
                            {
                                Console.WriteLine($"\nDU TRÄFFADE ETT SKEPP PÅ RAD: {row}, KOLUMN: {col}");
                                grid[row, col] = 'o'; // Markera träff

                                // Kontrollera om skeppet som träffades har sänkts
                                foreach (var ship in computerShips)
                                {
                                    if (ship.Positions.Contains((row, col)) && ship.IsSunk(grid))
                                    {
                                        Console.WriteLine($"{ship.Name} har sänkts!");
                                        // Ändra alla 'o' till 'x' för detta skepp
                                        foreach (var position in ship.Positions)
                                        {
                                            int shipRow = position.Item1;
                                            int shipCol = position.Item2;

                                            if (grid[shipRow, shipCol] == 'o')
                                            {
                                                grid[shipRow, shipCol] = 'x'; // Markera sänkt skepp
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                // Om spelaren missar
                                Console.WriteLine($"\nDU MISSADE PÅ RAD: {row}, KOLUMN: {col}");
                                grid[row, col] = '/'; // Markera miss
                            }
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

            } while (!validShot); // Fortsätter tills spelaren gör ett giltigt skott
        }


        //hur datorn ska skjuta efter att den träffat ett skepp
        private void AddAdjacentTargets(int row, int col)
        {
            var directions = new List<(int, int)> { (-1, 0), (1, 0), (0, -1), (0, 1) };
            foreach (var (dr, dc) in directions)
            {
                int newRow = row + dr, newCol = col + dc;
                if (newRow >= 0 && newRow < 10 && newCol >= 0 && newCol < 10)
                {
                    targetQueue.Add((newRow, newCol));
                }
            }
        }


        //kontroll om alla skepp är sänkta
        private bool AllShipsSunk(char[,] grid)
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
        private void PrintGrid(char[,] grid)
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
        private void PrintComputerGrid(char[,] grid)
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
        private void InitializeGrid(char[,] grid)
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
