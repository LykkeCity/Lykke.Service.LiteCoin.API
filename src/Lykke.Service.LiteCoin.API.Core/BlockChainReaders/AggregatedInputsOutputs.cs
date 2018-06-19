using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LiteCoin.API.Core.BlockChainReaders
{
    public class AggregatedInputsOutputs
    {
        public IEnumerable<InputOutput> Inputs { get; set; }

        public IEnumerable<InputOutput> Outputs { get; set; }

        public string TxHash { get; set; }

        public DateTime TimeStamp { get; set; }

        public class InputOutput
        {
            public string Address { get; set; }

            public long AmountSatoshi { get; set; }
        }
    }
}
