using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WPFPenHelpers;

namespace WebNExTHMI.PlatformComponents
{
    public class StatesChartData
    {
        public List<MyDataValue> Values { get; set; } = new List<MyDataValue>();
        public List<MyDataValue> SecondValues { get; set; } = new List<MyDataValue>();
        public long StartDateTimeTicks { get; set; }
        public long EndDateTimeTicks { get; set; }
        public long TimezoneDiff { get; set; }

        public StatesChartData()
        {

        }


    }
}
