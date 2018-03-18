using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings
{
    public class LiteCoinApiSettings
    {
        public DbSettings Db { get; set; }
        public string Network { get; set; }
        public string InsightAPIUrl { get; set; }

        [Optional]
        public int FeePerByte { get; set; } = 100; // use value greater than default value 100 000 litoshi per kb = 100 litoshi per b. Ref https://bitcoin.stackexchange.com/questions/53821/where-can-i-find-the-current-fee-level-for-ltc

        [Optional]
        public long MinFeeValue { get; set; } = 100000;
        [Optional]
        public long MaxFeeValue { get; set; } = 10000000;
        
        [Optional]
        public int MinConfirmationsToDetectOperation { get; set; } = 3;

        [Optional]
        public int WalletBalanceBatchSizeOnUpdate { get; set; } = 100;
    }
}
