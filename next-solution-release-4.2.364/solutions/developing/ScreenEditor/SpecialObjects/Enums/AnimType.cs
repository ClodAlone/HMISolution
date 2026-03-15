using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenManager.SpecialObjects
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum AnimType
    {
        None,
        Fade,
        SlideHorizontal,
        SlideVertical
    }
}
