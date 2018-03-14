using System;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Operation
{
    public interface IOperationService
    {
        Task<Transaction> GetOrBuildTransferTransaction(Guid operationId, 
            BitcoinAddress fromAddress,
            PubKey fromAddressPubkey,
            BitcoinAddress toAddress,
            string assetId,
            Money amountToSend, 
            bool includeFee);
    }
}
