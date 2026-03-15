using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UFInterfaces.Converters;

namespace DocumentManager.ComponentService
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ThemeType
    {
        None,
        Default,
        Blend,
        VS2010,
        VS2017Light,
        VS2017Dark,
        Office2010Black,
        Office2010Silver,
        Office2010Blue,
        Office2013,
        TouchlineDark
    };
}
