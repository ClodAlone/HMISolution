using AlarmWindow.Converters;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlarmWindow.Enums
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum SeverityFilterCondition
    {
        [Display(Order = 0)]
        Equal,
        [Display(Order = 1)]
        MajorEqual,
        [Display(Order = 2)]
        MinorEqual
    }
}
