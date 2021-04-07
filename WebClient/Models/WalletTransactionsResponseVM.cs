using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Models
{
    public class WalletTransactionsResponseVM
    {

        public WalletTransactionsResponseVM(ICollection<TransactionVM> transactionsVM)
        {
            TransactionVMs = transactionsVM;
        }

        public ICollection<TransactionVM> TransactionVMs { get; set; }
    }
}
