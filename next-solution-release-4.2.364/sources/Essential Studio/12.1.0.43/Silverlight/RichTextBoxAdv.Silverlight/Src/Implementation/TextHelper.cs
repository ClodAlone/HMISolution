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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Syncfusion.Windows.Tools.Controls
{
    internal class TextHelper
    {
        [ThreadStatic]
        private static TextBlock textMeasurer;

        internal static TextBlock TextMeasurer
        {
            get
            {
                if (textMeasurer == null)
                {
                    textMeasurer = new TextBlock() { IsHitTestVisible = false };
                    textMeasurer.FontSize = (11 * 96) / 72;
                }

                return textMeasurer;
            }
        }
        private static double baseFont = 100;
        private static Dictionary<string, Size> sizePerCharacter = new Dictionary<string, Size>();
        private static Dictionary<string, double> baseline = new Dictionary<string, double>();

        static TextHelper()
        {
            textMeasurer = new TextBlock() { IsHitTestVisible = false };
            textMeasurer.FontSize = (11 * 96) / 72;
        }

        /// <summary>
        /// Measures the text size
        /// </summary>
        /// <param name="str">String for which the size has to be measured</param>
        /// <param name="fontStyle">FontStyle of the string</param>
        /// <param name="fontWeight">FontWeight of the string</param>
        /// <param name="fontFamily">font family of the string</param>
        /// <param name="size">font size of the string</param>
        /// <returns>Size of the text</returns>
        public static Size MeasureText(string str, FontStyle fontStyle, FontWeight fontWeight, FontFamily fontFamily, double size)
        {
            TextMeasurer.FontFamily = fontFamily;
            TextMeasurer.FontSize = size;
            TextMeasurer.FontStyle = fontStyle;
            TextMeasurer.FontWeight = fontWeight;
            TextMeasurer.Text = str;
#if WPF
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return new Size(TextMeasurer.DesiredSize.Width, TextMeasurer.DesiredSize.Height);
#endif

#if !WPF
            return new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
#endif
        }

        /// <summary>
        /// Measures the text size
        /// </summary>
        /// <param name="str"></param>
        /// <param name="textElement"></param>
        /// <returns></returns>
        public static Size MeasureText(string str, TextElementBox textElement)
        {
            return Measure(str, textElement);

            //TextMeasurer.FontFamily = textElement.FontFamily;
            //if (textElement.Baseline != Baseline.Normal)
            //{
            //    TextMeasurer.FontSize = textElement.BaselineFontSize;
            //}
            //else
            //{
            //    TextMeasurer.FontSize = textElement.FontSize;
            //}

            //TextMeasurer.FontStyle = textElement.FontStyle;
            //TextMeasurer.FontWeight = textElement.FontWeight;
            //TextMeasurer.Text = str;
            //textElement.BaselineOffset = TextMeasurer.BaselineOffset;
            //return new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
        }

        public static Size MeasureText(string str, InlineStyle style)
        {
            TextMeasurer.FontFamily = style.FontFamily;
            TextMeasurer.FontSize = (style.FontSize * 96) / 72;
            TextMeasurer.FontStyle = style.FontStyle;
            TextMeasurer.FontWeight = style.FontWeight;
            TextMeasurer.Text = str;

#if WPF
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return new Size(TextMeasurer.DesiredSize.Width, TextMeasurer.DesiredSize.Height);
#endif

#if !WPF
            return new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="textElement"></param>
        /// <returns></returns>
        public static Size MeasureText(string str, ElementBox textElement)
        {
            if (textElement is TextElementBox)
            {
                TextElementBox element = textElement as TextElementBox;
                TextMeasurer.FontFamily = element.FontFamily;
                if (element.Baseline != Baseline.Normal)
                {
                    TextMeasurer.FontSize = element.BaselineFontSize;
                }
                else
                {
                    TextMeasurer.FontSize = element.FontSize;
                }

                TextMeasurer.FontStyle = element.FontStyle;
                TextMeasurer.FontWeight = element.FontWeight;
                TextMeasurer.Text = str;
#if WPF
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
#endif
                textElement.BaselineOffset = TextMeasurer.BaselineOffset;
            }
            else if (textElement is HyperlinkElementBox)
            {
                HyperlinkElementBox element = textElement as HyperlinkElementBox;
                TextMeasurer.FontFamily = element.FontFamily;
                if (element.Baseline != Baseline.Normal)
                {
                    TextMeasurer.FontSize = element.BaselineFontSize;
                }
                else
                {
                    TextMeasurer.FontSize = element.FontSize;
                }

                TextMeasurer.FontStyle = element.FontStyle;
                TextMeasurer.FontWeight = element.FontWeight;
                TextMeasurer.Text = str;
#if WPF
                TextMeasurer.Measure(new Size(double.PositiveInfinity,double.PositiveInfinity));
#endif
                textElement.BaselineOffset = TextMeasurer.BaselineOffset;
            }

#if WPF
            TextMeasurer.Measure(new Size(double.PositiveInfinity,double.PositiveInfinity));
            return new Size(TextMeasurer.DesiredSize.Width, TextMeasurer.DesiredSize.Height);
#endif

#if !WPF
            return new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
#endif
        }

        /// <summary>
        /// Measures the text size
        /// </summary>
        /// <param name="str"></param>
        /// <param name="textElement"></param>
        /// <returns></returns>
        public static Size MeasureText(string str, HyperlinkElementBox textElement)
        {
            TextMeasurer.FontFamily = textElement.FontFamily;
            TextMeasurer.FontSize = textElement.FontSize;
            TextMeasurer.FontStyle = textElement.FontStyle;
            TextMeasurer.FontWeight = textElement.FontWeight;
            TextMeasurer.Text = str;
            textElement.BaselineOffset = textElement.BaselineOffset;
#if WPF
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return new Size(TextMeasurer.DesiredSize.Width, TextMeasurer.DesiredSize.Height);
#endif

#if !WPF
            return new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
#endif
        }

        /// <summary>
        /// Measures the text size
        /// </summary>
        /// <param name="str"></param>
        /// <param name="textElement"></param>
        /// <returns></returns>
        public static Size MeasureText(string str, SpanAdv span)
        {
            SetDefaultStyle();
            TextMeasurer.FontFamily = span.FontFamily;
            TextMeasurer.FontSize = (span.FontSize * 96) / 72;
            TextMeasurer.FontStyle = span.FontStyle;
            TextMeasurer.FontWeight = span.FontWeight;
            TextMeasurer.Text = str;
#if WPF
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return new Size(TextMeasurer.DesiredSize.Width, TextMeasurer.DesiredSize.Height);
#endif

#if !WPF
            return new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">String for which the size has to be measured</param>
        /// <returns>Size of the text</returns>
        public static Size MeasureText(string str)
        {
            SetDefaultStyle();
            TextMeasurer.Text = str;
#if WPF
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return new Size(TextMeasurer.DesiredSize.Width, TextMeasurer.DesiredSize.Height);
#endif

#if !WPF
            return new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
#endif
        }

        /// <summary>
        /// Measures the text size
        /// </summary>
        /// <param name="str">String for which the size has to be measured</param>
        /// <param name="fontWeight">FontWeight of the string</param>
        /// <param name="size">font size of the string</param>
        /// <returns>Size of the text</returns>
        public static Size MeasureText(string str, FontWeight fontWeight, double size)
        {
            SetDefaultStyle();
            TextMeasurer.FontSize = size;
            TextMeasurer.FontWeight = fontWeight;
            TextMeasurer.Text = str;
#if WPF
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return new Size(TextMeasurer.DesiredSize.Width, TextMeasurer.DesiredSize.Height);
#endif

#if !WPF
            return new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
#endif
        }

        /// <summary>
        /// Measures the text size
        /// </summary>
        /// <param name="str">String for which the size has to be measured</param>
        /// <param name="size">font size of the string</param>
        /// <returns>Size of the text</returns>
        public static Size MeasureText(string str, double size)
        {
            SetDefaultStyle();
            TextMeasurer.FontSize = size;
            TextMeasurer.Text = str;
#if WPF
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return new Size(TextMeasurer.DesiredSize.Width, TextMeasurer.DesiredSize.Height);
#endif

#if !WPF
            return new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
#endif
        }

        /// <summary>
        /// Sets default style for TextMeasurer
        /// </summary>
        public static void SetDefaultStyle()
        {
            TextMeasurer.FontFamily = new FontFamily("Verdana");
            TextMeasurer.FontSize = (11 * 96) / 72;
            TextMeasurer.FontStyle = FontStyles.Normal;
            TextMeasurer.FontWeight = FontWeights.Normal;
            TextMeasurer.Foreground = new SolidColorBrush(Colors.Black);
            TextMeasurer.Text = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="elementBox"></param>
        /// <returns></returns>
        public static Size Measure(string str, TextElementBox elementBox)
        {
            Size charSize = new Size();
            double width = 0.0;
            double fontSize = elementBox.Baseline != Baseline.Normal ? elementBox.BaselineFontSize : elementBox.FontSize;
            for (int i = 0; i < str.Length; i++)
            {
                charSize = GetCharSize(str[i], elementBox);
                width += charSize.Width;
            }
            Size size = new Size((width * fontSize) / baseFont, (charSize.Height * fontSize) / baseFont);
            return size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="elementBox"></param>
        /// <returns></returns>
        public static Size GetCharSize(char character, TextElementBox elementBox)
        {
            Size charSize;
            double baselineOffset = 0;
            double fontSize = elementBox.Baseline != Baseline.Normal ? elementBox.BaselineFontSize : elementBox.FontSize;
            string key = character + "_" + elementBox.FontWeight + elementBox.FontStyle + elementBox.FontFamily.Source;

            if (!sizePerCharacter.TryGetValue(key, out charSize))
            {
                TextMeasurer.FontSize = 100;
                TextMeasurer.Text = character.ToString();
                TextMeasurer.FontFamily = elementBox.FontFamily;
                TextMeasurer.FontWeight = elementBox.FontWeight;
                TextMeasurer.FontStyle = elementBox.FontStyle;

#if WPF
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                charSize = new Size(TextMeasurer.DesiredSize.Width, TextMeasurer.DesiredSize.Height);
#endif
#if !WPF
                charSize = new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
#endif
                sizePerCharacter[key] = charSize;
            }
            if (!baseline.TryGetValue(key, out baselineOffset))
            {
                baseline[key] = TextMeasurer.BaselineOffset;
                baselineOffset = TextMeasurer.BaselineOffset;
            }
            elementBox.BaselineOffset = (baselineOffset * fontSize) / baseFont;

            return charSize;
        }
    }
}
