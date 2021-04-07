using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Models
{
    public class WalletTransactionsVM
    {
        public WalletTransactionsVM(WalletTransactionsRequestVM walletTransactionsRequestVM, WalletTransactionsResponseVM walletTransactionsResponseVM)
        {
            WalletTransactionsRequestVM = walletTransactionsRequestVM;
            WalletTransactionsResponseVM = walletTransactionsResponseVM;
        }

        public WalletTransactionsRequestVM WalletTransactionsRequestVM { get; set; }
        public WalletTransactionsResponseVM WalletTransactionsResponseVM { get; set; }
    }
}
