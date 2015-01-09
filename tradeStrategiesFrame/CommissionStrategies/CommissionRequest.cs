using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tradeStrategiesFrame.CommissionStrategies
{
    class CommissionRequest
    {
        public double value { get; set;}
        public int volume { get; set; }
        public bool intraday { get; set; }

        public CommissionRequest(double value, int volume, bool intraday)
        {
            this.value = value;
            this.volume = volume;
            this.intraday = intraday;
        }
    }
}
