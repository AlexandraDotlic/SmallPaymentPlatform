using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities
{
    public enum TransactionType: short
    {
        Undefined = 0,
        Deposit = 1,
        Withdraw = 2,
        TransferPayOut = 3,
        TransferPayIn = 4,
        FeePayOut = 5,
    }
}
