using AlarmWindow.Enums;
using System;
using Utilities.Converters;

namespace AlarmWindow.Converters
{
    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter() :
            base(typeof(SeverityFilterCondition), Properties.Resources.ResourceManager)
        { }

        public LocalizedEnumConverter(Type type) :
            base(type, Properties.Resources.ResourceManager)
        { }
    }
}
