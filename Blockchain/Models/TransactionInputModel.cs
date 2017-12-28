using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    public class TransactionInputModel
    {
        public string sender { get; set; }
        public string recipient { get; set; }
        public int amount { get; set; }
    }
}
