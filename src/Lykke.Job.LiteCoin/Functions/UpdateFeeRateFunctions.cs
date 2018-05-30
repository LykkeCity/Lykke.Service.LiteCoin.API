using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.Fee;

namespace Lykke.Job.LiteCoin.Functions
{
    public class UpdateFeeRateFunctions
    {
        private readonly IFeeRateFacade _feeRateFacade;

        public UpdateFeeRateFunctions(IFeeRateFacade feeRateFacade)
        {
            _feeRateFacade = feeRateFacade;
        }


        public Task UpdateDynamicFee()
        {
            return _feeRateFacade.UpdateFeeRate();
        }
    }
}
