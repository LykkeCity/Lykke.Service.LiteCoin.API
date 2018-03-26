using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LiteCoin.API.Core.Fee
{
    public class FeeSettings
    {
        public int DefaultFeePerKyloByte { get; set; }

        public int MinFeePerKyloByte { get; set; }

        public int MaxFeePerKyloByte { get; set; }

        public string DynamicFeeProviderUrl { get; set; }
    }
}
