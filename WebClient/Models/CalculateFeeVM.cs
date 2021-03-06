using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Models
{
    public class CalculateFeeVM
    {
        [Required]
        [StringLength(13, ErrorMessage = "JMBG must be 13 characters long. ", MinimumLength = 13)]
        public string JMBG { get; set; }
        [Required]
        public string PASS { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
