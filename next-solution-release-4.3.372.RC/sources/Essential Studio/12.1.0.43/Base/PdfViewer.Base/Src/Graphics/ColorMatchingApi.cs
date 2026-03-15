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

namespace Syncfusion.PdfViewer.Base
{
    internal class ColorMatchingApi
    {
        [DllImport("mscms.dll", SetLastError = true, EntryPoint = "OpenColorProfileW", CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr OpenColorProfile(
            [MarshalAs(UnmanagedType.LPStruct)] ProfileFilename profile,
            uint desiredAccess,
            FileShare shareMode,
            CreateDisposition creationMode);

        [DllImport("mscms.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern bool CloseColorProfile(IntPtr hProfile);

        [DllImport("mscms.dll", SetLastError = true, EntryPoint = "GetStandardColorSpaceProfileW", CallingConvention = CallingConvention.Winapi)]
        public static extern bool GetStandardColorSpaceProfile(
            uint machineName,
            LogicalColorSpace profileID,
            [MarshalAs(UnmanagedType.LPTStr), In, Out] StringBuilder profileName,
            ref uint size);

        [DllImport("mscms.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr CreateMultiProfileTransform(
            [In] IntPtr[] profiles,
            uint nProfiles,
            [In] uint[] intents,
            uint nIntents,
            ColorTransformMode flags,
            uint indexPreferredCMM);

        [DllImport("mscms.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern bool DeleteColorTransform(IntPtr hTransform);

        [DllImport("mscms.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, PreserveSig = true, ExactSpelling = true)]
        public static extern bool TranslateColors(
           IntPtr hColorTransform,
           [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2), In] CMYKColor[] inputColors,
           uint nColors,
           ColorType ctInput,
           [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2), Out] RGBColor[] outputColors,
           ColorType ctOutput);
    }
}
