using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPenHelpers
{
    public class SCDataGeneratorSettings : DataGeneratorSettings
    {
        public int DataCount { get; set; }

        public int MDataCount { get; set; }

        public SettingsStorage Storage { get; set; }

        public override bool IsValid
        {
            get
            {
                return base.IsValid && DataCount > 0;
            }
        }
    }
}
