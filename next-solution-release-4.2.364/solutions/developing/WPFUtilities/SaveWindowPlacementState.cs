using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Runtime.InteropServices;


namespace Utilities
{
    // RECT structure required by WINDOWPLACEMENT structure
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }

    // POINT structure required by WINDOWPLACEMENT structure
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    // WINDOWPLACEMENT stores the position, size, and state of a window
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public POINT minPosition;
        public POINT maxPosition;
        public RECT normalPosition;
    }

    public class SaveWindowPlacementState
    {
        #region Win32 API declarations to set and get window placement
        [DllImport("user32.dll")]
        static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        const int SW_HIDE = 0;
        const int SW_SHOWNORMAL = 1;
        const int SW_SHOWMINIMIZED = 2;
        #endregion

        public static void Load(Window wnd, WINDOWPLACEMENT wp)
        {
            try
            {
                // Load window placement details for previous application session from application settings
                // Note - if window was closed on a monitor that is now disconnected from the computer,
                //        SetWindowPlacement will place the window onto a visible monitor.
                wp.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                // wp.flags = 0;
                int nShow = wp.showCmd;
                wp.showCmd = SW_HIDE;
                IntPtr hwnd = new WindowInteropHelper(wnd).Handle;
                SetWindowPlacement(hwnd, ref wp);
                wp.showCmd = (nShow == SW_SHOWMINIMIZED ? SW_SHOWNORMAL : nShow);
                SetWindowPlacement(hwnd, ref wp);
            }
            catch 
            { 
            }
        }

        public static WINDOWPLACEMENT Save(Window wnd)
        {
            // Persist window placement details to application settings
            WINDOWPLACEMENT wp = new WINDOWPLACEMENT();
            IntPtr hwnd = new WindowInteropHelper(wnd).Handle;
            GetWindowPlacement(hwnd, out wp);

            return wp;
        }
    }
}
