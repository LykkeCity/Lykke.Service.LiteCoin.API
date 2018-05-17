using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.Transactions
{
    public interface IBroadcastedTransaction
    {
        string Address { get; }

        string TxHash { get; }

        Guid OperationId { get; }
    }

    public class BroadcastedTransaction:IBroadcastedTransaction
    {
        public string Address { get; set; }
        public string TxHash { get; set; }
        public Guid OperationId { get; set; }

        public static BroadcastedTransaction Create(Guid operationId, string address, string txhash)
        {
            return new BroadcastedTransaction
            {
                Address = address,
                OperationId = operationId,
                TxHash = address
            };
        }
    }

    public interface IBroadcastedTransactionRepository
    {
        Task<IEnumerable<IBroadcastedTransaction>> GetTransactionsForAddress(string address);

        Task InsertOrReplace(IBroadcastedTransaction transaction);
    }
}
