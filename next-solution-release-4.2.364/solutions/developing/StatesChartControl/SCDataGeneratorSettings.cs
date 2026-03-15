using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatesChartControl
{
    public class SCDataGeneratorSettings : WPFPenHelpers.DataGeneratorSettings
    {
        public int DataCount { get; set; }

        public int MDataCount { get; set; }

        public WPFPenHelpers.SettingsStorage Storage { get; set; }

        public override bool IsValid
        {
            get
            {
                return base.IsValid && DataCount > 0;
            }
        }
    }
}
