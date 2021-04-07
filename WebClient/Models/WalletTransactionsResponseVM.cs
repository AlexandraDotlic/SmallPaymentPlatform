using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Models
{
    public class WalletTransactionsResponseVM
    {
        private decimal balance;
        private IEnumerable<TransactionVM> transactionsVM;

        public WalletTransactionsResponseVM(string jMBG, decimal balance, IEnumerable<TransactionVM> transactionsVM)
        {
            JMBG = jMBG;
            this.balance = balance;
            this.transactionsVM = transactionsVM;
        }

        public string JMBG { get; set; }
        public decimal WalletBalance { get; set; }
        public ICollection<TransactionVM> TransactionVMs { get; set; }
    }
}
