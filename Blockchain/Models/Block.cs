using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Models
{
    public class Block
    {
        public int Index { get; set; }

        public long Timestamp { get; set; }

        public IEnumerable<Transaction> Transactions { get; set; }

        public int Proof { get; set; }

        public string PreviousHash { get; set; }
    }
}
