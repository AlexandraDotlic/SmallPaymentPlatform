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
        public DateTime LastTransactionDateTime { get; private set; }
        public decimal UsedDepositForCurrentMonth { get; private set; }
        public decimal UsedWithdrawalForCurrentMonth { get; private set; }
        public bool IsBlocked { get; private set; }
        public DateTime WalletCreationTime { get; private set; }
        public DateTime LastTransferDateTime { get; private set; }

        private string PASS;

        public Wallet()
        {
            Transactions = new List<Transaction>();
        }
        public Wallet(
            string jMBG, 
            string firstName,
            string lastName,
            BankType bank, 
            string bankAccountNumber, 
            string bankPIN,
            string password
            )
        {
            JMBG = jMBG;
            FirstName = firstName;
            LastName = lastName;
            Bank = bank;
            BankAccountNumber = bankAccountNumber;
            BankPIN = bankPIN;
            PASS = password;
            Transactions = new List<Transaction>();
            WalletCreationTime = DateTime.Now;
        }

        public bool IsPassValid(string inputPass)
        {
            return PASS == inputPass;
        }

        public void ChangePass(string newPass)
        {
            PASS = newPass;
        }

        public void PayIn(decimal amount, TransactionType transactionType, decimal maxDeposit)
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

            var transaction = new Transaction(amount, transactionType, this);

            Transactions.Add(transaction);

            LastTransactionDateTime = transaction.TransactionDateTime;
        }
        public void PayOut(decimal amount, TransactionType transactionType, decimal maxWithdrawal)
        {
            if (UsedWithdrawalForCurrentMonth + amount > maxWithdrawal)
            {
                throw new InvalidOperationException($"Transaction not allowed: Monthly withdrawal limit ({maxWithdrawal} RSD) would be exceeded.");
            }
            if(amount > Balance)
            {
                throw new InvalidOperationException($"insufficient funds in wallet with JMBG = {JMBG}");
            }
            Balance -= amount;

            if (LastTransactionDateTime.Month != DateTime.Now.Month
               || LastTransactionDateTime.Year != DateTime.Now.Year)
            {
                UsedDepositForCurrentMonth = 0m;
                UsedWithdrawalForCurrentMonth = 0m;
            }

            UsedWithdrawalForCurrentMonth += amount;
            var transaction = new Transaction(amount, transactionType, this);
            Transactions.Add(transaction);
            LastTransactionDateTime = transaction.TransactionDateTime;
            if(transactionType == TransactionType.TransferPayOut)
            {
                LastTransferDateTime = transaction.TransactionDateTime;
            }
        }

        public void Block()
        {
            IsBlocked = true;
        }

        public void Unblock()
        {
            IsBlocked = false;
        }

    }
}
