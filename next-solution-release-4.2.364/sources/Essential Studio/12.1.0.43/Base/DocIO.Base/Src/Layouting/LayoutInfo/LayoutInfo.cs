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

#if !SILVERLIGHT

using System.Drawing;

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Summary description for ILayoutSpacingsInfo.
    /// </summary>
    internal interface ILayoutSpacingsInfo
    {
        /// <summary>
        /// Gets the paddings.
        /// </summary>
        /// <value>The paddings.</value>
        Spacings Paddings { get; }

        /// <summary>
        /// Gets the margins.
        /// </summary>
        /// <value>The margins.</value>
        Spacings Margins { get; }
    }

    /// <summary>
    /// Summary description for ILayoutInfo.
    /// </summary>
    internal interface ILayoutInfo : ILayoutSpacingsInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is clipped.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is clipped; otherwise, <c>false</c>.
        /// </value>
        bool IsClipped { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is skip.
        /// </summary>
        /// <value><c>true</c> if this instance is skip; otherwise, <c>false</c>.</value>
        bool IsSkip { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is skip bottom align.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is skip bottom align; otherwise, <c>false</c>.
        /// </value>
        bool IsSkipBottomAlign { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is line container.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is line container; otherwise, <c>false</c>.
        /// </value>
        bool IsLineContainer { get; }

        /// <summary>
        /// Gets the children layout direction.
        /// </summary>
        /// <value>The children layout direction.</value>
        ChildrenLayoutDirection ChildrenLayoutDirection { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is line break.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is line break; otherwise, <c>false</c>.
        /// </value>
        bool IsLineBreak { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [text wrap].
        /// </summary>
        /// <value><c>true</c> if [text wrap]; otherwise, <c>false</c>.</value>
        bool TextWrap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is page break item.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is page break item; otherwise, <c>false</c>.
        /// </value>
        bool IsPageBreakItem { get; set; }
        bool IsVerticalText { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance is first Text Body Item of current page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is first Text Body Item of current page; otherwise, <c>false</c>.
        /// </value>
        bool IsFirstItemInPage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is keep with next.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is keep with next; otherwise, <c>false</c>.
        /// </value>
        bool IsKeepWithNext { get; set; }
        /// <summary>
        /// Gets/Sets the Size of the widget
        /// </summary>
        SizeF Size { get; set; }
    }

    /// <summary>
    /// Summary description for LayoutInfo.
    /// </summary>
    internal class LayoutInfo : ILayoutInfo
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsClipped = false;
        private bool m_bIsVerticalText = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsSkip = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsSkipBottomAlign = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsLineContainer = false;
        /// <summary>
        /// 
        /// </summary>
        private ChildrenLayoutDirection m_childrenLayoutDirection = ChildrenLayoutDirection.Horizontal;
        /// <summary>
        /// 
        /// </summary>
        private Spacings m_paddings;
        /// <summary>
        /// 
        /// </summary>
        private Spacings m_margins;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isLineBreak = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_textWrap = true;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bPageBreakItem = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsFirstItemInPage = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsLastItemInPage = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsKeepWithNext = false;
        /// <summary>
        /// 
        /// </summary>
        private float m_lineSpacing;
        /// <summary>
        /// 
        /// </summary>
        private SizeF m_size;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutInfo"/> class.
        /// </summary>
        public LayoutInfo()
        {
            m_bIsSkip = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutInfo"/> class.
        /// </summary>
        /// <param name="childLayoutDirection">The child layout direction.</param>
        public LayoutInfo(ChildrenLayoutDirection childLayoutDirection)
        {
            m_childrenLayoutDirection = childLayoutDirection;
        }
        #endregion

        #region ILayoutInfo Members
        /// <summary>
        /// Gets or sets a value indicating whether this instance is clipped.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is clipped; otherwise, <c>false</c>.
        /// </value>
        public bool IsClipped
        {
            get
            {
                return m_bIsClipped;
            }
            set
            {
                m_bIsClipped = value;
            }
        }

        /// <summary>
        /// Gets/Sets the Size of the Widget 
        /// Currently We have handled the Size property as specifically to get the text size of the TextRange and Empty textrange size of the paragraph.
        /// In Feature, We need to use this property to get the size of the all widgets
        /// </summary>
        public SizeF Size
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is skip.
        /// </summary>
        /// <value><c>true</c> if this instance is skip; otherwise, <c>false</c>.</value>
        public bool IsSkip
        {
            get
            {
                return m_bIsSkip;
            }
            set
            {
                m_bIsSkip = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is skip bottom align.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is skip bottom align; otherwise, <c>false</c>.
        /// </value>
        public bool IsSkipBottomAlign
        {
            get
            {
                return m_bIsSkipBottomAlign;
            }
            set
            {
                m_bIsSkipBottomAlign = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the owner table cell is having text direction as vertical
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is having the text direction as vertical; otherwise, <c>false</c>.
        /// </value>
        public bool IsVerticalText
        {
            get
            {
                return m_bIsVerticalText;
            }
            set
            {
                m_bIsVerticalText = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is line container.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is line container; otherwise, <c>false</c>.
        /// </value>
        public bool IsLineContainer
        {
            get
            {
                return m_bIsLineContainer;
            }
            set
            {
                m_bIsLineContainer = value;
            }
        }

        /// <summary>
        /// Gets the children layout direction.
        /// </summary>
        /// <value>The children layout direction.</value>
        public ChildrenLayoutDirection ChildrenLayoutDirection
        {
            get
            {
                return m_childrenLayoutDirection;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is line break.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is line break; otherwise, <c>false</c>.
        /// </value>
        public bool IsLineBreak
        {
            get
            {
                return m_isLineBreak;
            }
            set
            {
                m_isLineBreak = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [text wrap].
        /// </summary>
        /// <value><c>true</c> if [text wrap]; otherwise, <c>false</c>.</value>
        public bool TextWrap
        {
            get
            {
                return m_textWrap;
            }
            set
            {
                m_textWrap = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is page break item.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is page break item; otherwise, <c>false</c>.
        /// </value>
        public bool IsPageBreakItem
        {
            get
            {
                return m_bPageBreakItem;
            }
            set
            {
                m_bPageBreakItem = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is first Text Body Item of current page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is first Text Body Item of current page; otherwise, <c>false</c>.
        public bool IsFirstItemInPage
        {
            get
            {
                return m_bIsFirstItemInPage;
            }
            set
            {
                m_bIsFirstItemInPage = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is keep with next.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is keep with next; otherwise, <c>false</c>.
        /// </value>
        public bool IsKeepWithNext
        {
            get
            {
                return m_bIsKeepWithNext;
            }
            set
            {
                m_bIsKeepWithNext = value;
            }
        }
        #endregion

        #region ILayoutSpacingsInfo Members
        /// <summary>
        /// Gets the paddings.
        /// </summary>
        /// <value>The paddings.</value>
        public Spacings Paddings
        {
            get
            {
                if (m_paddings == null)
                    m_paddings = new Spacings();

                return m_paddings;
            }
        }

        /// <summary>
        /// Gets the margins.
        /// </summary>
        /// <value>The margins.</value>
        public Spacings Margins
        {
            get
            {
                if (m_margins == null)
                    m_margins = new Spacings();

                return m_margins;
            }
        }
        #endregion
    }
}

#endif
