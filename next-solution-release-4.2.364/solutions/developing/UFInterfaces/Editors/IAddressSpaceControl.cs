using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Converters;

namespace UFInterfaces.Editors
{
    public interface IAddressSpaceControl
    {
        SelectionType SelectionType { get; set; }
        FilterType FilterType { get; set; }
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum SelectionType
    {
        OPCUAEntityReference,
        TagEntityReference
    };

    public enum FilterType
    {
        None,
        Historians
    };

    public enum TargetType
    {
        None,
        Historian,
        DataloggerColumn,
        EngineeringUnit,
        AlarmThreshold,
        View
    };


    public enum SelectionMode
    {
        SingleRow,
        MultipleRow
    };
}
