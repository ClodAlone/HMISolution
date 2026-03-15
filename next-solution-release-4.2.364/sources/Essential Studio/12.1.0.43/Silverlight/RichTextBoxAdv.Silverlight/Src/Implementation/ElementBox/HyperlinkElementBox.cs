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

namespace Syncfusion.Windows.Tools.Controls
{
    public class HyperlinkElementBox : ElementBox
    {
        internal TextBlock textBlock;
        private string text;
        private Baseline baseline = Baseline.Normal;
        private double fontSize = 11;
        private FontFamily fontFamily = new FontFamily("Verdana");
        private FontStyle fontStyle = FontStyles.Normal;
        private FontWeight fontWeight = FontWeights.Normal;
        private Color foreground = Colors.Black;
        private string navigationUrl;
        private HyperlinkTargetType hyperlinkTargetType = HyperlinkTargetType.Self;
        internal Line Underline;
        internal Line SingleStrike;
        internal List<Line> DoubleStrikes;
        internal Path Highlightpath;

        internal double BaselineFontSize = 0;

        /// <summary>
        /// Initializes the new instance of HyperlinkElementBox
        /// </summary>
        public HyperlinkElementBox()
        {
            textBlock = new TextBlock();
            Element = textBlock;
            //DoubleStrikes = new List<Line>();
        }

        /// <summary>
        /// Initializes the new instance of HyperlinkElementBox
        /// </summary>
        /// <param name="uiElement"></param>
        public HyperlinkElementBox(FrameworkElement element)
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
                fontSize = value;
                if (textBlock != null)
                {
                    textBlock.FontSize = value;
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
                if (textBlock != null)
                {
                    textBlock.FontStyle = value;
                }
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
                if (textBlock != null)
                {
                    textBlock.FontWeight = value;
                }
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
                if (textBlock != null)
                {
                    textBlock.FontFamily = value;
                }
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
                CreateUIElement();
                //ElementSize = TextHelper.MeasureText(text, this);
            }
        }

        /// <summary>
        /// Gets or Sets the navigation url
        /// </summary>
        public string NavigationUrl
        {
            get
            {
                return navigationUrl;
            }
            set
            {
                navigationUrl = value;
            }
        }

#if !WPF
        /// <summary>
        /// Gets or Sets the hyperlink target type
        /// </summary>
        public HyperlinkTargetType HyperlinkTargetType
        {
            get
            {
                return hyperlinkTargetType;
            }
            set
            {
                hyperlinkTargetType = value;
            }
        }
#endif
        /// <summary>
        /// Creates the text block 
        /// </summary>
        /// <returns></returns>
        internal TextBlock CreateUIElement()
        {
            if (Baseline == Baseline.Subscript || Baseline == Baseline.Superscript)
            {
                BaselineFontSize = (60 * FontSize) / 100;
                textBlock.Text = text;
                textBlock.FontStyle = FontStyle;
                textBlock.Foreground = new SolidColorBrush(Foreground);
                textBlock.FontWeight = FontWeight;
                textBlock.FontSize = fontSize;
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

#if WPF
            textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
#endif
            BaselineOffset = textBlock.BaselineOffset;
#if WPF
            ElementSize = new Size(textBlock.DesiredSize.Width, textBlock.DesiredSize.Height);
#else
            ElementSize = new Size(textBlock.ActualWidth, textBlock.ActualHeight);
#endif

            if (Uri.IsWellFormedUriString(NavigationUrl, UriKind.RelativeOrAbsolute))
            {
                ToolTipService.SetToolTip(textBlock, string.Format("{0}\nCTRL + Click to follow link", NavigationUrl));
            }

            textBlock.MouseLeftButtonDown += new MouseButtonEventHandler(textBlock_MouseLeftButtonDown);
            textBlock.MouseLeftButtonUp += new MouseButtonEventHandler(textBlock_MouseLeftButtonUp);
            textBlock.MouseLeave += new MouseEventHandler(textBlock_MouseLeave);
            textBlock.MouseMove += new MouseEventHandler(textBlock_MouseMove);

            return textBlock;
        }

        internal override void MeasureSize()
        {
            CreateUIElement();
        }
        /// <summary>
        /// Handles the mouse move event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBlock_MouseMove(object sender, MouseEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                ((TextBlock)sender).Cursor = Cursors.Hand;
            }
            else
            {
                ((TextBlock)sender).Cursor = Cursors.IBeam;
            }
        }

        /// <summary>
        /// Handles the mouse leave event for hyperlink
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).Cursor = Cursors.IBeam;
        }

        /// <summary>
        /// Handles the mouse left button up event for hyperlink
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //See the link http://msdn.microsoft.com/en-us/library/ms536651(VS.85).aspx for browser Targets

#if !WPF
            string str = "_self";

            if (HyperlinkTargetType == HyperlinkTargetType.Blank)
                str = "_blank";
#endif

            if (NavigationUrl != null && Uri.IsWellFormedUriString(NavigationUrl, UriKind.RelativeOrAbsolute))
            {

#if !WPF
                HtmlPage.Window.Navigate(new Uri(NavigationUrl), str);
#endif
#if WPF
                System.Diagnostics.Process.Start(NavigationUrl);
#endif
            }

            RichTextBoxAdv richtextbox = base.Inline.Paragraph.BaseParent;
            if (richtextbox != null)
            {
                richtextbox.FireHyperlinkClicked();
            }
        }

        /// <summary>
        /// Handles the mouse left button down for hyperlink
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
            }
        }

        protected override void OnTextChanged()
        {
            text = InternalText;
            CreateUIElement();
        }

        internal override ElementBox CreateElementBox()
        {
            HyperlinkElementBox hyperlink = new HyperlinkElementBox();
            hyperlink.Foreground = Foreground;
            hyperlink.Baseline = Baseline;
            hyperlink.NavigationUrl = NavigationUrl;
            hyperlink.FontWeight = FontWeight;
            hyperlink.FontStyle = FontStyle;
            hyperlink.FontFamily = FontFamily;
            hyperlink.FontSize = FontSize;
#if !WPF
            hyperlink.HyperlinkTargetType = HyperlinkTargetType;
#endif
            return hyperlink;
        }

        /// <summary>
        /// Sets the ui element positions
        /// </summary>
        internal override void SetElementPosition()
        {
            try
            {
                Point point = Location;

                if (textBlock != null)
                    BaselineOffset = textBlock.BaselineOffset;

                double offset = LineInfo.GetMaximumAscent() - BaselineOffset;

                if (Baseline == Baseline.Subscript)
                {
                    Size size = TextHelper.MeasureText(Text, FontStyle, FontWeight, FontFamily, FontSize);
                    offset = Size.Height - TextHelper.TextMeasurer.BaselineOffset / 2 - CalculatedLineSpacing - AfterSpacing - Offset;
                    point = new Point(Location.X, Location.Y + offset);
                }
                else if (Baseline == Baseline.Superscript)
                {
                    Size size = TextHelper.MeasureText(Text, this);
                    offset = BeforeSpacing;
                    //point = new Point(Location.X, Location.Y + (Size.Height - size.Height - (Offset / 2)));
                    point = new Point(Location.X, Location.Y + offset);
                }
                else
                {
                    //point = new Point(Location.X, Location.Y + (Size.Height - textBlock.ActualHeight - (Offset / 2)));
                    point = new Point(Location.X, Location.Y + offset);
                }

                ElementLocation = point;

                Point pt = new Point(ElementLocation.X - LineInfo.BoundingRectangle.Left, 0);
                Rect rect = new Rect(pt, new Size(Size.Width, Size.Height - 1));

                InternalRect = rect;

                // DrawHighlight(rect);
                //DrawText();
                //  DrawUnderline(rect);
                //  DrawStrikeThrough(rect);
                //  textBlock.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, textBlock.ActualWidth, textBlock.BaselineOffset) };
                Canvas.SetLeft(textBlock, point.X);
                Canvas.SetTop(textBlock, point.Y);
                Canvas.SetZIndex(textBlock, 1);
            }
            catch { }
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

        /// <summary>
        /// Draws the text
        /// </summary>
        public void DrawText()
        {
            SetElementPosition();
            //TextHelper.TextMeasurer.Text = Text;
            //TextHelper.TextMeasurer.FontFamily = FontFamily;
            //TextHelper.TextMeasurer.FontSize = FontSize;
            //TextHelper.TextMeasurer.FontStyle = FontStyle;
            //TextHelper.TextMeasurer.FontWeight = FontWeight;
            //TextHelper.TextMeasurer.Foreground = new SolidColorBrush(Foreground);
            //Point pt = new Point(ElementLocation.X - LineInfo.BoundingRectangle.Left, ElementLocation.Y - LineInfo.BoundingRectangle.Top);
            //LineInfo.DrawText(pt);
        }

        /// <summary>
        /// Draws the highlighting rectangle
        /// </summary>
        public void DrawHighlight(Rect rect)
        {
            HyperlinkAdv hyperlink = Inline as HyperlinkAdv;
            if (hyperlink != null && hyperlink.HighlightColor != null && !IsWhiteColor(hyperlink.HighlightColor))
            {
                LineInfo.DrawRectangle(rect, hyperlink.HighlightColor);
            }
        }

        public void DrawUnderline(Rect rect)
        {
            HyperlinkAdv hyperlink = Inline as HyperlinkAdv;
            if (hyperlink != null && hyperlink.Underline)
            {
                double remainingSpace = CalculatedLineSpacing + AfterSpacing + Offset;
                if (Baseline == Baseline.Subscript)
                {
                    Size size = TextHelper.MeasureText(Text, FontStyle, FontWeight, FontFamily, BaselineFontSize);
                    if (remainingSpace - (TextHelper.TextMeasurer.BaselineOffset / 2) > 0)
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
                //int y1 = (int)rect.Bottom;
                int y1 = (int)((Size.Height - CalculatedLineSpacing - AfterSpacing - Offset) + imaginaryPoint);
                int x2 = (int)rect.Right;
                int y2 = y1;
                LineInfo.DrawLine(x1, y1, x2, y2, Foreground);
            }
        }

        public void DrawStrikeThrough(Rect rect)
        {
            HyperlinkAdv hyperlink = Inline as HyperlinkAdv;
            if (hyperlink != null)
            {
                int x1 = (int)rect.X;
                int y1 = (int)(rect.Top + (ElementLocation.Y - Location.Y) + ElementSize.Height / 2);
                int x2 = (int)rect.Right;
                int y2 = y1;
                if (hyperlink.StrikeThrough == StrikeThrough.SingleStrike)
                {
                    LineInfo.DrawLine(x1, y1, x2, y2, Foreground);
                }
                else if (hyperlink.StrikeThrough == StrikeThrough.DoubleStrike)
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
