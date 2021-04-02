using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Models
{
    public class WalletVM
    {
        public WalletVM()
        {
            PASS = "123456";
        }

        public WalletVM(
            string jMBG, 
            string firstName, 
            string lastName, 
            short bankType,
            string bankAccountNumber, 
            string bankPIN)
        {
            JMBG = jMBG;
            FirstName = firstName;
            LastName = lastName;
            BankType = bankType;
            BankAccountNumber = bankAccountNumber;
            BankPIN = bankPIN;
        }

        [Required]
        [StringLength(13, ErrorMessage = "JMBG must be 13 characters long. ", MinimumLength = 13)]
        public string JMBG { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [StringLength(4, ErrorMessage = "BankPIN must be 4 characters long. ", MinimumLength = 4)]
        public string BankPIN { get; set; }
        [Required]
        [StringLength(18, ErrorMessage = "BankAccountNumber cannot be longer than 18 characters.")]
        public string BankAccountNumber { get; set; }
        [Required]
        public short BankType { get; set; }
        public string PASS { get; set; }
    }
}
