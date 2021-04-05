using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Models
{
    public class WalletTransferVM
    {
        [Required]
        [StringLength(13, ErrorMessage = "JMBG must be 13 characters long. ", MinimumLength = 13)]
        public string SourceJMBG { get; set; }
        [Required]
        public string SourcePASS { get; set; }
        [Required]
        [StringLength(13, ErrorMessage = "JMBG must be 13 characters long. ", MinimumLength = 13)]
        public string DestinationJMBG { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
