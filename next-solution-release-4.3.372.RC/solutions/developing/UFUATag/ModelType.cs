using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using UFUAModel.Converters;

namespace UFUAModel
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ModelType
    {
        [Display(Order = 0)]
        Variable,
        [Display(Order = 3)]
        Digital,
        [Display(Order = 4)]
        Enumerated,
        [Display(Order = 1)]
        Analog,
        [Display(Order = 5)]
        Method,
        [Display(Order = 2)]
        ObjectType
    }
}
