using System;
using Utilities.Converters;
namespace DataLoggerModel.Converters
{
    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter(Type type) :
            base(type, Properties.Resources.ResourceManager)
        { }
    }
}
