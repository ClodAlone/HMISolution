#region Copyright Syncfusion Inc. 2001 - 2014
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !SILVERLIGHT

using System;

using Syncfusion.DocIO.DLS;

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Summary description for ParagraphLayoutInfo.
    /// </summary>
    internal class ParagraphLayoutInfo : LayoutInfo
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsPageBreak = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsFirstLine = true;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsNotFitted = false;
        /// <summary>
        /// 
        /// </summary>
        private double m_topMargin;
        /// <summary>
        /// 
        /// </summary>
        private double m_bottomMargin;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsKeepTogether = false;
        /// <summary>
        /// 
        /// </summary>
        private int m_levelNumber = -1;
        /// <summary>
        /// 
        /// </summary>
        private HorizontalAlignment m_justification;
        /// <summary>
        /// 
        /// </summary>
        private float m_firstLineIndent;
        /// <summary>
        /// 
        /// </summary>
        private float m_listTab;
        /// <summary>
        /// 
        /// </summary>
        private float m_yPosition;
        /// <summary>
        /// 
        /// </summary>
        private string m_listValue = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        private WCharacterFormat m_characterFormat;
        /// <summary>
        /// 
        /// </summary>
        private ListNumberAlignment m_listAlignment;
        /// <summary>
        /// 
        /// </summary>
        private TabsLayoutInfo.LayoutTab m_listTabStop;
        /// <summary>
        /// 
        /// </summary>
        private float m_xPosition;
        /// <summary>
        /// 
        /// </summary>
        private ListType m_listType;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance is page break.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is page break; otherwise, <c>false</c>.
        /// </value>
        public bool IsPageBreak
        {
            get
            {
                return m_bIsPageBreak;
            }
            set
            {
                m_bIsPageBreak = value;
            }
        }

        /// <summary>
        /// Gets or sets the level number.
        /// </summary>
        /// <value>The level number.</value>
        public int LevelNumber
        {
            get
            {
                return m_levelNumber;
            }
            set
            {
                m_levelNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the justification.
        /// </summary>
        /// <value>The justification.</value>
        public HorizontalAlignment Justification
        {
            get
            {
                return m_justification;
            }
            set
            {
                m_justification = value;
            }
        }

        /// <summary>
        /// Gets or sets the first line indent.
        /// </summary>
        /// <value>The first line indent.</value>
        public float FirstLineIndent
        {
            get
            {
                return m_firstLineIndent;
            }
            set
            {
                m_firstLineIndent = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is keep together.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is keep together; otherwise, <c>false</c>.
        /// </value>
        public bool IsKeepTogether
        {
            get
            {
                return m_bIsKeepTogether;
            }
            set
            {
                m_bIsKeepTogether = value;
            }
        }

        /// <summary>
        /// Gets or sets the list tab.
        /// </summary>
        /// <value>The list tab.</value>
        public float ListTab
        {
            get
            {
                return m_listTab;
            }
            set
            {
                m_listTab = value;
            }
        }
        /// <summary>
        /// Gets or sets the Y position.
        /// </summary>
        /// <value>The Y position.</value>
        internal float YPosition
        {
            get
            {
                return m_yPosition;
            }
            set
            {
                m_yPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets the list value.
        /// </summary>
        /// <value>The list value.</value>
        public string ListValue
        {
            get
            {
                return m_listValue;
            }
            set
            {
                m_listValue = value;
            }
        }
        /// <summary>
        /// Gets or sets the Current list Type.
        /// </summary>
        /// <value>The list value.</value>
        public ListType CurrentListType
        {
            get
            {
                return m_listType;
            }
            set
            {
                m_listType = value;
            }
        }
        /// <summary>
        /// Gets or sets the character format for the list.
        /// </summary>
        /// <value>The character format.</value>
        public WCharacterFormat CharacterFormat
        {
            get
            {
                return m_characterFormat;
            }
            set
            {
                m_characterFormat = value;
            }
        }


        /// <summary>
        /// Gets or sets alignment of the list number (left, right, or centered). 
        /// </summary>
        public ListNumberAlignment ListAlignment
        {
            get
            {
                return m_listAlignment;
            }
            set
            {
                m_listAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the current list tab stop.
        /// </summary>
        public TabsLayoutInfo.LayoutTab ListTabStop
        {
            get
            {
                return m_listTabStop;
            }
            set
            {
                m_listTabStop = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the line is the first line of the paragraph.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this line is first line of the paragraph; otherwise, <c>false</c>.
        /// </value>
        public bool IsFirstLine
        {
            get
            {
                return m_bIsFirstLine;
            }
            set
            {
                m_bIsFirstLine = value;
            }
        }
        /// <summary>
        /// Gets or sets TopMargin of the paragraph.
        /// </summary>
        public double TopMargin
        {
            get
            {
                return m_topMargin;
            }
            set
            {
                m_topMargin = value;
            }
        }
        /// <summary>
        /// Gets or sets BottomMargin of the paragraph.
        /// </value>
        public double BottomMargin
        {
            get
            {
                return m_bottomMargin;
            }
            set
            {
                m_bottomMargin = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the paragraph is last paragraph of the page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this paragraph paragraph is last paragraph of the page; otherwise, <c>false</c>.
        /// </value>
        public bool IsNotFitted
        {
            get
            {
                return m_bIsNotFitted;
            }
            set
            {
                m_bIsNotFitted = value;
            }
        }
        /// <summary>
        /// Gets or sets the X position.
        /// </summary>
        /// <value>The Y position.</value>
        internal float XPosition
        {
            get
            {
                return m_xPosition;
            }
            set
            {
                m_xPosition = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutParagraphInfo"/> class.
        /// </summary>
        /// <param name="childLayoutDirection">The child layout direction.</param>
        public ParagraphLayoutInfo(ChildrenLayoutDirection childLayoutDirection)
            : base(childLayoutDirection)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutParagraphInfo"/> class.
        /// </summary>
        /// <param name="childLayoutDirection">The child layout direction.</param>
        /// <param name="isPageBreak">Is page break.</param>
        public ParagraphLayoutInfo(ChildrenLayoutDirection childLayoutDirection, bool isPageBreak)
            : this(childLayoutDirection)
        {
            m_bIsPageBreak = isPageBreak;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutParagraphInfo"/> class.
        /// </summary>
        public ParagraphLayoutInfo()
            : base()
        { }
        #endregion
    }
}

#endif
