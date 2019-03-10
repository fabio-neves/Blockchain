using Blockchain.Helpers;
using Blockchain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Services
{
    public class BlockchainOptions
    {
        public string NodeId { get; set; }
    }

    public class Blockchain : IBlockchainService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private BlockchainOptions _options;
        private List<Block> _chain;
        private List<Transaction> _currentTransactions;
        private HashSet<string> _nodes;

        public Blockchain(BlockchainOptions options)
        {
            _options = options;
            _chain = new List<Block>();
            _currentTransactions = new List<Transaction>();
            _nodes = new HashSet<string>();
            AddBlock(1, "100");
        }

        private Block LastBlock
        {
            get
            {
                return _chain.LastOrDefault();
            }
        }


        private int AddTransaction(string sender, string recipient, int amount)
        {
            _currentTransactions.Add(new Transaction
            {
                Sender = sender,
                Recipient = recipient,
                Amount = amount
            });

            return _chain.Count + 1;
        }

        private Block AddBlock(int proof, string previousHash = "")
        {
            var block = new Block
            {
                Index = _chain.Count + 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                PreviousHash = previousHash,
                Transactions = _currentTransactions.ToList(),
                Proof = proof
            };

            _currentTransactions.Clear();

            _chain.Add(block);

            return block;
        }

        private int ProofOfWork(int lastProof)
        {
            var guess = 0;

            while (!IsValidProof(lastProof, guess))
            {
                guess++;
            }

            return guess;
        }

        private bool IsValidProof(int lastProof, int guess)
        {
            var guessHash = Hash(string.Format("{0}{1}", lastProof, guess));

            if (guessHash.Length < 4)
            {
                return false;
            }

            return guessHash.Substring(guessHash.Length - 4) == "0000";
        }

        private string Hash(Block block)
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new OrderedContractResolver()
            };

            var json = JsonConvert.SerializeObject(block, Formatting.None, settings);

            return Hash(json);
        }

        private string Hash(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            SHA256Managed hashstring = new SHA256Managed();

            byte[] hash = hashstring.ComputeHash(bytes);

            var result = new StringBuilder();

            foreach (byte c in hash)
            {
                result.Append(String.Format("{0:x2}", c));
            }

            return result.ToString();
        }

        private bool IsValidChain(List<Block> chain)
        {
            if (chain == null || chain.Count == 0)
            {
                return false;
            }

            var lastBlock = chain[0];

            for (var idx = 1; idx < chain.Count; idx++)
            {
                var block = chain[idx];

                if (block.PreviousHash == Hash(lastBlock))
                {
                    return false;
                }

                if (!IsValidProof(lastBlock.Proof, block.Proof))
                {
                    return false;
                }

                lastBlock = block;
            }

            return true;
        }

        public async Task<bool> ResolveConflicts()
        {
            var maxsize = _chain.Count;

            List<Block> newChain = null;

            foreach (var s in _nodes)
            {
                var result = await _httpClient.GetAsync(s);
                if (result.IsSuccessStatusCode)
                {
                    var json = await result.Content.ReadAsStringAsync();

                    var chain = JsonConvert.DeserializeObject<List<Block>>(json);

                    if (IsValidChain(chain) && maxsize < chain.Count)
                    {
                        maxsize = chain.Count;
                        newChain = chain;
                    }
                }
            }

            if (newChain != null)
            {
                _chain = newChain;
                return true;
            }

            return false;
        }

        public int Add(string sender, string recipient, int amount)
        {
            return AddTransaction(sender, recipient, amount);
        }

        public Block Mine()
        {
            AddTransaction("0", _options.NodeId, 1);

            return AddBlock(ProofOfWork(LastBlock.Proof), Hash(LastBlock));
        }

        public IEnumerable<Block> Chain()
        {
            return _chain;
        }

        public bool AddNode(string url)
        {
            Uri result;
            if (Uri.TryCreate(url, UriKind.Absolute, out result))
            {
                _nodes.Add(string.Format("{0}:{1}", result.Host, result.Port));
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<string> ListNodes()
        {
            return _nodes;
        }
    }
}
