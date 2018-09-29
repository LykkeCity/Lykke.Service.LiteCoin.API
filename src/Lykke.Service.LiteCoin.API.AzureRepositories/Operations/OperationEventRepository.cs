using System;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.LiteCoin.API.Core.Operation;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Operations
{
    public class OperationEventTableEntity : TableEntity, IOperationEvent
    {
        OperationEventType IOperationEvent.Type => Enum.Parse<OperationEventType>(Type);

        public string Type { get; set; }

        public DateTime DateTime { get; set; }
        public Guid OperationId { get; set; }

        public string Context { get; set; }

        public static string GeneratePartitionKey(Guid operationId)
        {
            return operationId.ToString().CalculateHexHash32(3);
        }

        public static string GenerateRowKey(OperationEventType type, Guid operationId)
        {
            return $"{type.ToString()}_{operationId}";
        }

        public static OperationEventTableEntity Create(IOperationEvent source)
        {
            return new OperationEventTableEntity
            {
                DateTime = source.DateTime,
                OperationId = source.OperationId,
                PartitionKey = GeneratePartitionKey(source.OperationId),
                RowKey = GenerateRowKey(source.Type, source.OperationId),
                Context = source.Context
            };
        }
    }

    public class OperationEventRepository: IOperationEventRepository
    {
        private readonly INoSQLTableStorage<OperationEventTableEntity> _storage;

        public OperationEventRepository(INoSQLTableStorage<OperationEventTableEntity> storage)
        {
            _storage = storage;
        }

        public Task InsertIfNotExist(IOperationEvent operationEvent)
        {
            return _storage.TryInsertAsync(OperationEventTableEntity.Create(operationEvent));
        }

        public async Task<bool> Exist(Guid operationId, OperationEventType type)
        {
            return await _storage.GetDataAsync(OperationEventTableEntity.GeneratePartitionKey(operationId),
                       OperationEventTableEntity.GenerateRowKey(type, operationId)) != null;
        }
    }
}
