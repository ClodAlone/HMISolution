using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeMethods {
  public static class SimulateClick {
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
    static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    public static void DoClick(System.Windows.Forms.WebBrowser browser, int x, int y) {
      IntPtr handle = browser.Handle;
      StringBuilder className = new StringBuilder(100);
      while (className.ToString() != "Internet Explorer_Server") {
        handle = GetWindow(handle, 5); // 5 == child
        GetClassName(handle, className, className.Capacity);
      }
      IntPtr lParam = (IntPtr)((y << 16) | x); // X and Y coordinates of the click
      IntPtr wParam = IntPtr.Zero; // change this if you want to simulate Ctrl-Click and such
      const uint downCode = 0x201; // these codes are for single left clicks
      const uint upCode = 0x202;
      SendMessage(handle, downCode, wParam, lParam); // mousedown
      SendMessage(handle, upCode, wParam, lParam); // mouseup
    }

  }


}