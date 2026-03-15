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
using System.ComponentModel;
#if WPF
using System.Windows.Media;
using FontStyleEnum = System.Windows.FontStyles;
#else
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI;
using FontStyleEnum = Windows.UI.Text.FontStyle;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class InlineStyle : DependencyObject, INotifyPropertyChanged
    {
        private SfRichTextBoxAdv richTextBox;
        private double fontSize = 11d;
        private FontFamily fontFamily = new FontFamily("Verdana");
        private FontWeight fontWeight;
        private FontStyle fontStyle;
        private bool isUnderline = false;
        private StrikeThrough strikeThrough = StrikeThrough.None;
        private BaselineAlignment baseLineAlignment = BaselineAlignment.Normal;
        private Color highlightColor;
        private Color foreground = Colors.Black;
        bool isStyleChanged = false;
        private Type inlineType;

        /// <summary>
        /// Initializes the new instance of InlineStyle class
        /// </summary>
        /// <param name="richTextBoxAdv"></param>
        public InlineStyle(SfRichTextBoxAdv richTextBoxAdv)
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
                return baseLineAlignment == BaselineAlignment.Superscript;
            }
        }

        /// <summary>
        /// Indicates whether the current inline is subscript
        /// </summary>
        public bool IsSubscript
        {
            get
            {
                return baseLineAlignment == BaselineAlignment.Subscript;
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
                    return inlineType == typeof(FieldBeginAdv);
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
        public SfRichTextBoxAdv RichTextBox
        {
            get
            {
                return richTextBox;
            }
        }

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
#if WPF
                isStyleChanged = fontWeight != value;
#else
                isStyleChanged = fontWeight.Weight != value.Weight;
#endif
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
        public BaselineAlignment BaselineAlignment
        {
            get
            {
                return baseLineAlignment;
            }
            internal set
            {
                isStyleChanged = baseLineAlignment != value;
                baseLineAlignment = value;
                OnPropertyChanged("BaselineAlignment");
                OnStyleChanged();
            }
        }

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
            BaselineAlignment = BaselineAlignment.Normal;
            StrikeThrough = StrikeThrough.None;
            FontWeight = FontWeights.Normal;
            FontStyle = FontStyleEnum.Normal;
            FontSize = 11d;
            HighlightColor = Color.FromArgb(0, 0, 0, 0);
            FontFamily = new FontFamily("Verdana");
        }
        internal void SetCurrentStyle()
        {
            Foreground =((SolidColorBrush)(RichTextBox.Foreground)).Color;
            IsUnderline = false;
            BaselineAlignment = BaselineAlignment.Normal;
            StrikeThrough = StrikeThrough.None;
            FontWeight = RichTextBox.FontWeight;
            FontStyle = RichTextBox.FontStyle;
            FontSize = RichTextBox.FontSize;
            HighlightColor = Color.FromArgb(0, 0, 0, 0);
            FontFamily = RichTextBox.FontFamily;
        }

        //public bool IsEqualInStyle(Inline inline)
        //{
        //        InlineStyle span2 = this as InlineStyle;
        //        return inline.CharacterFormat.Baseline == span2.Baseline && inline.CharacterFormat.FontFamily.Source == span2.FontFamily.Source
        //            && inline.CharacterFormat.FontSize == span2.FontSize && span1.FontStyle == span2.FontStyle && span1.FontWeight.Weight == span2.FontWeight.Weight
        //            && span1.Foreground == span2.Foreground && span1.HighlightColor == span2.HighlightColor && span1.StrikeThrough == span2.StrikeThrough
        //            && span1.Underline == span2.IsUnderline;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inline"></param>
        //internal void SetInlineStyle(Inline inline)
        //{
        //    if (inline != null && (IsSpan || IsHyperlink))
        //    {
        //        inlineType = inline.GetType();
        //        FontFamily = IsSpan ? (inline as SpanAdv).FontFamily : (inline as HyperlinkAdv).FontFamily;
        //        Foreground = IsSpan ? (inline as SpanAdv).Foreground : (inline as HyperlinkAdv).Foreground;
        //        FontSize = IsSpan ? (inline as SpanAdv).FontSize : (inline as HyperlinkAdv).FontSize;
        //        StrikeThrough = IsSpan ? (inline as SpanAdv).StrikeThrough : (inline as HyperlinkAdv).StrikeThrough;
        //        IsUnderline = IsSpan ? (inline as SpanAdv).Underline : (inline as HyperlinkAdv).Underline;
        //        HighlightColor = IsSpan ? (inline as SpanAdv).HighlightColor : (inline as HyperlinkAdv).HighlightColor;
        //        Baseline = IsSpan ? (inline as SpanAdv).Baseline : (inline as HyperlinkAdv).Baseline;
        //        FontStyle = IsSpan ? (inline as SpanAdv).FontStyle : (inline as HyperlinkAdv).FontStyle;
        //        FontWeight = IsSpan ? (inline as SpanAdv).FontWeight : (inline as HyperlinkAdv).FontWeight;
        //    }
        //}

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
