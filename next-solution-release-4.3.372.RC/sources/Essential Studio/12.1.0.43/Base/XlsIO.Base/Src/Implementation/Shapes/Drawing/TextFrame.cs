#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Syncfusion.XlsIO.Implementation;
namespace Syncfusion.XlsIO.Drawing
{
    public class TextFrame:ITextFrame
    {
        private bool wrapTextInShape = true;
        private bool isAutoMargins = true;

        private bool isTextOverFlow;
        private int marginLeftPt;
        private int topMarginPt;
        private int rightMarginPt;
        private int bottomMarginPt;
        private TextDirection textDirection;
        private ExcelVerticalAlignment verticalAlignment;
        private ExcelHorizontalAlignment horizontalAlignment;
        private TextVertOverflowType textVertOverflowType;
        private TextHorzOverflowType textHorzOverflowType;
        private ShapeImplExt shape;
        private bool isAutoSize;
        private TextRange m_textRange;
        internal TextFrameColumns Columns;
        internal TextFrame(ShapeImplExt shape)
        {
            this.shape = shape;
        }

       
        public bool IsTextOverFlow
        {
            get { return this.isTextOverFlow; }
            set 
            { 
                this.isTextOverFlow = value;
                SetVisible();
            }
        }
        public bool WrapTextInShape
        {
            get { return this.wrapTextInShape; }
            set 
            { 
                this.wrapTextInShape = value;
                SetVisible();
            }
        }
        public int MarginLeftPt
        {
            get { return marginLeftPt; }
            set { marginLeftPt = value; }
        }
        public int TopMarginPt
        {
            get { return topMarginPt; }
            set { topMarginPt = value; }
        }
        public int RightMarginPt
        {
            get { return rightMarginPt; }
            set { rightMarginPt = value; }
        }
        public int BottomMarginPt
        {
            get { return bottomMarginPt; }
            set { bottomMarginPt = value; }

        }
        public bool IsAutoMargins
        {
            get { return isAutoMargins; }
            set 
            { 
                isAutoMargins = value;
                SetVisible();
            }
        }
        public bool IsAutoSize
        {
            get
            {
                return this.isAutoSize;
            }
            set
            {
                this.isAutoSize = value;
                SetVisible();
            }
        }
        public TextVertOverflowType TextVertOverflowType
        {
            get { return textVertOverflowType; }
            set 
            { 
                textVertOverflowType = value;
                SetVisible();
            }
        }
        public TextHorzOverflowType TextHorzOverflowType
        {
            get { return textHorzOverflowType; }
            set 
            { 
                textHorzOverflowType = value;
                SetVisible();
            }
        }
        public ExcelHorizontalAlignment HorizontalAlignment
        {
            get { return this.horizontalAlignment; }
            set 
            { 
                this.horizontalAlignment = value;
                SetVisible();
            }
        }
        public ExcelVerticalAlignment VerticalAlignment
        {
            get { return this.verticalAlignment; }
            set 
            { 
                this.verticalAlignment = value;
                SetVisible();
            }
        }
        public TextDirection TextDirection
        {
            get { return this.textDirection; }
            set 
            { 
                this.textDirection = value;
                SetVisible();
            }
        }
        public ITextRange TextRange
        {
            get 
            {
                if (this.m_textRange == null)
                    this.m_textRange = new TextRange(this,this.shape.Logger);
                return this.m_textRange;
            }
        }


        internal bool GetAnchorPosition(TextDirection textDirection, ExcelVerticalAlignment verticalAlignment, ExcelHorizontalAlignment horizontalAlignment, out string anchor)
        {
            anchor = "t";
            switch (textDirection)
            {
                case TextDirection.Horizontal:
                    {
                        switch (verticalAlignment)
                        {
                            case ExcelVerticalAlignment.Top:
                                anchor = "t";
                                return false;
                            case ExcelVerticalAlignment.Middle:
                                anchor = "ctr";
                                return false;
                            case ExcelVerticalAlignment.Bottom:
                                anchor = "b";
                                return false;
                            case ExcelVerticalAlignment.TopCentered:
                                anchor = "t";
                                return true;
                            case ExcelVerticalAlignment.MiddleCentered:
                                anchor = "ctr";
                                return true;
                            case ExcelVerticalAlignment.BottomCentered:
                                anchor = "b";
                                return true;
                        }
                        break;
                    }
                case TextDirection.RotateAllText90:
                case TextDirection.StackedRightToLeft:
                    {
                        switch (horizontalAlignment)
                        {
                            case ExcelHorizontalAlignment.Right:
                                anchor = "t";
                                return false;
                            case ExcelHorizontalAlignment.Center:
                                anchor = "ctr";
                                return false;
                            case ExcelHorizontalAlignment.Left:
                                anchor = "b";
                                return false;
                            case ExcelHorizontalAlignment.RightMiddle:
                                anchor = "t";
                                return true;
                            case ExcelHorizontalAlignment.CenterMiddle:
                                anchor = "ctr";
                                return true;
                            case ExcelHorizontalAlignment.LeftMiddle:
                                anchor = "b";
                                return true;

                        }
                        break;
                    }
                case TextDirection.RotateAllText270:
                case TextDirection.StackedLeftToRight:
                    {
                        switch (horizontalAlignment)
                        {
                            case ExcelHorizontalAlignment.Left:
                                anchor = "t";
                                return false;
                            case ExcelHorizontalAlignment.Center:
                                anchor = "ctr";
                                return false;
                            case ExcelHorizontalAlignment.Right:
                                anchor = "b";
                                return false;
                            case ExcelHorizontalAlignment.LeftMiddle:
                                anchor = "t";
                                return true;
                            case ExcelHorizontalAlignment.CenterMiddle:
                                anchor = "ctr";
                                return true;
                            case ExcelHorizontalAlignment.RightMiddle:
                                anchor = "b";
                                return true;
                        }
                        break;
                    }
            }
            return false;
        }
        internal string GetTextDirection(TextDirection textDirection)
        {
            switch (textDirection)
            {
                case TextDirection.Horizontal:
                    return "horz";
                case TextDirection.RotateAllText90:
                    return "vert";
                case TextDirection.RotateAllText270:
                    return "vert270";
                case TextDirection.StackedLeftToRight:
                    return "wordArtVert";
                case TextDirection.StackedRightToLeft:
                    return "wordArtVertRtl";
            }
            return "horz";
        }
        internal int GetLeftMargin()
        {
            return (int)((this.marginLeftPt * 12700.0) + 0.5);
        }
        internal void SetLeftMargin(int value)
        {
            this.marginLeftPt = (int)(value / 12700.0);
        }
        internal int GetTopMargin()
        {
            return (int)((this.topMarginPt * 12700.0) + 0.5);
        }
        internal void SetTopMargin(int value)
        {
            this.topMarginPt = (int)(value / 12700.0);
        }
        internal int GetRightMargin()
        {
            return (int)((this.rightMarginPt * 12700.0) + 0.5);
        }
        internal void SetRightMargin(int value)
        {
            this.rightMarginPt = (int)(value / 12700.0);
        }
        internal int GetBottomMargin()
        {
            return (int)((this.bottomMarginPt * 12700.0) + 0.5);
        }
        internal void SetBottomMargin(int value)
        {
            this.bottomMarginPt = (int)(value / 12700.0);
        }
        internal bool GetAnchorPosition(out string anchor)
        {
            return this.GetAnchorPosition(this.textDirection, this.verticalAlignment, this.horizontalAlignment, out anchor);
        }
        internal IWorkbook GetWorkbook()
        {
            return this.shape.Worksheet.Workbook;
        }
        internal void SetVisible()
        {
            this.shape.Logger.SetFlag(PreservedFlag.RichText);
        }

    }
    public class TextRange : ITextRange
    {
        private string m_text;
        private TextFrame textFrame;
       
        private RichTextString m_strText;
        private PreservationLogger preservationLogger;

        
        internal TextRange(TextFrame textFrame, PreservationLogger preservationLogger)
        {
            // TODO: Complete member initialization
            this.textFrame = textFrame;
            this.preservationLogger = preservationLogger;
        }
        public string Text
        {
            get
            {
                return RichText.Text;
            }
            set
            {
                RichText.Text = value;
            }
        }
        /// <summary>
        /// Comment text.
        /// </summary>
        public IRichTextString RichText
        {
            get
            {
                if (m_strText == null)
                    InitializeVariables();

                return m_strText;
            }
            
        }
        /// <summary>
        /// Initializes variables.
        /// </summary>
        protected virtual void InitializeVariables()
        {
            IWorkbook workbook = this.textFrame.GetWorkbook();
            m_strText = new RichTextString(workbook.Application, workbook, false, true,this.preservationLogger);
        }

    }

}
