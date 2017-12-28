using Blockchain.Models;
using Blockchain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlockchainService _service;

        public HomeController(IBlockchainService service)
        {
            this._service = service;
        }

        public IActionResult Chain()
        {
            return new JsonResult(_service.Chain());
        }

        public IActionResult Mine()
        {
            return new JsonResult(_service.Mine());
        }

        [HttpPost]
        public IActionResult New([FromBody] TransactionInputModel transaction)
        {
            var index = _service.Add(transaction.sender, transaction.recipient, transaction.amount);
            return new JsonResult(new { message = string.Format("Transaction will be added to Block {0}", index) });
        }

        [HttpPost]
        public IActionResult Register([FromBody] NodesInputModel nodes)
        {
            if ( nodes != null && nodes.nodes != null)
            {
                foreach (var n in nodes.nodes)
                {
                    _service.AddNode(n);
                }
                return new JsonResult(new { message = "New nodes have been added.", total_nodes = _service.ListNodes() });
            }
            else
            {
                return new StatusCodeResult(400);
            }           
        }

        public async Task<IActionResult> ConsensusAsync()
        {
            var replaced = await _service.ResolveConflicts();

            if (replaced)
            {
                return new JsonResult(new
                {
                    message = "Our chain was replaced.",
                    chain = _service.Chain()
                });
            }
            else
            {
                return new JsonResult(new
                {
                    message = "Our chain is authoritative.",
                    chain = _service.Chain()
                });
            }
        }
    }
}
