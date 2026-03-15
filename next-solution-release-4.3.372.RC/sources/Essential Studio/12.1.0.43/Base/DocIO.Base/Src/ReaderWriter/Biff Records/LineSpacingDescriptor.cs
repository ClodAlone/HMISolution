#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for LineSpacingDescriptor.
    /// </summary>
    internal class LineSpacingDescriptor
    {
        #region Class members
        private short m_dyaLine;
        private bool m_fMultLinespace;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal short LineSpacing
        {
            get
            {
                return m_dyaLine;
            }
            set
            {
                switch (LineSpacingRule)
                {
                    case LineSpacingRule.AtLeast:
                        {
                            m_dyaLine = value;
                            return;
                        }
                    case LineSpacingRule.Exactly:
                        {
                            m_dyaLine = (short)-value;
                            return;
                        }
                }
                throw new Exception("Trying to set unsupported line spacing rule.");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal LineSpacingRule LineSpacingRule
        {
            get
            {
                if (m_fMultLinespace)
                {
                    return LineSpacingRule.Multiple;
                }
                else if (m_dyaLine < 0)
                {
                    return LineSpacingRule.Exactly;
                }
                return LineSpacingRule.AtLeast;
            }
            set
            {
                switch (value)
                {
                    case LineSpacingRule.AtLeast:
                        {
                            m_fMultLinespace = false;
                            m_dyaLine = Math.Abs(m_dyaLine);
                            return;
                        }
                    case LineSpacingRule.Exactly:
                        {
                            m_fMultLinespace = false;
                            m_dyaLine = (short)(-Math.Abs(m_dyaLine));
                            return;
                        }
                    case LineSpacingRule.Multiple:
                        {
                            m_fMultLinespace = true;
                            m_dyaLine = Math.Abs(m_dyaLine);
                            return;
                        }
                }
                //	      throw new Exception("Unsupported line spacing rule.");
            }
        }
        #endregion

        #region Class initialize / finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal LineSpacingDescriptor()
        { }
        /// <summary>
        /// 
        /// </summary>
        internal LineSpacingDescriptor(byte[] operand)
        {
            Parse(operand);
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operand"></param>
        internal void Parse(byte[] operand)
        {
            m_dyaLine = BitConverter.ToInt16(operand, 0);
            m_fMultLinespace = (BitConverter.ToInt16(operand, 2) != 0);
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte[] Save()
        {
            byte[] operand = new byte[4];
            (BitConverter.GetBytes(m_dyaLine)).CopyTo(operand, 0);
            operand[2] = (byte)((m_fMultLinespace) ? 1 : 0);

            return operand;
        }
        #endregion

    }
}
