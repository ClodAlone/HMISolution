using HistoricalHelpers;
using WPFUtilities.HistoricalHelpers;

namespace WPFPenHelpers
{
    public class SCDataGeneratorSettings : DataGeneratorSettings
    {
        public int DataCount { get; set; }

        public int MDataCount { get; set; }

        public SettingsStorageBase Storage { get; set; }

        public override bool IsValid
        {
            get
            {
                return base.IsValid && DataCount > 0;
            }
        }
    }
}
