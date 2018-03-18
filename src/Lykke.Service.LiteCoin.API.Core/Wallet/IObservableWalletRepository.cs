using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Pagination;

namespace Lykke.Service.LiteCoin.API.Core.Wallet
{
    public interface IObservableWallet
    {
        string Address { get; }
    }

    public class ObservableWallet:IObservableWallet
    {
        public string Address { get; set; }

        public static ObservableWallet Create(string address)
        {
            return new ObservableWallet
            {
                Address = address
            };
        }
    }
    public interface IObservableWalletRepository
    {
        Task Insert(IObservableWallet wallet);
        Task<IEnumerable<IObservableWallet>> GetAll();
        Task<IPaginationResult<IObservableWallet>> GetPaged(int take, string continuation);
        Task Delete(string address);
        Task<IObservableWallet> Get(string address);
    }
}
