using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.Fee
{

    public interface IFeeRateFacade
    {
        Task<int> GetFeePerKiloByte();
        Task UpdateFeeRate();
    }
}
