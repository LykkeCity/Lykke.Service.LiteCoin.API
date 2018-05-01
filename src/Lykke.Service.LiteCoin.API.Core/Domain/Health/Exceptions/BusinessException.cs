using System;

namespace Lykke.Service.LiteCoin.API.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public ErrorCode Code { get; private set; }
        public string Text { get; private set; }

        public BusinessException(string text, ErrorCode code, Exception innerEx = null)
            : base(text, innerEx)
        {
            Code = code;
            Text = text;
        }
    }

    public enum ErrorCode
    {
        Exception = 0,
        CantFindAddressToSignTx = 1,
        TransactionConcurrentInputsProblem = 2,
        BadInputParameter = 3,
        NotEnoughFundsAvailable = 4,
        OperationNotFound = 5,
        WalletNotFound = 7,
        SignError = 9,
        BalanceIsLessThanFee = 10,
        EntityAlreadyExist = 11,
        EntityNotExist = 12,
        TransactionAlreadyBroadcasted = 13,
        BlockChainApiError = 14,
        NotEnoughFunds = 15,
        FailedToUpdateBalance = 16

    }
}
