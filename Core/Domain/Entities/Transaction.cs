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
        public string WalletJMBG { get; private set; }
        public Wallet Wallet { get; private set; }

        public Transaction()
        {

        }
        public Transaction(decimal amount, TransactionType type, Wallet wallet)
        {
            Amount = amount;
            Type = type;
            Wallet = wallet;
            WalletJMBG = wallet.JMBG;
            TransactionDateTime = DateTime.Now;
        }


    }
}
