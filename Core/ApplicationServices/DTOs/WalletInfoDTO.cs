using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ApplicationServices.DTOs
{
    public class WalletInfoDTO
    {
        public WalletInfoDTO(
            string jMBG, 
            string firstName, 
            string lastName, 
            BankType bank, 
            string bankAccountNumber, 
            decimal balance,
            bool isBlocked, 
            DateTime walletCreationTime)
        {
            JMBG = jMBG;
            FirstName = firstName;
            LastName = lastName;
            Bank = bank;
            BankAccountNumber = bankAccountNumber;
            Balance = balance;
            IsBlocked = isBlocked;
            WalletCreationTime = walletCreationTime;
        }

        public string JMBG { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public BankType Bank { get; set; }
        public string BankAccountNumber { get; set; }
        public decimal Balance { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime WalletCreationTime { get; set; }


    }
}
