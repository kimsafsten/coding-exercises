
using SafstensBank;
using System;

class Program
{
    static void Main(string[] args)
    {
        Bank bank = new Bank();
        
        // Load existing accounts or create test accounts if none found
        bank.LoadFromFile();
        if (bank.AccountCount == 0)
        {
            bank.TestAccounts();
            bank.SaveToFile();
        }
                
        BankAccount account = LogIn(bank);

        while (true)
        {
            Console.WriteLine($"\n(Inloggad som: {account.Owner} - {account.PersonalNumber})");
            Console.WriteLine("\n1. Sätt in pengar");
            Console.WriteLine("2. Gör uttag");
            Console.WriteLine("3. Visa saldo");
            Console.WriteLine("4. Visa transaktioner");
            Console.WriteLine("5. Byt personnummer");
            Console.WriteLine("6. Avsluta");
            Console.Write("Val: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Belopp att sätta in: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal deposit))
                    {
                        bank.Deposit(account, deposit);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Ogiltlig inmatning! Skriv ett tal, t.ex. 100 eller 99,50.");
                        Console.ResetColor();
                    }
                    PauseAndClear();
                    break;

                case "2":
                    Console.Write("Belopp att ta ut: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal withdraw))
                    {
                        bank.Withdraw(account, withdraw); 
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Ogiltlig inmatning! Skriv ett tal, t.ex. 100 eller 99,50.");
                        Console.ResetColor();
                    }
                    PauseAndClear();
                    break;
                case "3":
                    Console.WriteLine($"Ditt saldo är: {account.Balance:F2} kr");
                    PauseAndClear();
                    break;
                case "4":
                    account.ShowTransactions();
                    PauseAndClear();
                    break;
                case "5":
                    account = LogIn(bank);
                    break;
                case "6":
                    return;
                default:
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ogiltligt val, försök igen.");
                    Console.ResetColor();
                    break;
            }
        }

        static BankAccount LogIn(Bank bank)
        {
            string pNr;

            while (true)
            {
                Console.Clear();
                Console.Write("Ange ditt personnummer (10 eller 12 siffror): ");
                pNr = Console.ReadLine();

                string nomalizedPn = NormalizePersonalNumber(pNr);

                if (nomalizedPn == null) 
                {
                    Console.WriteLine("\nOgiltligt personnummer. Måste vara 10 siffror (ÅÅMMDDXXXX)" + 
                        " eller 12 siffror som börjar med 19/20 (ÅÅÅÅMMDDXXXX).\n");
                    PauseAndClear();
                    continue;
                }

                pNr = nomalizedPn;
                break;
            }

            BankAccount account = bank.GetOrCreateAccount(pNr);

            return account;

            // Local method for normalizing 10/12 digit inputs to the standardized format YYMMDD-XXXX
            static string NormalizePersonalNumber(string input)
            {
                var digits = new string(input.Where(char.IsDigit).ToArray());

                if (digits.Length == 12)
                {
                    // Require either 19xx or 20xx for full birth year
                    var prefix = digits.Substring(0, 2);
                    if (prefix != "19" && prefix != "20")
                    {
                        return null;
                    }
                    return digits.Substring(2, 6) + "-" + digits.Substring(8, 4);
                }

                if (digits.Length == 10)
                {
                    return digits.Substring(0, 6) + "-" + digits.Substring(6, 4);
                }

                return null;
            }
        }

        static void PauseAndClear()
        {
            Console.WriteLine("\nTryck på valfri tangent för att återgå till menyn...");
            Console.ReadKey();
            Console.Clear();
        }
    }  
}
