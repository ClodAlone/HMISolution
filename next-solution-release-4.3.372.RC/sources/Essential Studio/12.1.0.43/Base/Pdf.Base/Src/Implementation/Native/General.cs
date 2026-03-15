#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;
using System.Runtime.InteropServices;
using Syncfusion.Pdf.Graphics;

namespace Syncfusion.Pdf.Native
{
    /// <summary>
    /// WinAPi functions.
    /// </summary>
    internal sealed class KernelApi
    {
#region Constructors
        /// <summary>
        /// To prevent construction of a class, we make a private constructor.
        /// </summary>
        private KernelApi()
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// The GetLastError function retrieves the calling thread's last-error code value.
        /// </summary>
        /// <returns>The return value is the calling thread's last-error code value.</returns>
        [DllImport("kernel32.dll")]
        internal static extern uint GetLastError();

        /// <summary>
        /// Retrieves character-type information for the characters in the specified source string.
        /// </summary>
        /// <param name="Locale">Value that specifies the locale identifier.</param>
        /// <param name="dwInfoType">Value that specifies the type of character information the user wants to retrieve.</param>
        /// <param name="lpSrcStr">Pointer to the string for which character types are requested.</param>
        /// <param name="cchSrc">Size, in characters, of the string pointed to by the lpSrcStr parameter.</param>
        /// <param name="lpCharType">Pointer to an array of 16-bit values.</param>
        /// <returns>Boolean result, indicates success of WinAPI call</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool GetStringTypeExW(
            uint Locale, StringInfoType dwInfoType, string lpSrcStr, int cchSrc, [Out] ushort[] lpCharType);
        [DllImport("kernel32.dll", EntryPoint = "FileTimeToSystemTime", CharSet = CharSet.Ansi)]
        internal static extern bool FileTimeToSystemTime(IntPtr lpFileTime, ref SYSTEMTIME lpSystemTime);
        [DllImport("kernel32.dll", EntryPoint = "FormatMessage", CharSet = CharSet.Ansi)]
        internal static extern uint FormatMessage(FormatMessageFlags dwFlags, IntPtr lpSource,
            uint messageId, uint dwLanguageId, IntPtr lpBuffer, uint nSize, IntPtr Arguments);
    }


    /// <summary>
    /// Class containing API for RTL support.
    /// </summary>
    internal class RtlApi
    {
#region Constants
        /// <summary>
        /// Operation succeed.
        /// </summary>
        public const int S_OK = 0;
        /// <summary>
        /// Out of memory to suceed an operation.
        /// </summary>
        public const uint E_OUTOFMEMORY = 0x8007000E;
        /// <summary>
        /// Default size of the buffer.
        /// </summary>
        public const int DefaultBuffSize = 16;
        /// <summary>
        /// Font doesn't support such glyphs.
        /// </summary>
        public const uint USP_E_SCRIPT_NOT_IN_FONT = 0x80040200;
        /// <summary>
        /// Default script program.
        /// </summary>
        public const uint ScriptUndefined = 0;
        /// <summary>
        /// Mask for setting script as SCRIPT_UNDEFINED.
        /// </summary>
        public const ushort ScriptUndefinedMask = 0xFC00;
        /// <summary>
        /// Identifies that layout is RTL.
        /// </summary>
        public const ushort RtlLayout = 0x0001;
        #endregion

#region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        private RtlApi()
        {
        }
        #endregion

#region Structures
        [StructLayout(LayoutKind.Sequential)]
        internal struct SCRIPT_STATE
        {
            /*
            WORD uBidiLevel :5; 
            WORD fOverrideDirection :1; 
            WORD fInhibitSymSwap :1; 
            WORD fCharShape :1; 
            WORD fDigitSubstitute :1; 
            WORD fInhibitLigate :1; 
            WORD fDisplayZWG :1; 
            WORD fArabicNumContext :1; 
            WORD fGcpClusters :1; 
            WORD fReserved :1; 
            WORD fEngineReserved :2;
            */
            public UInt16 val;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SCRIPT_ITEM
        {
            public int iCharPos;
            public SCRIPT_ANALYSIS a;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SCRIPT_ANALYSIS
        {
            /*
            WORD eScript      :10; 
            WORD fRTL          :1; 
            WORD fLayoutRTL    :1; 
            WORD fLinkBefore   :1; 
            WORD fLinkAfter    :1; 
            WORD fLogicalOrder :1; 
            WORD fNoGlyphIndex :1;
            */
            public UInt16 val;
            public SCRIPT_STATE s;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SCRIPT_CONTROL
        {
            /*
            DWORD uDefaultLanguage :16; 
            DWORD fContextDigits :1; 
            DWORD fInvertPreBoundDir :1; 
            DWORD fInvertPostBoundDir :1; 
            DWORD fLinkStringBefore :1; 
            DWORD fLinkStringAfter :1; 
            DWORD fNeutralOverride :1; 
            DWORD fNumericOverride :1; 
            DWORD fLegacyBidiClass :1; 
            DWORD fReserved :8; 
            */
            public int val;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SCRIPT_VISATTR
        {
            /*
            WORD uJustification :4; 
            WORD fClusterStart :1; 
            WORD fDiacritic :1; 
            WORD fZeroWidth :1; 
            WORD fReserved :1; 
            WORD fShapeReserved :8; 
            */
            public UInt16 val;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct GOFFSET
        {
            public int du;
            public int dv;
        }
        #endregion

#region Exported functions
        /*[DllImport( "usp10.dll", EntryPoint = "ScriptItemize", CharSet =
			 CharSet.Unicode,

			 ExactSpelling = true )]
		public static extern int ScriptItemize(
			string pwcInChars,

			int cInChars,
			int cMaxItems,
			ref SCRIPT_CONTROL psControls,
			ref SCRIPT_STATE psState,
			ref SCRIPT_ITEM pItems,
			out int pcItems
			);
        */

        [DllImport("Usp10.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        internal static extern uint ScriptItemize(
            string pwcInChars,
            int cInChars,
            int cMaxItems,
            ref SCRIPT_CONTROL psControl,
            ref SCRIPT_STATE psState,
            ref SCRIPT_ITEM pItems,
            ref int pcItems
            );


        [DllImport("Usp10.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        internal static extern uint ScriptItemize(
            string pwcInChars,
            int cInChars,
            int cMaxItems,
            ref SCRIPT_CONTROL psControl,
            ref SCRIPT_STATE psState,
            SCRIPT_ITEM pItems,
            ref int pcItems
            );

        [DllImport("Usp10.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        internal static extern uint ScriptShape(
            IntPtr hdc,
            ref IntPtr psc,
            string pwcChars,
            int cChars,
            int cMaxGlyphs,
            ref SCRIPT_ANALYSIS psa,
            ref UInt16 pwOutGlyphs,
            ref UInt16 pwLogClust,
            ref SCRIPT_VISATTR psva,
            ref int pcGlyphs
            );

        [DllImport("Usp10.dll")]
        internal static extern uint ScriptLayout(
            int cRuns,
            ref byte pbLevel,
            ref int piVisualToLogical,
            ref int piLogicalToVisual
            );

        [DllImport("Usp10.dll")]
        internal static extern uint ScriptPlace(
            IntPtr hdc,
            ref IntPtr psc,
            ref UInt16 pwGlyphs,
            int cGlyphs,
            ref SCRIPT_VISATTR psva,
            ref SCRIPT_ANALYSIS psa,
            ref int piAdvance,
            ref GOFFSET pGoffset,
            ref ABC pABC
            );

        #endregion

#region Implementation
        /// <summary>
        /// Retieves value from the structure.
        /// </summary>
        /// <param name="val">Value of the structure.</param>
        /// <param name="pos">Start position of the item inside of the structure.</param>
        /// <param name="len">Length of the item in bits.</param>
        /// <returns>Value from the structure.</returns>
        public static int Decrypt(int val, int pos, int len)
        {
            int result = 0;

            for (int index = pos, l = pos + len; index < l; index++)
            {
                int flag = (int)Math.Pow(2, index);
                int flag2 = (int)(val & flag);
                result |= flag2;
            }

            return result;
        }
        #endregion
    }

    /// <summary>
    /// Class containing API for RTF support.
    /// </summary>
    internal class RtfApi
    {
#region Constants
        public const int EM_FORMATRANGE = 1081;

        public const int WM_USER = 0x0400;
        public const int EM_GETCHARFORMAT = WM_USER + 58;
        public const int EM_SETCHARFORMAT = WM_USER + 68;

        public const int EM_SETEVENTMASK = 1073;
        public const int EM_GETPARAFORMAT = 1085;
        public const int EM_SETPARAFORMAT = 1095;
        public const int EM_SETTYPOGRAPHYOPTIONS = 1226;
        public const int WM_SETREDRAW = 11;
        public const int TO_ADVANCEDTYPOGRAPHY = 1;


        public const Int32 SCF_SELECTION = 0x0001;
        public const Int32 SCF_WORD = 0x0002;
        public const Int32 SCF_ALL = 0x0004;
        public const int LF_FACESIZE = 32;

        public const UInt32 CFM_BOLD = 0x00000001;
        public const UInt32 CFM_ITALIC = 0x00000002;
        public const UInt32 CFM_UNDERLINE = 0x00000004;
        public const UInt32 CFM_STRIKEOUT = 0x00000008;
        public const UInt32 CFM_PROTECTED = 0x00000010;
        public const UInt32 CFM_LINK = 0x00000020;
        public const UInt32 CFM_SIZE = 0x80000000;
        public const UInt32 CFM_COLOR = 0x40000000;
        public const UInt32 CFM_FACE = 0x20000000;
        public const UInt32 CFM_OFFSET = 0x10000000;
        public const UInt32 CFM_CHARSET = 0x08000000;
        public const UInt32 CFM_SUBSCRIPT = CFE_SUBSCRIPT | CFE_SUPERSCRIPT;
        public const UInt32 CFM_SUPERSCRIPT = CFM_SUBSCRIPT;

        public const UInt32 CFE_BOLD = 0x00000001;
        public const UInt32 CFE_ITALIC = 0x00000002;
        public const UInt32 CFE_UNDERLINE = 0x00000004;
        public const UInt32 CFE_STRIKEOUT = 0x00000008;
        public const UInt32 CFE_PROTECTED = 0x00000010;
        public const UInt32 CFE_LINK = 0x00000020;
        public const UInt32 CFE_AUTOCOLOR = 0x40000000;
        public const UInt32 CFE_SUBSCRIPT = 0x00010000;
        public const UInt32 CFE_SUPERSCRIPT = 0x00020000;

        public const byte CFU_UNDERLINENONE = 0x00;
        public const byte CFU_UNDERLINE = 0x01;
        public const byte CFU_UNDERLINEWORD = 0x02;
        public const byte CFU_UNDERLINEDOUBLE = 0x03;
        public const byte CFU_UNDERLINEDOTTED = 0x04;
        public const byte CFU_UNDERLINEDASH = 0x05;
        public const byte CFU_UNDERLINEDASHDOT = 0x06;
        public const byte CFU_UNDERLINEDASHDOTDOT = 0x07;
        public const byte CFU_UNDERLINEWAVE = 0x08;
        public const byte CFU_UNDERLINETHICK = 0x09;
        public const byte CFU_UNDERLINEHAIRLINE = 0x0A;

        public const int CFM_SMALLCAPS = 0x0040;
        public const int CFM_ALLCAPS = 0x0080;
        public const int CFM_HIDDEN = 0x0100;
        public const int CFM_OUTLINE = 0x0200;
        public const int CFM_SHADOW = 0x0400;
        public const int CFM_EMBOSS = 0x0800;
        public const int CFM_IMPRINT = 0x1000;
        public const int CFM_DISABLED = 0x2000;
        public const int CFM_REVISED = 0x4000;

        public const int CFM_BACKCOLOR = 0x04000000;
        public const int CFM_LCID = 0x02000000;
        public const int CFM_UNDERLINETYPE = 0x00800000;
        public const int CFM_WEIGHT = 0x00400000;
        public const int CFM_SPACING = 0x00200000;
        public const int CFM_KERNING = 0x00100000;
        public const int CFM_STYLE = 0x00080000;
        public const int CFM_ANIMATION = 0x00040000;
        public const int CFM_REVAUTHOR = 0x00008000;

        public const short FW_DONTCARE = 0;
        public const short FW_THIN = 100;
        public const short FW_EXTRALIGHT = 200;
        public const short FW_LIGHT = 300;
        public const short FW_NORMAL = 400;
        public const short FW_MEDIUM = 500;
        public const short FW_SEMIBOLD = 600;
        public const short FW_BOLD = 700;
        public const short FW_EXTRABOLD = 800;
        public const short FW_HEAVY = 900;

        public const short FW_ULTRALIGHT = FW_EXTRALIGHT;
        public const short FW_REGULAR = FW_NORMAL;
        public const short FW_DEMIBOLD = FW_SEMIBOLD;
        public const short FW_ULTRABOLD = FW_EXTRABOLD;
        public const short FW_BLACK = FW_HEAVY;

        public const UInt32 PFM_STARTINDENT = 0x00000001;
        public const UInt32 PFM_RIGHTINDENT = 0x00000002;
        public const UInt32 PFM_OFFSET = 0x00000004;
        public const UInt32 PFM_ALIGNMENT = 0x00000008;
        public const UInt32 PFM_TABSTOPS = 0x00000010;
        public const UInt32 PFM_NUMBERING = 0x00000020;
        public const UInt32 PFM_OFFSETINDENT = 0x80000000;

        public const UInt16 PFN_BULLET = 0x0001;

        public const UInt16 PFA_LEFT = 0x0001;
        public const UInt16 PFA_RIGHT = 0x0002;
        public const UInt16 PFA_CENTER = 0x0003;
        public const UInt16 PFA_JUSTIFY = 0x0004;

        #endregion

#region Exported Functions
        /// <summary>
        /// Exported funtion.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(HandleRef hWnd,
            int msg,
            int wParam,
            int lParam);

        /// <summary>
        /// Exported funtion.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(HandleRef hWnd,
            int msg,
            int wParam,
            ref PARAFORMAT lp);

        /// <summary>
        /// Exported funtion.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(HandleRef hWnd,
            int msg,
            int wParam,
            ref CHARFORMAT lp);
        #endregion

    }
}
#endif