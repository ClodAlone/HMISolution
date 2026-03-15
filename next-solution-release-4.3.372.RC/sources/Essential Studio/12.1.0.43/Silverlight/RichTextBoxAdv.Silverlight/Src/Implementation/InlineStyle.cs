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
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    public class InlineStyle :DependencyObject,INotifyPropertyChanged
    {
        private RichTextBoxAdv richTextBox;
        private double fontSize = 11d;
        private FontFamily fontFamily = new FontFamily("Verdana");
        private FontWeight fontWeight;
        private FontStyle fontStyle;
        private bool isUnderline = false;
        private StrikeThrough strikeThrough = StrikeThrough.None;
        private Baseline baseLine = Baseline.Normal;
        private Color highlightColor;
        private Color foreground = Colors.Black;
        bool isStyleChanged = false;
        private Type inlineType;

        /// <summary>
        /// Initializes the new instance of InlineStyle class
        /// </summary>
        /// <param name="richTextBoxAdv"></param>
        public InlineStyle(RichTextBoxAdv richTextBoxAdv)
        {
            richTextBox = richTextBoxAdv;
        }


        /// <summary>
        /// Indicates whether the current inline is superscript
        /// </summary>
        public bool IsSuperscript
        {
            get
            {
                return baseLine == Baseline.Superscript;
            }
        }

        /// <summary>
        /// Indicates whether the current inline is subscript
        /// </summary>
        public bool IsSubscript
        {
            get
            {
                return baseLine == Baseline.Subscript;
            }
        }

        /// <summary>
        /// Gets the type of the current inline
        /// </summary>
        public Type InlineType
        {
            get
            {
                return inlineType;
            }
            internal set
            {
                inlineType = value;
            }
        }

        /// <summary>
        /// Indicates whether the current inline is hyperlink
        /// </summary>
        public bool IsHyperlink
        {
            get
            {
                if (inlineType != null)
                {
                    return inlineType == typeof(HyperlinkAdv);
                }

                return false;
            }
        }

        /// <summary>
        /// Indicates whether the current inline is SpanAdv
        /// </summary>
        public bool IsSpan
        {
            get
            {
                if (inlineType != null)
                {
                    return inlineType == typeof(SpanAdv);
                }

                return false;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public RichTextBoxAdv RichTextBox
        {
            get
            {
                return richTextBox;
            }
        }

#if !WPF
        /// <summary>
        /// Gets the font size of the current inline
        /// </summary>
        public double FontSize
        {
            get
            {
                return fontSize; 
            }
            internal set
            {
                isStyleChanged = fontSize != value;
                fontSize = value;
                OnPropertyChanged("FontSize");
                OnStyleChanged();
            }
        }

        /// <summary>
        /// Gets the font family of the current inline
        /// </summary>
        public FontFamily FontFamily
        {
            get
            {
                return fontFamily;
            }
            internal set
            {
                isStyleChanged = fontFamily!=null? fontFamily.Source != value.Source: true;
                fontFamily = value;
                OnPropertyChanged("FontFamily");
                OnStyleChanged();
            }
        }

        /// <summary>
        /// Gets the font weight of the current inline
        /// </summary>
        public FontWeight FontWeight
        {
            get
            {
                return fontWeight;
            }
            set
            {
                isStyleChanged = fontWeight != value;
                fontWeight = value;
                OnPropertyChanged("FontWeight");
                OnStyleChanged();
            }
        }

        /// <summary>
        /// Gets the font style of the current inline
        /// </summary>
        public FontStyle FontStyle
        {
            get
            {
                return fontStyle;
            }
            internal set
            {
                isStyleChanged = fontStyle != value;
                fontStyle = value;
                OnPropertyChanged("FontStyle");
                OnStyleChanged();
            }
        }
        
        /// <summary>
        /// Gets the foreground color of the current inline
        /// </summary>
        public Color Foreground
        {
            get
            {
                return foreground;
            }
            internal set
            {
                //isStyleChanged = foreground.A != value.A && foreground.B != value.B && foreground.G !=value.G && foreground.R != value.R;
                isStyleChanged = foreground != value;
                foreground = value;
                OnPropertyChanged("Foreground");
                OnStyleChanged();
            }
        }

        /// <summary>
        /// Gets the highlight color of the current inline
        /// </summary>
        public Color HighlightColor
        {
            get
            {
                return highlightColor;
            }
            internal set
            {
                //isStyleChanged = highlightColor.A == value.A && highlightColor.B == value.B && highlightColor.G == value.G && highlightColor.R == value.R;
                isStyleChanged = highlightColor != value;
                highlightColor = value;
                OnPropertyChanged("HighlightColor");
                OnStyleChanged();
            }
        }

        /// <summary>
        /// Indicates whether the current inline is underlined
        /// </summary>
        public bool IsUnderline
        {
            get
            {
                return isUnderline;
            }
            internal set
            {
                isStyleChanged = isUnderline != value;
                isUnderline = value;
                OnPropertyChanged("IsUnderline");
                OnStyleChanged();
            }
        }

        /// <summary>
        /// Gets the StrikeThrough of the current inline
        /// </summary>
        public StrikeThrough StrikeThrough
        {
            get
            {
                return strikeThrough;
            }
            internal set
            {
                isStyleChanged = strikeThrough != value;
                strikeThrough = value;
                OnPropertyChanged("StrikeThrough");
                OnStyleChanged();
            }
        }

        /// <summary>
        /// Gets the baseline of the current inline
        /// </summary>
        public Baseline Baseline
        {
            get
            {
                return baseLine;
            }
            internal set
            {
                isStyleChanged = baseLine != value;
                baseLine = value;
                OnPropertyChanged("Baseline");
                OnStyleChanged();
            }
        }

#else
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            internal set { SetValue(FontFamilyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontFamily.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(InlineStyle), new FrameworkPropertyMetadata(new FontFamily("Verdana"), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnFontFamilyChanged)));



        private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InlineStyle inlinestyle = d as InlineStyle;
            inlinestyle.OnFontFamilyChanged(e);
        }

        private void OnFontFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            FontFamily oldvalue = (FontFamily)e.OldValue;
            isStyleChanged = oldvalue != null ? oldvalue.Source != (e.NewValue as FontFamily).Source : true;
            OnStyleChanged();
        }


        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            internal set { SetValue(FontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontSizeProperty =
           DependencyProperty.Register("FontSize", typeof(double), typeof(InlineStyle), new FrameworkPropertyMetadata(11d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,new PropertyChangedCallback(OnFontSizeChanged)));

        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InlineStyle inlinestyle = d as InlineStyle;
            inlinestyle.OnFontSizeChanged(e);
        }

        private void OnFontSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.OldValue != e.NewValue;
            OnStyleChanged();
        }


        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            internal set { SetValue(FontWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(InlineStyle), new FrameworkPropertyMetadata(FontWeights.Regular,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,new PropertyChangedCallback(OnFontWeightChanged)));


        private static void OnFontWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InlineStyle inlinestyle = d as InlineStyle;
            inlinestyle.OnFontWeightChanged(e);
        }

        private void OnFontWeightChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.OldValue != e.NewValue;
            OnStyleChanged();
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            internal set { SetValue(FontStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(InlineStyle), new FrameworkPropertyMetadata(FontStyles.Normal,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,new PropertyChangedCallback(OnFontStyleChanged)));

        private static void OnFontStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InlineStyle inlinestyle = d as InlineStyle;
            inlinestyle.OnFontStyleChanged(e);
        }

        private void OnFontStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.OldValue != e.NewValue;
            OnStyleChanged();
        }

        public Color Foreground
        {
            get { return (Color)GetValue(ForegroundProperty); }
            internal set { SetValue(ForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Color), typeof(InlineStyle), new FrameworkPropertyMetadata(Colors.Black,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,new PropertyChangedCallback(OnForegroundChanged)));

        private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InlineStyle inlinestyle = d as InlineStyle;
            inlinestyle.OnForegroundChanged(e);
        }

        private void OnForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.OldValue != e.NewValue;
            OnStyleChanged();
        }

        public Color HighlightColor
        {
            get { return (Color)GetValue(HighlightColorProperty); }
            internal set { SetValue(HighlightColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightColorProperty =
            DependencyProperty.Register("HighlightColor", typeof(Color), typeof(InlineStyle), new FrameworkPropertyMetadata(Colors.Transparent,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,new PropertyChangedCallback(OnHighlightColorChanged)));

        private static void OnHighlightColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InlineStyle inlinestyle = d as InlineStyle;
            inlinestyle.OnHighlightColorChanged(e);
        }

        private void OnHighlightColorChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.OldValue != e.NewValue;
            OnStyleChanged();
        }

        public bool IsUnderline
        {
            get { return (bool)GetValue(IsUnderlineProperty); }
            internal set { SetValue(IsUnderlineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsUnderline.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUnderlineProperty =
            DependencyProperty.Register("IsUnderline", typeof(bool), typeof(InlineStyle), new FrameworkPropertyMetadata(false,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,new PropertyChangedCallback(OnIsUnderlineChanged)));

        private static void OnIsUnderlineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InlineStyle inlinestyle = d as InlineStyle;
            inlinestyle.OnIsUnderlineChanged(e);
        }

        private void OnIsUnderlineChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.OldValue != e.NewValue;
            OnStyleChanged();
        }

        public StrikeThrough StrikeThrough
        {
            get { return (StrikeThrough)GetValue(StrikeThroughProperty); }
            internal set { SetValue(StrikeThroughProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrikeThrough.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrikeThroughProperty =
            DependencyProperty.Register("StrikeThrough", typeof(StrikeThrough), typeof(InlineStyle), new FrameworkPropertyMetadata(StrikeThrough.None,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,new PropertyChangedCallback(OnStrikeThroughChanged)));

        private static void OnStrikeThroughChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InlineStyle inlinestyle = d as InlineStyle;
            inlinestyle.OnStrikeThroughChanged(e);
        }

        private void OnStrikeThroughChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.OldValue != e.NewValue;
            OnStyleChanged();
        }

        public Baseline Baseline
        {
            get { return (Baseline)GetValue(BaselineProperty); }
            set { SetValue(BaselineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Baseline.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BaselineProperty =
            DependencyProperty.Register("Baseline", typeof(Baseline), typeof(InlineStyle), new FrameworkPropertyMetadata(Baseline.Normal,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,new PropertyChangedCallback(OnBaselineChanged)));

        private static void OnBaselineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InlineStyle inlinestyle = d as InlineStyle;
            inlinestyle.OnBaselineChanged(e);
        }

        private void OnBaselineChanged(DependencyPropertyChangedEventArgs e)
        {
            isStyleChanged = e.OldValue != e.NewValue;
            OnStyleChanged();
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        private void OnStyleChanged()
        {
            if (isStyleChanged)
            {
                richTextBox.OnStyleChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void SetDefaultStyle()
        {
            Foreground = Colors.Black;
            IsUnderline = false;
            Baseline = Baseline.Normal;
            StrikeThrough = StrikeThrough.None;
            FontWeight = FontWeights.Normal;
            FontStyle = FontStyles.Normal;
            FontSize = 11d;
            HighlightColor = Color.FromArgb(0, 0, 0, 0);
            FontFamily = new FontFamily("Verdana");
        }
        internal void SetCurrentStyle()
        {
            Foreground =((System.Windows.Media.SolidColorBrush)(RichTextBox.Foreground)).Color;
            IsUnderline = false;
            Baseline = Baseline.Normal;
            StrikeThrough = StrikeThrough.None;
            FontWeight = RichTextBox.FontWeight;
            FontStyle = RichTextBox.FontStyle;
            FontSize = RichTextBox.FontSize;
            HighlightColor = Color.FromArgb(0, 0, 0, 0);
            FontFamily = RichTextBox.FontFamily;
        }
        internal SpanAdv GetSpan()
        {
            SpanAdv span = new SpanAdv();
            span.Baseline = Baseline;
            span.FontFamily = FontFamily;
            span.FontSize = FontSize;
            span.FontStyle = FontStyle;
            span.FontWeight = FontWeight;
            span.Foreground = Foreground;
            span.HighlightColor = HighlightColor;
            span.StrikeThrough = StrikeThrough;
            span.Underline = IsUnderline;
            return span;
        }

        public bool IsEqualInStyle(Inline inline)
        {
            if (inline is SpanAdv)
            {
                SpanAdv span1 = inline as SpanAdv;
                InlineStyle span2 = this as InlineStyle;

                return span1.Baseline == span2.Baseline && span1.FontFamily.Source == span2.FontFamily.Source
                    && span1.FontSize == span2.FontSize && span1.FontStyle == span2.FontStyle && span1.FontWeight == span2.FontWeight
                    && span1.Foreground == span2.Foreground && span1.HighlightColor == span2.HighlightColor && span1.StrikeThrough == span2.StrikeThrough
                    && span1.Underline == span2.IsUnderline;
            }
            else if (inline is HyperlinkAdv)
            {
                HyperlinkAdv hyperlink1 = inline as HyperlinkAdv;
                InlineStyle hyperlink2 = this as InlineStyle;
                return hyperlink1.Baseline == hyperlink2.Baseline && hyperlink1.FontFamily.Source == hyperlink2.FontFamily.Source
                    && hyperlink1.FontSize == hyperlink2.FontSize && hyperlink1.FontStyle == hyperlink2.FontStyle && hyperlink1.FontWeight == hyperlink2.FontWeight
                    && hyperlink1.Foreground == hyperlink2.Foreground && hyperlink1.HighlightColor == hyperlink2.HighlightColor && hyperlink1.StrikeThrough == hyperlink2.StrikeThrough
                    && hyperlink1.Underline == hyperlink2.IsUnderline;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inline"></param>
        internal void SetInlineStyle(Inline inline)
        {
            if (inline != null && (IsSpan || IsHyperlink))
            {
                inlineType = inline.GetType();
                FontFamily = IsSpan ? (inline as SpanAdv).FontFamily : (inline as HyperlinkAdv).FontFamily;
                Foreground = IsSpan ? (inline as SpanAdv).Foreground : (inline as HyperlinkAdv).Foreground;
                FontSize = IsSpan ? (inline as SpanAdv).FontSize : (inline as HyperlinkAdv).FontSize;
                StrikeThrough = IsSpan ? (inline as SpanAdv).StrikeThrough : (inline as HyperlinkAdv).StrikeThrough;
                IsUnderline = IsSpan ? (inline as SpanAdv).Underline : (inline as HyperlinkAdv).Underline;
                HighlightColor = IsSpan ? (inline as SpanAdv).HighlightColor : (inline as HyperlinkAdv).HighlightColor;
                Baseline = IsSpan ? (inline as SpanAdv).Baseline : (inline as HyperlinkAdv).Baseline;
                FontStyle = IsSpan ? (inline as SpanAdv).FontStyle : (inline as HyperlinkAdv).FontStyle;
                FontWeight = IsSpan ? (inline as SpanAdv).FontWeight : (inline as HyperlinkAdv).FontWeight;
            }
        }

        internal void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
