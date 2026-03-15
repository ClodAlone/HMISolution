using System;
using Utilities.Converters;

namespace StatDef.Converters
{
    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter() :
            base(typeof(StatProps), Properties.Resources.ResourceManager)
        { }

        public LocalizedEnumConverter(Type type) : 
            base(type, Properties.Resources.ResourceManager)
        { }
    }
}
