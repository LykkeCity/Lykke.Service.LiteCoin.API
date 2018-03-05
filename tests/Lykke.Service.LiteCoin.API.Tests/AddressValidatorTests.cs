using Lykke.Service.LiteCoin.API.Services.Address;
using NBitcoin;
using Xunit;

namespace Lykke.Service.LiteCoin.API.Tests
{
    public class AddressValidatorTests
    {
        [Fact]
        public void PassTestNetLTCAddress()
        {
            PrepareNetworks.EnsureLiteCoinPrepared();


            var addresses = new[]
            {
                "mu5a17UQDh2hsRk9ZJzFkTfCbzZhMVBHY3",
                "mifUh8hTMomrQL1dyVykffhcsYAfExzdxa",
                "msiJHQf1BVXD6fuUyLn9D8mD6gMbPibiDV"
            };
            var addressValidator = new AddressValidator(NBitcoin.Litecoin.Networks.Testnet);

            foreach (var address in addresses)
            {
                Assert.True(addressValidator.IsValid(address));

            }


        }

        [Fact]
        public void PassMainNetLTCAddress()
        {
            PrepareNetworks.EnsureLiteCoinPrepared();


            var addresses = new[]
            {
                "13xWF5cWE1Byyvq8FiKUafTHTX4kx3MD7p",
                "1Q7Jmho4FixWBiTVcZ5aKXv4rTMMp6CjiD",
                "1LU4xEKf7SFnQ3GX8CDfDHvWkszaKrYRcD"
            };
            var addressValidator = new AddressValidator(NBitcoin.Litecoin.Networks.Mainnet);

            foreach (var address in addresses)
            {
                Assert.False(addressValidator.IsValid(address));

            }


        }

        [Fact]
        public void DoNotPassBTCAddress()
        {
            PrepareNetworks.EnsureLiteCoinPrepared();


            var addresses = new[]
            {
                "LLgJTbzZMsRTCUF1NtvvL9SR1a4pVieW89",
                "Le6rZj8bLTbUATVhcZBxd3Z1u8b542C63T"
            };
            var addressValidator = new AddressValidator(NBitcoin.Litecoin.Networks.Mainnet);

            foreach (var address in addresses)
            {
                Assert.True(addressValidator.IsValid(address));

            }


        }

        [Fact]
        public void CanDetectInvalidAddress()
        {
            PrepareNetworks.EnsureLiteCoinPrepared();

            var invalidAddress = "invalid";
            var addressValidator = new AddressValidator(NBitcoin.Litecoin.Networks.Testnet);

            Assert.False(addressValidator.IsValid(invalidAddress));
        }
    }

}
