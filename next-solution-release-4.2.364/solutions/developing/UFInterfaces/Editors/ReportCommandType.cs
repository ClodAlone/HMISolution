using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UFInterfaces.Converters;

namespace DocumentManager.ComponentService
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ReportCommandType
    {
        Show,
        Print,
        PrintDialog,
        Save,
        Send
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum AlarmReportType
    {
        OrderByDateTime,
        OrderByDuration,
        OrderByOccurrence
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ExportFileType
    {
        Csv,
        Html,
        Xls,
        Pdf
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ReportPeriodType
    {
        Today,
        Yesterday,
        LastWeek,
        LastMonth,
        LastYear,
        Custom
    }
}
