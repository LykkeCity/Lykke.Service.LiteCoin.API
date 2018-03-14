using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.Fee;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Fee
{
    public class DynamicFeeRateEntity:TableEntity, IDynamicFeeRate
    {
        public int FeePerKb { get; set; }

        public static string GeneratePartitionKey()
        {
            return "FR";
        }

        public static string GenerateRowKey()
        {
            return "FR";
        }

        public static DynamicFeeRateEntity Create(IDynamicFeeRate source)
        {
            return new DynamicFeeRateEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(),
                FeePerKb = source.FeePerKb
            };
        }
    }

    public class DynamicFeeRateRepository:IDynamicFeeRateRepository
    {
        private readonly INoSQLTableStorage<DynamicFeeRateEntity> _storage;

        public DynamicFeeRateRepository(INoSQLTableStorage<DynamicFeeRateEntity> storage)
        {
            _storage = storage;
        }

        public Task InsertOrReplace(IDynamicFeeRate dynamicFeeRate)
        {
            return _storage.InsertOrReplaceAsync(DynamicFeeRateEntity.Create(dynamicFeeRate));
        }

        public async Task<IDynamicFeeRate> Get()
        {
            return await _storage.GetDataAsync(DynamicFeeRateEntity.GeneratePartitionKey(), DynamicFeeRateEntity.GenerateRowKey());
        }
    }
}
