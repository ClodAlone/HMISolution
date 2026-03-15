using System;
using System.ComponentModel;
using UFInterfaces;
using Utilities.Converters;

namespace PropertyControl
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum PropertyFilterEnum
    {
        GroupByCategory,
        GroupByType,
        SortAlphabetically,
        SortByPriority
    }
    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter(Type type)
            : base(type, Properties.Resources.ResourceManager)
        {

        }
    }
}
