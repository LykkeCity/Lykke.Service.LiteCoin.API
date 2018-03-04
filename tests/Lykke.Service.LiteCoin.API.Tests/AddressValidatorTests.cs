using Lykke.Service.LiteCoin.API.Services.Address;
using Xunit;

namespace Lykke.Service.LiteCoin.API.Tests
{
    public class AddressValidatorTests
    {
        [Fact]
        public void CanPassValidAddress()
        {
            PrepareNetworks.EnsureLiteCoinPrepared();


            var addresses = new[]
            {
                "mu5a17UQDh2hsRk9ZJzFkTfCbzZhMVBHY3",
                "mifUh8hTMomrQL1dyVykffhcsYAfExzdxa",
                "msiJHQf1BVXD6fuUyLn9D8mD6gMbPibiDV",
                "LLgJTbzZMsRTCUF1NtvvL9SR1a4pVieW89",
                "Le6rZj8bLTbUATVhcZBxd3Z1u8b542C63T"
            };
            var addressValidator = new AddressValidator();

            foreach (var address in addresses)
            {
                Assert.True(addressValidator.IsAddressValid(address));

            }


        }

        [Fact]
        public void CanDetectInvalidAddress()
        {
            PrepareNetworks.EnsureLiteCoinPrepared();

            var invalidAddress = "invalid";
            var addressValidator = new AddressValidator();

            Assert.False(addressValidator.IsAddressValid(invalidAddress));
        }
    }

}
