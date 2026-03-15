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

#region File using directives
using System;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for TableBorders.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class TableBorders
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        BorderCode[] m_brcArr = new BorderCode[6];
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal TableBorders()
        {
            Init();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TableBorders"/> class.
        /// </summary>
        /// <param name="sprms">The single property modifier array.</param>
        internal TableBorders(SinglePropertyModifierArray sprms)
        {
            if (sprms == null)
            {
                Init();
            }
            else
            {

                byte[] buf = sprms.GetByteArray(WordSprmOptions.sprmTTableBorders);

                if (buf == null)
                {
                    buf = new byte[ParagraphProperties.DEF_BORDER_COUNT * Constants.BytesInInt];
                }

                for (int i = 0; i < ParagraphProperties.DEF_BORDER_COUNT; i++)
                {
                    m_brcArr[i] = new BorderCode(buf, i * Constants.BytesInInt);
                }
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode this[int index]
        {
            get
            {
                return m_brcArr[index];
            }
            set
            {
                m_brcArr[index] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode LeftBorder
        {
            get
            {
                return m_brcArr[1];
            }
            set
            {
                m_brcArr[1] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode RightBorder
        {
            get
            {
                return m_brcArr[3];
            }
            set
            {
                m_brcArr[3] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode TopBorder
        {
            get
            {
                return m_brcArr[0];
            }
            set
            {
                m_brcArr[0] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode BottomBorder
        {
            get
            {
                return m_brcArr[2];
            }
            set
            {
                m_brcArr[2] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode HorizontalBorder
        {
            get
            {
                return m_brcArr[4];
            }
            set
            {
                m_brcArr[4] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode VerticalBorder
        {
            get
            {
                return m_brcArr[5];
            }
            set
            {
                m_brcArr[5] = value;
            }
        }
        #endregion

        #region Class helper method
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Init()
        {
            for (int i = 0; i < 6; i++)
            {
                m_brcArr[i] = new BorderCode();
            }
        }
        #endregion
    }
}
