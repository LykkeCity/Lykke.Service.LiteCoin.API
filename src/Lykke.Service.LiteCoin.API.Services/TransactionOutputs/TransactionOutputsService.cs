﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.TransactionOutputs
{
    public class TransactionOutputsService : ITransactionOutputsService
    {
        private readonly IBlockChainProvider _blockChainProvider;

        public TransactionOutputsService(IBlockChainProvider blockChainProvider)
        {
            _blockChainProvider = blockChainProvider;
        }

        public async Task<IEnumerable<Coin>> GetUnspentOutputs(string address, int confirmationsCount = 0)
        {
            return (await _blockChainProvider.GetUnspentOutputs(address, confirmationsCount)).ToList();
        }

    }
}
