using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using UFInterfaces.Converters;

namespace DocumentManager.ComponentService
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum StartType
    {
        MainScreen,
        TilePage,
        GeoPage,
        GalleryPage
    };
}