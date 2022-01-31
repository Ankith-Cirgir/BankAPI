using BankAPI.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI
{
    public class CustomerAccount
    {

        [MaxLength(45)]
        public string BankId { get; set; }

        [MaxLength(45)]
        public string AccountId { get; set; }

        public float Balance { get; set; }

        [MaxLength(45)]
        public string Name { get; set; }

        [MaxLength(45)]
        public string Password { get; set; }

        public int IsActive { get; set; }
    }
}
