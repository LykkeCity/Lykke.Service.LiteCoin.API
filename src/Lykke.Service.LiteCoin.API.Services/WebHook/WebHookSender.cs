﻿using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Flurl.Http;
using Lykke.Service.LiteCoin.API.Core.WebHook;
using Lykke.Service.LiteCoin.API.Services.WebHook.Contracts;

namespace Lykke.Service.LiteCoin.API.Services.WebHook
{
    internal enum WebHookType
    {
        CashIn,
        CashOutStarted,
        CashOutCompleted
    }

    internal class WebHookEvent
    {
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
        public object RequestData { get; set; }

        public static WebHookEvent Ok(object requestData)
        {
            return new WebHookEvent
            {
                IsSuccess = true,
                RequestData = requestData
            };
        }

        public static WebHookEvent Fail(object requestData, string error)
        {
            return new WebHookEvent
            {
                Error = error,
                IsSuccess = false,
                RequestData = requestData
            };
        }
    }

    public class WebHookSender: IWebHookSender
    {
        private readonly WebHookSettings _settings;
        private readonly ILog _log;
        private readonly IFailedWebHookEventRepository _failedWebHookEventRepository;

        public WebHookSender(WebHookSettings settings, ILog log, IFailedWebHookEventRepository failedWebHookEventRepository)
        {
            _settings = settings;
            _log = log;
            _failedWebHookEventRepository = failedWebHookEventRepository;
        }

        public async Task ProcessCashIn(string operationId, DateTime dateTime, string walletId, string assetId, decimal amount,
            string sourceAddress)
        {
            var requestData = new WebHookCashInRequestContract
            {
                OperationId = operationId,
                Context = new CashInContextContract
                {
                    Address = sourceAddress,
                    Amount = amount,
                    AssetId = assetId
                },
                DateTime = dateTime,
                Type = WebHookType.CashIn.ToString(),
                WalletId = walletId

            };

            await ProcessWebHook(requestData, operationId);
        }

        public async Task ProcessCashOutStarted(string operationId, DateTime dateTime, string walletId, string assetId, decimal amount,
            string destAddress, string txHash)
        {
            var requestData = new WebHookCashOutStartedRequestContract
            {
                OperationId = operationId,
                Context = new CashOutStartedContextContract
                {
                    Address = destAddress,
                    Amount = amount,
                    AssetId = assetId,
                },
                DateTime = dateTime,
                Type = WebHookType.CashOutStarted.ToString(),
                WalletId = walletId
            };

            await ProcessWebHook(requestData, operationId);
        }

        public async Task ProcessCashOutCompleted(string operationId, DateTime dateTime, string walletId, string assetId, decimal amount,
            string destAddress, string txHash)
        {
            var requestData = new WebHookCashOutStartedRequestContract
            {
                OperationId = operationId,
                Context = new CashOutStartedContextContract
                {
                    Address = destAddress,
                    Amount = amount,
                    AssetId = assetId
                },
                DateTime = dateTime,
                Type = WebHookType.CashOutCompleted.ToString(),
                WalletId = walletId
            };

            await ProcessWebHook(requestData, operationId);
        }

        private async Task ProcessWebHook(object requestData, string operationId)
        {
            try
            {
                await _log.WriteInfoAsync(nameof(WebHookSender), nameof(ProcessWebHook), requestData.ToJson(),
                    $"Processing web hook for {operationId} started");

                await _settings.Url.PostJsonAsync(requestData);

                await _log.WriteInfoAsync(nameof(WebHookSender), nameof(ProcessWebHook), requestData.ToJson(),
                    $"Processing web hook for {operationId} done");

                //remove previous attempt data 
                await _failedWebHookEventRepository.DeleteIfExist(operationId);
            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync(nameof(WebHookSender), nameof(ProcessWebHook), operationId, e);
                
                var failedEvent =  WebHookEvent.Fail(requestData, e.ToString());
                await _failedWebHookEventRepository.Insert(failedEvent, operationId);

                throw;
            }
        }
    }
}