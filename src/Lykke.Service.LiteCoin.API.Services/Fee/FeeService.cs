using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Fee;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Fee
{
    public class FeeService : IFeeService
    {
        private readonly IFeeRateFacade _feeRateFacade;

        public FeeService(IFeeRateFacade feeRateFacade)
        {
            _feeRateFacade = feeRateFacade;
        }

        public async Task<Money> CalcFeeForTransaction(Transaction tx)
        {
            var size = tx.ToBytes().Length;

            return (await GetFeeRate()).GetFee(size);
        }

        public async Task<Money> CalcFeeForTransaction(TransactionBuilder builder)
        {
            var feeRate = await GetFeeRate();

            return builder.EstimateFees(builder.BuildTransaction(false), feeRate);
        }

        public async Task<FeeRate> GetFeeRate()
        {
            var feePerKiloByte = await _feeRateFacade.GetFeePerKiloByte();

            return new FeeRate(new Money(feePerKiloByte, MoneyUnit.Satoshi));
        }
    }
}
