using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Models
{
    public class TransactionVM
    {
        public TransactionVM(int id, decimal amount, string type, DateTime date)
        {
            Id = id;
            Amount = amount;
            Type = type;
            AmountString = string.Format("{0:n}", Amount);
            TransactionDate = date;
        }

        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string AmountString { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
