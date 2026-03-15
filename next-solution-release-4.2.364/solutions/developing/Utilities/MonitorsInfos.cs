using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class MonitorsInfos
    {
#if NET48
        const int ENUM_CURRENT_SETTINGS = -1;

        [DllImport("user32.dll")]
        internal static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DISPLAY_SETTINGS lpDevMode);


        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAY_SETTINGS
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public System.Windows.Forms.ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        public DISPLAY_SETTINGS? GetDisplaySettings(int screenNumber)
        {
            for (int i = 0; i < System.Windows.Forms.Screen.AllScreens.Count(); i++)
            {
                if (i == screenNumber)
                {
                    var dm = new DISPLAY_SETTINGS();
                    dm.dmSize = (short)Marshal.SizeOf(typeof(DISPLAY_SETTINGS));
                    EnumDisplaySettings(System.Windows.Forms.Screen.AllScreens[i].DeviceName, ENUM_CURRENT_SETTINGS, ref dm);
                    return dm;
                }
            }
            return null;
        }
#endif
    }
}
