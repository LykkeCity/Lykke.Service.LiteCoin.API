using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Address
{
    public interface IAddressValidator
    {
        bool IsAddressValid(string address);
        BitcoinAddress GetBitcoinAddress(string address);

        bool IsPubkeyValid(string pubkey);
        PubKey GetPubkey(string pubkey);
    }
}
