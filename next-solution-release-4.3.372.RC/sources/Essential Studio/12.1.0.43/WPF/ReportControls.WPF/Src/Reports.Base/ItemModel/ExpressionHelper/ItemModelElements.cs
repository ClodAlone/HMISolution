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
using Syncfusion.RDL.DOM;

namespace Syncfusion.RDL.ItemModel
{
    internal class ReportItemExpval
    {
        public BorderExpval Border
        {
            get;
            set;
        }

        public ThicknessExpval Padding
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

        public bool Hidden
        {
            get;
            set;
        }

        public String WritingMode
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

        public VerticalAlign VerticalAlignment
        {
            get;
            set;
        }

        public TextboxActionInfoExpVal  TextboxActionInfo 
        {
            get;
            set;
        }
    }

    internal class TextboxActionInfoExpVal
    {
        public string Hyperlink { get; set; }
        public string BookmarkLink { get; set; }
        public string ReportName { get; set; }
        public List<TextboxParameterExpVal> Parameters { get; set; }
    }

    internal class TextboxParameterExpVal
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Omit { get; set; }
    }

    internal class BorderExpval
    {
        public BorderExpvalProperties Default
        {
            get;
            set;
        }

        public BorderExpvalProperties TopBorder
        {
            get;
            set;
        }

        public BorderExpvalProperties LeftBorder
        {
            get;
            set;
        }

        public BorderExpvalProperties RightBorder
        {
            get;
            set;
        }

        public BorderExpvalProperties BottomBorder
        {
            get;
            set;
        }
    }

    internal class BorderExpvalProperties
    {
        double thickness = 1;

        internal double Thickness
        {
            get
            {
                return thickness;
            }
            set
            {
                this.thickness = value;
            }
        }

        internal string BorderBrush { get; set; }
        internal BorderStyles BorderStyle { get; set; }
    }

    public class ThicknessExpval
    {
        public ThicknessExpval()
        {
        }

        public ThicknessExpval(double uniformLength)
        {
            this.Bottom = this.Left = this.Right = this.Top = uniformLength;
        }

        public ThicknessExpval(double left, double top, double right, double bottom)
        {
            this.Bottom = bottom;
            this.Left = left;
            this.Right = right;
            this.Top = top;
        }

        public double Bottom { get; set; }

        public double Left { get; set; }

        public double Right { get; set; }

        public double Top { get; set; }
    }

    internal class ParagraphExpval
    {
        internal string TextAlignment
        {
            get;
            set;
        }

        internal double LeftIndent
        {
            get;
            set;
        }

        internal double RightIndent
        {
            get;
            set;
        }

        internal double HangingIndent
        {
            get;
            set;
        }

        internal double SpaceBefore
        {
            get;
            set;
        }

        internal double SpaceAfter
        {
            get;
            set;
        }

        internal ListStyle ListStyle
        {
            get; 
            set;
        }

        internal int ListLevel
        {
            get; 
            set;
        }

        internal List<TextRunExpval> Runs
        {
            get;
            set;
        }
    }

    internal class TextRunExpval
    {
        public StyleExpval Style
        {
            get; 
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public object RunText
        {
            get;
            set;
        }

        public TextboxActionInfoExpVal ActionInfoExpVal
        {
            get;
            set;
        }
    }

    internal class StyleExpval
    {
        string format = String.Empty;

        string language = String.Empty;

        FontExpval font = new FontExpval();

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

        public string WritingMode
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

        public FontExpval Font
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

        public StyleExpval()
        {
            this.Font = new FontExpval();
        }
    }

    internal class FontExpval
    {
        private double fontSize = 13;

        public double FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }

        private string fontFamily = "Arial";

        public string FontFamily
        {
            get { return fontFamily; }
            set { fontFamily = value; }
        }

        private FontWeight fontWeight = FontWeight.Normal;

        public FontWeight FontWeight
        {
            get { return fontWeight; }
            set { fontWeight = value; }
        }

        private FontStyle fontStyle = FontStyle.Normal;

        public FontStyle FontStyle
        {
            get { return fontStyle; }
            set { fontStyle = value; }
        }
    }

    internal class ReportExpval
    {
        public BorderExpval Border
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

        public Source ImageSource
        {
            get;
            set;
        }
    }
}