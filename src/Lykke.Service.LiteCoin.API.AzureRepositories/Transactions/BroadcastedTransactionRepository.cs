using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.Transactions;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Transactions
{
    public class BroadcastedTransactionEntity : TableEntity, IBroadcastedTransaction
    {
        public string Address { get; set; }
        public string TxHash { get; set; }
        public Guid OperationId { get; set; }

        public static string GeneratePartitionKey(string address)
        {
            return address;
        }

        public static string GenerateRowKey(string txHash)
        {
            return txHash;
        }

        public static BroadcastedTransactionEntity Create(IBroadcastedTransaction source)
        {
            return new BroadcastedTransactionEntity
            {
                OperationId = source.OperationId,
                TxHash = source.TxHash,
                Address = source.Address,
                PartitionKey = GeneratePartitionKey(source.Address),
                RowKey = GenerateRowKey(source.TxHash)
            };
        }
    }

    public class BroadcastedTransactionRepository: IBroadcastedTransactionRepository
    {
        private readonly INoSQLTableStorage<BroadcastedTransactionEntity> _storage;

        public BroadcastedTransactionRepository(INoSQLTableStorage<BroadcastedTransactionEntity> storage)
        {
            _storage = storage;
        }


        public async Task<IEnumerable<IBroadcastedTransaction>> GetTransactionsForAddress(string address)
        {
            return await _storage.GetDataAsync(BroadcastedTransactionEntity.GeneratePartitionKey(address));
        }

        public Task InsertOrReplace(IBroadcastedTransaction transaction)
        {
            return _storage.InsertOrReplaceAsync(BroadcastedTransactionEntity.Create(transaction));
        }
    }
}
