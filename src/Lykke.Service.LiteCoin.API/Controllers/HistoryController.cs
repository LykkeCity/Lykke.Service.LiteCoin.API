using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    public class HistoryController:Controller
    {
        [HttpPost("history/from/{address}/observation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult ObserveFrom(
            [FromRoute]string address)
        {
            return Ok();
        }

        [HttpPost("history/from/{address}/observation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult ObserveTo(
            [FromRoute]string address)
        {
            return Ok();
        }

        [HttpDelete("history/from/{address}/observation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult DeleteObservationFrom(
            [FromRoute]string address)
        {
            return Ok();
        }

        [HttpDelete("history/to/{address}/observation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult DeleteObservationTo(
            [FromRoute]string address)
        {
            return Ok();
        }
    }
}
