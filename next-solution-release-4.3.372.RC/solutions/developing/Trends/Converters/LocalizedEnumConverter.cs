using System;
using Utilities.Converters;

namespace Trends.Converters
{
    public class LocalizedPenKindsEnumConverter : ResourceEnumConverter
    {
        public LocalizedPenKindsEnumConverter() :
            this(typeof(PredefinedPenKinds))
        { }

        public LocalizedPenKindsEnumConverter(Type type) :
            base(type, Properties.Resources.ResourceManager)
        { }
    }
}
