using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Model
{
    public class StaffAccount
    {
        [MaxLength(45)]
        public string AccountId { get; set; }

        [MaxLength(45)]
        public string Name { get; set; }

        [MaxLength(45)]
        public string Password { get; set; }

        public string BankId { get; set; }
    }
}
