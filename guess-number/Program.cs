namespace ConsoleApp1;

class Program
{
    static void Main(string[] args)
    {
        Random rnd = new Random();
        List<int> highscores = new List<int>();
        bool playAgain = true;
        
        Console.WriteLine("Välkommen till gissa talet");
        while (playAgain)
        {
            
            int secretNr = rnd.Next(0, 500);
            int guess = -1;
            int nrGuess = 1;
            int maxGuess = 15;

            Console.WriteLine("Du ska nu gissa ett tal mellan 0-500");
            Console.WriteLine($"Du har {maxGuess} försök på dig att gissa rätt");

            do
            {
                if (nrGuess == maxGuess)
                {
                    Console.WriteLine("Det här är ditt sista försök");
                }
                else
                {
                    Console.Write($"Gissning {nrGuess}: ");
                }

                if (!int.TryParse(Console.ReadLine(), out guess))
                {
                    Console.WriteLine("Skriv in ett giltligt nummer tack!");
                    continue;
                }

                if (guess > 500 || guess < 0)
                {
                    Console.WriteLine("Du måste skriva en siffra mellan 0-500, gissa igen");
                    continue;
                }

                if (guess == secretNr)
                {
                    Console.WriteLine($"Grattis du gissade rätt på {nrGuess} försöket!");
                    highscores.Add(nrGuess);
                    highscores = highscores.Order().ToList();
                    break;
                }
                else if (guess < secretNr)
                {
                    Console.WriteLine("Du gissade för lågt, försök igen.");
                }
                else
                {
                    Console.WriteLine("Du gissade för högt, försök igen");
                }

                if (Math.Abs(secretNr - guess) <= 10)
                {
                    Console.WriteLine("Du är dock nära");
                }

                nrGuess++;
            } while (guess != secretNr && nrGuess <= maxGuess);

            if (guess != secretNr)
            {
                Console.WriteLine($"Du lyckades dessvärre inte gissa rätt på {maxGuess} försök");
            }

            Console.WriteLine("Här är dina resultat, sorterat med bästa först");
            if (highscores.Count == 0)
            {
                Console.WriteLine("Du har inga tidigare resultat");
            }
            else
            {
                foreach (int highscore in highscores)
                {
                    Console.WriteLine($"Antal försök {highscore}");
                }
            }

            Console.WriteLine("Vill du spela igen? (ja)");
            string answer = Console.ReadLine()?.Trim().ToLower();
            if (answer != "ja")
            {
                playAgain = false;
                Console.WriteLine("Tack för att du spelade, tryck enter för att avsluta");
                Console.ReadKey();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Startar ny omgång");
            }
        }
    }
}