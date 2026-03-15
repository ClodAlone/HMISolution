#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for SymbolDescriptor.
    /// </summary>
    internal class SymbolDescriptor
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        internal const int DEF_STRUCT_SIZE = 4;
        internal const int DEF_EXT_VALUE = 240;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private short m_fontCode = 0;
        private byte m_charSpecifier = 0;
        private byte m_charSpecifierExt = DEF_EXT_VALUE;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal short FontCode
        {
            get
            {
                return m_fontCode;
            }
            set
            {
                m_fontCode = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte CharCode
        {
            get
            {
                return m_charSpecifier;
            }
            set
            {
                m_charSpecifier = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte CharCodeExt
        {
            get
            {
                return m_charSpecifierExt;
            }
            set
            {
                m_charSpecifierExt = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal SymbolDescriptor()
        { }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operand"></param>
        internal void Parse(byte[] operand)
        {
            m_fontCode = BitConverter.ToInt16(operand, 0);
            //      m_charSpecifier = BitConverter.ToInt16( operand, 2 );
            m_charSpecifier = operand[2];
            m_charSpecifierExt = operand[3];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal byte[] Save()
        {
            byte[] operand = new byte[DEF_STRUCT_SIZE];
            (BitConverter.GetBytes(m_fontCode)).CopyTo(operand, 0);
            //      (BitConverter.GetBytes( m_charSpecifier )).CopyTo( operand, 2 );
            operand[2] = m_charSpecifier;
            operand[3] = m_charSpecifierExt;

            return operand;
        }
        #endregion
    }
}
