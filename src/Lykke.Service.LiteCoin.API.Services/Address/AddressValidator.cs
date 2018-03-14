using Lykke.Service.LiteCoin.API.Core.Address;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Address
{
    public class AddressValidator: IAddressValidator
    {
        private readonly Network _network;

        public AddressValidator(Network network)
        {
            _network = network;
        }

    
        public bool IsAddressValid(string address)
        {
            var addr = GetBitcoinAddress(address);

            return addr != null;
        }

        public BitcoinAddress GetBitcoinAddress(string address)
        {
            try
            {
                
                return BitcoinAddress.Create(address, _network);
            }
            catch
            {
                return null;
            }
        }

        public bool IsPubkeyValid(string pubkey)
        {
            return GetPubkey(pubkey) != null;
        }

        public PubKey GetPubkey(string pubkey)
        {
            try
            {
                return new PubKey(pubkey);
            }
            catch
            {
                return null;
            }        
        }
    }
}
