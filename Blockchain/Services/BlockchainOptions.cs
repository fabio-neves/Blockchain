using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Services
{
    public class BlockchainOptions
    {
        public string NodeId { get; set; }
        public IEnumerable<string> NodesAddresses { get; set; }
    }
}
