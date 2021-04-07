using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ApplicationServices.DTOs
{
    public class WalletTransactionsDTO
    {
        public WalletTransactionsDTO(string jMBG, decimal balance, ICollection<TransactionDTO> transactions)
        {
            JMBG = jMBG;
            Balance = balance;
            Transactions = transactions;
        }

        public string JMBG { get; set; }
        public decimal Balance { get; set; }
        public ICollection<TransactionDTO> Transactions { get; set; }
    }
}
