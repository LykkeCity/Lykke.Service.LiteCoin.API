using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.Constants;
using Lykke.Service.LiteCoin.API.Core.Transactions;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Transactions
{ 
    public class HistoryService:IHistoryService
    {
        private readonly IBlockChainProvider _blockChainProvider;

        public HistoryService( IBlockChainProvider blockChainProvider)
        {
            _blockChainProvider = blockChainProvider;
        }

        public Task<IEnumerable<HistoricalTransactionDto>> GetHistoryFrom(BitcoinAddress address, string afterHash, int take)
        {
            return GetHistory(address, afterHash, take, isSend: true);
        }

        public Task<IEnumerable<HistoricalTransactionDto>> GetHistoryTo(BitcoinAddress address, string afterHash, int take)
        {
            return GetHistory(address, afterHash, take, isSend: false);
        }

        public async Task<IEnumerable<HistoricalTransactionDto>> GetHistory(BitcoinAddress address, string afterHash, int take, bool isSend)
        {
            var txIds = (await _blockChainProvider.GetTransactionsForAddress(address)).Reverse();

            if (!string.IsNullOrEmpty(afterHash))
            {
                txIds = txIds.SkipWhile(p => p != afterHash).Skip(1);
            }

            var result = new List<HistoricalTransactionDto>();

            foreach (var txId in txIds)
            {
                if (result.Count >= take)
                {
                    break;
                }

                var tx = await _blockChainProvider.GetAggregatedInputsOutputs(txId);

                if (IsSend(tx, address.ToString()) == isSend)
                {
                    result.Add(MapToHistoricalTransaction(tx, address.ToString()));
                }
            }

            return result;
        }

        private bool IsSend(AggregatedInputsOutputs tx, string requestedAddress)
        {
            return tx.Inputs.Where(p => p.Address == requestedAddress).Sum(p => p.AmountSatoshi) >=
                   tx.Outputs.Where(p => p.Address == requestedAddress).Sum(p => p.AmountSatoshi);
        }

        private HistoricalTransactionDto MapToHistoricalTransaction(AggregatedInputsOutputs tx, string requestedAddress)
        {
            var from = tx.Inputs.OrderByDescending(p => p.AmountSatoshi).FirstOrDefault();
            var to = tx.Outputs.OrderByDescending(p => p.AmountSatoshi).FirstOrDefault(p => p.Address != from?.Address);

            return new HistoricalTransactionDto
            {
                TxHash = tx.TxHash,
                IsSend = IsSend(tx, requestedAddress),
                AmountSatoshi = to?.AmountSatoshi ?? 0,
                FromAddress = from?.Address,
                AssetId = Constants.Assets.LiteCoin.AssetId,
                ToAddress = to?.Address,
                TimeStamp = tx.TimeStamp
            };
        }
    }
}
