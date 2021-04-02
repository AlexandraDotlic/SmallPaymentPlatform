﻿using System;
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

    }
}