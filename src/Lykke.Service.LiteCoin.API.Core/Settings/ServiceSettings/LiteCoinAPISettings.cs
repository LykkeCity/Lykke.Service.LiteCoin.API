using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings
{
    public class LiteCoinApiSettings
    {
        public DbSettings Db { get; set; }
        public string Network { get; set; }
        public string InsightAPIUrl { get; set; }

        [Optional]
        public int MinFeePerKyloByte { get; set; } = 200 * 1024;

        [Optional]
        public int MaxFeePerKyloByte { get; set; } = 500 * 1024;

        [Optional]
        public int DefaultFeePerKyloByte { get; set; } = 100 * 1024; // use value greater than default value 100 litoshi per b. Ref https://bitcoin.stackexchange.com/questions/53821/where-can-i-find-the-current-fee-level-for-ltc


        [Optional]
        public string DynamicFeeProviderUrl { get; set; } = "https://api.blockcypher.com/";

        [Optional]
        public int MinConfirmationsToDetectOperation { get; set; } = 3;
    }
}
