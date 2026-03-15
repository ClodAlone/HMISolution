using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;

namespace Utilities
{
    public static class GlassHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [DllImport("dwmapi.dll")]
        static extern int
           DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        extern static int DwmIsCompositionEnabled(ref int en);

        /// <summary>
        /// Extends the glass area into the client area of the window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="top"></param>
        public static void ExtendGlass(Window window, Thickness thikness)
        {
            try
            {
                int isGlassEnabled = 0;
                DwmIsCompositionEnabled(ref isGlassEnabled);
                if (Environment.OSVersion.Version.Major > 5 && isGlassEnabled > 0)
                {
                    // Get the window handle
                    WindowInteropHelper helper = new WindowInteropHelper(window);
                    HwndSource mainWindowSrc = (HwndSource)HwndSource.
                        FromHwnd(helper.Handle);
                    mainWindowSrc.CompositionTarget.BackgroundColor =
                        Colors.Transparent;

                    // Get the dpi of the screen
                    System.Drawing.Graphics desktop =
                       System.Drawing.Graphics.FromHwnd(mainWindowSrc.Handle);
                    float dpiX = desktop.DpiX / 96;
                    float dpiY = desktop.DpiY / 96;

                    // Set Margins
                    MARGINS margins = new MARGINS();
                    margins.cxLeftWidth = (int)(thikness.Left * dpiX);
                    margins.cxRightWidth = (int)(thikness.Right * dpiX);
                    margins.cyBottomHeight = (int)(thikness.Bottom * dpiY);
                    margins.cyTopHeight = (int)(thikness.Top * dpiY);

                    window.Background = Brushes.Transparent;

                    int hr = DwmExtendFrameIntoClientArea(mainWindowSrc.Handle,
                                ref margins);
                }
                else
                {
                    window.Background = SystemColors.WindowBrush;
                }
            }
            catch (DllNotFoundException)
            {

            }
        }
    }
}
