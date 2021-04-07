using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Models
{
    public class TransactionVM
    {
        public TransactionVM(int id, decimal amount, string type)
        {
            Id = id;
            Amount = amount;
            Type = type;
        }

        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
    }
}
