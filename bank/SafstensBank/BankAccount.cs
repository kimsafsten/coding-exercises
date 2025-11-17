using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafstensBank;
public class BankAccount
{
    public string Owner { get; set; }
    public string PersonalNumber { get; set; }
    public decimal Balance { get; private set; }
    public List<Transaction> Transactions { get; set; }

    public BankAccount(string owner, string personalNumber)
    {
        Owner = owner;
        PersonalNumber = personalNumber;
        Balance = 0;
        Transactions = new List<Transaction>();
    }
    //Constructor used by JSON for file loading
    public BankAccount() { }

    //Internal function used by JSON to ensure the balance is correct
    internal void RestoreBalance(decimal amount)
    {
        Balance = amount;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) 
        {
            Console.WriteLine("Beloppet måste vara större än 0.");
            return; 
        }
        
        Balance += amount;
        Transactions.Add(new Transaction
        {
            Date = DateTime.Now,
            Type = "Insättning",
            Amount = amount,
            BalanceAfter = Balance
        });
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Du satte in {amount} kr. Nytt saldo: {Balance} kr.");
        Console.ResetColor();
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Beloppet måste vara större än 0.");
            return;
        }
        
        if (amount > Balance)
        {
            Console.WriteLine("Du har inte tillräckligt med pengar på kontot!");
            
            return;
        }

        Balance -= amount;
        Transactions.Add(new Transaction
        {
            Date = DateTime.Now,
            Type = "Uttag",
            Amount = amount,
            BalanceAfter = Balance
        });

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Du tog ut {amount} kr. Nytt saldo: {Balance} kr.");
        Console.ResetColor();
    }

    public void ShowTransactions()
    {
        Console.WriteLine("\n--- TRANSAKTIONSHISTORIK ---");
        if (Transactions.Count == 0)
        {
            Console.WriteLine("Inga transaktioner ännu.");
            return;
        }

        foreach (var t in Transactions)
        {
            if (t.Type == "Insättning")
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (t.Type == "Uttag")
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ResetColor();
            }

            Console.WriteLine(t);

            Console.ResetColor();
        }
    }
}
