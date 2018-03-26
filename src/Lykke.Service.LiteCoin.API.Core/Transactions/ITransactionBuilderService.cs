using System.Collections.Generic;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Transactions
{
    public interface IBuildedTransaction
    {
        Transaction TransactionData { get; }
        Money Fee { get; }
        Money Amount { get; }
        IEnumerable<ICoin> SpentCoins { get; }
    }
    public interface ITransactionBuilderService
    {
        Task<IBuildedTransaction> GetTransferTransaction(BitcoinAddress source, PubKey fromAddressPubkey, BitcoinAddress destination, Money amount, bool includeFee);

    }
}
