using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WPFPenHelpers;

namespace WebNExTHMI.PlatformComponents
{
    public class ChartData
    {
        public List<MyDataValue> Values { get; set; } = new List<MyDataValue>();
        
        public ChartData()
        {

        }
    }

    public class XYChartData
    {
        public List<XYDataValue> Values { get; set; } = new List<XYDataValue>();

        public XYChartData()
        {

        }
    }
}
