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
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if WPF
using System.Windows.Controls;
using FontStyleEnum = System.Windows.FontStyles;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
using Windows.UI;
using FontStyleEnum = Windows.UI.Text.FontStyle;
using Windows.UI.Xaml;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class TextHelper
    {
        private const string PaddingCharacter = "a";
        //Cell Mark - Unicode as U+00A4 ¤ currency sign (HTML: &#164; &curren;). 
        /// <summary>
        /// The pilcrow (¶), also called the paragraph mark, paragraph sign. Unicode is U+00B6 pilcrow sign (HTML: &#182; &para;).
        /// </summary>
        private const string ParagraphMark = "¶";
        [ThreadStatic]
        private static TextBlock textMeasurer = null;
        internal static TextBlock TextMeasurer
        {
            get
            {
                if (textMeasurer == null)
                {
                    textMeasurer = new TextBlock()
                    {
#if WPF
#if SyncfusionFramework4_0
                        UseLayoutRounding = false
#endif
#else
                        UseLayoutRounding = false
#endif
                    };
                }
                return textMeasurer;
            }
        }
        static TextHelper()
        {
        }
        /// <summary>
        /// Measures the text size
        /// </summary>
        /// <param name="str"></param>
        /// <param name="textElement"></param>
        /// <returns></returns>
        internal static Size MeasureText(string str, Inline inline)
        {
            Size size = Size.Empty;
            TextMeasurer.FontFamily = inline.CharacterFormat.FontFamily;
            TextMeasurer.FontSize = inline.CharacterFormat.FontSize;
            TextMeasurer.FontStyle = inline.CharacterFormat.Italic ? FontStyleEnum.Italic : FontStyleEnum.Normal;
            TextMeasurer.FontWeight = inline.CharacterFormat.Bold ? FontWeights.Bold : FontWeights.Normal;
            TextMeasurer.Text = EscapeText(str);
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
#if WPF
            size = TextMeasurer.DesiredSize;
            if (inline.CharacterFormat.BaselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                size.Width = TextMeasurer.DesiredSize.Width;
            }
#else
            size = new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
            if (inline.CharacterFormat.BaselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                size.Width = TextMeasurer.ActualWidth;
            }
#endif
            return size;
        }
        internal static Size GetParagraphMarkSize(CharacterFormat characterFormat)
        {
            Size size = Size.Empty;
            TextMeasurer.Text = ParagraphMark;
            TextMeasurer.FontSize = characterFormat.FontSize;
            TextMeasurer.FontFamily = characterFormat.FontFamily;
            TextMeasurer.FontWeight = characterFormat.Bold ? FontWeights.Bold : FontWeights.Normal;
            TextMeasurer.FontStyle = characterFormat.Italic ? FontStyleEnum.Italic : FontStyleEnum.Normal;
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
#if WPF
            size = TextMeasurer.DesiredSize;
            if (characterFormat.BaselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                size.Width = TextMeasurer.DesiredSize.Width;
            }
#else
            size = new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
            if (characterFormat.BaselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                size.Width = TextMeasurer.ActualWidth;
            }
#endif
            return size;
        }
        internal static void UpdateTextSize(TextElementBox textElementBox)
        {
            CharacterFormat format = textElementBox.Inline.CharacterFormat;
            TextMeasurer.FontSize = format.FontSize;
            TextMeasurer.FontFamily = format.FontFamily;
            TextMeasurer.FontWeight = format.Bold ? FontWeights.Bold : FontWeights.Normal;
            TextMeasurer.FontStyle = format.Italic ? FontStyleEnum.Italic : FontStyleEnum.Normal;
            string text = textElementBox.Text;
            TextMeasurer.Text = text;
#if WPF
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            textElementBox.BaselineOffset = TextMeasurer.BaselineOffset;
            textElementBox.Width = TextMeasurer.DesiredSize.Width;
            textElementBox.Height = TextMeasurer.DesiredSize.Height;
            if (format.BaselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                textElementBox.Width = TextMeasurer.DesiredSize.Width;
            }
#else
            double paddingCharacterWidth = 0;
            if (text.EndsWith(" "))
            {
                TextMeasurer.Text = PaddingCharacter;
                if (format.BaselineAlignment != BaselineAlignment.Normal)
                    TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                paddingCharacterWidth = TextMeasurer.ActualWidth;
                TextMeasurer.Text = text + PaddingCharacter;
                if (format.BaselineAlignment != BaselineAlignment.Normal)
                    TextMeasurer.FontSize = format.FontSize;
            }
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            textElementBox.BaselineOffset = TextMeasurer.BaselineOffset;
            textElementBox.Width = TextMeasurer.ActualWidth;
            textElementBox.Height = TextMeasurer.ActualHeight;
            if (format.BaselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                textElementBox.Width = TextMeasurer.ActualWidth;
            }
            textElementBox.Width -= paddingCharacterWidth;
#endif
            TextMeasurer.Inlines.Clear();
        }
        internal static void UpdateTextSize(TextElementBox textElementBox, string text, double fontSize, FontFamily fontFamily, BaselineAlignment baselineAlignment, bool isBold, bool isItalic)
        {
            TextMeasurer.FontSize = fontSize;
            TextMeasurer.FontFamily = fontFamily;
            TextMeasurer.FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal;
            TextMeasurer.FontStyle = isItalic ? FontStyleEnum.Italic : FontStyleEnum.Normal;
            TextMeasurer.Text = text;
#if WPF
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            textElementBox.BaselineOffset = TextMeasurer.BaselineOffset;
            textElementBox.Width = TextMeasurer.DesiredSize.Width;
            textElementBox.Height = TextMeasurer.DesiredSize.Height;
            if (baselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                textElementBox.Width = TextMeasurer.DesiredSize.Width;
            }
#else
            double paddingCharacterWidth = 0;
            if (text.EndsWith(" "))
            {
                TextMeasurer.Text = PaddingCharacter;
                if (baselineAlignment != BaselineAlignment.Normal)
                    TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                paddingCharacterWidth = TextMeasurer.ActualWidth;
                TextMeasurer.Text = text + PaddingCharacter;
                if (baselineAlignment != BaselineAlignment.Normal)
                    TextMeasurer.FontSize = fontSize;
            }
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            textElementBox.BaselineOffset = TextMeasurer.BaselineOffset;
            textElementBox.Width = TextMeasurer.ActualWidth;
            textElementBox.Height = TextMeasurer.ActualHeight;
            if (baselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                textElementBox.Width = TextMeasurer.ActualWidth;
            }
            textElementBox.Width -= paddingCharacterWidth;
#endif
            TextMeasurer.Inlines.Clear();
        }
        internal static double GetTextSize(TextElementBox textElementBox, double fontSize, FontFamily fontFamily, BaselineAlignment baselineAlignment, bool isBold, bool isItalic)
        {
            TextMeasurer.FontSize = fontSize;
            TextMeasurer.FontFamily = fontFamily;
            TextMeasurer.FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal;
            TextMeasurer.FontStyle = isItalic ? FontStyleEnum.Italic : FontStyleEnum.Normal;
            string text = textElementBox.Text;
            TextMeasurer.Text = text;
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            textElementBox.BaselineOffset = TextMeasurer.BaselineOffset;
            double textTrimEndWidth = 0;
#if WPF
            textTrimEndWidth = textElementBox.Width = TextMeasurer.DesiredSize.Width;
            textElementBox.Height = TextMeasurer.DesiredSize.Height;
            if (baselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                textTrimEndWidth = textElementBox.Width = TextMeasurer.DesiredSize.Width;
            }
            if (text.EndsWith(" "))
            {
                TextMeasurer.Text = text.TrimEnd(' ');
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                textTrimEndWidth = TextMeasurer.DesiredSize.Width;
            }
#else
            textTrimEndWidth = textElementBox.Width = TextMeasurer.ActualWidth;
            textElementBox.Height = TextMeasurer.ActualHeight;
            if (baselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                textTrimEndWidth = textElementBox.Width = TextMeasurer.DesiredSize.Width;
            }
            if (text.EndsWith(" "))
            {
                TextMeasurer.Text = PaddingCharacter;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double paddingCharacterWidth = TextMeasurer.ActualWidth;
                TextMeasurer.Text = text + PaddingCharacter;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                textElementBox.Width = TextMeasurer.ActualWidth - paddingCharacterWidth;
            }
#endif
            TextMeasurer.Inlines.Clear();
            return textTrimEndWidth;
        }
        internal static Size MeasureText(string text, double fontSize, FontFamily fontFamily, BaselineAlignment baselineAlignment, bool isBold, bool isItalic)
        {
#if WPF
            TextMeasurer.Text = text.TrimEnd(' ');
#else
            TextMeasurer.Text = text;
#endif
            TextMeasurer.FontSize = fontSize;
            TextMeasurer.FontFamily = fontFamily;
            TextMeasurer.FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal;
            TextMeasurer.FontStyle = isItalic ? FontStyleEnum.Italic : FontStyleEnum.Normal;
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
#if WPF
            Size size = TextMeasurer.DesiredSize;
            if (baselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                size.Width = TextMeasurer.DesiredSize.Width;
            }
#else
            Size size = new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
            if (baselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                size.Width = TextMeasurer.ActualWidth;
            }
#endif
            return size;
        }
        internal static double GetTextWidth(string text)
        {
#if WPF
            TextMeasurer.Text = text.TrimEnd(' ');
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            TextMeasurer.Inlines.Clear();
            return TextMeasurer.DesiredSize.Width;
#else
            TextMeasurer.Text = text;
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            TextMeasurer.Inlines.Clear();
            return TextMeasurer.ActualWidth;
#endif
        }
        internal static int GetTextSplitIndexByWord(double clientActiveWidth, string text, double fontSize, FontFamily fontFamily, BaselineAlignment baselineAlignment, bool isBold, bool isItalic, double width)
        {
            TextMeasurer.FontSize = fontSize / (baselineAlignment == BaselineAlignment.Normal ? 1 : 1.5);
            TextMeasurer.FontFamily = fontFamily;
            TextMeasurer.FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal;
            TextMeasurer.FontStyle = isItalic ? FontStyleEnum.Italic : FontStyleEnum.Normal;
            int index = 0;
            while (index < text.Length)
            {
                int nextIndex = GetTextIndexAfterSpace(text, index);
                if (nextIndex == 0 || nextIndex == text.Length)
                    nextIndex = text.Length - 1;
                double splitWidth = width;
                if ((nextIndex < text.Length - 1 || (nextIndex == text.Length - 1 && text[nextIndex - 1] == ' ')) && index != nextIndex)
                    splitWidth = GetTextWidth(text.Remove(nextIndex));
                if (splitWidth <= clientActiveWidth)
                    index = nextIndex;
                else
                {
                    if (index == 0 && text.StartsWith(" "))
                        index = GetTextIndexAfterSpace(text, 0);
                    break;
                }
            }
            return index;
        }
        internal static int GetTextIndexAfterSpace(string text, int startIndex)
        {
            int nextIndex = text.IndexOf(' ', startIndex) + 1;
            if (nextIndex == 0 || nextIndex == text.Length)
                return nextIndex;
            while (text[nextIndex] == ' ')
            {
                nextIndex++;
                if (nextIndex == text.Length)
                    break;
            }
            return nextIndex;
        }
        internal static int GetTextSplitIndexByCharacter(double totalClientWidth, double clientActiveWidth, string text, double fontSize, FontFamily fontFamily, BaselineAlignment baselineAlignment, bool isBold, bool isItalic, double width)
        {
            TextMeasurer.FontSize = fontSize / (baselineAlignment == BaselineAlignment.Normal ? 1 : 1.5);
            TextMeasurer.FontFamily = fontFamily;
            TextMeasurer.FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal;
            TextMeasurer.FontStyle = isItalic ? FontStyleEnum.Italic : FontStyleEnum.Normal;
            for (int i = 0; i < text.Length; i++)
            {
                double splitWidth = width;
                if (i + 1 < text.Length)
                    splitWidth = GetTextWidth(text.Remove(i + 1));
                if (splitWidth > clientActiveWidth)
                {
                    if (i == 0 && splitWidth > totalClientWidth)
                        //Handle for cell/section having client width less than a character's width.
                        return (text.Length > 1 && text[1] == ' ') ? GetTextIndexAfterSpace(text, 1) : 1;
                    return i;
                }
            }
            return 0;
        }
        internal static void UpdateTextSize(ListTextElementBox elementBox)
        {
            CharacterFormat format = elementBox.ListLevel.CharacterFormat;
            double fontSize = format.FontSize;
            if (elementBox.Paragraph.CharacterFormat.ReadLocalValue(CharacterFormat.FontSizeProperty) is double)
                fontSize = elementBox.Paragraph.CharacterFormat.FontSize;
            FontFamily fontFamily = format.FontFamily;
            if (elementBox.ListLevel.ReadLocalValue(ListLevelAdv.BulletCharacterProperty) == DependencyProperty.UnsetValue
                && elementBox.Paragraph.CharacterFormat.ReadLocalValue(CharacterFormat.FontFamilyProperty) is FontFamily)
                fontFamily = elementBox.Paragraph.CharacterFormat.FontFamily;
            bool bold = format.Bold;
            if (elementBox.Paragraph.CharacterFormat.ReadLocalValue(CharacterFormat.BoldProperty) is bool)
                bold = elementBox.Paragraph.CharacterFormat.Bold;
            bool italic = format.Italic;
            if (elementBox.Paragraph.CharacterFormat.ReadLocalValue(CharacterFormat.ItalicProperty) is bool)
                italic = elementBox.Paragraph.CharacterFormat.Italic;
            TextMeasurer.FontSize = fontSize;
            TextMeasurer.FontFamily = fontFamily;
            TextMeasurer.FontWeight = bold ? FontWeights.Bold : FontWeights.Normal;
            TextMeasurer.FontStyle = italic ? FontStyleEnum.Italic : FontStyleEnum.Normal;
            string text = elementBox.Text;
            TextMeasurer.Text = text;
#if WPF
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            elementBox.BaselineOffset = TextMeasurer.BaselineOffset;
            elementBox.Width = TextMeasurer.DesiredSize.Width;
            elementBox.Height = TextMeasurer.DesiredSize.Height;
            if (format.BaselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                elementBox.Width = TextMeasurer.DesiredSize.Width;
            }
#else
            double paddingCharacterWidth = 0;
            if (text.EndsWith(" "))
            {
                TextMeasurer.Text = PaddingCharacter;
                if (format.BaselineAlignment != BaselineAlignment.Normal)
                    TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                paddingCharacterWidth = TextMeasurer.ActualWidth;
                TextMeasurer.Text = text + PaddingCharacter;
                if (format.BaselineAlignment != BaselineAlignment.Normal)
                    TextMeasurer.FontSize = format.FontSize;
            }
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            elementBox.BaselineOffset = TextMeasurer.BaselineOffset;
            elementBox.Width = TextMeasurer.ActualWidth;
            elementBox.Height = TextMeasurer.ActualHeight;
            if (format.BaselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                elementBox.Width = TextMeasurer.ActualWidth;
            }
            elementBox.Width -= paddingCharacterWidth;
#endif
            TextMeasurer.Inlines.Clear();
        }
        internal static double GetWidthExcludeSpaceAtEnd(TextElementBox textElementBox)
        {
            CharacterFormat format = textElementBox.Inline.CharacterFormat;
            TextMeasurer.FontSize = format.FontSize / (format.BaselineAlignment == BaselineAlignment.Normal ? 1 : 1.5);
            TextMeasurer.FontFamily = format.FontFamily;
            TextMeasurer.FontWeight = format.Bold ? FontWeights.Bold : FontWeights.Normal;
            TextMeasurer.FontStyle = format.Italic ? FontStyleEnum.Italic : FontStyleEnum.Normal;
#if WPF
            TextMeasurer.Text = textElementBox.Text.TrimEnd(' ');
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            TextMeasurer.Inlines.Clear();
            return TextMeasurer.DesiredSize.Width;
#else
            TextMeasurer.Text = textElementBox.Text;
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            TextMeasurer.Inlines.Clear();
            return TextMeasurer.ActualWidth;
#endif
        }
        internal static Size MeasureText(CharacterFormat characterFormat)
        {
            TextMeasurer.Text = "";
            TextMeasurer.FontSize = characterFormat.FontSize;
            TextMeasurer.FontFamily = characterFormat.FontFamily;
            TextMeasurer.FontWeight = characterFormat.Bold ? FontWeights.Bold : FontWeights.Normal;
            TextMeasurer.FontStyle = characterFormat.Italic ? FontStyleEnum.Italic : FontStyleEnum.Normal;
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
#if WPF
            Size size = TextMeasurer.DesiredSize;
            if (characterFormat.BaselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                size.Width = TextMeasurer.DesiredSize.Width;
            }
#else
            Size size = new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
            if (characterFormat.BaselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                size.Width = TextMeasurer.ActualWidth;
            }
#endif
            return size;
        }
        /// <summary>
        /// Measures text with trialing space at end.
        /// </summary>
        /// <param name="textElementBox">The text element box.</param>
        /// <param name="delStartIndex">Start index of the del.</param>
        /// <returns></returns>
        internal static Size MeasureTextWithSpace(TextElementBox textElementBox, int delStartIndex)
        {
            CharacterFormat format = textElementBox.Inline.CharacterFormat;
            TextMeasurer.FontSize = format.FontSize;
            TextMeasurer.FontFamily = format.FontFamily;
            TextMeasurer.FontWeight = format.Bold ? FontWeights.Bold : FontWeights.Normal;
            TextMeasurer.FontStyle = format.Italic ? FontStyleEnum.Italic : FontStyleEnum.Normal;
            string text = textElementBox.Text.Remove(delStartIndex);
            TextMeasurer.Text = text;
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
#if WPF
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Size size = TextMeasurer.DesiredSize;
            if (format.BaselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                size.Width = TextMeasurer.DesiredSize.Width;
            }
#else
            double paddingCharacterWidth = 0;
            if (text.EndsWith(" "))
            {
                TextMeasurer.Text = PaddingCharacter;
                if (format.BaselineAlignment != BaselineAlignment.Normal)
                    TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                paddingCharacterWidth = TextMeasurer.ActualWidth;
                TextMeasurer.Text = text + PaddingCharacter;
                if (format.BaselineAlignment != BaselineAlignment.Normal)
                    TextMeasurer.FontSize = format.FontSize;
            }
            TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Size size = new Size(TextMeasurer.ActualWidth, TextMeasurer.ActualHeight);
            if (format.BaselineAlignment != BaselineAlignment.Normal)
            {
                TextMeasurer.FontSize /= 1.5;
                TextMeasurer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                size.Width = TextMeasurer.ActualWidth;
            }
            size.Width -= paddingCharacterWidth;
#endif
            TextMeasurer.Inlines.Clear();
            return size;
        }
        private static string EscapeText(string input)
        {
            //Replace the white space with non-breaking white space.
            input = input.Replace((char)32, (char)160);
            return input;
        }
    }
}
