using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceStatusProcessorServer.Models;

public class Invoice
{
    [NotMapped]
    public int Id { get; set; }
    public string BankName { get; set; }
    public double Amount { get; set; }
    public string Status { get; set; }
    public DateTime UpdateAt { get; set; }
    public int RetryCount { get; set; }
    public DateTime LastAttemptAt { get; set; }

    public override string ToString()
    {
        Console.WriteLine("=========");
        Console.WriteLine(Id);
        Console.WriteLine(BankName);
        Console.WriteLine(Amount);
        Console.WriteLine(Status);
        Console.WriteLine(UpdateAt);
        Console.WriteLine(RetryCount);
        Console.WriteLine(LastAttemptAt);
        Console.WriteLine("=========");
        return base.ToString();
    }
}
