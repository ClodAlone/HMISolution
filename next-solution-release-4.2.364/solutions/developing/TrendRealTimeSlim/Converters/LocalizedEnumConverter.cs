using System;
using Utilities.Converters;

namespace TrendRealTimeSlim.Converters
{
    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter() :
            this(typeof(PredefinedPenKinds))
        { }

        public LocalizedEnumConverter(Type type) : 
            base(type, Properties.Resources.ResourceManager)
        { }
    }
}
