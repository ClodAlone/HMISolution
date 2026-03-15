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

using System;
using System.Diagnostics;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#if !WINRT && !WP
using System.Drawing;
#endif

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for ShadingDescriptor.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class ShadingDescriptor
    {
        #region Class constants
        internal const int DEF_SHD_LENGTH = 2;
        internal const int DEF_SHD_NEW_LENGTH = 10;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private uint m_foreColorExt = 0xff000000;
        private uint m_backColorExt = 0xff000000;
        private TextureStyle m_pattern = TextureStyle.TextureNone;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets/sets foreground color
        /// </summary>
        internal Color ForeColor
        {
            get
            {
                if (m_foreColorExt != 0xff000000)
                {
                    return WordColor.ConvertRGBToColor(m_foreColorExt);
                }
                else
                {
                    return Color.Empty;
                }
            }
            set
            {
                m_foreColorExt = WordColor.ConvertColorToRGB(value);
            }
        }
        /// <summary>
        /// Gets/sets background color
        /// </summary>
        internal Color BackColor
        {
            get
            {
                if (m_backColorExt != 0xff000000)
                {
                    return WordColor.ConvertRGBToColor(m_backColorExt);
                }
                else
                {
                    return Color.Empty;
                }
            }
            set
            {
                m_backColorExt = WordColor.ConvertColorToRGB(value);
            }
        }
        /// <summary>
        /// Gets/sets pattern
        /// </summary>
        internal TextureStyle Pattern
        {
            get
            {
                return m_pattern;
            }
            set
            {
                m_pattern = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal static int StructLength
        {
            get
            {
                return DEF_SHD_LENGTH;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializing constructor
        /// </summary>
        /// <param name="shd"></param>
        internal ShadingDescriptor(short shd)
        {
            Read(shd);
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        internal ShadingDescriptor()
        { }
        #endregion

        #region Class internal methods
        /// <summary>
        /// Clones the ShadingDescriptor instance.
        /// </summary>
        /// <returns></returns>
        internal ShadingDescriptor Clone()
        {
            ShadingDescriptor shadeDescriptor = new ShadingDescriptor();
            shadeDescriptor.m_foreColorExt = m_foreColorExt;
            shadeDescriptor.m_backColorExt = m_backColorExt;
            shadeDescriptor.m_pattern = m_pattern;
            return shadeDescriptor;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shd"></param>
        internal void Read(short shd)
        {
            m_foreColorExt = WordColor.ConvertIdToRGB(shd & 0x001F);
            m_backColorExt = WordColor.ConvertIdToRGB((shd & 0x03E0) >> 5);
            m_pattern = (TextureStyle)((shd & 0xFC00) >> 10);
        }
        /// <summary>
        /// Returns non-parsed structure as short value
        /// </summary>
        /// <returns></returns>
        internal short Save()
        {
            int shd = 0;
            shd = (int)WordColor.ArgbArray[0] | (int)WordColor.ConvertRGBToId(this.m_foreColorExt);
            shd = (int)WordColor.ArgbArray[0] | (int)(WordColor.ConvertRGBToId(this.m_backColorExt) << 5);
            //if shading pattern is TextureStyle.TextureNil then set as TextureStyle.TextureNone beacuse
            //sprmTDefTableShd80 not suport TextureNil also it will ignore If nFib is greater than 0x00D9
            // (i.e if shading pattern is TextureStyle.TextureNil)
            if (m_pattern == TextureStyle.TextureNil)
                shd |= (((int)TextureStyle.TextureNone) << 10);
            else
                shd |= (((int)m_pattern) << 10);
            return (short)shd;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shd"></param>
        /// <param name="offset"></param>
        internal void ReadNewShd(byte[] shd, int offset)
        {
            m_foreColorExt = BitConverter.ToUInt32(shd, offset);
            m_backColorExt = BitConverter.ToUInt32(shd, offset + Constants.BytesInInt);
            m_pattern = (TextureStyle)BitConverter.ToUInt16(shd, offset + Constants.BytesInInt * 2);
        }
        /// <summary>
        /// Returns non-parsed structure as short value
        /// </summary>
        /// <returns></returns>
        internal byte[] SaveNewShd()
        {
            byte[] operand = new byte[DEF_SHD_NEW_LENGTH];
            byte[] buf = BitConverter.GetBytes(m_foreColorExt);
            buf.CopyTo(operand, 0);

            buf = BitConverter.GetBytes(m_backColorExt);
            buf.CopyTo(operand, 4);

            buf = BitConverter.GetBytes((ushort)m_pattern);
            buf.CopyTo(operand, 8);

            return operand;
        }
        #endregion
    }
}
