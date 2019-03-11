using Blockchain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blockchain.Services
{
    public interface IBlockchainService
    {
        int Add(string sender, string recipient, int amount);

        Block Mine();

        IEnumerable<Block> Chain();

        bool AddNode(string url);

        IEnumerable<string> ListNodes();

        Task<bool> ResolveConflicts();
    }
}
