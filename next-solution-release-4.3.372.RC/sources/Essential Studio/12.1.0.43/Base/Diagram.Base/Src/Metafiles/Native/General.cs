#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Diagram
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

        #region Class Extern methods
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
        #endregion
    }
}
