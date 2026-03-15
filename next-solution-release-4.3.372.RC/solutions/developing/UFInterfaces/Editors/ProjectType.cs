using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UFInterfaces.Converters;

namespace DocumentManager.ComponentService
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ProjectType
    {
        Standard,
        WebHMI
    };
}
