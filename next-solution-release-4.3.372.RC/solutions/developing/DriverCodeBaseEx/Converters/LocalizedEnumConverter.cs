using System;
using Utilities.Converters;

namespace DriverCodeBaseEx.Converters
{
    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter(Type type) : 
            base(type, Properties.Resources.ResourceManager)
        { }
    }
}
