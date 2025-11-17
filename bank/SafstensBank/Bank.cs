using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SafstensBank;
public class Bank
{
    private readonly string dataFilePath = "bankdata.json";
    private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    private Dictionary<string, BankAccount> accounts = new();

    public int AccountCount => accounts.Count;

    /*Loads all accounts from the JSON file (if found). 
     * If no file exists, the bank will start with an empty set of accounts.
     */ 
    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(dataFilePath))
            {
                Console.WriteLine("Ingen bankdata hittades - startar med en tom fil.");
                return;
            }

            var json = File.ReadAllText(dataFilePath);
            var list = JsonSerializer.Deserialize<List<BankAccount>>(json, jsonOptions);
            accounts = new Dictionary<string, BankAccount>(StringComparer.Ordinal);

            if (list != null)
            {
                foreach (var acc in list)
                {
                    if (!string.IsNullOrWhiteSpace(acc.PersonalNumber))
                    {
                        if (acc.Transactions != null && acc.Transactions.Any())
                        {
                            acc.RestoreBalance(acc.Transactions.Last().BalanceAfter);
                        }
                        else
                        {
                            acc.RestoreBalance(0);
                        }
                        accounts[acc.PersonalNumber] = acc;
                    }
                }
            }

            Console.WriteLine($"(Laddade {accounts.Count} konton från {dataFilePath})");
        }
        catch (Exception e)
        {
            Console.WriteLine($"[FEL] Kunde inte läsa {dataFilePath}: {e.Message}");
        }
    }

    //Storing current account state to the JSON file.
    public void SaveToFile()
    {
        try
        {
            var list = new List<BankAccount>(accounts.Values);
            var json = JsonSerializer.Serialize(list, jsonOptions);
            File.WriteAllText(dataFilePath, json);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[FEL] Kunde inte spara {dataFilePath}: {e.Message}");
        }
    }


    public BankAccount GetOrCreateAccount(string personalNumber)
    {
        if (!accounts.ContainsKey(personalNumber))
        {
            Console.Write("Ange namn för nytt konto: ");
            string name = Console.ReadLine();
            accounts[personalNumber] = new BankAccount(name, personalNumber);
            Console.WriteLine("Nytt konto skapat!");
            SaveToFile();
        }
        return accounts[personalNumber];
    }

    public void Deposit(BankAccount account, decimal amount)
    { 
        account.Deposit(amount);
        SaveToFile();
    }

    public void Withdraw(BankAccount account, decimal amount)
    {
        account.Withdraw(amount);
        SaveToFile();
    }

    //Adding premade testaccounts with transactions, run at start
    public void TestAccounts()
    {
        var p1 = "010101-0101";
        if (!accounts.ContainsKey(p1))
        {
            var a1 = new BankAccount("Stina Andersson", p1);
            accounts[p1] = a1;
            Deposit(a1, 4750m);
        }

        var p2 = "101010-1010";
        if (!accounts.ContainsKey(p2))
        {
            var a2 = new BankAccount("Åke Persson", p2);
            accounts[p2] = a2;
            Deposit(a2, 1200m);
        }

        var p3 = "800808-0808";
        if (!accounts.ContainsKey(p3))
        {
            var a3 = new BankAccount("Ulf Eriksson", p3);
            accounts[p3] = a3;
            Deposit(a3, 580m);
        }

        Console.WriteLine("(3 testkonton har laddats: Stina, Åke och Ulf)");
        SaveToFile();
    }
}
