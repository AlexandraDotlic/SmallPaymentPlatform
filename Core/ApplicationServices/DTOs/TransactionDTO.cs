using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ApplicationServices.DTOs
{
    public class TransactionDTO
    {
        public TransactionDTO(int id, decimal amount, DateTime transactionDateTime, TransactionType type)
        {
            Id = id;
            Amount = amount;
            TransactionDateTime = transactionDateTime;
            Type = type;
        }

        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public TransactionType Type { get; set; }
    }
}
