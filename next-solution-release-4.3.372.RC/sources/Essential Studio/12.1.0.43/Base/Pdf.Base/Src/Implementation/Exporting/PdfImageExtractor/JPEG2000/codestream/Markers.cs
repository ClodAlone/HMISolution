#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
namespace Syncfusion.Pdf.JPEG2000.codestream
{
    public struct Markers
    {
        public const short SOC = unchecked((short)0xff4f);
        public const short SOT = unchecked((short)0xff90);
        public const short SOD = unchecked((short)0xff93);
        public const short EOC = unchecked((short)0xffd9);
        public const short SIZ = unchecked((short)0xff51);
        public const int RSIZ_BASELINE = 0x00;
        public const int RSIZ_ER_FLAG = 0x01;
        public const int RSIZ_ROI = 0x02;
        public const int SSIZ_DEPTH_BITS = 7;
        public const int MAX_COMP_BITDEPTH = 38;
        public const short COD = unchecked((short)0xff52);
        public const short COC = unchecked((short)0xff53);
        public const int SCOX_PRECINCT_PARTITION = 1;
        public const int SCOX_USE_SOP = 2;
        public const int SCOX_USE_EPH = 4;
        public const int SCOX_HOR_CB_PART = 8;
        public const int SCOX_VER_CB_PART = 16;
        public const int PRECINCT_PARTITION_DEF_SIZE = 0xffff;
        public const short RGN = unchecked((short)0xff5e);
        public const int SRGN_IMPLICIT = 0x00;
        public const short QCD = unchecked((short)0xff5c);
        public const short QCC = unchecked((short)0xff5d);
        public const int SQCX_GB_SHIFT = 5;
        public const int SQCX_GB_MSK = 7;
        public const int SQCX_NO_QUANTIZATION = 0x00;
        public const int SQCX_SCALAR_DERIVED = 0x01;
        public const int SQCX_SCALAR_EXPOUNDED = 0x02;
        public const int SQCX_EXP_SHIFT = 3;
        public const int SQCX_EXP_MASK = (1 << 5) - 1;
        public const int ERS_SOP = 1;
        public const int ERS_SEG_SYMBOLS = 2;
        public const short POC = unchecked((short)0xff5f);
        public const short TLM = unchecked((short)0xff55);
        public const short PLM = unchecked((short)0xff57);
        public const short PLT = unchecked((short)0xff58);
        public const short PPM = unchecked((short)0xff60);
        public const short PPT = unchecked((short)0xff61);
        public const int MAX_LPPT = 65535;
        public const int MAX_LPPM = 65535;
        public const short SOP = unchecked((short)0xff91);
        public const short SOP_LENGTH = 6;
        public const short EPH = unchecked((short)0xff92);
        public const short EPH_LENGTH = 2;
        public const short CRG = unchecked((short)0xff63);
        public const short COM = unchecked((short)0xff64);
        public const short RCOM_GEN_USE = unchecked((short)0x0001);
    }
}