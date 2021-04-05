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
    }
}
