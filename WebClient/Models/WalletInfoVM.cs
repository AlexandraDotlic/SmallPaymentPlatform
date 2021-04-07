using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Models
{
    public class WalletInfoVM
    {
        public WalletInfoVM(WalletInfoRequestVM walletInfoRequestVM, WalletInfoResponseVM walletInfoResponseVM)
        {
            WalletInfoRequestVM = walletInfoRequestVM;
            WalletInfoResponseVM = walletInfoResponseVM;
        }

        public WalletInfoRequestVM WalletInfoRequestVM { get; set; }
        public WalletInfoResponseVM WalletInfoResponseVM { get; set; }
    }
}
