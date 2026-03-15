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
using System.Runtime.InteropServices;

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Summary description for Border Structure.
    /// </summary>
    [CLSCompliant(false)]
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Sequential)]
#endif
    internal class BorderStructure : DataStructure
    {
        #region Constants
        private const int DEF_RECORD_SIZE = 4;
        #endregion

        #region Class members
        /// <summary>
        /// width of a single line in 1/8 pt, max of 32 pt.
        /// </summary>
        private byte m_dptLineWidth = 0;

        /// <summary>
        /// border type code:
        /// </summary>
        private byte m_brcType = 0;

        /// <summary>
        /// color code
        /// </summary>
        private byte m_color = 0;

        //private Color m_colorExt = Color.Empty;
        /// <summary>
        /// :5 1F00 width of space to maintain between border and text within border. 
        ///         Must be 0 when BRC is a substructure of TC. Stored in points. 
        /// :1 2000 when 1, border is drawn with shadow. Must be 0 when BRC is a substructure of the TC 
        /// </summary>
        private byte m_props = 0;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="BorderStructure"/> class.
        /// </summary>
        /// <param name="arr">The arr.</param>
        /// <param name="iOffset">The i offset.</param>
        public BorderStructure(byte[] arr, int iOffset)
        {
            Parse(arr, iOffset);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BorderStructure"/> class.
        /// </summary>
        public BorderStructure()
        {

        }
        #endregion

        #region Class properties
        /// <summary>
        /// Width of a Line 
        /// </summary>
        public byte LineWidth
        {
            get
            {
                return m_dptLineWidth;
            }
            set
            {
                m_dptLineWidth = value;
            }
        }

        /// <summary>
        /// Border type code
        /// </summary>
        public byte BorderType
        {
            get
            {
                return m_brcType;
            }
            set
            {
                m_brcType = value;
            }
        }

        /// <summary>
        /// Width of space to maintain between border and text within border
        /// </summary>
        public byte Space
        {
            get
            {
                return (byte)(m_props & 0x1F);
            }
            set
            {
                byte res = value;
                m_props = (byte)(m_props & 0xE0);
                m_props += res;
            }
        }

        /// <summary>
        /// when true, border is drawn with shadow
        /// </summary>
        public bool Shadow
        {
            get
            {
                byte val = (byte)(m_props & 0x20);
                val = (byte)(val >> 5);
                return (val == 1);
            }
            set
            {
                byte res = (value) ? (byte)1 : (byte)0;
                m_props = (byte)(m_props & 0xDF);
                res = (byte)(res << 5);
                m_props += res;
            }
        }

        /// <summary>
        /// Gets or sets the color of the line.
        /// </summary>
        /// <value>The color of the line.</value>
        public byte LineColor
        {
            get
            {
                return m_color;
            }

            set
            {
                m_color = value;
            }
        }
       
        /// <summary>
        /// Gets a value indicating whether this instance is clear.
        /// </summary>
        /// <value>if this instance is clear, set to <c>true</c>.</value>
        public bool IsClear
        {
            get
            {
                return m_dptLineWidth == 255;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal byte Props
        {
            get
            {
                return m_props;
            }
            set
            {
                m_props = value;
            }
        }

        /// <summary>
        /// Gets the size of the structure.
        /// </summary>
        /// <value>The length.</value>
        internal override int Length
        {
            get
            {
                return DEF_RECORD_SIZE;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Clones the BorderStructure instance.
        /// </summary>
        /// <returns></returns>
        internal BorderStructure Clone()
        {
            BorderStructure border = new BorderStructure();
            border.m_brcType = m_brcType;
            border.m_color = m_color;
            border.m_dptLineWidth = m_dptLineWidth;
            border.m_props = m_props;
            return border;
        }
        /// <summary>
        /// Parse byte array to BorderCode structure
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="iOffset"></param>
        internal override void Parse(byte[] arr, int iOffset)
        {
            m_dptLineWidth = arr[iOffset + 0];
            m_brcType = arr[iOffset + 1];
            m_color = arr[iOffset + 2];
            m_props = arr[iOffset + 3];
        }

        /// <summary>
        /// Saves the specified arr.
        /// </summary>
        /// <param name="arr">The arr.</param>
        /// <param name="iOffset">The i offset.</param>
        internal override int Save(byte[] arr, int iOffset)
        {
            arr[iOffset + 0] = m_dptLineWidth;
            arr[iOffset + 1] = m_brcType;
            arr[iOffset + 2] = m_color;
            arr[iOffset + 3] = m_props;

            return DEF_RECORD_SIZE; // record length
        }
        #endregion
    }
}
