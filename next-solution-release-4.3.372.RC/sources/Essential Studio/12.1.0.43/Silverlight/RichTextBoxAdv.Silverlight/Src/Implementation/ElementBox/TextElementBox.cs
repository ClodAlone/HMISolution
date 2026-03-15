#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Collections.Generic;

#if !WPF
using System.Windows.Browser;
#endif

using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Tools.Controls
{
    public class TextElementBox : ElementBox
    {
        private string text;
        private Baseline baseline = Baseline.Normal;
        private double fontSize = 11;
        private FontFamily fontFamily = new FontFamily("Verdana");
        private FontStyle fontStyle = FontStyles.Normal;
        private FontWeight fontWeight = FontWeights.Normal;
        private Color foreground = Colors.Black;
        internal TextBlock textBlock;
        private Line underline;
        private Line singleStrike;
        internal List<Line> DoubleStrikes;
        private Path highlightPath;

        /// <summary>
        /// Initializes the new instance of TextLayoutBox
        /// </summary>
        public TextElementBox():base()
        {
            //textBlock = new TextBlock();
            //Element = textBlock;
            //DoubleStrikes = new List<Line>();
        }

        internal double BaselineFontSize
        {
            get
            {
                return (60 * FontSize) / 100;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal Line Underline
        {
            get
            {
                return underline;
            }
            set
            {
                underline = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal Line SingleStrike
        {
            get
            {
                return singleStrike;
            }
            set
            {
                singleStrike = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal Path Highlightpath
        {
            get
            {
                return highlightPath;
            }
            set
            {
                highlightPath = value;
            }
        }

        /// <summary>
        /// Initializes the new instance of TextLayoutBox
        /// </summary>
        /// <param name="uiElement"></param>
        public TextElementBox(FrameworkElement element)
            : base(element)
        {
            
        }

        /// <summary>
        /// Gets or Sets the baseline of the text
        /// </summary>
        public Baseline Baseline
        {
            get
            {
                return baseline;
            }
            set
            {
                baseline = value;
            }
        }

        /// <summary>
        /// Gets or Sets the font size 
        /// </summary>
        public double FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                if (value > 0)
                {
                    //textBlock.FontSize = value;
                    fontSize = value;
                }
                else if (value < 0)
                {
                    //textBlock.FontSize = 1;
                    fontSize = 1;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the font style
        /// </summary>
        public FontStyle FontStyle
        {
            get
            {
                return fontStyle;
            }
            set
            {
                fontStyle = value;
                //textBlock.FontStyle = value;
            }
        }

        /// <summary>
        /// Gets or Sets the font weight
        /// </summary>
        public FontWeight FontWeight
        {
            get
            {
                return fontWeight;
            }
            set
            {
                fontWeight = value;
                //textBlock.FontWeight = value;
            }
        }

        /// <summary>
        /// Gets or Sets the font family
        /// </summary>
        public FontFamily FontFamily
        {
            get
            {
                return fontFamily;
            }
            set
            {
                fontFamily = value;
                //textBlock.FontFamily = value;
            }
        }

        /// <summary>
        /// Gets or Sets the foreground
        /// </summary>
        public Color Foreground
        {
            get
            {
                return foreground;
            }
            set
            {
                foreground = value;
                if (textBlock != null)
                {
                    textBlock.Foreground = new SolidColorBrush(value);
                }
            }
        }

        /// <summary>
        /// Gets or Sets the text present in the TextBlock
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                SetText(value);
                //CreateUIElement(); 
                //TextHelper.MeasureText(text, this);
                //size.Width = TextHelper.TextMeasurer.ActualWidth;
                //size.Height = TextHelper.TextMeasurer.ActualHeight;
                ElementSize = TextHelper.MeasureText(text, this);

            }
        }

        internal override void MeasureSize()
        {
            ElementSize = TextHelper.MeasureText(text, this);           
        }

        /// <summary>
        /// Creates the text block 
        /// </summary>
        /// <returns></returns>
        internal TextBlock CreateUIElement()
        {
            if (Baseline == Baseline.Subscript || Baseline == Baseline.Superscript)
            {
                //BaselineFontSize = (60 * FontSize) / 100;
                textBlock.Text = text;
                textBlock.FontStyle = FontStyle;
                textBlock.Foreground = new SolidColorBrush(Foreground);
                textBlock.FontWeight = FontWeight;
                textBlock.FontSize = BaselineFontSize;
                textBlock.FontFamily = FontFamily;
            }
            else
            {
                textBlock.Text = text;
                textBlock.FontStyle = FontStyle;
                textBlock.Foreground = new SolidColorBrush(Foreground);
                textBlock.FontWeight = FontWeight;
                textBlock.FontSize = FontSize;
                textBlock.FontFamily = FontFamily;
            }

            //TranslateTransform transform = new TranslateTransform();
            //transform.X = 0;
            //transform.Y = 0;

            //WriteableBitmap bmp = new WriteableBitmap(textBlock,transform);
            //Image img = new Image();
            //img.Source = bmp;
            //Element = img;
            //img.Width = textBlock.ActualWidth;
            //img.Height = textBlock.ActualHeight;

            return textBlock;
        }

        /// <summary>
        /// Sets the uielements position
        /// </summary>
        internal override void SetElementPosition()
        {
            Point point = Location;

            double offset = LineInfo.GetMaximumAscent() - BaselineOffset ;

            if (Baseline == Baseline.Subscript)
            {
                Size size = TextHelper.MeasureText(Text, FontStyle, FontWeight, FontFamily, FontSize);
                offset = Size.Height -TextHelper.TextMeasurer.BaselineOffset/2- CalculatedLineSpacing - AfterSpacing - Offset;
                point = new Point(Location.X, Location.Y + offset);
            }
            else if (Baseline == Baseline.Superscript)
            {
                Size size = TextHelper.MeasureText(Text, FontStyle, FontWeight, FontFamily, FontSize);
                offset = BeforeSpacing;
                point = new Point(Location.X, Location.Y + offset);
            }
            else
            {
                point = new Point(Location.X, Location.Y + offset);
            }

            ElementLocation = point;

            Point pt = new Point(ElementLocation.X - LineInfo.BoundingRectangle.Left, 0);
            Rect rect = new Rect(pt, Size);
            InternalRect = rect;
           // Render();
          //  DrawHighlight(rect);
          //  DrawText();
            //Dispatcher.BeginInvoke(DrawText);
         //   DrawUnderline(rect);
         //   DrawStrikeThrough(rect);
            //Canvas.SetLeft(Element, point.X);
            //Canvas.SetTop(Element, point.Y);
        }

        protected override void OnTextChanged()
        {
            text = InternalText;
            ElementSize = TextHelper.MeasureText(text, this);
        }

        internal override ElementBox CreateElementBox()
        {
            TextElementBox textElement = new TextElementBox();
            textElement.FontStyle = FontStyle;
            textElement.Foreground = Foreground;
            textElement.FontWeight = FontWeight;
            textElement.FontSize = FontSize;
            textElement.FontFamily = FontFamily;
            textElement.Baseline = Baseline;
            return textElement;
        }

        public double GetBaselineOffset()
        {
            TextHelper.TextMeasurer.Text = Text;
            TextHelper.TextMeasurer.FontFamily = FontFamily;
            TextHelper.TextMeasurer.FontSize = FontSize;
            TextHelper.TextMeasurer.FontStyle = FontStyle;
            TextHelper.TextMeasurer.FontWeight = FontWeight;
            TextHelper.TextMeasurer.Foreground = new SolidColorBrush(Foreground);
            //if (Baseline == Baseline.Subscript || Baseline == Baseline.Superscript)
            //{
            //    TextHelper.TextMeasurer.FontSize = BaselineFontSize;
            //}

            return TextHelper.TextMeasurer.BaselineOffset;
        }

        public void DrawText()
        {
            TextHelper.TextMeasurer.Text = Text;
            TextHelper.TextMeasurer.FontFamily = FontFamily;
            TextHelper.TextMeasurer.FontSize = FontSize;
            TextHelper.TextMeasurer.FontStyle = FontStyle;
            TextHelper.TextMeasurer.FontWeight = FontWeight;
            TextHelper.TextMeasurer.Foreground = new SolidColorBrush(Foreground);
            Point pt = new Point(ElementLocation.X - LineInfo.BoundingRectangle.Left, ElementLocation.Y - LineInfo.BoundingRectangle.Top);
            if (Baseline == Baseline.Subscript || Baseline == Baseline.Superscript)
            {
                TextHelper.TextMeasurer.FontSize = BaselineFontSize;
                //if (Baseline == Baseline.Subscript)
                //{
                //    double offset = Size.Height - TextHelper.TextMeasurer.ActualHeight;
                //    pt = new Point(ElementLocation.X - LineInfo.BoundingRectangle.Left, (Location.Y - LineInfo.BoundingRectangle.Top) + offset);
                //}
            }

           // double offset = (TextHelper.TextMeasurer.BaselineOffset/ TextHelper.TextMeasurer.ActualHeight )* FontSize;

            //Point pt = new Point(ElementLocation.X - LineInfo.BoundingRectangle.Left, ElementLocation.Y - LineInfo.BoundingRectangle.Top);// - offset);
            LineInfo.DrawText(pt);
        }

        public void DrawHighlight(Rect rect)
        {
            SpanAdv span = Inline as SpanAdv;
            if (span != null && span.HighlightColor != null && span.HighlightColor.ToString() != "#00000000")
            {
                LineInfo.DrawRectangle(rect, span.HighlightColor);
            }
        }

        public void DrawUnderline(Rect rect)
        {
            SpanAdv span = Inline as SpanAdv;
            if (span != null && span.Underline)
            {
                double remainingSpace = CalculatedLineSpacing + AfterSpacing + Offset;
                if (Baseline == Baseline.Subscript)
                {
                    Size size = TextHelper.MeasureText(Text, FontStyle, FontWeight, FontFamily, BaselineFontSize);
                    if(remainingSpace - (TextHelper.TextMeasurer.BaselineOffset / 2) > 0)
                        remainingSpace -= (TextHelper.TextMeasurer.BaselineOffset / 2);
                }
                double start = Size.Height - remainingSpace;
                double end = Size.Height;
                double imaginaryPoint = 2;
                if (start + 2 >= end)
                {
                    imaginaryPoint = end - start;
                }
                
                int x1 = (int)rect.X;
                //int y1 = (int)rect.Bottom - 1;
                int y1 = (int)((Size.Height - CalculatedLineSpacing - AfterSpacing - Offset) + imaginaryPoint);
                int x2 = (int)rect.Right;
                int y2 = y1;
                LineInfo.DrawLine(x1, y1, x2, y2, Foreground);
            }
        }

        public void DrawStrikeThrough(Rect rect)
        {
            SpanAdv span = Inline as SpanAdv;
            if (span != null && span.StrikeThrough != StrikeThrough.None)
            {
                int x1 = (int)rect.X;
                int y1 = (int)(rect.Top + (ElementLocation.Y - Location.Y) + TextHelper.MeasureText(Text, this).Height / 2);
                int x2 = (int)rect.Right;
                int y2 = y1;
                if (span.StrikeThrough == StrikeThrough.SingleStrike)
                {                   
                    LineInfo.DrawLine(x1, y1, x2, y2, Foreground);
                }
                else if (span.StrikeThrough == StrikeThrough.DoubleStrike)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        y1 = y2 = y1 + (i * 2);
                        LineInfo.DrawLine(x1, y1, x2, y2, Foreground);
                    }
                }
            }
        }

        public override void Render()
        {
            DrawHighlight(InternalRect);
            DrawText();
            DrawUnderline(InternalRect);
            DrawStrikeThrough(InternalRect);
        }

        internal override Point GetApproxRight(int index)
        {
            double y = BoundingRectangle.Top + BoundingRectangle.Height / 2;
            double x = 0;
            if (index >= 0 && index <= Text.Length)
            {
                x = ElementLocation.X + TextHelper.MeasureText(Text.Substring(0, index), this).Width;
            }

            return new Point(x, y);
        }
    }
}
