using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Log;
using Flurl;
using Flurl.Http;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Services.BlockChainProviders.Helpers;
using Lykke.Service.LiteCoin.API.Services.BlockChainProviders.InsightApi.Contracts;
using Lykke.Service.LiteCoin.API.Services.Helpers;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.BlockChainProviders.InsightApi
{
    internal class InsightApiBlockChainProvider: IBlockChainProvider
    {
        private readonly InsightApiSettings _insightApiSettings;
        private readonly ILog _log;

        public InsightApiBlockChainProvider(InsightApiSettings insightApiSettings, ILog log)
        {
            _insightApiSettings = insightApiSettings;
            _log = log;
        }

        public async Task<IEnumerable<string>> GetTransactionsForAddress(string address)
        {
            var result = new List<string>();

            const int batchSize = 1000;
            
            var allTxLoaded = false;
            int counter = 0;

            while (!allTxLoaded)
            {
                var url = _insightApiSettings.Url
                    .AppendPathSegment($"insight-lite-api/addr/{address}")
                    .SetQueryParam("from", counter)
                    .SetQueryParam("to", counter + batchSize);

                var resp = await GetJson<AddressBalanceResponceContract>(url);

                result.AddRange(resp.Transactions);
                allTxLoaded = !resp.Transactions.Any();

                counter += batchSize;
            }

            return result;
        }

        public Task<IEnumerable<string>> GetTransactionsForAddress(BitcoinAddress address)
        {
            return GetTransactionsForAddress(address.ToString());
        }

        public async Task<int> GetLastBlockHeight()
        {
            var url = _insightApiSettings.Url
                .AppendPathSegment("insight-lite-api/status");


            var resp = await GetJson<StatusResponceContract>(url);

            return resp.Info.LastBlockHeight;
        }

        public async Task<Transaction> GetRawTx(string tx)
        {
            var url = _insightApiSettings.Url
                .AppendPathSegment($"insight-lite-api/rawtx/{tx}");

            var resp = await GetJson<RawTxResponce>(url);
            
            return Transaction.Parse(resp.RawTx);
        }

        public async Task BroadCastTransaction(Transaction tx)
        {
            try
            {
                await _insightApiSettings.Url.AppendPathSegment("insight-lite-api/tx/send")
                    .PostJsonAsync(new BroadcastTransactionRequestContract
                    {
                        RawTx = tx.ToHex()
                    })
                    .ReceiveJson<BroadcastTransactionResponceContract>();
            }
            catch (FlurlHttpException e)
            {
                throw new BusinessException($"Error while proceeding operation within Blockchain Insight Api. Responce: {await e.GetResponseStringAsync()}", ErrorCode.BlockChainApiError, e);
            }

        }

        public async Task<int> GetTxConfirmationCount(string txHash)
        {
            return (await GetTx(txHash))?.Confirmation ?? 0;
        }

        public async Task<IEnumerable<Coin>> GetUnspentOutputs(string address, int minConfirmationCount)
        {
            var resp = await GetUnspentOutputsResponce(address);

            return resp.Where(p => p.Confirmation >= minConfirmationCount).Select(MapUnspentCoun);
        }

        public async Task<string> GetDestinationAddress(string txHash, uint n)
        {
            var tx = await GetTx(txHash);

            return tx?.Outputs?.FirstOrDefault(p => p.N == n)?.ScriptPubKey?.Addresses?.FirstOrDefault();
        }

        public async Task<long> GetBalanceSatoshiFromUnspentOutputs(string address, int minConfirmationCount)
        {
            var unspentOutputs = (await GetUnspentOutputsResponce(address)).Where(p => p.Confirmation >= minConfirmationCount);

            return unspentOutputs.Sum(p => p.Satoshi);
        }

        public async Task<IEnumerable<(string address, long balance)>> GetBalancesSatoshiFromUnspentOutputsBatched(IEnumerable<string> addresses, int minConfirmationCount)
        {
            var allUnspent = (await GetUnspentOutputsResponceBatched(addresses))
                .Where(p => p.Confirmation >= minConfirmationCount)
                .ToList();
            
            var result = new List<(string address, long balance)>();

            foreach (var address in addresses)
            {
                var unspentBalance = allUnspent.Where(p => p.Address == address).Sum(p => p.Satoshi);
                result.Add((address, unspentBalance));
            }

            return result;
        }

        public async Task<AggregatedInputsOutputs> GetAggregatedInputsOutputs(string txHash)
        {
            var tx = await GetTx(txHash);

            if (tx == null)
            {
                return null;
            }

            var inputs = tx.Inputs.Select(p => new
            {
                p.Address,
                p.AmountSatoshi
            }).GroupBy(p => p.Address).Select(p => new AggregatedInputsOutputs.InputOutput
            {
                Address = p.Key,
                AmountSatoshi = p.Sum(x => x.AmountSatoshi)
            }).ToList();

            var outputs = tx.Outputs.Select(p => new
            {
                Address = p.ScriptPubKey.Addresses?.FirstOrDefault(),
                AmountSatoshi = Money.FromUnit(p.ValueBtc, MoneyUnit.BTC).Satoshi
            }).GroupBy(p => p.Address).Select(p => new AggregatedInputsOutputs.InputOutput
            {
                Address = p.Key,
                AmountSatoshi = p.Sum(x => x.AmountSatoshi)
            }).ToList();

            //tx change calculation
            foreach(var output in outputs)
            {
                var sourceInputForChange = inputs.FirstOrDefault(p => p.Address == output.Address);
                if(sourceInputForChange != null)
                {
                    sourceInputForChange.AmountSatoshi -= output.AmountSatoshi;
                    output.AmountSatoshi = 0;
                }
            }

            return new AggregatedInputsOutputs
            {
                Inputs = inputs.Where(p => p.AmountSatoshi > 0),
                Outputs = outputs.Where(p => p.AmountSatoshi > 0),
                TxHash = txHash,
                TimeStamp = GetTimeStampFromBlockTime(tx.BlockTime)
            };
        }

        private DateTime GetTimeStampFromBlockTime(long blockTime)
        {
            var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);

            return posixTime.AddMilliseconds(blockTime * 1000);
        }

        private async Task<IEnumerable<AddressUnspentOutputsResponce>> GetUnspentOutputsResponceBatched(IEnumerable<string> addresses)
        {
            var url = _insightApiSettings.Url
                .AppendPathSegment($"insight-lite-api/addrs/{string.Join(",", addresses)}/utxo");

            return await GetJson<AddressUnspentOutputsResponce[]>(url);
        }

        private async Task<IEnumerable<AddressUnspentOutputsResponce>> GetUnspentOutputsResponce(string address)
        {
            var url = _insightApiSettings.Url
                .AppendPathSegment($"insight-lite-api/addr/{address}/utxo");

            return await GetJson<AddressUnspentOutputsResponce[]>(url);
        }

        private Coin MapUnspentCoun(AddressUnspentOutputsResponce source)
        {
            return new Coin(new OutPoint(uint256.Parse(source.TxHash), source.N), new TxOut(new Money(source.Satoshi, MoneyUnit.Satoshi), source.ScriptPubKey.ToScript()));
        }

        private async Task<TxResponceContract> GetTx(string txHash)
        {
            try
            {
                var url = _insightApiSettings.Url
                    .AppendPathSegment($"insight-lite-api/tx/{txHash}");

                var resp = await GetJson<TxResponceContract>(url);
                
                return resp;
            }
            catch (FlurlHttpException e) when (e.Call.Response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        private async Task<T> GetJson<T>(Url url, int tryCount = 3)
        {
            bool NeedToRetryException(Exception ex)
            {

                if (!(ex is FlurlHttpException flurlException))
                {
                    return false;
                }

                var isTimeout = flurlException is FlurlHttpTimeoutException;
                if (isTimeout)
                {
                    return true;
                }

                if (flurlException.Call.HttpStatus == HttpStatusCode.ServiceUnavailable ||
                    flurlException.Call.HttpStatus == HttpStatusCode.InternalServerError)
                {
                    return true;
                }

                return false;
            }


            try
            {

                return await Retry.Try(() => url.GetJsonAsync<T>(), NeedToRetryException, tryCount, _log);
            }
            catch (FlurlHttpException e)
            {
                throw new BusinessException("Error while proceeding operation within Blockchain Insight Api", ErrorCode.BlockChainApiError, e);
            }
        }
    }
}
