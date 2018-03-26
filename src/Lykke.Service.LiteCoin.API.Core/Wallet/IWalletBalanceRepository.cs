﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Pagination;

namespace Lykke.Service.LiteCoin.API.Core.Wallet
{
    public interface IWalletBalance
    {
        string Address { get; }
        long BalanceSatoshi { get; }
        DateTime Updated { get; }

        int UpdatedAtBlockHeight { get; }
    }



    public class WalletBalance:IWalletBalance
    {
        public string Address { get; set; }
        public long BalanceSatoshi { get; set; }
        public DateTime Updated { get; set; }
        public int UpdatedAtBlockHeight { get; set; }
        public static WalletBalance Create(string address, long balanceSatoshi, int updatedAtBlock, DateTime? updated = null)
        {
            return new WalletBalance
            {
                Address = address,
                BalanceSatoshi = balanceSatoshi,
                Updated = updated ?? DateTime.UtcNow,
                UpdatedAtBlockHeight = updatedAtBlock
            };
        }
    }

    public interface IWalletBalanceRepository
    {
        Task InsertOrReplace(IWalletBalance balance);

        Task DeleteIfExist(string address);
        Task<IPaginationResult<IWalletBalance>> GetBalances(int take, string continuation);
    }
}
