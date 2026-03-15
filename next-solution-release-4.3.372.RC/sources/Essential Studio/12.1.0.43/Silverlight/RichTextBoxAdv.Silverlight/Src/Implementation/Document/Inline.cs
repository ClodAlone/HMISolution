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
using System.Windows.Resources;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    public abstract class Inline : DependencyObject
    {

        /// <summary>
        /// Initializes the new instance of Inline class
        /// </summary>
        public Inline()
        {

        }

        private string internalText = string.Empty;

        internal string InternalText
        {
            get
            {
                return internalText;
            }
            set
            {
                internalText = value;
                OnTextChanged();
            }
        }

        /// <summary>
        /// Gets or Sets the parent paragraph for this inline
        /// </summary>
        internal ParagraphAdv Paragraph;

        internal bool IsUIContainer = false;

        internal bool IsImageContainer = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        internal abstract void MeasureElements();

        /// <summary>
        /// 
        /// </summary>
        internal abstract List<ElementBox> ElementBoxes
        {
            get;
            set;
        }

        protected virtual void OnTextChanged()
        {

        }

        internal void SetText(string str)
        {
            internalText = str;
        }

        /// <summary>
        /// Determines whether the two inlines are equal in style
        /// </summary>
        /// <param name="inline"></param>
        /// <returns></returns>
        public bool IsEqualInStyle(Inline inline)
        {
            if (this == inline)
            {
                return true;
            }
            else if (this is SpanAdv && inline is SpanAdv)
            {
                SpanAdv span1 = this as SpanAdv;
                SpanAdv span2 = inline as SpanAdv;

                return span1.Baseline == span2.Baseline && span1.FontFamily.Source == span2.FontFamily.Source
                    && span1.FontSize == span2.FontSize && span1.FontStyle == span2.FontStyle && span1.FontWeight == span2.FontWeight
                    && span1.Foreground == span2.Foreground && span1.HighlightColor == span2.HighlightColor && span1.StrikeThrough == span2.StrikeThrough
                    && span1.Underline == span2.Underline;
            }
            else if (this is HyperlinkAdv && inline is HyperlinkAdv)
            {
                HyperlinkAdv hyperlink1 = this as HyperlinkAdv;
                HyperlinkAdv hyperlink2 = inline as HyperlinkAdv;
                return hyperlink1.Baseline == hyperlink2.Baseline && hyperlink1.FontFamily.Source == hyperlink2.FontFamily.Source
                    && hyperlink1.FontSize == hyperlink2.FontSize && hyperlink1.FontStyle == hyperlink2.FontStyle && hyperlink1.FontWeight == hyperlink2.FontWeight
                    && hyperlink1.Foreground == hyperlink2.Foreground && hyperlink1.HighlightColor == hyperlink2.HighlightColor && hyperlink1.StrikeThrough == hyperlink2.StrikeThrough
                    && hyperlink1.Underline == hyperlink2.Underline;
            }
            return false;
        }

        public bool HasEqualUnderlineStyle(Inline inline)
        {
            if (this == inline)
            {
                return true;
            }
            else if (this is SpanAdv && inline is SpanAdv)
            {
                SpanAdv span1 = this as SpanAdv;
                SpanAdv span2 = inline as SpanAdv;

                return span1.Baseline == span2.Baseline && span1.Foreground == span2.Foreground && span1.Underline == span2.Underline;
            }
            else if (this is HyperlinkAdv && inline is HyperlinkAdv)
            {
                HyperlinkAdv hyperlink1 = this as HyperlinkAdv;
                HyperlinkAdv hyperlink2 = inline as HyperlinkAdv;

                return hyperlink1.Baseline == hyperlink2.Baseline && hyperlink1.Foreground == hyperlink2.Foreground && hyperlink1.Underline == hyperlink2.Underline;
            }
            else if (this is HyperlinkAdv && inline is SpanAdv)
            {
                HyperlinkAdv hyperlink1 = this as HyperlinkAdv;
                SpanAdv span1 = inline as SpanAdv;

                return hyperlink1.Baseline == span1.Baseline && hyperlink1.Foreground == span1.Foreground && hyperlink1.Underline == span1.Underline;
            }
            else if (this is SpanAdv && inline is HyperlinkAdv)
            {
                HyperlinkAdv hyperlink1 = inline as HyperlinkAdv;
                SpanAdv span1 = this as SpanAdv;

                return hyperlink1.Baseline == span1.Baseline && hyperlink1.Foreground == span1.Foreground && hyperlink1.Underline == span1.Underline;
            }

            return false;
        }

        public bool HasEqualStrikeThroStyle(Inline inline)
        {
            if (this == inline)
            {
                return true;
            }
            else if (this is SpanAdv && inline is SpanAdv)
            {
                SpanAdv span1 = this as SpanAdv;
                SpanAdv span2 = inline as SpanAdv;

                return span1.Baseline == span2.Baseline && span1.Foreground == span2.Foreground && span1.StrikeThrough == span2.StrikeThrough && span1.FontSize == span2.FontSize;
            }
            else if (this is HyperlinkAdv && inline is HyperlinkAdv)
            {
                HyperlinkAdv hyperlink1 = this as HyperlinkAdv;
                HyperlinkAdv hyperlink2 = inline as HyperlinkAdv;

                return hyperlink1.Baseline == hyperlink2.Baseline && hyperlink1.Foreground == hyperlink2.Foreground && hyperlink1.StrikeThrough == hyperlink2.StrikeThrough && hyperlink1.FontSize == hyperlink2.FontSize;
            }
            else if (this is HyperlinkAdv && inline is SpanAdv)
            {
                HyperlinkAdv hyperlink1 = this as HyperlinkAdv;
                SpanAdv span1 = inline as SpanAdv;

                return hyperlink1.Baseline == span1.Baseline && hyperlink1.Foreground == span1.Foreground && hyperlink1.StrikeThrough == span1.StrikeThrough && hyperlink1.FontSize == span1.FontSize;
            }
            else if (this is SpanAdv && inline is HyperlinkAdv)
            {
                HyperlinkAdv hyperlink1 = inline as HyperlinkAdv;
                SpanAdv span1 = this as SpanAdv;

                return hyperlink1.Baseline == span1.Baseline && hyperlink1.Foreground == span1.Foreground && hyperlink1.StrikeThrough == span1.StrikeThrough && hyperlink1.FontSize == span1.FontSize;
            }

            return false;
        }

        public bool HasEqualHighlightStyle(Inline inline)
        {
            if (this == inline)
            {
                return true;
            }
            else if (this is SpanAdv && inline is SpanAdv)
            {
                SpanAdv span1 = this as SpanAdv;
                SpanAdv span2 = inline as SpanAdv;

                return span1.HighlightColor == span2.HighlightColor;
            }
            else if (this is HyperlinkAdv && inline is HyperlinkAdv)
            {
                HyperlinkAdv hyperlink1 = this as HyperlinkAdv;
                HyperlinkAdv hyperlink2 = inline as HyperlinkAdv;

                return hyperlink1.HighlightColor == hyperlink2.HighlightColor;
            }
            else if (this is HyperlinkAdv && inline is SpanAdv)
            {
                HyperlinkAdv hyperlink1 = this as HyperlinkAdv;
                SpanAdv span1 = inline as SpanAdv;

                return hyperlink1.HighlightColor == span1.HighlightColor;
            }
            else if (this is SpanAdv && inline is HyperlinkAdv)
            {
                HyperlinkAdv hyperlink1 = inline as HyperlinkAdv;
                SpanAdv span1 = this as SpanAdv;

                return hyperlink1.HighlightColor == span1.HighlightColor;
            }

            return false;
        }

        public bool HasEqualTextStyle(Inline inline)
        {
            if (this == inline)
            {
                return true;
            }
            else if (this is SpanAdv && inline is SpanAdv)
            {
                SpanAdv span1 = this as SpanAdv;
                SpanAdv span2 = inline as SpanAdv;

                return span1.Baseline == span2.Baseline && span1.FontFamily.Source == span2.FontFamily.Source
                    && span1.FontSize == span2.FontSize && span1.FontStyle == span2.FontStyle && span1.FontWeight == span2.FontWeight
                    && span1.Foreground == span2.Foreground;
            }
            else if (this is HyperlinkAdv && inline is HyperlinkAdv)
            {
                HyperlinkAdv span1 = this as HyperlinkAdv;
                HyperlinkAdv span2 = inline as HyperlinkAdv;

                return span1.Baseline == span2.Baseline && span1.FontFamily.Source == span2.FontFamily.Source
                    && span1.FontSize == span2.FontSize && span1.FontStyle == span2.FontStyle && span1.FontWeight == span2.FontWeight
                    && span1.Foreground == span2.Foreground;
            }
            //else if (this is HyperlinkAdv && inline is SpanAdv)
            //{
            //    HyperlinkAdv span2 = this as HyperlinkAdv;
            //    SpanAdv span1 = inline as SpanAdv;

            //    return span1.Baseline == span2.Baseline && span1.FontFamily.Source == span2.FontFamily.Source
            //       && span1.FontSize == span2.FontSize && span1.FontStyle == span2.FontStyle && span1.FontWeight == span2.FontWeight
            //       && span1.Foreground == span2.Foreground;
            //}
            //else if (this is SpanAdv && inline is HyperlinkAdv)
            //{
            //    HyperlinkAdv span2 = inline as HyperlinkAdv;
            //    SpanAdv span1 = this as SpanAdv;

            //    return span1.Baseline == span2.Baseline && span1.FontFamily.Source == span2.FontFamily.Source
            //       && span1.FontSize == span2.FontSize && span1.FontStyle == span2.FontStyle && span1.FontWeight == span2.FontWeight
            //       && span1.Foreground == span2.Foreground;
            //}

            return false;
        }

        internal virtual Inline CreatInline()
        {
            return null;
        }

        public abstract double GetLength();
    }

    [ContentProperty("Text")]
    public class SpanAdv : Inline
    {
        #region fields

        private string text = string.Empty;
        private double fontSize;
        private FontFamily fontFamily;
        private Baseline baseline = Baseline.Normal;
        private FontStyle fontStyle = FontStyles.Normal;
        private FontWeight fontWeight;
        private Color highlightColor;
        private StrikeThrough strikeThrough = StrikeThrough.None;
        private bool underline = false;
        private Color foreground;

        /// <summary>
        /// Gets or Sets the Element Box
        /// </summary>
        internal override List<ElementBox> ElementBoxes
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Initializes the instance of span class
        /// </summary>
        public SpanAdv()
        {
            ElementBoxes = new List<ElementBox>();
        }

        #region public properties
        /// <summary>
        /// Gets or Sets the text
        /// </summary>
        public string Text
        {
            get
            {
                // return (string)GetValue(TextProperty);
                return text;
            }
            set
            {
                //SetValue(TextProperty,value);
                text = value;
                SetText(value);
            }
        }

        /// <summary>
        /// Registers the Text dependency property
        /// </summary>
        //   public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SpanAdv), new PropertyMetadata(OnTextPropertyChanged));

        /// <summary>
        /// Gets or Sets the font style
        /// </summary>
        public FontStyle FontStyle
        {
            get
            {
                //return (FontStyle)GetValue(FontStyleProperty);
                return fontStyle;
            }
            set
            {
                //SetValue(FontStyleProperty, value);
                fontStyle = value;

                foreach (ElementBox element in ElementBoxes)
                {
                    TextElementBox textElement = (TextElementBox)element;
                    textElement.FontStyle = value;
                    textElement.MeasureSize();
                }
            }
        }

        /// <summary>
        /// Registers the FontStyle dependency property
        /// </summary>
        //    public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(SpanAdv), new PropertyMetadata(OnFontStylePropertyChanged));

        /// <summary>
        /// Gets or Sets the highlight color
        /// </summary>
        public Color HighlightColor
        {
            get
            {
                // return (Color)GetValue(HighlightColorProperty);
                return highlightColor;
            }
            set
            {
                // SetValue(HighlightColorProperty, value);
                highlightColor = value;
            }
        }

        /// <summary>
        /// Registers the highlight color dependency property
        /// </summary>
        //     public static readonly DependencyProperty HighlightColorProperty = DependencyProperty.Register("HighlightColor", typeof(Color), typeof(SpanAdv), null);

        /// <summary>
        /// Gets or Sets the Strike through
        /// </summary>
        public StrikeThrough StrikeThrough
        {
            get
            {
                //return (StrikeThrough)GetValue(StrikeThroughProperty);
                return strikeThrough;
            }
            set
            {
                //SetValue(StrikeThroughProperty, value);
                strikeThrough = value;
            }
        }

        /// <summary>
        /// Registers the Strike through dependency property
        /// </summary>
        //    public static readonly DependencyProperty StrikeThroughProperty = DependencyProperty.Register("StrikeThrough", typeof(StrikeThrough), typeof(SpanAdv), new PropertyMetadata(StrikeThrough.None));

        /// <summary>
        /// Gets or Sets the under line
        /// </summary>
        public bool Underline
        {
            get
            {
                //return (bool)GetValue(UnderlineProperty);
                return underline;
            }
            set
            {
                //SetValue(UnderlineProperty, value);
                underline = value;
            }
        }

        /// <summary>
        /// Registers the Underline dependency property
        /// </summary>
        //   public static readonly DependencyProperty UnderlineProperty = DependencyProperty.Register("Underline", typeof(bool), typeof(SpanAdv), new PropertyMetadata(false));

        /// <summary>
        /// Gets or Sets the font style
        /// </summary>
        public FontWeight FontWeight
        {
            get
            {
                if (fontWeight == FontWeights.Normal)
                {
                    if (this.Paragraph !=null && this.Paragraph.layoutViewer != null)
                    {
                        return this.Paragraph.layoutViewer.OwnerControl.FontWeight;
                    }
                    else
                    {
                        return FontWeights.Normal;
                    }
                }
                else
                {
                    return fontWeight;
                }
            }
            set
            {
                // SetValue(FontWeightProperty, value);
                fontWeight = value;
                foreach (ElementBox element in ElementBoxes)
                {
                    TextElementBox textElement = (TextElementBox)element;
                    textElement.FontWeight = value;
                    textElement.MeasureSize();
                }
            }
        }

        /// <summary>
        /// Registers the FontWeight dependency property
        /// </summary>
        //    public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(SpanAdv), new PropertyMetadata(OnFontWeightPropertyChanged));

        /// <summary>
        /// Gets or Sets the font size. Reffered in pixels
        /// </summary>
        /// 
#if WPF
 [TypeConverter(typeof(FontSizeConverter))]
#endif
        public double FontSize
        {
            get
            {
                if (fontSize == 0.0)
                {
                    if (this.Paragraph !=null && this.Paragraph.layoutViewer != null)
                    {
                        return this.Paragraph.layoutViewer.OwnerControl.FontSize;
                    }
                    else
                    {
                        return 11d;
                    }
                }
                else
                {
                    return fontSize;
                }
     
            }
            set
            {
                //SetValue(FontSizeProperty, value);
                fontSize = value;

                foreach (ElementBox element in ElementBoxes)
                {
                    TextElementBox textElement = (TextElementBox)element;
                    textElement.FontSize = (value * 96) / 72;
                    textElement.MeasureSize();
                }
            }
        }

        /// <summary>
        /// Registers the FontSize dependency property
        /// </summary>
        //    public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(SpanAdv), new PropertyMetadata(11d, OnFontSizeChanged));

        /// <summary>
        /// Gets or Sets the foreground
        /// </summary>
        public Color Foreground
        {
            get
            {
                if (foreground == Color.FromArgb(0, 0, 0, 0))
                {
                    if (this.Paragraph !=null && this.Paragraph.layoutViewer != null)
                    {

                        return ((System.Windows.Media.SolidColorBrush)(this.Paragraph.layoutViewer.OwnerControl.Foreground)).Color;
                    }
                    else
                    {
                        return Colors.Black;
                    }
                }
                else
                {
                    return foreground;
                }
            }
            set
            {
                // SetValue(ForegroundProperty, value);
                foreground = value;

                foreach (ElementBox element in ElementBoxes)
                {
                    TextElementBox elementBox = element as TextElementBox;
                    if (elementBox != null)
                    {
                        elementBox.Foreground = value;
                    }
                }
            }
        }

        /// <summary>
        /// Registers the foreground dependency property
        /// </summary>
        //   public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Color), typeof(SpanAdv), new PropertyMetadata(Colors.Black, OnForegroundPropertyChanged));

        /// <summary>
        /// Gets or Sets the font family
        /// </summary>
        public FontFamily FontFamily
        {
            get
            {
                if (fontFamily == null)
                {
                    if (this.Paragraph !=null && this.Paragraph.layoutViewer != null)
                    {
                        return this.Paragraph.layoutViewer.OwnerControl.FontFamily;
                    }
                    else
                    {
                        return new FontFamily("Verdana"); 
                    }
                }
                else
                {
                    return fontFamily;
                }
            }
            set
            {
                // SetValue(FontFamilyProperty, value);
                fontFamily = value;

                foreach (ElementBox element in ElementBoxes)
                {
                    TextElementBox textElement = (TextElementBox)element;
                    textElement.FontFamily = value;
                    textElement.MeasureSize();
                }
            }
        }

        /// <summary>
        /// Registers the FontFamily dependency property
        /// </summary>
        //  public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(SpanAdv), new PropertyMetadata(new FontFamily("Verdana"), OnFontFamilyChanged));

        /// <summary>
        /// Gets or Sets the baseline
        /// </summary>
        public Baseline Baseline
        {
            get
            {
                // return (Baseline)GetValue(BaselineProperty);
                return baseline;
            }
            set
            {
                // SetValue(BaselineProperty, value);
                baseline = value;
            }
        }

        /// <summary>
        /// Registers the baseline dependency property
        /// </summary>
        //    public static readonly DependencyProperty BaselineProperty = DependencyProperty.Register("Baseline", typeof(Baseline), typeof(SpanAdv), new PropertyMetadata(Baseline.Normal));

        /// <summary>
        /// Gets a value indicating whether the text is Superscript
        /// </summary>
        public bool IsSuperscript
        {
            get
            {
                return this.Baseline == Baseline.Superscript;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the text is Subscript
        /// </summary>
        public bool IsSubScript
        {
            get
            {
                return this.Baseline == Baseline.Subscript;
            }
        }

        /// <summary>
        /// Sets the value for Parent Span property
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="span"></param>
        public static void SetParentSpan(UIElement uiElement, SpanAdv span)
        {
            if (uiElement == null)
            {
                throw new ArgumentNullException("uiElement");
            }

            uiElement.SetValue(ParentSpanProperty, span);
        }

        protected override void OnTextChanged()
        {
            text = InternalText;
        }

        /// <summary>
        /// Gets the Parent Span
        /// </summary>
        /// <param name="uiElement"></param>
        /// <returns></returns>
        public static SpanAdv GetParentSpan(UIElement uiElement)
        {
            if (uiElement == null)
            {
                throw new ArgumentNullException("uiElement");
            }

            return (SpanAdv)uiElement.GetValue(ParentSpanProperty);
        }

        /// <summary>
        /// Registers the attached property ParentSpan
        /// </summary>
        public static readonly DependencyProperty ParentSpanProperty = DependencyProperty.RegisterAttached("ParentSpan", typeof(SpanAdv), typeof(SpanAdv), null);

        #endregion

        #region methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        internal override void MeasureElements()
        {
            ElementBoxes.Clear();
            List<string> words = SpanAdv.SplitInToWord(Text);

            foreach (string word in words)
            {
                TextElementBox textElementBox = new TextElementBox();

                textElementBox.Baseline = Baseline;
                textElementBox.Foreground = Foreground;
#if WPF
                textElementBox.FontSize = (FontSize * 96) / 72;
#endif
#if !WPF
                textElementBox.FontSize = (FontSize * 96)/72;
#endif
                textElementBox.FontWeight = FontWeight;
                textElementBox.FontStyle = FontStyle;
                textElementBox.FontFamily = FontFamily;
                textElementBox.Text = word;
                textElementBox.Inline = this;
                ElementBoxes.Add(textElementBox);

            }
        }

        internal override Inline CreatInline()
        {
            SpanAdv span = new SpanAdv();
            if (fontFamily != null)
                span.fontFamily = fontFamily;
            span.fontSize = fontSize;
            span.fontStyle = fontStyle;
            span.fontWeight = fontWeight;
            span.foreground = foreground;
            span.baseline = baseline;
            //span.Paragraph = Paragraph;
            span.highlightColor = highlightColor;
            span.strikeThrough = strikeThrough;
            span.underline = underline;
            return span;
        }

        /// <summary>
        /// Copies properties to specified span 
        /// </summary>
        /// <param name="span"></param>
        public void Clone(SpanAdv span)
        {
            span.FontFamily = FontFamily;
            span.FontSize = FontSize;
            span.FontStyle = FontStyle;
            span.FontWeight = FontWeight;
            span.Foreground = Foreground;
            span.Baseline = Baseline;
            //span.Paragraph = Paragraph;
            span.HighlightColor = HighlightColor;
            span.StrikeThrough = StrikeThrough;
            span.Underline = Underline;
        }

        /// <summary>
        /// Returns the length of the inline
        /// </summary>
        /// <returns></returns>
        public override double GetLength()
        {
            return Text.Length;
        }

        /// <summary>
        /// Splits the text into words
        /// </summary>
        public static List<string> SplitInToWord(string text)
        {
            char[] strToChar = text.ToCharArray();
            List<string> words = new List<string>();
            string str = string.Empty;
            foreach (char c in strToChar)
            {
                if (c.Equals(' '))
                {
                    if (str != string.Empty)
                        words.Add(str);
                    words.Add(" ");
                    str = string.Empty;
                }
                else
                {
                    str = str + c.ToString();
                }
            }

            if (str != string.Empty)
            {
                words.Add(str);
            }

            return words;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="words"></param>
        /// <param name="tillIndex"></param>
        internal static double GetSumOfTheWords(List<string> words, double tillIndex)
        {
            double sumOfWords = 0;

            for (int i = 0; i < tillIndex; i++)
            {
                sumOfWords = sumOfWords + words[i].Length;
            }

            return sumOfWords;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="words"></param>
        /// <param name="tillIndex"></param>
        internal static double GetSumOfTheWords(List<ElementBox> elementBox, double tillIndex)
        {
            double sumOfWords = 0;

            for (int i = 0; i < tillIndex; i++)
            {
                sumOfWords = sumOfWords + elementBox[i].InternalText.Length;
            }

            return sumOfWords;
        }

        /// <summary>
        /// 
        /// </summary>
        internal int GetStartIndexOfTheElement(ElementBox element)
        {
            int index = 0;
            foreach (TextElementBox elementBox in ElementBoxes)
            {
                if (elementBox != (element as TextElementBox))
                {
                    index = index + elementBox.Text.Length;
                }
                else
                {
                    break;
                }
            }

            return index;
        }

        internal static SpanAdv CreateNewSpan(InlineStyle style)
        {
            SpanAdv span = new SpanAdv();

            span.Foreground = style.Foreground;
            span.StrikeThrough = style.StrikeThrough;
            span.Underline = style.IsUnderline;
            span.HighlightColor = style.HighlightColor;
            span.FontStyle = style.FontStyle;
            span.FontWeight = style.FontWeight;
            span.FontSize = style.FontSize;
            span.FontFamily = style.FontFamily;
            span.Baseline = style.Baseline;

            return span;
        }

        /// <summary>
        /// Creates the text block 
        /// </summary>
        /// <returns></returns>
        internal TextElementBox CreateTextElement(bool flag)
        {
            TextElementBox textElement = new TextElementBox();
            textElement.FontStyle = FontStyle;
            textElement.Foreground = Foreground;
            textElement.FontWeight = FontWeight;
            textElement.FontSize = (FontSize * 96) / 72;
            textElement.FontFamily = FontFamily;
            textElement.Baseline = Baseline;
            textElement.Inline = this;
            if (flag)
            {
                ElementBoxes.Add(textElement);
            }
            return textElement;
        }

        /// <summary>
        /// Returns the current span
        /// </summary>
        /// <returns></returns>
        public static SpanAdv GetCurrentSpan()
        {
            return new SpanAdv();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnTextPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnFontStylePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SpanAdv span = (SpanAdv)dependencyObject;
            span.OnFontStylePropertyChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnFontStylePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            foreach (ElementBox element in ElementBoxes)
            {
                TextElementBox textElement = (TextElementBox)element;
                textElement.FontStyle = (FontStyle)args.NewValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnFontFamilyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SpanAdv span = (SpanAdv)dependencyObject;
            span.OnFontFamilyChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnFontFamilyChanged(DependencyPropertyChangedEventArgs args)
        {
            foreach (ElementBox element in ElementBoxes)
            {
                TextElementBox textElement = (TextElementBox)element;
                textElement.FontFamily = (FontFamily)args.NewValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnFontSizeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SpanAdv span = (SpanAdv)dependencyObject;
            span.OnFontSizeChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnFontSizeChanged(DependencyPropertyChangedEventArgs args)
        {
            foreach (ElementBox element in ElementBoxes)
            {
                TextElementBox textElement = (TextElementBox)element;
                textElement.FontSize = (double)args.NewValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnFontWeightPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SpanAdv span = (SpanAdv)dependencyObject;
            span.OnFontWeightPropertyChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnFontWeightPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            foreach (ElementBox element in ElementBoxes)
            {
                TextElementBox textElement = (TextElementBox)element;
                textElement.FontWeight = (FontWeight)args.NewValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        private static void OnForegroundPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SpanAdv span = (SpanAdv)dependencyObject;
            span.OnForegroundPropertyChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            foreach (ElementBox element in ElementBoxes)
            {
                TextElementBox elementBox = element as TextElementBox;
                if (elementBox != null)
                {
                    elementBox.Foreground = (Color)args.NewValue;
                }
            }
        }

        public SpanAdv CreateNewSpan()
        {
            SpanAdv span = new SpanAdv();
            span.baseline = baseline;
            if (fontFamily != null)
                span.fontFamily = fontFamily;
            span.fontSize = fontSize;
            span.fontStyle = fontStyle;
            span.fontWeight = fontWeight;
            span.foreground = foreground;
            span.highlightColor = highlightColor;
            span.strikeThrough = strikeThrough;
            span.underline = underline;

            return span;
        }

        #endregion
    }

    [ContentProperty("UIElement")]
    public class UIContainerAdv : Inline
    {
        private UIElementBox elementBox;

        /// <summary>
        /// Initializes the new instance of richEditUIContainer class
        /// </summary>
        public UIContainerAdv()
        {
            ElementBoxes = new List<ElementBox>();
            IsUIContainer = true;
        }

        /// <summary>
        /// Gets or Sets the UIelement
        /// </summary>
        public UIElement UIElement
        {
            get
            {
                return (UIElement)GetValue(UIElementProperty);
            }
            set
            {
                SetValue(UIElementProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the Element Box
        /// </summary>
        internal override List<ElementBox> ElementBoxes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the width of the image
        /// </summary>
        public double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }
            set
            {
                SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(UIContainerAdv), new PropertyMetadata(OnWidthChanged));

        /// <summary>
        /// Gets or Sets the FitToContent
        /// </summary>
        public bool FitToContent
        {
            get
            {
                return (bool)GetValue(FitToContentProperty);
            }
            set
            {
                SetValue(FitToContentProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FitToContentProperty = DependencyProperty.Register("FitToContent", typeof(bool), typeof(UIContainerAdv), null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnWidthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            UIContainerAdv richTextBox = (UIContainerAdv)dependencyObject;
            richTextBox.OnWidthChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (elementBox != null)
            {
                elementBox.Width = Width;
            }
        }

        /// <summary>
        /// Gets or Sets the height of the image
        /// </summary>
        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(UIContainerAdv), new PropertyMetadata(OnHeightChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnHeightChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            UIContainerAdv richTextBox = (UIContainerAdv)dependencyObject;
            richTextBox.OnHeightChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (elementBox != null)
            {
                elementBox.Height = Height;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal UIElementBox ElementBox
        {
            get
            {
                return elementBox;
            }
            set
            {
                elementBox = value;
            }
        }

        /// <summary>
        /// Registers the UIElement dependency property
        /// </summary>
        public static readonly DependencyProperty UIElementProperty = DependencyProperty.Register("UIElement", typeof(UIElement), typeof(UIContainerAdv), new PropertyMetadata(OnUIElementPropertyChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnUIElementPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        internal override void MeasureElements()
        {
            ElementBoxes.Clear();

            if (ElementBox == null)
            {
                ElementBox = new UIElementBox();
                ElementBox.Width = Width + 2;
                ElementBox.Height = Height + 2;
                ElementBox.UIElement = UIElement;
            }

            ElementBox.Inline = this;

            if (!ElementBoxes.Contains(ElementBox))
            {
                ElementBoxes.Add(ElementBox);
            }
        }

        /// <summary>
        /// Return the length of the inline
        /// </summary>
        /// <returns></returns>
        public override double GetLength()
        {
            return 1;
        }
    }

    [ContentProperty("Text")]
    public class HyperlinkAdv : Inline
    {
        private string text;
        private double fontSize = 11d;
        private FontFamily fontFamily = new FontFamily("Verdana");
        private Baseline baseline = Baseline.Normal;
        private FontStyle fontStyle = FontStyles.Normal;
        private FontWeight fontWeight = FontWeights.Normal;
        private Color highlightColor;
        private StrikeThrough strikeThrough = StrikeThrough.None;
        private bool underline = true;
        private Color foreground = Color.FromArgb(255, 0, 0, 255);
        private string navigationUrl;
        private HyperlinkTargetType hyperlinkType = HyperlinkTargetType.Self;

        /// <summary>
        /// Initializes the new instance of RichEditHyperlink class
        /// </summary>
        public HyperlinkAdv()
        {
            ElementBoxes = new List<ElementBox>();
            DecorativeElements = new List<UIElement>();
        }

        /// <summary>
        /// Gets or Sets the decorative elements
        /// </summary>
        internal List<UIElement> DecorativeElements
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the Element Box
        /// </summary>
        internal override List<ElementBox> ElementBoxes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the highlight color
        /// </summary>
        public Color HighlightColor
        {
            get
            {
                // return (Color)GetValue(HighlightColorProperty);
                return highlightColor;
            }
            set
            {
                //SetValue(HighlightColorProperty, value);
                highlightColor = value;
            }
        }

        /// <summary>
        /// Registers the highlight color dependency property
        /// </summary>
        //     public static readonly DependencyProperty HighlightColorProperty = DependencyProperty.Register("HighlightColor", typeof(Color), typeof(HyperlinkAdv), null);

        /// <summary>
        /// Gets or Sets the font style
        /// </summary>
        public FontWeight FontWeight
        {
            get
            {
                // return (FontWeight)GetValue(FontWeightProperty);
                return fontWeight;
            }
            set
            {
                // SetValue(FontWeightProperty, value);
                fontWeight = value;
                foreach (HyperlinkElementBox elementBox in ElementBoxes)
                {
                    elementBox.FontWeight = value;
                    elementBox.MeasureSize();
                }
            }
        }

        /// <summary>
        /// Registers the FontWeight dependency property
        /// </summary>
        //    public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(HyperlinkAdv), null);

        /// <summary>
        /// Gets or Sets the font style
        /// </summary>
        public FontStyle FontStyle
        {
            get
            {
                // return (FontStyle)GetValue(FontStyleProperty);
                return fontStyle;
            }
            set
            {
                //SetValue(FontStyleProperty, value);
                fontStyle = value;
                foreach (HyperlinkElementBox elementBox in ElementBoxes)
                {
                    elementBox.FontStyle = value;
                    elementBox.MeasureSize();
                }
            }
        }

        /// <summary>
        /// Registers the FontStyle dependency property
        /// </summary>
        //     public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(HyperlinkAdv), null);

        /// <summary>
        /// Gets or Sets the font size. Reffered in pixels
        /// </summary>
        public double FontSize
        {
            get
            {
                // return (double)GetValue(FontSizeProperty);
                return fontSize;
            }
            set
            {
                //SetValue(FontSizeProperty, value);
                fontSize = value;
                foreach (HyperlinkElementBox elementBox in ElementBoxes)
                {
                    elementBox.FontSize = (value * 96) / 72;
                    elementBox.MeasureSize();
                }
            }
        }

        /// <summary>
        /// Registers the FontSize dependency property
        /// </summary>
        //     public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(HyperlinkAdv), new PropertyMetadata(11d));

        /// <summary>
        /// Gets or Sets the font family
        /// </summary>
        public FontFamily FontFamily
        {
            get
            {
                // return (FontFamily)GetValue(FontFamilyProperty);
                return fontFamily;
            }
            set
            {
                //SetValue(FontFamilyProperty, value);
                fontFamily = value;
                foreach (HyperlinkElementBox elementBox in ElementBoxes)
                {
                    elementBox.FontFamily = value;
                    elementBox.MeasureSize();
                }
            }
        }

        /// <summary>
        /// Registers the FontFamily dependency property
        /// </summary>
        //     public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(HyperlinkAdv), new PropertyMetadata(new FontFamily("Verdana")));

        /// <summary>
        /// Gets or Sets the baseline
        /// </summary>
        public Baseline Baseline
        {
            get
            {
                //return (Baseline)GetValue(BaselineProperty);
                return baseline;
            }
            set
            {
                //SetValue(BaselineProperty, value);
                baseline = value;
            }
        }

        /// <summary>
        /// Registers the baseline dependency property
        /// </summary>
        //     public static readonly DependencyProperty BaselineProperty = DependencyProperty.Register("Baseline", typeof(Baseline), typeof(HyperlinkAdv), new PropertyMetadata(Baseline.Normal));

        /// <summary>
        /// Gets a value indicating whether the text is Superscript
        /// </summary>
        public bool IsSuperscript
        {
            get
            {
                return this.Baseline == Baseline.Superscript;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the text is Subscript
        /// </summary>
        public bool IsSubScript
        {
            get
            {
                return this.Baseline == Baseline.Subscript;
            }
        }

        /// <summary>
        /// Gets or Sets the Strike through
        /// </summary>
        public StrikeThrough StrikeThrough
        {
            get
            {
                //return (StrikeThrough)GetValue(StrikeThroughProperty);
                return strikeThrough;
            }
            set
            {
                //SetValue(StrikeThroughProperty, value);
                strikeThrough = value;
            }
        }

        /// <summary>
        /// Registers the Strike through dependency property
        /// </summary>
        //    public static readonly DependencyProperty StrikeThroughProperty = DependencyProperty.Register("StrikeThrough", typeof(StrikeThrough), typeof(HyperlinkAdv), new PropertyMetadata(StrikeThrough.None));

        /// <summary>
        /// Gets or Sets the under line
        /// </summary>
        public bool Underline
        {
            get
            {
                //return (bool)GetValue(UnderlineProperty);
                return underline;
            }
            set
            {
                //SetValue(UnderlineProperty, value);
                underline = value;
            }
        }

        /// <summary>
        /// Registers the Underline dependency property
        /// </summary>
        //    public static readonly DependencyProperty UnderlineProperty = DependencyProperty.Register("Underline", typeof(bool), typeof(HyperlinkAdv), new PropertyMetadata(true));

        /// <summary>
        /// Gets or Sets the text
        /// </summary>
        public string Text
        {
            get
            {
                // return (string)GetValue(TextProperty);
                return text;
            }
            set
            {
                // SetValue(TextProperty, value);
                text = value;
                SetText(value);
            }
        }

        /// <summary>
        /// Registers the Text dependency property
        /// </summary>
        //  public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(HyperlinkAdv), new PropertyMetadata(OnTextPropertyChanged));

        /// <summary>
        /// Gets or Sets the Hyperlink Foreground
        /// </summary>
        public Color Foreground
        {
            get
            {
                // return (Color)GetValue(ForegroundProperty);
                return foreground;
            }
            set
            {
                //SetValue(ForegroundProperty, value);
                foreground = value;
                foreach (HyperlinkElementBox elementBox in ElementBoxes)
                {
                    elementBox.Foreground = value;
                }
            }
        }

        /// <summary>
        /// Registers the HyperlinkForeground dependency property
        /// </summary>
        //   public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Color), typeof(HyperlinkAdv), new PropertyMetadata(Color.FromArgb(255,0,0,255)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnTextPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// Gets or Sets the navigation url
        /// </summary>
        public string NavigationUrl
        {
            get
            {
                //return (string)GetValue(NavigationUrlProperty);
                return navigationUrl;
            }
            set
            {
                //SetValue(NavigationUrlProperty, value);
                navigationUrl = ValidateUrl(value);
            }
        }

        /// <summary>
        /// Registers the NavigationUrl dependency property
        /// </summary>
        //    public static readonly DependencyProperty NavigationUrlProperty = DependencyProperty.Register("NavigationUrl", typeof(string), typeof(HyperlinkAdv), null);

#if !WPF
        /// <summary>
        /// Gets or Sets the HyperlinkTargetType
        /// </summary>
        public HyperlinkTargetType TargetType
        {
            get
            {
                // return (HyperlinkTargetType)GetValue(TargetTypeProperty);
                return hyperlinkType;
            }
            set
            {
                // SetValue(TargetTypeProperty, value);
                hyperlinkType = value;
            }
        }
#endif
        /// <summary>
        /// Registers the TargetTypeProperty dependency property
        /// </summary>
        //      public static readonly DependencyProperty TargetTypeProperty = DependencyProperty.Register("TargetType", typeof(HyperlinkTargetType), typeof(HyperlinkAdv), new PropertyMetadata(HyperlinkTargetType.Self));

        /// <summary>
        /// Splits the words in to word and creates UIElement
        /// </summary>
        /// <param name="size"></param>
        internal override void MeasureElements()
        {
            ElementBoxes.Clear();

            List<string> Words = SpanAdv.SplitInToWord(Text);
            foreach (string word in Words)
            {
                HyperlinkElementBox hyperlink = new HyperlinkElementBox();
                hyperlink.Foreground = Foreground;
                hyperlink.Baseline = Baseline;
                hyperlink.Inline = this;
                hyperlink.NavigationUrl = NavigationUrl;
                hyperlink.FontWeight = FontWeight;
                hyperlink.FontStyle = FontStyle;
                hyperlink.FontFamily = FontFamily;
                hyperlink.FontSize = (FontSize * 96) / 72;
                hyperlink.Text = word;
#if !WPF
                hyperlink.HyperlinkTargetType = TargetType;
#endif
                ElementBoxes.Add(hyperlink);
            }
        }
        /// <summary>
        /// Validates the URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        private string ValidateUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                url = url.Trim();
                string urlLower = url.ToLower();
                if (!urlLower.StartsWith("http://") && !urlLower.StartsWith("https://"))
                {
                    if (urlLower.StartsWith("www."))
                        url = "http://" + url;
                    else if (!urlLower.StartsWith("mailto:") && url.Contains("@"))
                        url = "mailto:" + url;
                }
            }
            return url;
        }

        protected override void OnTextChanged()
        {
            text = InternalText;
        }

        /// <summary>
        /// Creates the text block 
        /// </summary>
        /// <returns></returns>
        internal HyperlinkElementBox CreateTextElement(bool flag)
        {
            HyperlinkElementBox textElement = new HyperlinkElementBox();
            textElement.FontStyle = FontStyle;
            textElement.Foreground = Foreground;
            textElement.FontWeight = FontWeight;
            textElement.FontSize = (FontSize * 96 )/ 72;
            textElement.FontFamily = FontFamily;
            textElement.Baseline = Baseline;
            textElement.Inline = this;

            if (flag)
            {
                ElementBoxes.Add(textElement);
            }
            return textElement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hyperlink"></param>
        internal void Clone(HyperlinkAdv hyperlink)
        {
            hyperlink.Foreground = Foreground;
            hyperlink.StrikeThrough = StrikeThrough;
#if !WPF
            hyperlink.TargetType = TargetType;
#endif
            hyperlink.Underline = Underline;
            hyperlink.NavigationUrl = NavigationUrl;
            hyperlink.HighlightColor = HighlightColor;
            hyperlink.FontStyle = FontStyle;
            hyperlink.FontWeight = FontWeight;
            hyperlink.FontSize = FontSize;
            hyperlink.FontFamily = FontFamily;
            hyperlink.Baseline = Baseline;
        }

        internal override Inline CreatInline()
        {
            HyperlinkAdv hyperlink = new HyperlinkAdv();
            hyperlink.Foreground = Foreground;
            hyperlink.StrikeThrough = StrikeThrough;
#if !WPF
            hyperlink.TargetType = TargetType;
#endif
            hyperlink.Underline = Underline;
            hyperlink.NavigationUrl = NavigationUrl;
            hyperlink.HighlightColor = HighlightColor;
            hyperlink.FontStyle = FontStyle;
            hyperlink.FontWeight = FontWeight;
            hyperlink.FontSize = FontSize;
            hyperlink.FontFamily = FontFamily;
            hyperlink.Baseline = Baseline;
            return hyperlink;
        }

        /// <summary>
        /// Return the length of the inline
        /// </summary>
        /// <returns></returns>
        public override double GetLength()
        {
            return Text.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HyperlinkAdv CreateNewHyperlink()
        {
            HyperlinkAdv hyperlink = new HyperlinkAdv();

            hyperlink.Foreground = Foreground;
            hyperlink.StrikeThrough = StrikeThrough;
#if !WPF
            hyperlink.TargetType = TargetType;
#endif
            hyperlink.Underline = Underline;
            hyperlink.NavigationUrl = NavigationUrl;
            hyperlink.HighlightColor = HighlightColor;
            hyperlink.FontStyle = FontStyle;
            hyperlink.FontWeight = FontWeight;
            hyperlink.FontSize = FontSize;
            hyperlink.FontFamily = FontFamily;
            hyperlink.Baseline = Baseline;

            return hyperlink;
        }
    }

    public class ImageContainerAdv : Inline
    {
        private ImageElementBox elementBox;

        /// <summary>
        /// Initializes the new instance of ImageContainerAdv class
        /// </summary>
        public ImageContainerAdv()
        {
            ElementBoxes = new List<ElementBox>();
            IsImageContainer = true;
        }

        /// <summary>
        /// Gets or Sets the image source
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)GetValue(ImageSourceProperty);
            }
            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageContainerAdv), new PropertyMetadata(null,new PropertyChangedCallback(OnImageSourceChanged)));
        

        public static readonly DependencyProperty ImageStringProperty = DependencyProperty.Register("ImageString", typeof(string), typeof(ImageContainerAdv), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnImageStringChanged)));

        public string ImageString
        {
            get
            {
                return (string)GetValue(ImageStringProperty);
            }
            set
            {
                SetValue(ImageStringProperty, value);
            }
        }


        private static void OnImageStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageContainerAdv imageContainer = d as ImageContainerAdv;

            if (e.NewValue != null)
            {
                string imageString = e.NewValue.ToString();

                byte[] btyeArr = Convert.FromBase64String(imageString);

                imageContainer.ImageBytes = btyeArr;

                var bmp = new BitmapImage();
#if WPF
                bmp.BeginInit();
#endif
                bmp.SetSource(new MemoryStream(btyeArr));
#if WPF
                bmp.EndInit();
#endif
                imageContainer.ImageSource = bmp;
                imageContainer.Width = bmp.PixelWidth;
                imageContainer.Height = bmp.PixelHeight;

            }
        }


        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageContainerAdv imagecontainer = d as ImageContainerAdv;
            if (e.NewValue != null)
            {
#if WPF
                string location = e.NewValue.ToString();
                if (location.Contains("file:"))
                {
                    imagecontainer.ImageBytes = File.ReadAllBytes(location.Replace("file:///",""));
                }
                else if (location.Contains("pack://application:,,,"))
                {
                    Uri uri = new Uri(location, UriKind.RelativeOrAbsolute);
                    if (Uri.IsWellFormedUriString(uri.OriginalString, UriKind.RelativeOrAbsolute))
                    {
                        StreamResourceInfo streaminfo = Application.GetResourceStream(uri);
                        imagecontainer.ImageBytes = streaminfo.Stream.GetBytes();
                    }
                }
#else
                if (e.NewValue is BitmapImage)
                {
                    if ((e.NewValue as BitmapImage).UriSource != null && !string.IsNullOrEmpty((e.NewValue as BitmapImage).UriSource.OriginalString)
                        && Uri.IsWellFormedUriString((e.NewValue as BitmapImage).UriSource.OriginalString, UriKind.RelativeOrAbsolute))
                    {
                        Uri uri = new Uri((e.NewValue as BitmapImage).UriSource.OriginalString, UriKind.RelativeOrAbsolute);
                        if (!uri.IsAbsoluteUri)
                        {
                            Stream stream = Application.GetResourceStream(uri).Stream;
                            if (stream != null)
                                imagecontainer.ImageBytes = stream.GetBytes();
                        }
                    }
                }
#endif
            }
        }

        /// <summary>
        /// Gets or Sets the FitToContent
        /// </summary>
        public bool FitToContent
        {
            get
            {
                return (bool)GetValue(FitToContentProperty);
            }
            set
            {
                SetValue(FitToContentProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FitToContentProperty = DependencyProperty.Register("FitToContent", typeof(bool), typeof(ImageContainerAdv), null);

        /// <summary>
        /// Gets or Sets the width of the image
        /// </summary>
        public double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }
            set
            {
                SetValue(WidthProperty, value);
            }
        }

        public byte[] ImageBytes
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(ImageContainerAdv), new PropertyMetadata(OnWidthChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnWidthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            ImageContainerAdv richTextBox = (ImageContainerAdv)dependencyObject;
            richTextBox.OnWidthChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (elementBox != null)
            {
                elementBox.Width = Width;
            }
        }

        /// <summary>
        /// Gets or Sets the height of the image
        /// </summary>
        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(ImageContainerAdv), new PropertyMetadata(OnHeightChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnHeightChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            ImageContainerAdv richTextBox = (ImageContainerAdv)dependencyObject;
            richTextBox.OnHeightChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (elementBox != null)
            {
                elementBox.Height = Height;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ImageElementBox ElementBox
        {
            get
            {
                return elementBox;
            }
            set
            {
                elementBox = value;
                //elementBox.Element.MouseLeftButtonDown += new MouseButtonEventHandler(Element_MouseLeftButtonDown);
            }
        }

        internal override Inline CreatInline()
        {
            ImageContainerAdv image = new ImageContainerAdv();
            image.ImageSource = ImageSource;
            image.ImageBytes = ImageBytes;
            image.Width = Width;
            image.Height = Height;
            image.FitToContent = FitToContent;
            return image;
        }

        internal void SelectElement()
        {
            ImageResizer imageResizer = Paragraph.LayoutViewer.ImageResizer;
            //if (imageResizer.IsResizable)
            //{
                Canvas canvas = elementBox.Element.Parent as Canvas;
                imageResizer.OwnerControl.Viewer.SelectedImage = elementBox.Inline as ImageContainerAdv;
                if (canvas != null && !canvas.Children.Contains(imageResizer))
                {
                    if (imageResizer.Parent != null)
                    {
                        (imageResizer.Parent as Canvas).Children.Remove(imageResizer);
                    }
                    canvas.Children.Add(imageResizer);
                }
                imageResizer.OwnerControl.Viewer.IsImageResizerSelected = true;
                imageResizer.ImageSource = ImageSource;
                imageResizer.ImageWidth = Width;
                imageResizer.ImageHeight = Height;
                imageResizer.Left = Canvas.GetLeft(ElementBox.Element);
                imageResizer.Top = Canvas.GetTop(ElementBox.Element);
                imageResizer.ImageContainer = this;
                imageResizer.Visibility = Visibility.Visible;
                imageResizer.OwnerControl.Viewer.SelectImage(imageResizer.OwnerControl.Viewer.SelectedImage);
            //}
            //else
            //{
            //    imageResizer.Visibility = Visibility.Collapsed;
            //    imageResizer.OwnerControl.Viewer.SelectedImage = null;
            //    imageResizer.OwnerControl.Viewer.IsImageResizerSelected = false;
            //}
        }

        /// <summary>
        /// Return the length of the inline
        /// </summary>
        /// <returns></returns>
        public override double GetLength()
        {
            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void MeasureElements()
        {
            ElementBoxes.Clear();

            if (ElementBox == null)
            {
                ElementBox = new ImageElementBox();
                ElementBox.Width = Width + 2;
                ElementBox.Height = Height + 2;
                ElementBox.ImageSource = ImageSource;
            }

            ElementBox.Inline = this;

            if (!ElementBoxes.Contains(ElementBox))
            {
                ElementBoxes.Add(ElementBox);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override List<ElementBox> ElementBoxes
        {
            get;
            set;
        }

        internal Stream Load(Uri uri)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string filePath = string.Format(CultureInfo.CurrentCulture, "{0}." + uri.ToString().Replace("/", "."), assembly.FullName.Split(new char[] { ',' })[0]);
            Stream fileStream = assembly.GetManifestResourceStream(filePath);

            if (fileStream == null)
                throw new InvalidOperationException("Resource not found exception");
            else
                return fileStream;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum Baseline
    {
        /// <summary>
        /// Specifies whether the text to be rendered normally
        /// </summary>
        Normal,

        /// <summary>
        /// Specifies whether text should appear above the baseline of text
        /// </summary>
        Superscript,

        /// <summary>
        /// Specifies whether text should appear below the baseline of text
        /// </summary>
        Subscript
    }

    /// <summary>
    /// Specifies hyperlink target type
    /// </summary>
    public enum HyperlinkTargetType
    {
        /// <summary>
        /// Opens the link in new window
        /// </summary>
        Blank,

        /// <summary>
        /// Opens the link in the same window or tab
        /// </summary>
        Self
    }
}
;