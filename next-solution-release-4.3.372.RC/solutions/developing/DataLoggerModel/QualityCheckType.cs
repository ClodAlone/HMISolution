using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using  DataLoggerModel.Converters;

namespace DataLoggerModel
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum QualityCheckType
    {
        [Display(Order = 0)]
        Any,
        [Display(Order = 1)]
        AllGood,
        [Display(Order = 2)]
        AtLeastOneGood
    }
}
