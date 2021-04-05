using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities
{
    public class Wallet
    {
        public string JMBG { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public BankType Bank { get; private set; }
        public string BankAccountNumber { get; private set; }
        public string BankPIN { get; private set; }
        public decimal Balance { get; private set; }
        public ICollection<Transaction> Transactions { get; private set; }
        public string PASS { get; private set; }
        public DateTime LastTransactionDateTime { get; private set; }
        public decimal UsedDepositForCurrentMonth { get; private set; }
        public decimal UsedWithdrawalForCurrentMonth { get; private set; }
        public bool IsBlocked { get; private set; }
        public Wallet()
        {

        }
        public Wallet(
            string jMBG, 
            string firstName,
            string lastName,
            BankType bank, 
            string bankAccountNumber, 
            string bankPIN
            )
        {
            JMBG = jMBG;
            FirstName = firstName;
            LastName = lastName;
            Bank = bank;
            BankAccountNumber = bankAccountNumber;
            BankPIN = bankPIN;

            Transactions = new List<Transaction>();
            PASS = Guid.NewGuid().ToString().Substring(0, 6);
        }

        public void PayIn(decimal amount, TransactionType type, decimal maxDeposit)
        {
            if (UsedDepositForCurrentMonth + amount > maxDeposit)
            {
                throw new InvalidOperationException($"Transaction not allowed: Monthly deposit limit ({maxDeposit} RSD) would be exceeded.");
            }

            Balance += amount;

            if (LastTransactionDateTime.Month != DateTime.Now.Month 
                || LastTransactionDateTime.Year != DateTime.Now.Year)
            {
                UsedDepositForCurrentMonth = 0m;
                UsedWithdrawalForCurrentMonth = 0m;
            }

            UsedDepositForCurrentMonth += amount;

            var transaction = new Transaction(amount, type, this);

            Transactions.Add(transaction);

            LastTransactionDateTime = transaction.TransactionDateTime;
        }

    }
}
