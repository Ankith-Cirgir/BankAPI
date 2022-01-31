using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Model
{
    public class Bank
    {
        public float Profits { get; set; }

        [MaxLength(45)]
        public string BankId { get; set; }

        [MaxLength(45)]
        public string BankName { get; set; }

        public float sRTGSCharge { get; set; }

        public float sIMPSCharge { get; set; }

        public float oRTGSCharge { get; set; }

        public float oIMPSCharge { get; set; }

    }
}
