using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFUtilities.PropertyDataTemplate
{
    [Flags]
    public enum BitMask : uint
    {
        None = 0,
        Default = 0x7FFFFFFF,
        Level1 = 0x1,
        Level2 = 0x2,
        Level3 = 0x4,
        Level4 = 0x8,
        Level5 = 0x10,
        Level6 = 0x20,
        Level7 = 0x40,
        Level8 = 0x80,
        Level9 = 0x100,
        Level10 = 0x200,
        Level11 = 0x400,
        Level12 = 0x800,
        Level13 = 0x1000,
        Level14 = 0x2000,
        Level15 = 0x4000,
        Level16 = 0x8000,
        Level17 = 0x10000,
        Level18 = 0x20000,
        Level19 = 0x40000,
        Level20 = 0x80000,
        Level21 = 0x100000,
        Level22 = 0x200000,
        Level23 = 0x400000,
        Level24 = 0x800000,
        Level25 = 0x1000000,
        Level26 = 0x2000000,
        Level27 = 0x4000000,
        Level28 = 0x8000000,
        Level29 = 0x10000000,
        Level30 = 0x20000000,
        Level31 = 0x40000000
    }
}
