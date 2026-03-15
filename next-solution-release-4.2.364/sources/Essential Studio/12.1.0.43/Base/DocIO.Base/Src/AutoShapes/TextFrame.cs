#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.DocIO.DLS
{
    public class TextFrame
    {
        //private bool wrapTextInShape = true;
        //private bool isAutoMargins = true;

        //private bool isTextOverFlow;
        //private int marginLeftPt;
        //private int topMarginPt;
        //private int rightMarginPt;
        //private int bottomMarginPt;
        //private TextDirection textDirection;
        //private VerticalAlignment verticalAlignment;
        //private HorizontalAlignment horizontalAlignment;
        //private TextVertOverflowType textVertOverflowType;
        //private TextHorzOverflowType textHorzOverflowType;
        private TextDirection m_TextDirection;
        public TextDirection TextDirection
        {
            get
            {
                return m_TextDirection;
            }
            set { m_TextDirection = value; }
        }

        private VerticalAlignment m_TextVerticalAlignment;
        public VerticalAlignment TextVerticalAlignment
        {
            get { return m_TextVerticalAlignment; }
            set { m_TextVerticalAlignment = value; }
        }

        private float m_HorizontalRelativePercent = float.MinValue;
        internal float HorizontalRelativePercent
        {
            get { return m_HorizontalRelativePercent; }
            set { m_HorizontalRelativePercent = value; }
        }
        private float m_VerticalRelativePercent = float.MinValue;
        internal float VerticalRelativePercent
        {
            get { return m_VerticalRelativePercent; }
            set { m_VerticalRelativePercent = value; }
        }
        private InternalMargin m_intMargin;
        /// <summary>
        /// Gets the internal margin.
        /// </summary>
        /// <value>The internal margin.</value>
        internal InternalMargin InternalMargin
        {
            get
            {
                if (m_intMargin == null)
                {
                    m_intMargin = new InternalMargin();
                }
                return m_intMargin;
            }
        }
        //public HorizontalAlignment HorizontalAlignment
        //{
        //    get { return this.horizontalAlignment; }
        //    set { this.horizontalAlignment = value; }
        //}
        //public VerticalAlignment VerticalAlignment
        //{
        //    get { return this.verticalAlignment; }
        //    set { this.verticalAlignment = value; }
        //}
        //public TextDirection TextDirection
        //{
        //    get { return this.textDirection; }
        //    set { this.textDirection = value; }
        //}
        //private Shape shape;
        //private bool isAutoSize;

        //public bool IsTextOverFlow
        //{
        //    get { return this.isTextOverFlow; }
        //    set { this.isTextOverFlow = value; }
        //}
        //public bool WrapTextInShape
        //{
        //    get { return this.wrapTextInShape; }
        //    set { this.wrapTextInShape = value; }
        //}


        //public int MarginLeftPt
        //{
        //    get { return marginLeftPt; }
        //    set { marginLeftPt = value; }
        //}
        //public int TopMarginPt
        //{
        //    get { return topMarginPt; }
        //    set { topMarginPt = value; }
        //}
        //public int RightMarginPt
        //{
        //    get { return rightMarginPt; }
        //    set { rightMarginPt = value; }
        //}
        //public int BottomMarginPt
        //{
        //    get { return bottomMarginPt; }
        //    set { bottomMarginPt = value; }

        //}
        //public bool IsAutoMargins
        //{
        //    get { return isAutoMargins; }
        //    set { isAutoMargins = value; }
        //}
        //public TextFrameColumns Columns;

        //public TextVertOverflowType TextVertOverflowType
        //{
        //    get { return textVertOverflowType; }
        //    set { textVertOverflowType = value; }
        //}
        //public TextHorzOverflowType TextHorzOverflowType
        //{
        //    get { return textHorzOverflowType; }
        //    set { textHorzOverflowType = value; }
        //}
        private Shape m_shape;
        internal TextFrame(Shape shape)
        {
            m_shape = shape;
        }
    }
}
