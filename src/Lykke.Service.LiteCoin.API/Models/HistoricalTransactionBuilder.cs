using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.LiteCoin.API.Core.Transactions;
using Lykke.Service.LiteCoin.API.Helpers;

namespace Lykke.Service.LiteCoin.API.Models
{
    public static class HistoricalTransactionBuilder
    {
        public static HistoricalTransactionContract ToHistoricalTransaction(this HistoricalTransactionDto source)
        {
            return new HistoricalTransactionContract
            {
                ToAddress = source.ToAddress,
                FromAddress = source.FromAddress,
                AssetId = source.AssetId,
                Amount = MoneyConversionHelper.SatoshiToContract(source.AmountSatoshi),
                Hash = source.TxHash,
                Timestamp = source.TimeStamp
            };
        }
    }
}
