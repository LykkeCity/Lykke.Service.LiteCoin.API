using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Assets;
using Lykke.Service.LiteCoin.API.Core.Asset;
using Lykke.Service.LiteCoin.API.Extensions;
using Lykke.Service.LiteCoin.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    public class AssetsController: Controller
    {
        private readonly IAssetRepository _assetRepository;

        public AssetsController(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        [SwaggerOperation(nameof(GetPaged))]
        [ProducesResponseType(typeof(PaginationResponse<AssetResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [HttpGet("api/assets")]
        public async Task<IActionResult> GetPaged([FromQuery] int take, [FromQuery] string continuation)
        {
            if (!ModelState.IsValid || 
                !ModelState.IsValidContinuationToken(continuation) || 
                !ModelState.IsValidTakeParameter(take))
            {
                return BadRequest(ModelState.ToErrorResponce());
            }

            var paginationResult = await _assetRepository.GetPaged(take, continuation);

            return Ok(PaginationResponse.From(paginationResult.Continuation, paginationResult.Items.Select(p => new AssetResponse
            {
                Address = p.Address,
                AssetId = p.AssetId,
                Accuracy = p.Accuracy,
                Name = p.Name
            }).ToArray()));
        }

        [SwaggerOperation(nameof(GetById))]
        [ProducesResponseType(typeof(AssetResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(AssetResponse), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [HttpGet("api/assets/{assetId}")]
        public async Task<IActionResult> GetById(string assetId)
        {
            var asset = await _assetRepository.GetById(assetId);
            if (asset == null)
            {
                return NoContent();
            }

            return Ok(new AssetResponse
            {
                Address = asset.Address,
                AssetId = asset.AssetId,
                Accuracy = asset.Accuracy,
                Name = asset.Name
            });
        }
    }
}
