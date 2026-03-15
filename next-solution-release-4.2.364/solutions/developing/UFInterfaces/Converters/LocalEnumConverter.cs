using System;
using Utilities.Converters;

namespace UFInterfaces.Converters
{
    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter(Type type)
            : base(type, Properties.Resources.ResourceManager)
        { }
    }
}
