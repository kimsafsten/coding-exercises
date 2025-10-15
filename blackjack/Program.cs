using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack
{
    internal class Program
    {
        static void Main()
        {
            int playerMoney = 200;
            Random rnd = new Random();

            Console.WriteLine("-----Blackjack-----");
            Console.WriteLine("Välkommen till spelet Blackjack");
            Console.WriteLine("Målet är att få 21 eller så nära som möjligt.");
            Console.WriteLine("Får du mer än 21 förlorar du");
            Console.WriteLine("Försök få högre än dealern.");

            while (playerMoney >= 5)
            {
                List<int> deckOfCards = CreateDeckOfCards();
                
                Console.WriteLine($"\nDu har {playerMoney} kr att satsa");
                int stake = GetStake(playerMoney);
                playerMoney -= stake;

                int playerValue = 0;
                int dealerValue = 0;
                int playerAce = 0;
                int dealerAce = 0;

                List<int> playerHand = new List<int>();
                List<int> dealerHand = new List<int>();

                playerHand.Add(DealCard(rnd, deckOfCards, ref playerAce));
                playerHand.Add(DealCard(rnd, deckOfCards, ref playerAce));
                playerValue = playerHand[0] + playerHand[1];

                dealerHand.Add(DealCard(rnd, deckOfCards, ref dealerAce));
                dealerHand.Add(DealCard(rnd, deckOfCards, ref dealerAce));
                dealerValue = dealerHand[0] + dealerHand[1];

                
                Console.WriteLine($"Dealerns synliga kort har värde {dealerHand[1]}");
                Console.WriteLine("Dealerns andra kort är dolt tills du är klar med din hand");
                ShowHand("Din hand", playerHand, playerValue);

                // --- Blackjack-control ---
                if (playerValue == 21)
                {
                    Console.WriteLine("\nDu fick Blackjack! Vi kollar om dealern också har Blackjack...");

                    if (dealerValue == 21)
                    {
                        Console.WriteLine("Dealern har också Blackjack! Oavgjort — du får tillbaka din insats.");
                        playerMoney += stake;
                    }
                    else
                    {
                        Console.WriteLine("Du har Blackjack och dealern har inte! Du vinner direkt!");
                        playerMoney += stake * 2;
                    }

                    if (WantToPlayAgain(playerMoney))
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                
                bool playerDone = false;
                bool playerBusted = false;

                while (!playerDone)
                {
                    Console.WriteLine("Vill du få ett kort till (h) eller stanna (s)?");
                    string choise = Console.ReadLine()?.ToLower();

                    if (choise == "h")
                    {
                        Console.WriteLine("Vill du satsa mer innan du får ett nytt kort? (y/n)");
                        string betMore = Console.ReadLine()?.ToLower();

                        if (betMore == "y")
                        {
                            int higherStakes = GetStake(playerMoney);
                            playerMoney -= higherStakes;
                            stake += higherStakes;
                            Console.WriteLine($"Du satsade {higherStakes} kr extra. Den totala summan du satsat är: {stake} kr");
                        }

                        int newCard = DealCard(rnd, deckOfCards, ref playerAce);
                        playerHand.Add(newCard);
                        playerValue += newCard;
                        playerValue = AdjustAce(playerValue, ref playerAce);
                        ShowHand("Din hand", playerHand, playerValue);

                        if (playerValue > 21)
                        {
                            Console.WriteLine("Dessvärre fick du över 21 och förlorar rundan.");
                            playerMoney -= stake;
                            playerBusted = true;
                            break;
                        }
                    }

                    else if (choise == "s")
                    {
                        playerDone = true;
                    }
                    else
                    {
                        Console.WriteLine("Ogiltligt val. Skriv 'h' för nytt kort och 's' för att stanna");
                    }
                }

                if (playerBusted == true)
                {
                    if (WantToPlayAgain(playerMoney))
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                Console.WriteLine("-----Dealerns tur-----");
                Console.WriteLine($"Dealerns dolda kort hade värdet {dealerHand[0]}");
                ShowHand("Dealerns hand", dealerHand, dealerValue);

                while (dealerValue < 17)
                {
                    int newCard = DealCard(rnd, deckOfCards, ref dealerAce);
                    dealerHand.Add(newCard);
                    dealerValue += newCard;
                    dealerValue = AdjustAce(dealerValue, ref dealerAce);
                    ShowHand("Dealerns hand", dealerHand, dealerValue);
                }

                playerMoney += DetermineWinner(playerValue, dealerValue, stake);

                if (playerMoney < 5)
                {
                    Console.WriteLine($"\nDu har {playerMoney} kr kvar och kan inte spela vidare");
                }

                if (WantToPlayAgain(playerMoney))
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
        }

        static List<int> CreateDeckOfCards()
        {
            var deckOfCards = new List<int>();

            for (int i = 0; i < 4; i++)
            {
                for (int v = 2; v <= 10; v++)
                    deckOfCards.Add(v);

                //Add knight, dame and king
                deckOfCards.Add(10);
                deckOfCards.Add(10);
                deckOfCards.Add(10);
                //Add ace
                deckOfCards.Add(11);
            }
            return deckOfCards;
        }

        static int DealCard(Random rnd, List<int> deckOfCards, ref int nrOfAce)
        {
            int index = rnd.Next(deckOfCards.Count);
            int card = deckOfCards[index];
            deckOfCards.RemoveAt(index);

            if (card == 11) nrOfAce++;
            return card;
        }

        static int AdjustAce(int sum, ref int nrOfAce)
        {
            while (sum > 21 && nrOfAce > 0)
            {
                sum -= 10;
                nrOfAce--;
            }
            return sum;
        }

        static void ShowHand(string rubric, List<int> hand, int sum)
        {
            Console.WriteLine($"{rubric}: [{string.Join(", ", hand)}] = {sum}");
        }

        static int GetStake(int playerMoney)
        {
            while (true)
            {
                Console.WriteLine("Hur många 5 kr vill du satsa?");
                string input = Console.ReadLine() ?? "";

                if (int.TryParse(input, out int nrOfFives))
                {
                    int stake = nrOfFives * 5;

                    if (stake >= 5 && stake <= playerMoney)
                    {
                        return stake;
                    }
                }
                Console.WriteLine("Ogiltlig insats. Ange ett antal 5-kronor du har råd med (minst 1).");
            }
        }

        static int DetermineWinner(int playerValue, int dealerValue, int stake)
        {
            Console.WriteLine($"\nDin poäng: {playerValue}, Dealerns poäng: {dealerValue}");

            if (dealerValue > 21)
            {
                Console.WriteLine($"Dealern fick över 21! Du vinner {stake} kr!");
                return stake * 2;
            }

            if (playerValue > 21)
            {
                Console.WriteLine($"Du fick över 21! Du förlorar {stake} kr!");
                return 0;
            }

            if (playerValue > dealerValue)
            {
                Console.WriteLine($"Din hand är värd mer. Du vinner {stake} kr!");
                return stake * 2; 
            }

            if (playerValue == dealerValue)
            {

                if (playerValue == 20 || playerValue == 21)
                {
                    Console.WriteLine("Det blev oavvgjort på 20 eller 21, du får tillbaka din insats");
                    return stake;
                }
                else 
                {
                    Console.WriteLine("Det blev oavgjort under 20, dealern vinner.");
                    Console.WriteLine($"Du förlorar {stake} kr");
                    return 0;
                }
            }

            Console.WriteLine($"Dealern vinner och du förlorar {stake} kr");
            return 0;
        }

        static bool WantToPlayAgain(int playerMoney)
        {
            Console.WriteLine($"Ditt saldo är nu {playerMoney}");
            Console.WriteLine("\nVill du spela en ny omgång? (y/n)");
            string answer = Console.ReadLine()?.ToLower();

            if (answer == "y")
            {
                Console.Clear();
                Console.WriteLine("-----Ny omgång-----");
                return true;
            }
            return false;
        }
    }
}
