#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.RDL.ItemModel
{
    internal class ReportItemExp
    {
        public BorderExp Border
        {
            get;
            set;
        }

        public ThicknessExp Padding
        {
            get;
            set;
        }

        public bool CanGrow
        {
            get;
            set;
        }

        public bool CanShrink
        {
            get;
            set;
        }

        public string Hidden
        {
            get;
            set;
        }

        public string WrtingMode
        {
            get;
            set;
        }


        public string DocumentMap
        {
            get;
            set;
        }

        public string BackGroundColor
        {
            get;
            set;
        }

        public string Color
        {
            get;
            set;
        }

        public string VerticalAlignment
        {
            get;
            set;
        }

        public TextboxActionInfoExp TextboxActionInfo 
        {
            get;
            set; 
        }
    }

    internal class TextboxActionInfoExp
    {
        public string Hyperlink { get; set; }
        public string BookmarkLink { get; set; }
        public string ReportName { get; set; }
        public List<TextboxParameterExp> Parameters { get; set; }
    }

    internal class TextboxParameterExp
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Omit { get; set; }
    }

    internal class BorderExp
    {
        public BorderExpProperties Default
        {
            get;
            set;
        }

        public BorderExpProperties TopBorder
        {
            get;
            set;
        }

        public BorderExpProperties LeftBorder
        {
            get;
            set;
        }

        public BorderExpProperties RightBorder
        {
            get;
            set;
        }

        public BorderExpProperties BottomBorder
        {
            get;
            set;
        }
    }

    internal class BorderExpProperties
    {
        internal DOM.Size ThicknessSize { get; set; }
        internal string Thickness { get; set; }
        internal string BorderBrush { get; set; }
        internal string BorderStyle { get; set; }
    }

    public class ThicknessExp
    {
        public ThicknessExp()
        {
        }

        public ThicknessExp(string uniformLength)
        {
            this.Bottom = this.Left = this.Right = this.Top = uniformLength;
        }

        public ThicknessExp(string left, string top, string right, string bottom)
        {
            this.Bottom = bottom;
            this.Left = left;
            this.Right = right;
            this.Top = top;
        }

        public DOM.Size BottomSize { get; set; }

        public DOM.Size LeftSize { get; set; }

        public DOM.Size RightSize { get; set; }

        public DOM.Size TopSize { get; set; }

        public string Bottom { get; set; }

        public string Left { get; set; }

        public string Right { get; set; }

        public string Top { get; set; }
    }

    internal class ParagraphExp
    {
        internal string TextAlignment
        {
            get;
            set;
        }

        internal string LeftIndent
        {
            get;
            set;
        }

        internal DOM.ListStyle ListStyle
        {
            get; 
            set;
        }

        internal int ListLevel
        {
            get; 
            set;
        }

        internal DOM.Size SpaceBeforeSize
        {
            get;
            set;
        }

        internal DOM.Size SpaceAfterSize
        {
            get;
            set;
        }

        internal string SpaceBefore
        {
            get;
            set;
        }

        internal string SpaceAfter
        {
            get;
            set;
        }

        internal string RightIndent
        {
            get;
            set;
        }

        internal List<TextRunExp> Runs
        {
            get;
            set;
        }
    }

    internal class TextRunExp
    {
        public StyleExp Style
        {
            get; 
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public  TextboxActionInfoExp ActionInfo
        {
            get; 
            set;
        }
    }

    internal class StyleExp
    {
        string format = String.Empty;

        string language = String.Empty;

        FontExp font = new FontExp();

        public string TextColor
        {
            get;
            set;
        }

        public string TextDecoration
        {
            get;
            set;
        }

        public string Format
        {
            get
            {
                return this.format;
            }
            set
            {
                this.format = value;
            }
        }

        public string Language
        {
            get
            {
                return this.language;
            }
            set
            {
                this.language = value;
            }
        }

        public FontExp Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
            }
        }

        public StyleExp()
        {
            this.Font = new FontExp();
        }
    }

    internal class FontExp
    {
        public DOM.Size FontSizeValue { get; set; }

        public string FontSize { get; set; }

        public string FontFamily { get; set; }

        public string FontWeight { get; set; }

        public string FontStyle { get; set; }
    }


    internal class ReportExp
    {
        public BorderExp Border
        {
            get;
            set;
        }

        public string BackgroudColor
        {
            get;
            set;
        }

        public bool PrintOnFirstPage
        {
            get;
            set;
        }
        
        public bool PrintOnLastPage
        {
            get;
            set;
        }

        public string ImageValue
        {
            get;
            set;
        }

        public string ImageSource
        {
            get;
            set;
        }
    }
}