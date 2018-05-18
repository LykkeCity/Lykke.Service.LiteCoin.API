using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.LiteCoin.API.Core.Address;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.Transactions;
using Lykke.Service.LiteCoin.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    public class HistoryController:Controller
    {
        private readonly IHistoryService _historyService;
        private readonly IAddressValidator _addressValidator;

        public HistoryController(IHistoryService historyService, IAddressValidator addressValidator)
        {
            _historyService = historyService;
            _addressValidator = addressValidator;
        }

        [HttpPost("/api/transactions/history/from/{address}/observation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult ObserveFrom(
            [FromRoute]string address)
        {
            return Ok();
        }

        [HttpPost("/api/transactions/history/to/{address}/observation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult ObserveTo(
            [FromRoute]string address)
        {
            return Ok();
        }

        [HttpDelete("/api/transactions/history/from/{address}/observation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult DeleteObservationFrom(
            [FromRoute]string address)
        {
            return Ok();
        }

        [HttpDelete("/api/transactions/history/to/{address}/observation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult DeleteObservationTo(
            [FromRoute]string address)
        {
            return Ok();
        }
        
        [HttpGet("/api/transactions/history/from/{address}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(HistoricalTransactionContract[]))]
        public async Task<IActionResult> GetHistoryFrom(
            [FromRoute]string address,
            [FromQuery]string afterHash,
            [FromQuery]int take)
        {
            if (!_addressValidator.IsAddressValid(address))
            {
                throw new BusinessException($"Invalid LTC address ${address}", ErrorCode.BadInputParameter);
            }

            var btcAddress = _addressValidator.GetBitcoinAddress(address);
            var result = await _historyService.GetHistoryFrom(btcAddress, afterHash, take);

            return Ok(result.Select(p => p.ToHistoricalTransaction()));
        }

        [HttpGet("/api/transactions/history/to/{address}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(HistoricalTransactionContract[]))]
        public async Task<IActionResult> GetHistoryTo(
            [FromRoute]string address,
            [FromQuery]string afterHash,
            [FromQuery]int take)
        {
            if (!_addressValidator.IsAddressValid(address))
            {
                throw new BusinessException($"Invalid LTC address ${address}", ErrorCode.BadInputParameter);
            }

            var btcAddress = _addressValidator.GetBitcoinAddress(address);
            var result = await _historyService.GetHistoryTo(btcAddress, afterHash, take);

            return Ok(result.Select(p => p.ToHistoricalTransaction()));
        }
    }
}
