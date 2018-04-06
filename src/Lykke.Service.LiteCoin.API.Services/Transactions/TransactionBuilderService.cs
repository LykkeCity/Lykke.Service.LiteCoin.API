using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.Fee;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs;
using Lykke.Service.LiteCoin.API.Core.Transactions;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Transactions
{
    public class BuildedTransaction: IBuildedTransaction
    {
        public Transaction TransactionData { get; set; }
        public Money Fee { get; set; }
        public Money Amount { get; set; }

        public static BuildedTransaction Create(Transaction transaction, Money fee, Money amount)
        {
            return new BuildedTransaction
            {
                Amount = amount,
                Fee = fee,
                TransactionData = transaction
            };
        }
    }

    public class TransactionBuilderService : ITransactionBuilderService
    {
        private readonly ITransactionOutputsService _transactionOutputsService;
        private readonly IFeeService _feeService;

        public TransactionBuilderService(ITransactionOutputsService transactionOutputsService,
            IFeeService feeService)
        {
            _transactionOutputsService = transactionOutputsService;
            _feeService = feeService;
        }

        public async Task<IBuildedTransaction> GetTransferTransaction(BitcoinAddress source,
            PubKey fromAddressPubkey,
            BitcoinAddress destination, 
            Money amount, 
            bool includeFee)
        {
            var builder = new TransactionBuilder();

            return await TransferOneDirection(builder, source, fromAddressPubkey, amount.Satoshi, destination, includeFee);
            
        }

        private async Task<IBuildedTransaction> TransferOneDirection(TransactionBuilder builder, 
            BitcoinAddress @from,
            PubKey fromAddressPubkey, 
            long amount,
            BitcoinAddress to,
            bool includeFee)
        {
            var coins = await _transactionOutputsService.GetUnspentOutputs(from.ToString());

            if (fromAddressPubkey != null)
            {
                var reedem = fromAddressPubkey.WitHash.ScriptPubKey;
                coins = coins.Select(p => p.ToScriptCoin(reedem));
            }

            return await SendWithChange(builder, 
                coins, 
                to, 
                new Money(amount),
                from, 
                includeFee);
        }

        public async Task<IBuildedTransaction> SendWithChange(TransactionBuilder builder, 
            IEnumerable<ICoin> coins, IDestination destination, Money amount, IDestination changeDestination, bool includeFee)
        {
            if (amount.Satoshi <= 0)
                throw new BusinessException("Amount can't be less or equal to zero", ErrorCode.BadInputParameter);
            
            builder.AddCoins(coins)
                .Send(destination, amount)
                .SetChange(changeDestination);
            
            var calculatedFee = await _feeService.CalcFeeForTransaction(builder);

            if (includeFee)
            {
                builder.SubtractFees();
                amount = amount - calculatedFee;
            }

            builder.SendFees(calculatedFee);

            var tx = builder.BuildTransaction(false);

            return BuildedTransaction.Create(tx, calculatedFee, amount);
        }
    }

}
