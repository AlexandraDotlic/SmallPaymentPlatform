using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Models
{
    public class WalletManagementVM
    {
        [Required]
        [StringLength(13, ErrorMessage = "JMBG must be 13 characters long. ", MinimumLength = 13)]
        public string WalletJMBG { get; set; }
        [Required]
        public string AdminPASS { get; set; }
        [Required]
        public string Action { get; set; }
    }
}
