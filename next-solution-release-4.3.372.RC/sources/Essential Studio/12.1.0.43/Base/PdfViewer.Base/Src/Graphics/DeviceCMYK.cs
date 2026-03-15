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
using System.IO;

namespace Syncfusion.PdfViewer.Base
{
    internal class DeviceCMYK
    {
#if MVC
        public DeviceCMYK()
        {
        }
#else
        public const uint IntentPerceptual = 0;
        public const uint IntentRelativeColorimetric = 1;
        public const uint IntentSaturation = 2;
        public const uint IntentAbsoluteColorimetric = 3;
        public const uint ProfileFilenameType = 1;
        public const uint ProfileMembufferType = 2;
        public const uint ProfileRead = 1;
        public const uint ProfileReadWrite = 2;
        public const uint IndexDontCare = 0;

        CMYKColor[] cmykColors = new CMYKColor[3];
        RGBColor[] rgbColors = new RGBColor[3];

        bool success;
        IntPtr hSRGBProfile;
        IntPtr hIsoCoatedProfile;
        IntPtr transform;

        public DeviceCMYK()
        {
            StringBuilder profileName = new StringBuilder(256);
            uint size = (uint)profileName.Capacity * 2;
            success = ColorMatchingApi.GetStandardColorSpaceProfile(0, LogicalColorSpace.sRGB, profileName, ref size);
            ProfileFilename sRGBFilename = new ProfileFilename(profileName.ToString());
            hSRGBProfile = ColorMatchingApi.OpenColorProfile(sRGBFilename, ProfileRead, FileShare.Read, CreateDisposition.OpenExisting);
            InitializeTransform();
        }

        ~DeviceCMYK()
        {
            ColorMatchingApi.DeleteColorTransform(transform);
        }
        private void InitializeTransform()
        {
#if WPF
            byte[] ICCBytes = global::Syncfusion.PdfViewer.WPF.Properties.Resources.ISOcoated_v2_300_eci_;
#else
            byte[] ICCBytes = global::Syncfusion.Windows.PdfViewer.Properties.Resources.ISOcoated_v2_300_eci_;
#endif
            try
            {
                using (FileStream stream = new FileStream("ISOcoated_v2_300_eci.icc", FileMode.OpenOrCreate))
                {
                    stream.Write(ICCBytes, 0, ICCBytes.Length);
                }
            }
            catch
            {
            }
            ProfileFilename isoCoatedFilename = new ProfileFilename(@"ISOcoated_v2_300_eci.icc");

            hIsoCoatedProfile = ColorMatchingApi.OpenColorProfile(isoCoatedFilename, ProfileRead, FileShare.Read, CreateDisposition.OpenExisting);

            IntPtr[] profiles = new IntPtr[] { hIsoCoatedProfile, hSRGBProfile };
            FileInfo info = new FileInfo("ISOcoated_v2_300_eci.icc");
            info.Delete();
            uint[] intents = new uint[] { IntentPerceptual };
            transform = ColorMatchingApi.CreateMultiProfileTransform(profiles, 2, intents, 1, ColorTransformMode.BestMode, IndexDontCare);
            ColorMatchingApi.CloseColorProfile(hSRGBProfile);
            ColorMatchingApi.CloseColorProfile(hIsoCoatedProfile);
        }

        internal RGBColor[] ConvertAsRGB(CMYKColor cmykColor)
        {
            bool success;
            cmykColors[0] = cmykColor;
            rgbColors[0] = new RGBColor();
            success = ColorMatchingApi.TranslateColors(transform, cmykColors, 1, ColorType.CMYK, rgbColors, ColorType.RGB);
            if (success == false)
                return null;
            return rgbColors;
        }
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class ProfileFilename
    {
        /// <summary>
        /// 
        /// </summary>
        public uint type;
        /// <summary>
        /// 
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string profileData;
        /// <summary>
        /// 
        /// </summary>
        public uint dataSize;

        /// <summary>
        /// Initializes ProfileFilename class.
        /// </summary>
        /// <param name="filename">Name of the file.</param>
        public ProfileFilename(string filename)
        {
            type = 1;
            profileData = filename;
            dataSize = (uint)filename.Length * 2 + 2;
        }
    };

    /// <summary>
    /// Specifies FileShare enum.
    /// </summary>
    public enum FileShare : uint
    {
        /// <summary>
        /// Read is allowed.
        /// </summary>
        Read = 1,
        /// <summary>
        /// Both read and write are allowed.
        /// </summary>
        Write = 2,
        /// <summary>
        /// Delete is allowed.
        /// </summary>
        Delete = 4
    };

    /// <summary>
    /// Specifies CreateDisposition enum.
    /// </summary>
    public enum CreateDisposition : uint
    {
        /// <summary>
        /// 
        /// </summary>
        CreateNew = 1,
        /// <summary>
        /// 
        /// </summary>
        CreateAlways = 2,
        /// <summary>
        /// 
        /// </summary>
        OpenExisting = 3,
        /// <summary>
        /// 
        /// </summary>
        OpenAlways = 4,
        /// <summary>
        /// 
        /// </summary>
        TruncateExisting = 5
    };

    /// <summary>
    /// Specifies LogicalColorSpace.
    /// </summary>
    public enum LogicalColorSpace : uint
    {
        /// <summary>
        /// 
        /// </summary>
        CalibratedRGB = 0x00000000,
        /// <summary>
        /// 
        /// </summary>
        sRGB = 0x73524742,
        /// <summary>
        /// 
        /// </summary>
        WindowsColorSpace = 0x57696E20
    };

    /// <summary>
    /// Specifies ColorTransformMode
    /// </summary>
    public enum ColorTransformMode : uint
    {
        /// <summary>
        /// 
        /// </summary>
        ProofMode = 0x00000001,
        /// <summary>
        /// 
        /// </summary>
        NormalMode = 0x00000002,
        /// <summary>
        /// 
        /// </summary>
        BestMode = 0x00000003,
        /// <summary>
        /// 
        /// </summary>
        EnableGamutChecking = 0x00010000,
        /// <summary>
        /// 
        /// </summary>
        UseRelativeColorimetric = 0x00020000,
        /// <summary>
        /// 
        /// </summary>
        FastTranslate = 0x00040000,
        /// <summary>
        /// 
        /// </summary>
        PreserveBlack = 0x00100000,
        /// <summary>
        /// 
        /// </summary>
        WCSAlways = 0x00200000
    };

    enum ColorType : int
    {
        Gray = 1,
        RGB = 2,
        XYZ = 3,
        Yxy = 4,
        Lab = 5,
        _3_Channel = 6,
        CMYK = 7,
        _5_Channel = 8,
        _6_Channel = 9,
        _7_Channel = 10,
        _8_Channel = 11,
        Named = 12
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct RGBColor
    {
        public ushort red;
        public ushort green;
        public ushort blue;
        public ushort pad;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct CMYKColor
    {
        public ushort cyan;
        public ushort magenta;
        public ushort yellow;
        public ushort black;
    };
}
