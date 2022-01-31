using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Model
{
    public class Transaction
    {
        [MaxLength(45)]
        public string TransactionId { get; set; }

        [MaxLength(45)]
        public string SenderId { get; set; }

        [MaxLength(45)]
        public string ReceiverId { get; set; }

        public float Amount { get; set; }

        public int Type { get; set; }

        [MaxLength(45)]
        public string Time { get; set; }

        public enum TransactionType
        {
            Withdraw = 1,
            Deposit,
            Transfer,
        }
    }
}