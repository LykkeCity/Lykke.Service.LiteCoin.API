using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.LiteCoin.API.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Lykke.Service.LiteCoin.API.Extensions
{
    public static class ModelStateExtensions
    {
        public static ErrorResponse ToErrorResponce(this ModelStateDictionary modelState)
        {
            var response = new ErrorResponse() {ModelErrors = new Dictionary<string, List<string>>()};

            foreach (var state in modelState)
            {
                var messages = state.Value.Errors
                    .Where(e => !string.IsNullOrWhiteSpace(e.ErrorMessage))
                    .Select(e => e.ErrorMessage)
                    .Concat(state.Value.Errors
                        .Where(e => string.IsNullOrWhiteSpace(e.ErrorMessage))
                        .Select(e => e.Exception.Message))
                    .ToList();

                response.ModelErrors.Add(state.Key, messages);
            }

            return response;
        }

        public static bool IsValidOperationId(this ModelStateDictionary self, Guid operationId)
        {
            if (operationId == Guid.Empty)
            {
                self.AddModelError(nameof(operationId), "Operation identifier must not be empty GUID");
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool ValidateContinuationToken(this ModelStateDictionary self, string continuation)
        {
            if (!string.IsNullOrEmpty(continuation))
            {
                try
                {
                    JsonConvert.DeserializeObject<TableContinuationToken>(CommonUtils.HexToString(continuation));
                }
                catch
                {
                    self.AddModelError(nameof(continuation), "Invalid continuation token");

                    return false;
                }
            }

            return true;
        }
    }
}
