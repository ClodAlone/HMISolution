#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Syncfusion.HtmlConverter
{
    [CLSCompliantAttribute(false)]
    [StructLayout(LayoutKind.Sequential)]
    internal struct FILETIME
    {
        public UInt32 dwLowDateTime;
        public UInt32 dwHighDateTime;
    }
    [CLSCompliantAttribute(false)]
    [StructLayout(LayoutKind.Sequential)]
    internal struct SYSTEMTIME
    {
        public UInt16 Year;
        public UInt16 Month;
        public UInt16 DayOfWeek;
        public UInt16 Day;
        public UInt16 Hour;
        public UInt16 Minute;
        public UInt16 Second;
        public UInt16 Milliseconds;
    }
    [CLSCompliantAttribute(false)]
    [StructLayout(LayoutKind.Sequential)]
    internal struct SECURITY_ATTRIBUTES
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint nLength;
        public IntPtr lpSecurityDescriptor;
        [MarshalAs(UnmanagedType.Bool)]
        public bool bInheritHandle;
    }
    [CLSCompliantAttribute(false)]
    [StructLayout(LayoutKind.Sequential)]
    internal struct BINDINFO
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint cbSize;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string szExtraInfo;
        [MarshalAs(UnmanagedType.Struct)]
        public STGMEDIUM stgmedData;
        [MarshalAs(UnmanagedType.U4)]
        public uint grfBindInfoF;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwBindVerb;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string szCustomVerb;
        [MarshalAs(UnmanagedType.U4)]
        public uint cbstgmedData;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwOptions;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwOptionsFlags;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwCodePage;
        [MarshalAs(UnmanagedType.Struct)]
        public SECURITY_ATTRIBUTES securityAttributes;
        public Guid iid;
        [MarshalAs(UnmanagedType.IUnknown)]
        public object punk;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwReserved;
    }
    [CLSCompliantAttribute(false)]
    [StructLayout(LayoutKind.Sequential)]
    internal struct INTERNET_CACHE_ENTRY_INFO
    {
        public UInt32 dwStructSize;
        public string lpszSourceUrlName;
        public string lpszLocalFileName;
        public UInt32 CacheEntryType;
        public UInt32 dwUseCount;
        public UInt32 dwHitRate;
        public UInt32 dwSizeLow;
        public UInt32 dwSizeHigh;
        public FILETIME LastModifiedTime;
        public FILETIME ExpireTime;
        public FILETIME LastAccessTime;
        public FILETIME LastSyncTime;
        public IntPtr lpHeaderInfo;
        public UInt32 dwHeaderInfoSize;
        public string lpszFileExtension;
        public UInt32 dwExemptDelta;
    }
    [CLSCompliantAttribute(false)]
    [ComVisible(true), StructLayout(LayoutKind.Sequential)]
    internal struct tagOLECMD
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint cmdID;
        [MarshalAs(UnmanagedType.U4)]
        public uint cmdf;
    }

    [ComVisible(true), StructLayout(LayoutKind.Sequential)]
    internal struct tagPOINT
    {
        [MarshalAs(UnmanagedType.I4)]
        public int X;
        [MarshalAs(UnmanagedType.I4)]
        public int Y;
    }

    [ComVisible(true), StructLayout(LayoutKind.Sequential)]
    internal struct tagRECT
    {
        [MarshalAs(UnmanagedType.I4)]
        public int Left;
        [MarshalAs(UnmanagedType.I4)]
        public int Top;
        [MarshalAs(UnmanagedType.I4)]
        public int Right;
        [MarshalAs(UnmanagedType.I4)]
        public int Bottom;

        public tagRECT(int left_, int top_, int right_, int bottom_)
        {
            Left = left_;
            Top = top_;
            Right = right_;
            Bottom = bottom_;
        }
    }

}
