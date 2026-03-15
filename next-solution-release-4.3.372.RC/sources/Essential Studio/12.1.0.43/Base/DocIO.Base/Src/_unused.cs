#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Runtime.InteropServices;
using System.Text;
using Syncfusion.CompoundFile.DocIO.Native;

namespace Syncfusion.DocIO.Utilities
{
    /// <summary>
    /// Specifies structure of the clipped data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Syncfusion.Documentation.DocumentationExclude()]
    public struct CLIPDATA
    {
        /// <summary>
        /// Size of the data.
        /// </summary>
        internal uint uintSize;

        /// <summary>
        /// Pointer to the data.
        /// </summary>
        internal IntPtr intPtrClipData;

        /// <summary>
        /// Format of the data.
        /// </summary>
        internal int intClipFmt;
    }

    /// <summary>
    /// Represents Clip data wrapper class.
    /// </summary>
    internal class ClipDataWrapper
    {
        private CLIPDATA m_clipStruct = new CLIPDATA();
        private byte[] m_clipData;

        /// <summary>
        /// Reads the specified <see cref="PROPVARIANT">PROPVARIANT</see> variable.
        /// </summary>
        /// <param name="propVar">The PROPVARIANT variable.</param>
        public void Read(PROPVARIANT propVar)
        {
            object obj = Marshal.PtrToStructure(propVar.intPtr, typeof(CLIPDATA));
            m_clipStruct = (CLIPDATA)obj;
            m_clipData = new byte[m_clipStruct.uintSize];
            Marshal.Copy(m_clipStruct.intPtrClipData, m_clipData, 0, m_clipData.Length);
        }

        /// <summary>
        /// Reads the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Read(string data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("data");

            string[] parts = data.Split(' ');
            if (parts.Length < 2)
                throw new ArgumentException("data");

            byte[] fmtArr = Convert.FromBase64String(parts[0]);
            m_clipStruct.intClipFmt = Convert.ToInt32(fmtArr);
            m_clipData = Convert.FromBase64String(parts[1]);
            m_clipStruct.uintSize = (uint)m_clipData.Length;
        }

        /// <summary>
        /// Writes the specified <see cref="PROPVARIANT">PROPVARIANT</see> variable.
        /// </summary>
        /// <param name="propVar">The PROPVARIANT variable.</param>
        public void Write(PROPVARIANT propVar)
        {
            m_clipStruct.intPtrClipData = Marshal.AllocHGlobal(m_clipData.Length);
            Marshal.Copy(m_clipData, 0, m_clipStruct.intPtrClipData, m_clipData.Length);
            m_clipStruct.uintSize = (uint)m_clipData.Length;
            Marshal.StructureToPtr(m_clipStruct, propVar.intPtr, true);
        }

        /// <summary>
        /// Writes to string.
        /// </summary>
        /// <returns>
        /// Returns the updated string.
        /// </returns>
        public string WriteToString()
        {
            StringBuilder sb = new StringBuilder();
            byte[] fmtArr = BitConverter.GetBytes(m_clipStruct.intClipFmt);
            sb.Append(Convert.ToBase64String(fmtArr));
            sb.Append(" ");
            sb.Append(Convert.ToBase64String(m_clipData));
            return sb.ToString();
        }
    }
}
