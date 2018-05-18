using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Balances;
using Lykke.Service.LiteCoin.API.Core.Address;
using Lykke.Service.LiteCoin.API.Core.Constants;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Lykke.Service.LiteCoin.API.Extensions;
using Lykke.Service.LiteCoin.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    public class BalancesController:Controller
    {
        private readonly IAddressValidator _addressValidator;
        private readonly IWalletBalanceService _balanceService;

        public BalancesController(IAddressValidator addressValidator, IWalletBalanceService balanceService)
        {
            _addressValidator = addressValidator;
            _balanceService = balanceService;
        }

        [HttpPost("api/balances/{address}/observation")]
        [SwaggerOperation(nameof(Subscribe))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<IActionResult> Subscribe(string address)
        {
            if (!_addressValidator.IsAddressValid(address))
            {
                return BadRequest(ErrorResponse.Create("Invalid address"));
            }

            try
            {
                await _balanceService.Subscribe(address);
            }
            catch (BusinessException e) when(e.Code == ErrorCode.EntityAlreadyExist)
            {

                return StatusCode(409);
            }

            return Ok();
        }

        [HttpDelete("api/balances/{address}/observation")]
        [SwaggerOperation(nameof(Unsubscribe))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 204)]
        public async Task<IActionResult> Unsubscribe(string address)
        {
            if (!_addressValidator.IsAddressValid(address))
            {
                return BadRequest(ErrorResponse.Create("Invalid address"));
            }

            try
            {
                await _balanceService.Unsubscribe(address);
            }
            catch (BusinessException e) when (e.Code == ErrorCode.EntityNotExist)
            {

                return StatusCode((int)HttpStatusCode.NoContent);
            }

            return Ok();
        }
        
        [HttpGet("api/balances/")]
        [SwaggerOperation(nameof(GetBalances))]
        [ProducesResponseType(typeof(PaginationResponse<WalletBalanceContract>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetBalances([FromQuery]int take, [FromQuery] string continuation)
        {
            if (take <= 0)
            {
                ModelState.AddModelError(nameof(take), "Must be greater than zero");
            }

            ModelState.ValidateContinuationToken(continuation);


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToErrorResponce());
            }

            var padedResult = await _balanceService.GetBalances(take, continuation);

            return Ok(PaginationResponse.From(padedResult.Continuation, padedResult.Items.Select(p => new WalletBalanceContract
            {
                Address = p.Address,
                Balance = MoneyConversionHelper.SatoshiToContract(p.BalanceSatoshi),
                AssetId = Constants.Assets.LiteCoin.AssetId,
                Block = p.UpdatedAtBlockHeight
            }).ToList().AsReadOnly()));
        }
    }
}
