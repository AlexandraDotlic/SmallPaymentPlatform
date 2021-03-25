using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime TransactionDateTime { get; private set; }
        public TransactionType Type { get; private set; }


    }
}
