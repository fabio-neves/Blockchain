using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    public class Transaction
    {
        public string Sender { get; set; }

        public string Recipient { get; set; }

        public int Amount { get; set; }
    }
}
