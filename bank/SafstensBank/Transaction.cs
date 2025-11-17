using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafstensBank;
public class Transaction
{
    public DateTime Date { get; set; }
    public string Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }

    public override string ToString()
    {
        return $"{Date}: {Type} {Amount} kr – nytt saldo: {BalanceAfter} kr";
    }
}
