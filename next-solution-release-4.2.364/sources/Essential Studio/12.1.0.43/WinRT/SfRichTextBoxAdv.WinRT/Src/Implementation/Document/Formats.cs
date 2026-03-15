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
#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#else
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    public sealed class CharacterFormat : BaseNode
    {
        #region Properties
        /// <summary>
        /// Gets the owner document.
        /// </summary>
        /// <value>
        /// The owner document.
        /// </value>
        internal DocumentAdv OwnerDocument
        {
            get
            {
                if (OwnerBase is Inline
                    && (OwnerBase as Inline).OwnerParagraph != null
                    && (OwnerBase as Inline).OwnerParagraph.Document != null)
                    return (OwnerBase as Inline).OwnerParagraph.Document;
                else if (OwnerBase is ParagraphAdv
                    && (OwnerBase as ParagraphAdv).Document != null)
                    return (OwnerBase as ParagraphAdv).Document;
                else if (OwnerBase is ListLevelAdv)
                    return (OwnerBase as ListLevelAdv).OwnerDocument;
                else if (OwnerBase is DocumentAdv)
                    return OwnerBase as DocumentAdv;
                return null;
            }
        }
        /// <summary>
        /// Gets or Sets the font size.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public double FontSize
        {
            get
            {
                return (double)GetPropertyValue(FontSizeProperty);
            }
            set
            {
                SetValue(FontSizeProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public FontFamily FontFamily
        {      
            get
            {
                return (FontFamily)GetPropertyValue(FontFamilyProperty);
            }
            set
            {
                if (value != null)
                    SetValue(FontFamilyProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the font color of text.
        /// </summary>
        /// <value>
        /// The color of the font.
        /// </value>
        public Color FontColor
        {
            get
            {
                return (Color)GetPropertyValue(FontColorProperty);
            }
            set
            {
                if (value != null)
                    SetValue(FontColorProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CharacterFormat"/> is bold.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bold; otherwise, <c>false</c>.
        /// </value>
        public bool Bold
        {
            get
            {
                return (bool)GetPropertyValue(BoldProperty);
            }
            set
            {
                SetValue(BoldProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CharacterFormat"/> is italic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if italic; otherwise, <c>false</c>.
        /// </value>
        public bool Italic
        {
            get
            {
                return (bool)GetPropertyValue(ItalicProperty);
            }
            set
            {
                SetValue(ItalicProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the strike through.
        /// </summary>
        /// <value>
        /// The strike through.
        /// </value>
        public StrikeThrough StrikeThrough
        {
            get
            {
                return (StrikeThrough)GetPropertyValue(StrikeThroughProperty);
            }
            set
            {
                SetValue(StrikeThroughProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the baseline.
        /// </summary>
        /// <value>
        /// The baseline alignment.
        /// </value>
        public BaselineAlignment BaselineAlignment
        {
            get
            {
                return (BaselineAlignment)GetPropertyValue(BaselineAlignmentProperty);
            }
            set
            {
                SetValue(BaselineAlignmentProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the underline style.
        /// </summary>
        /// <value>
        /// The underline.
        /// </value>
        public Underline Underline
        {
            get
            {
                return (Underline)GetPropertyValue(UnderlineProperty);
            }
            set
            {
                SetValue(UnderlineProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the highlight color.
        /// </summary>
        /// <value>
        /// The color of the highlight.
        /// </value>
        public HighlightColor HighlightColor
        {
            get
            {
                return (HighlightColor)GetPropertyValue(HighlightColorProperty);
            }
            set
            {
                SetValue(HighlightColorProperty, value);
            }
        }
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Identifies the FontSize dependency property.
        /// </summary>
        /// <returns>The identifier of the FontSize dependency property.</returns>
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(CharacterFormat), new PropertyMetadata(11d, OnFontSizeChanged));
        /// <summary>
        /// Identifies the FontFamily dependency property.
        /// </summary>
        /// <returns>The identifier of the FontFamily dependency property.</returns>
        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(CharacterFormat), new PropertyMetadata(new FontFamily("Verdana"), OnFontFamilyChanged));
        /// <summary>
        /// Identifies the Bold dependency property.
        /// </summary>
        /// <returns>The identifier of the Bold dependency property.</returns>
        public static readonly DependencyProperty BoldProperty = DependencyProperty.Register("Bold", typeof(bool), typeof(CharacterFormat), new PropertyMetadata(false, OnBoldChanged));
        /// <summary>
        /// Identifies the Italic dependency property.
        /// </summary>
        /// <returns>The identifier of the Italic dependency property.</returns>
        public static readonly DependencyProperty ItalicProperty = DependencyProperty.Register("Italic", typeof(bool), typeof(CharacterFormat), new PropertyMetadata(false, OnItalicChanged));
        /// <summary>
        /// Identifies the StrikeThrough dependency property.
        /// </summary>
        /// <returns>The identifier of the StrikeThrough dependency property.</returns>
        public static readonly DependencyProperty StrikeThroughProperty = DependencyProperty.Register("StrikeThrough", typeof(StrikeThrough), typeof(CharacterFormat), new PropertyMetadata(StrikeThrough.None, OnStrikeThroughChanged));
        /// <summary>
        /// Identifies the BaselineAlignment dependency property.
        /// </summary>
        /// <returns>The identifier of the BaselineAlignment dependency property.</returns>
        internal static readonly DependencyProperty BaselineAlignmentProperty = DependencyProperty.Register("BaselineAlignment", typeof(BaselineAlignment), typeof(CharacterFormat), new PropertyMetadata(BaselineAlignment.Normal, OnBaselineAlignmentChanged));
        /// <summary>
        /// Identifies the Underline dependency property.
        /// </summary>
        /// <returns>The identifier of the Underline dependency property.</returns>
        public static readonly DependencyProperty UnderlineProperty = DependencyProperty.Register("Underline", typeof(Underline), typeof(CharacterFormat), new PropertyMetadata(Underline.None, OnUnderlineChanged));
        /// <summary>
        /// Identifies the FontColor dependency property.
        /// </summary>
        /// <returns>The identifier of the FontColor dependency property.</returns>
        public static readonly DependencyProperty FontColorProperty = DependencyProperty.Register("FontColor", typeof(Color), typeof(CharacterFormat), new PropertyMetadata(Color.FromArgb(0, 0, 0, 0), OnFontColorChanged));
        /// <summary>
        /// Identifies the HighlightColor dependency property.
        /// </summary>
        /// <returns>The identifier of the HighlightColor dependency property.</returns>
        public static readonly DependencyProperty HighlightColorProperty = DependencyProperty.Register("HighlightColor", typeof(HighlightColor), typeof(CharacterFormat), new PropertyMetadata(HighlightColor.NoColor, OnHighlightColorChanged));
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterFormat"/> class.
        /// </summary>
        public CharacterFormat()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterFormat" /> class.
        /// </summary>
        /// <param name="baseNode">The base node.</param>
        public CharacterFormat(BaseNode baseNode)
            : base(baseNode)
        {
        }
        #endregion

        #region Static Events
        /// <summary>
        /// Called when font size changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CharacterFormat).Relayout();
        }
        /// <summary>
        /// Called when font family changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CharacterFormat).Relayout();
        }
        /// <summary>
        /// Called when bold changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnBoldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CharacterFormat).Relayout();
        }
        /// <summary>
        /// Called when italic changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnItalicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CharacterFormat).Relayout();
        }
        /// <summary>
        /// Called when strike through changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStrikeThroughChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CharacterFormat).Relayout();
        }
        /// <summary>
        /// Called when baseline alignment changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnBaselineAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CharacterFormat).Relayout();
        }
        /// <summary>
        /// Called when underline changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnUnderlineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CharacterFormat).Relayout();
        }
        /// <summary>
        /// Called when font color changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFontColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CharacterFormat).Relayout();
        }
        /// <summary>
        /// Called when highlight color changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHighlightColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CharacterFormat).Relayout();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Applies the property value.
        /// </summary>
        /// <param name="historyInfo">The history info.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal void ApplyPropertyValue(HistoryInfo historyInfo, DependencyProperty property, object value)
        {
            if (historyInfo != null)
                value = historyInfo.AddModifiedProperties(this, property, value);
            if (value == DependencyProperty.UnsetValue)
            {
                ClearValue(property);
                return;
            }
            if (property == BoldProperty)
                Bold = (bool)value;
            else if (property == ItalicProperty)
                Italic = (bool)value;
            else if (property == FontColorProperty)
                FontColor = (Color)value;
            else if (property == FontFamilyProperty)
                FontFamily = (FontFamily)value;
            else if (property == FontSizeProperty)
                FontSize = (double)value;
            else if (property == HighlightColorProperty)
                HighlightColor = (HighlightColor)value;
            else if (property == BaselineAlignmentProperty)
                BaselineAlignment = (BaselineAlignment)value;
            else if (property == StrikeThroughProperty)
                StrikeThrough = (StrikeThrough)value;
            else if (property == UnderlineProperty)
                Underline = (Underline)value;
        }
        /// <summary>
        /// Relayouts this instance.
        /// </summary>
        internal void Relayout()
        {
            if (OwnerBase is DocumentAdv)
            {
                double verticalScrolValue = 0;
                double horizontalScrolValue = 0;
                if ((OwnerBase as DocumentAdv).OwnerControl != null
                    && (OwnerBase as DocumentAdv).OwnerControl.IsDocumentLoaded
                    && (OwnerBase as DocumentAdv).OwnerControl.IsLayoutEnabled)
                {
                    verticalScrolValue = (OwnerBase as DocumentAdv).OwnerControl.verticalScrollBar.Value;
                    horizontalScrolValue = (OwnerBase as DocumentAdv).OwnerControl.horizontalScrollBar.Value;
                }
                (OwnerBase as DocumentAdv).LayoutItems();
                if ((OwnerBase as DocumentAdv).OwnerControl != null
                    && (OwnerBase as DocumentAdv).OwnerControl.IsDocumentLoaded
                    && (OwnerBase as DocumentAdv).OwnerControl.IsLayoutEnabled)
                {
                    (OwnerBase as DocumentAdv).OwnerControl.verticalScrollBar.Value = verticalScrolValue;
                    (OwnerBase as DocumentAdv).OwnerControl.horizontalScrollBar.Value = horizontalScrolValue;
                }
            }
            else if (OwnerBase is ParagraphAdv)
                (OwnerBase as ParagraphAdv).Relayout(0);
            else if (OwnerBase is Inline
                && (OwnerBase as Inline).OwnerParagraph != null)
            {
                int index = (OwnerBase as Inline).OwnerParagraph.Inlines.IndexOf(OwnerBase as Inline);
                (OwnerBase as Inline).OwnerParagraph.Relayout(index);
            }
        }
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        internal object GetPropertyValue(DependencyProperty property)
        {
            if (ReadLocalValue(property) == DependencyProperty.UnsetValue)
            {
                if (OwnerBase is Inline || OwnerBase is ParagraphAdv)
                {
                    DocumentAdv doc = OwnerDocument;
                    if (doc != null)
                        return doc.CharacterFormat.GetPropertyValue(property);
                }
                else if (OwnerBase is DocumentAdv)
                    return GetDefaultValue(property);
            }
            return GetValue(property);
        }
        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        private object GetDefaultValue(DependencyProperty property)
        {
            object value = null;
            DocumentAdv document = OwnerBase as DocumentAdv;
            value = GetControlDefaultValue(document.OwnerControl, property);
            if (value == null)
                value = GetValue(property);
            return value;
        }
        /// <summary>
        /// Gets the control default value.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        private object GetControlDefaultValue(SfRichTextBoxAdv richTextBoxAdv, DependencyProperty property)
        {
            if (richTextBoxAdv == null)
                return null;
            if (property == FontColorProperty)
                return GetDefaultFontColor(richTextBoxAdv.Document);
            else if (property == FontFamilyProperty && richTextBoxAdv.FontFamily is FontFamily)
                return new FontFamily(richTextBoxAdv.FontFamily.Source);
            else if (property == FontSizeProperty)
                return richTextBoxAdv.FontSize;
            else if (property == BoldProperty)
#if WPF
                return richTextBoxAdv.FontWeight >= FontWeights.SemiBold;
#else
                return richTextBoxAdv.FontWeight.Weight >= FontWeights.SemiBold.Weight;
#endif
            else if (property == ItalicProperty)
#if WPF
                return richTextBoxAdv.FontStyle != FontStyles.Normal;
#else
                return richTextBoxAdv.FontStyle != FontStyle.Normal;
#endif
                return null;
        }
        /// <summary>
        /// Gets the default font color.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        internal Color GetDefaultFontColor(DocumentAdv document)
        {
            Color backColor = Color.FromArgb(0, 0, 0, 0);
            SfRichTextBoxAdv richTextBoxAdv = document.OwnerControl;
            if (richTextBoxAdv != null && richTextBoxAdv.LayoutType == LayoutType.Block)
            {
                if (richTextBoxAdv.Foreground is SolidColorBrush)
                    return (richTextBoxAdv.Foreground as SolidColorBrush).Color;
                else if (richTextBoxAdv.Background is SolidColorBrush)
                    backColor = (richTextBoxAdv.Background as SolidColorBrush).Color;
            }
            else
                backColor = document.Background;
            if (backColor.R > 0 || backColor.G > 0 || backColor.B > 0)
                return Colors.Black;
            else
                return Colors.White;
        }
        /// <summary>
        /// Sets the color of the highlight.
        /// </summary>
        /// <param name="color">The color.</param>
        internal void SetHighlightColor(Color color)
        {
            switch (color.ToString().ToLower())
            {
                case "#ffffff00":
                    HighlightColor = HighlightColor.Yellow;
                    break;
                case "#ff00ff00":
                    HighlightColor = HighlightColor.BrightGreen;
                    break;
                case "#ff00ffff":
                    HighlightColor = HighlightColor.Turquoise;
                    break;
                case "#ffff00ff":
                    HighlightColor = HighlightColor.Pink;
                    break;
                case "#ff0000ff":
                    HighlightColor = HighlightColor.Blue;
                    break;
                case "#ffff0000":
                    HighlightColor = HighlightColor.Red;
                    break;
                case "#ff000080":
                    HighlightColor = HighlightColor.DarkBlue;
                    break;
                case "#ff008080":
                    HighlightColor = HighlightColor.Teal;
                    break;
                case "#ff008000":
                    HighlightColor = HighlightColor.Green;
                    break;
                case "#ff800080":
                    HighlightColor = HighlightColor.Violet;
                    break;
                case "#ff800000":
                    HighlightColor = HighlightColor.DarkRed;
                    break;
                case "#ff808000":
                    HighlightColor = HighlightColor.DarkYellow;
                    break;
                case "#ff808080":
                    HighlightColor = HighlightColor.Gray50;
                    break;
                case "#ffc0c0c0":
                    HighlightColor = HighlightColor.Gray25;
                    break;
                case "#ff000000":
                    HighlightColor = HighlightColor.Black;
                    break;
            }
        }
        /// <summary>
        /// Gets the color of the highlight.
        /// </summary>
        /// <returns></returns>
        internal Color GetHighlightColor()
        {
            switch (HighlightColor)
            {
                case HighlightColor.Yellow:
                    return Color.FromArgb(255, 255, 255, 0);
                case HighlightColor.BrightGreen:
                    return Color.FromArgb(255, 0, 255, 0);
                case HighlightColor.Turquoise:
                    return Color.FromArgb(255, 0, 255, 255);
                case HighlightColor.Pink:
                    return Color.FromArgb(255, 255, 0, 255);
                case HighlightColor.Blue:
                    return Color.FromArgb(255, 0, 0, 255);
                case HighlightColor.Red:
                    return Color.FromArgb(255, 255, 0, 0);
                case HighlightColor.DarkBlue:
                    return Color.FromArgb(255, 0, 0, 128);
                case HighlightColor.Teal:
                    return Color.FromArgb(255, 0, 128, 128);
                case HighlightColor.Green:
                    return Color.FromArgb(255, 0, 128, 0);
                case HighlightColor.Violet:
                    return Color.FromArgb(255, 128, 0, 128);
                case HighlightColor.DarkRed:
                    return Color.FromArgb(255, 128, 0, 0);
                case HighlightColor.DarkYellow:
                    return Color.FromArgb(255, 128, 128, 0);
                case HighlightColor.Gray50:
                    return Color.FromArgb(255, 128, 128, 128);
                case HighlightColor.Gray25:
                    return Color.FromArgb(255, 192, 192, 192);
                case HighlightColor.Black:
                    return Color.FromArgb(255, 0, 0, 0);
                default:
                    return Color.FromArgb(0, 0, 0, 0);
            }
        }
        /// <summary>
        /// Copies the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CopyFormat(CharacterFormat format)
        {
            if (format.ReadLocalValue(FontSizeProperty) is double)
                FontSize = format.FontSize;
            if (format.ReadLocalValue(FontFamilyProperty) is FontFamily)
                FontFamily = format.FontFamily;
            if (format.ReadLocalValue(BoldProperty) is bool)
                Bold = format.Bold;
            if (format.ReadLocalValue(ItalicProperty) is bool)
                Italic = format.Italic;
            if (format.ReadLocalValue(BaselineAlignmentProperty) is BaselineAlignment)
                BaselineAlignment = format.BaselineAlignment;
            if (format.ReadLocalValue(UnderlineProperty) is Underline)
                Underline = format.Underline;
            if (format.ReadLocalValue(FontColorProperty) is Color)
                FontColor = format.FontColor;
            if (format.ReadLocalValue(HighlightColorProperty) is HighlightColor)
                HighlightColor = format.HighlightColor;
            if (format.ReadLocalValue(StrikeThroughProperty) is StrikeThrough)
                StrikeThrough = format.StrikeThrough;
        }
        /// <summary>
        /// Determines whether the specified format is equal.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        ///   <c>true</c> if  the specified format is equal; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsEqualFormat(CharacterFormat format)
        {
            return (FontSize == format.FontSize
                && FontFamily.Source == format.FontFamily.Source
                && Bold == format.Bold
                && Italic == format.Italic
                && BaselineAlignment == format.BaselineAlignment
                && Underline == format.Underline
                && FontColor == format.FontColor
                && HighlightColor == format.HighlightColor);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            SetOwner(null);
            ClearValue(FontSizeProperty);
            ClearValue(FontFamilyProperty);
            ClearValue(BoldProperty);
            ClearValue(ItalicProperty);
            ClearValue(BaselineAlignmentProperty);
            ClearValue(UnderlineProperty);
            ClearValue(FontColorProperty);
            ClearValue(HighlightColorProperty);
            ClearValue(StrikeThroughProperty);
        }
        #endregion
    }
    public sealed class ParagraphFormat : BaseNode
    {
        #region Properties
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        internal DocumentAdv Document
        {
            get
            {
                if (OwnerBase is ParagraphAdv
                    && (OwnerBase as ParagraphAdv).Document != null)
                    return (OwnerBase as ParagraphAdv).Document;
                else if (OwnerBase is ListLevelAdv)
                    return (OwnerBase as ListLevelAdv).OwnerDocument;
                else if (OwnerBase is DocumentAdv)
                    return OwnerBase as DocumentAdv;
                return null;
            }
        }
        /// <summary>
        /// Gets or Sets the left indent.
        /// </summary>
        /// <value>
        /// The left indent.
        /// </value>
        public double LeftIndent
        {
            get
            {
                return (double)GetPropertyValue(LeftIndentProperty);
            }
            set
            {
                SetValue(LeftIndentProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the right indent.
        /// </summary>
        /// <value>
        /// The right indent.
        /// </value>
        public double RightIndent
        {
            get
            {
                return (double)GetPropertyValue(RightIndentProperty);
            }
            set
            {
                    SetValue(RightIndentProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the first line indent.
        /// </summary>
        /// <value>
        /// The first line indent.
        /// </value>
        public double FirstLineIndent
        {
            get
            {
                return (double)GetPropertyValue(FirstLineIndentProperty);
            }
            set
            {
                SetValue(FirstLineIndentProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the text alignment.
        /// </summary>
        /// <value>
        /// The text alignment.
        /// </value>
        public TextAlignment TextAlignment
        {
            get
            {
                return (TextAlignment)GetPropertyValue(TextAlignmentProperty);
            }
            set
            {
                SetValue(TextAlignmentProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the after spacing.
        /// </summary>
        /// <value>
        /// The after spacing.
        /// </value>
        public double AfterSpacing
        {
            get
            {
                return (double)GetPropertyValue(AfterSpacingProperty);
            }
            set
            {
                SetValue(AfterSpacingProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the before spacing.
        /// </summary>
        /// <value>
        /// The before spacing.
        /// </value>
        public double BeforeSpacing
        {
            get
            {
                return (double)GetPropertyValue(BeforeSpacingProperty);
            }
            set
            {
                SetValue(BeforeSpacingProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the line spacing.
        /// </summary>
        /// <value>
        /// The line spacing.
        /// </value>
        public double LineSpacing
        {
            get
            {
                return (double)GetPropertyValue(LineSpacingProperty);
            }
            set
            {
                SetValue(LineSpacingProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the line spacing type.
        /// </summary>
        /// <value>
        /// The type of the line spacing.
        /// </value>
        public LineSpacingType LineSpacingType
        {
            get
            {
                return (LineSpacingType)GetPropertyValue(LineSpacingTypeProperty);
            }
            set
            {
                if (value == LineSpacingType.Multiple)
                    LineSpacing = 1;
                else
                    LineSpacing = 16;
                SetValue(LineSpacingTypeProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the list format.
        /// </summary>
        /// <value>
        /// The list format.
        /// </value>
        internal ListFormat ListFormat
        {
            get
            {
                return (ListFormat)GetPropertyValue(ListFormatProperty);
            }
            set
            {
                SetValue(ListFormatProperty, value);
            }
        }
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Identifies the ListFormat dependency property.
        /// </summary>
        /// <returns>The identifier of the ListFormat dependency property.</returns>
        internal static readonly DependencyProperty ListFormatProperty = DependencyProperty.Register("ListFormat", typeof(ListFormat), typeof(ParagraphFormat), new PropertyMetadata(null, OnListFormatChanged));
        /// <summary>
        /// Identifies the LeftIndent dependency property.
        /// </summary>
        /// <returns>The identifier of the LeftIndent dependency property.</returns>
        public static readonly DependencyProperty LeftIndentProperty = DependencyProperty.Register("LeftIndent", typeof(double), typeof(ParagraphFormat), new PropertyMetadata(0d, OnLeftIndentChanged));
        /// <summary>
        /// Identifies the RightIndent dependency property.
        /// </summary>
        /// <returns>The identifier of the RightIndent dependency property.</returns>
        public static readonly DependencyProperty RightIndentProperty = DependencyProperty.Register("RightIndent", typeof(double), typeof(ParagraphFormat), new PropertyMetadata(0d, OnRightIndentChanged));
        /// <summary>
        /// Identifies the FirstLineIndent dependency property.
        /// </summary>
        /// <returns>The identifier of the FirstLineIndent dependency property.</returns>
        public static readonly DependencyProperty FirstLineIndentProperty = DependencyProperty.Register("FirstLineIndent", typeof(double), typeof(ParagraphFormat), new PropertyMetadata(0d, OnFirstLineIndentChanged));
        /// <summary>
        /// Identifies the TextAlignment dependency property.
        /// </summary>
        /// <returns>The identifier of the TextAlignment dependency property.</returns>
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(ParagraphFormat), new PropertyMetadata(TextAlignment.Left, OnTextAlignmentChanged));
        /// <summary>
        /// Identifies the AfterSpacing dependency property.
        /// </summary>
        /// <returns>The identifier of the AfterSpacing dependency property.</returns>
        public static readonly DependencyProperty AfterSpacingProperty = DependencyProperty.Register("AfterSpacing", typeof(double), typeof(ParagraphFormat), new PropertyMetadata(0d, OnAfterSpacingChanged));
        /// <summary>
        /// Identifies the BeforeSpacing dependency property.
        /// </summary>
        /// <returns>The identifier of the BeforeSpacing dependency property.</returns>
        public static readonly DependencyProperty BeforeSpacingProperty = DependencyProperty.Register("BeforeSpacing", typeof(double), typeof(ParagraphFormat), new PropertyMetadata(0d, OnBeforeSpacingChanged));
        /// <summary>
        /// Identifies the LineSpacing dependency property.
        /// </summary>
        /// <returns>The identifier of the LineSpacing dependency property.</returns>
        public static readonly DependencyProperty LineSpacingProperty = DependencyProperty.Register("LineSpacing", typeof(double), typeof(ParagraphFormat), new PropertyMetadata(1d, OnLineSpacingChanged));
        /// <summary>
        /// Identifies the LineSpacingType dependency property.
        /// </summary>
        /// <returns>The identifier of the LineSpacingType dependency property.</returns>
        public static readonly DependencyProperty LineSpacingTypeProperty = DependencyProperty.Register("LineSpacingType", typeof(LineSpacingType), typeof(ParagraphFormat), new PropertyMetadata(LineSpacingType.Multiple, OnLineSpacingTypeChanged));
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphFormat"/> class.
        /// </summary>
        public ParagraphFormat()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphFormat" /> class.
        /// </summary>
        /// <param name="baseNode">The base node.</param>
        public ParagraphFormat(BaseNode baseNode)
            : base(baseNode)
        {
            ListFormat = new ListFormat(this);
        }
        #endregion

        #region Static Events
        /// <summary>
        /// Called when list format changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnListFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ListFormat)
            {
                if ((e.NewValue as ListFormat).OwnerBase != d)
                {
                    (e.NewValue as ListFormat).SetOwner(d as ParagraphFormat);
                    (e.NewValue as ListFormat).Relayout();
                }
            }
        }
        /// <summary>
        /// Called when left indent changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLeftIndentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ParagraphFormat).Relayout();
        }
        /// <summary>
        /// Called when right indent changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRightIndentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ParagraphFormat).Relayout();
        }
        /// <summary>
        /// Called when first line indent changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFirstLineIndentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ParagraphFormat).Relayout();
        }
        /// <summary>
        /// Called when text alignment changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTextAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ParagraphFormat).Relayout();
        }
        /// <summary>
        /// Called when after spacing changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAfterSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ParagraphFormat).Relayout();
        }
        /// <summary>
        /// Called when before spacing changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnBeforeSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ParagraphFormat).Relayout();
        }
        /// <summary>
        /// Called when line spacing changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLineSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ParagraphFormat).Relayout();
        }
        /// <summary>
        /// Called when line spacing type changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLineSpacingTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ParagraphFormat).Relayout();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Applies the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="listLevel">The list level.</param>
        internal void ApplyList(ListAdv list, ListLevelAdv listLevel)
        {
            ListFormat.ApplyList(list, listLevel);
        }
        /// <summary>
        /// Applies the property value.
        /// </summary>
        /// <param name="historyInfo">The history info.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal void ApplyPropertyValue(HistoryInfo historyInfo, DependencyProperty property, object value)
        {
            if (historyInfo != null)
                value = historyInfo.AddModifiedProperties(this, property, value);
            if (value == DependencyProperty.UnsetValue)
            {
                ClearValue(property);
                return;
            }
            if (property == AfterSpacingProperty)
                AfterSpacing = (double)value;
            else if (property == BeforeSpacingProperty)
                BeforeSpacing = (double)value;
            else if (property == LeftIndentProperty)
                LeftIndent = (double)value;
            else if (property == LineSpacingTypeProperty)
                LineSpacingType = (LineSpacingType)value;
            else if (property == LineSpacingProperty)
                LineSpacing = (double)value;
            else if (property == RightIndentProperty)
                RightIndent = (double)value;
            else if (property == FirstLineIndentProperty)
                FirstLineIndent = (double)value;
            else if (property == TextAlignmentProperty)
                TextAlignment = (TextAlignment)value;
            else if (property == ListFormatProperty)
                ListFormat = (ListFormat)value;
        }
        /// <summary>
        /// Relayouts this instance.
        /// </summary>
        internal void Relayout()
        {
            if (OwnerBase is DocumentAdv)
            {
                double verticalScrolValue = 0;
                double horizontalScrolValue = 0;
                if ((OwnerBase as DocumentAdv).OwnerControl != null
                    && (OwnerBase as DocumentAdv).OwnerControl.IsLayoutEnabled)
                {
                    if ((OwnerBase as DocumentAdv).OwnerControl.IsDocumentLoaded)
                    {
                        verticalScrolValue = (OwnerBase as DocumentAdv).OwnerControl.verticalScrollBar.Value;
                        horizontalScrolValue = (OwnerBase as DocumentAdv).OwnerControl.horizontalScrollBar.Value;
                    }
                    (OwnerBase as DocumentAdv).LayoutItems();
                    if ((OwnerBase as DocumentAdv).OwnerControl.IsDocumentLoaded)
                    {
                        (OwnerBase as DocumentAdv).OwnerControl.verticalScrollBar.Value = verticalScrolValue;
                        (OwnerBase as DocumentAdv).OwnerControl.horizontalScrollBar.Value = horizontalScrolValue;
                    }
                }
            }
            else if (OwnerBase is ParagraphAdv)
                (OwnerBase as ParagraphAdv).Relayout(0);
        }
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        internal object GetPropertyValue(DependencyProperty property)
        {
            if (ReadLocalValue(property) == DependencyProperty.UnsetValue)
            {
                if (OwnerBase is ParagraphAdv)
                {
                    DocumentAdv doc = Document;
                    if (doc != null)
                        return doc.ParagraphFormat.GetValue(property);
                }
            }
            return GetValue(property);
        }
        /// <summary>
        /// Copies the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CopyFormat(ParagraphFormat format)
        {
            if (format.ReadLocalValue(LeftIndentProperty) is double)
                LeftIndent = format.LeftIndent;
            if (format.ReadLocalValue(RightIndentProperty) is double)
                RightIndent = format.RightIndent;
            if (format.ReadLocalValue(FirstLineIndentProperty) is double)
                FirstLineIndent = format.FirstLineIndent;
            if (format.ReadLocalValue(AfterSpacingProperty) is double)
                AfterSpacing = format.AfterSpacing;
            if (format.ReadLocalValue(BeforeSpacingProperty) is double)
                BeforeSpacing = format.BeforeSpacing;
            if (format.ReadLocalValue(LineSpacingProperty) is double)
                LineSpacing = format.LineSpacing;
            if (format.ReadLocalValue(LineSpacingTypeProperty) is LineSpacingType)
                LineSpacingType = format.LineSpacingType;
            if (format.ReadLocalValue(TextAlignmentProperty) is TextAlignment)
                TextAlignment = format.TextAlignment;
            if (format.ReadLocalValue(ListFormatProperty) is ListFormat)
                ListFormat.CopyFormat(format.ListFormat);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            SetOwner(null);
            ClearValue(LeftIndentProperty);
            ClearValue(RightIndentProperty);
            ClearValue(FirstLineIndentProperty);
            ClearValue(AfterSpacingProperty);
            ClearValue(BeforeSpacingProperty);
            ClearValue(LineSpacingProperty);
            ClearValue(LineSpacingTypeProperty);
            ClearValue(TextAlignmentProperty);
            ListFormat.Dispose();
            ClearValue(ListFormatProperty);
        }
        #endregion
    }
    internal sealed class ListFormat : BaseNode
    {
        #region Properties
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        internal DocumentAdv Document
        {
            get
            {
                if (OwnerBase is ParagraphFormat)
                    return (OwnerBase as ParagraphFormat).Document;
                return null;
            }
        }
        /// <summary>
        /// Gets or sets the list id.
        /// </summary>
        /// <value>
        /// The list id.
        /// </value>
        internal string ListId
        {
            get
            {
                return (string)GetPropertyValue(ListIdProperty);
            }
            set
            {
                SetValue(ListIdProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the list level number.
        /// </summary>
        /// <value>
        /// The list level number.
        /// </value>
        internal int ListLevelNumber
        {
            get
            {
                return (int)GetPropertyValue(ListLevelNumberProperty);
            }
            set
            {
                SetValue(ListLevelNumberProperty, value);
            }
        }
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Identifies the List Id dependency property.
        /// </summary>
        /// <returns>The identifier of the List Id dependency property.</returns>
        internal static readonly DependencyProperty ListIdProperty = DependencyProperty.Register("ListId", typeof(string), typeof(ListFormat), new PropertyMetadata(null, OnListIdChanged));
        /// <summary>
        /// Identifies the ListLevelNumber dependency property.
        /// </summary>
        /// <returns>The identifier of the ListLevelNumber dependency property.</returns>
        internal static readonly DependencyProperty ListLevelNumberProperty = DependencyProperty.Register("ListLevelNumber", typeof(int), typeof(ListFormat), new PropertyMetadata(0, OnListLevelNumberChanged));
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ListFormat"/> class.
        /// </summary>
        internal ListFormat()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ListFormat" /> class.
        /// </summary>
        /// <param name="paragraphFormat">The paragraphFormat.</param>
        internal ListFormat(ParagraphFormat paragraphFormat)
            : base(paragraphFormat)
        {
        }
        #endregion

        #region Static Events
        private static void OnListLevelNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ListFormat).Relayout();
        }
        private static void OnListIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ListFormat).Relayout();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Removes the list.
        /// </summary>
        internal void RemoveList()
        {
            ListId = null;
            ListLevelNumber = 0;
        }
        /// <summary>
        /// Applies the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="listLevel">The list level.</param>
        internal void ApplyList(ListAdv list, ListLevelAdv listLevel)
        {
            if (list == null || listLevel == null)
                RemoveList();
            else
            {
                ListId = list.Name;
                ListLevelNumber = listLevel.OwnerAbstractList.Levels.IndexOf(listLevel);
            }
            Relayout();
        }
        /// <summary>
        /// Applies the property value.
        /// </summary>
        /// <param name="historyInfo">The history info.</param>
        /// <param name="value">The value.</param>
        internal void ApplyPropertyValue(HistoryInfo historyInfo, object value)
        {
            if (value == DependencyProperty.UnsetValue)
            {
                ClearValue(ListIdProperty);
                ClearValue(ListLevelNumberProperty);
                return;
            }
            ListFormat listFormat = value as ListFormat;
            if (listFormat.ReadLocalValue(ListIdProperty) is string)
                ListId = (string)value;
            if (listFormat.ReadLocalValue(ListLevelNumberProperty) is int)
                ListLevelNumber = (int)value;
        }
        /// <summary>
        /// Relayouts this instance.
        /// </summary>
        internal void Relayout()
        {
            if (OwnerBase is ParagraphFormat && (OwnerBase as ParagraphFormat).OwnerBase is ParagraphAdv)
                ((OwnerBase as ParagraphFormat).OwnerBase as ParagraphAdv).Layout();
            else if (Document is DocumentAdv)
            {
                double verticalScrolValue = 0;
                double horizontalScrolValue = 0;
                if (Document.OwnerControl != null
                    && Document.OwnerControl.IsLayoutEnabled)
                {
                    if (Document.OwnerControl.IsDocumentLoaded)
                    {
                        verticalScrolValue = Document.OwnerControl.verticalScrollBar.Value;
                        horizontalScrolValue = Document.OwnerControl.horizontalScrollBar.Value;
                    }
                    Document.LayoutItems();
                    if (Document.OwnerControl.IsDocumentLoaded)
                    {
                        Document.OwnerControl.verticalScrollBar.Value = verticalScrolValue;
                        Document.OwnerControl.horizontalScrollBar.Value = horizontalScrolValue;
                    }
                }
            }
        }
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        internal object GetPropertyValue(DependencyProperty property)
        {
            if (ReadLocalValue(property) == DependencyProperty.UnsetValue)
            {
                if (OwnerBase is ParagraphFormat && (OwnerBase as ParagraphFormat).OwnerBase is ParagraphAdv)
                {
                    DocumentAdv doc = Document;
                    if (doc != null)
                        return doc.ParagraphFormat.GetValue(property);
                }
            }
            return GetValue(property);
        }
        /// <summary>
        /// Copies the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CopyFormat(ListFormat format)
        {
            if (format.ReadLocalValue(ListIdProperty) is string)
                ListId = format.ListId;
            if (format.ReadLocalValue(ListLevelNumberProperty) is int)
                ListLevelNumber = format.ListLevelNumber;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            SetOwner(null);
            ClearValue(ListIdProperty);
            ClearValue(ListLevelNumberProperty);
        }
        #endregion
    }
    public sealed class SectionFormat : BaseNode
    {
        #region Properties
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        internal DocumentAdv Document
        {
            get
            {
                if (OwnerBase is SectionAdv
                    && (OwnerBase as SectionAdv).Document != null)
                    return (OwnerBase as SectionAdv).Document;
                return null;
            }
        }
        /// <summary>
        /// Gets or Sets the page size.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public Size PageSize
        {
            get
            {
                return (Size)GetValue(PageSizeProperty);
            }
            set
            {
                SetValue(PageSizeProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the page margins.
        /// </summary>
        /// <value>
        /// The page margin.
        /// </value>
        public Thickness PageMargin
        {
            get
            {
                return (Thickness)GetValue(PageMarginProperty);
            }
            set
            {
                SetValue(PageMarginProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the header distance from page top.
        /// </summary>
        /// <value>
        /// The header distance.
        /// </value>
        public double HeaderDistance
        {
            get
            {
                return (double)GetValue(HeaderDistanceProperty);
            }
            set
            {
                SetValue(HeaderDistanceProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the footer distance from page bottom.
        /// </summary>
        /// <value>
        /// The footer distance.
        /// </value>
        public double FooterDistance
        {
            get
            {
                return (double)GetValue(FooterDistanceProperty);
            }
            set
            {
                SetValue(FooterDistanceProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the section has different first page.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the section has different first page; otherwise, <c>false</c>.
        /// </value>
        public bool DifferentFirstPage
        {
            get
            {
                return (bool)GetValue(DifferentFirstPageProperty);
            }
            set
            {
                SetValue(DifferentFirstPageProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the section has different odd and even pages.
        /// </summary>
        /// <value>
        /// <c>true</c> if the section has different odd and even pages; otherwise, <c>false</c>.
        /// </value>
        internal bool DifferentOddAndEvenPages
        {
            get
            {
                return (bool)GetValue(DifferentOddAndEvenPagesProperty);
            }
            set
            {
                SetValue(DifferentOddAndEvenPagesProperty, value);
            }
        }
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Identifies the PageSize dependency property.
        /// </summary>
        /// <returns>The identifier of the PageSize dependency property.</returns>
        public static readonly DependencyProperty PageSizeProperty = DependencyProperty.Register("PageSize", typeof(Size), typeof(SectionFormat), new PropertyMetadata(new Size(816, 1056), OnPageSizeChanged));
        /// <summary>
        /// Identifies the PageMargin dependency property.
        /// </summary>
        /// <returns>The identifier of the PageMargin dependency property.</returns>
        public static readonly DependencyProperty PageMarginProperty = DependencyProperty.Register("PageMargin", typeof(Thickness), typeof(SectionFormat), new PropertyMetadata(new Thickness(96), OnPageMarginChanged));
        /// <summary>
        /// Identifies the HeaderDistance dependency property.
        /// </summary>
        /// <returns>The identifier of the HeaderDistance dependency property.</returns>
        public static readonly DependencyProperty HeaderDistanceProperty = DependencyProperty.Register("HeaderDistance", typeof(double), typeof(SectionFormat), new PropertyMetadata(48d, OnHeaderDistanceChanged));
        /// <summary>
        /// Identifies the FooterDistance dependency property.
        /// </summary>
        /// <returns>The identifier of the FooterDistance dependency property.</returns>
        public static readonly DependencyProperty FooterDistanceProperty = DependencyProperty.Register("FooterDistance", typeof(double), typeof(SectionFormat), new PropertyMetadata(48d, OnFooterDistanceChanged));
        /// <summary>
        /// Identifies the DifferentFirstPage dependency property.
        /// </summary>
        /// <returns>The identifier of the DifferentFirstPage dependency property.</returns>
        public static readonly DependencyProperty DifferentFirstPageProperty = DependencyProperty.Register("DifferentFirstPage", typeof(bool), typeof(SectionFormat), new PropertyMetadata(false, OnDifferentFirstPageChanged));
        /// <summary>
        /// Identifies the DifferentOddAndEvenPages dependency property.
        /// </summary>
        /// <returns>The identifier of the DifferentOddAndEvenPages dependency property.</returns>
        public static readonly DependencyProperty DifferentOddAndEvenPagesProperty = DependencyProperty.Register("DifferentOddAndEvenPages", typeof(bool), typeof(SectionFormat), new PropertyMetadata(false, OnDifferentOddAndEvenPagesChanged));
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionFormat"/> class.
        /// </summary>
        public SectionFormat()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionFormat"/> class.
        /// </summary>
        /// <param name="sectionAdv">The section adv.</param>
        public SectionFormat(SectionAdv sectionAdv)
            : base(sectionAdv)
        {
        }
        #endregion

        #region Static Events
        /// <summary>
        /// Called when page size changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnPageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SectionFormat).Relayout();
        }
        /// <summary>
        /// Called when page margin changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPageMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SectionFormat).Relayout();
        }
        /// <summary>
        /// Called when header distance changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHeaderDistanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SectionFormat).Relayout();
        }
        /// <summary>
        /// Called when footer distance changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnFooterDistanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SectionFormat).Relayout();
        }
        /// <summary>
        /// Called when different first page changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDifferentFirstPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SectionFormat).Relayout();
        }
        /// <summary>
        /// Called when different odd and even pages changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDifferentOddAndEvenPagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SectionFormat).Relayout();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Applies the property value.
        /// </summary>
        /// <param name="historyInfo">The history info.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        internal void ApplyPropertyValue(HistoryInfo historyInfo, DependencyProperty property, object value)
        {
            if (historyInfo != null)
                value = historyInfo.AddModifiedProperties(this, property, value);
            if (value == DependencyProperty.UnsetValue)
            {
                ClearValue(property);
                return;
            }
            if (property == PageMarginProperty)
                PageMargin = (Thickness)value;
            else if (property == PageSizeProperty)
                PageSize = (Size)value;
        }
        /// <summary>
        /// Relayouts this instance.
        /// </summary>
        internal void Relayout()
        {
            if (OwnerBase is SectionAdv && (OwnerBase as SectionAdv).BaseParent != null)
            {
                double verticalScrolValue = 0;
                double horizontalScrolValue = 0;
                if ((OwnerBase as SectionAdv).BaseParent != null
                    && (OwnerBase as SectionAdv).BaseParent.IsDocumentLoaded
                    && (OwnerBase as SectionAdv).BaseParent.IsLayoutEnabled)
                {
                    verticalScrolValue = (OwnerBase as SectionAdv).BaseParent.verticalScrollBar.Value;
                    horizontalScrolValue = (OwnerBase as SectionAdv).BaseParent.horizontalScrollBar.Value;
                }
                (OwnerBase as SectionAdv).Document.LayoutItems();
                if ((OwnerBase as SectionAdv).BaseParent != null
                    && (OwnerBase as SectionAdv).BaseParent.IsDocumentLoaded
                    && (OwnerBase as SectionAdv).BaseParent.IsLayoutEnabled)
                {
                    (OwnerBase as SectionAdv).BaseParent.verticalScrollBar.Value = verticalScrolValue;
                    (OwnerBase as SectionAdv).BaseParent.horizontalScrollBar.Value = horizontalScrolValue;
                }
            }
        }
        /// <summary>
        /// Copies the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CopyFormat(SectionFormat format)
        {
            if (format.ReadLocalValue(PageSizeProperty) is Size)
                PageSize = format.PageSize;
            if (format.ReadLocalValue(PageMarginProperty) is Thickness)
                PageMargin = format.PageMargin;
            if (format.ReadLocalValue(HeaderDistanceProperty) is double)
                HeaderDistance = format.HeaderDistance;
            if (format.ReadLocalValue(FooterDistanceProperty) is double)
                FooterDistance = format.FooterDistance;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            SetOwner(null);
            ClearValue(PageSizeProperty);
            ClearValue(PageMarginProperty);
            ClearValue(HeaderDistanceProperty);
            ClearValue(FooterDistanceProperty);
            ClearValue(DifferentFirstPageProperty);
            ClearValue(DifferentOddAndEvenPagesProperty);
        }
        #endregion
    }
    public sealed class TableFormat : BaseNode
    {
        #region Properties
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        internal DocumentAdv Document
        {
            get
            {
                if (OwnerBase is TableAdv
                    && (OwnerBase as TableAdv).Document != null)
                    return (OwnerBase as TableAdv).Document;
                return null;
            }
        }
        /// <summary>
        /// Gets or Sets the table left indent.
        /// </summary>
        /// <value>
        /// The left indent.
        /// </value>
        public double LeftIndent
        {
            get
            {
                return (double)GetValue(LeftIndentProperty);
            }
            set
            {
                SetValue(LeftIndentProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the table background color.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public Color Background
        {
            get { return (Color)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }
        //public Borders Borders
        //{
        //    get { return (Borders)GetValue(BordersProperty); }
        //    set { SetValue(BordersProperty, value); }
        //}
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Identifies the Background dependency property.
        /// </summary>
        /// <returns>The identifier of the Background dependency property.</returns>
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Color), typeof(TableFormat), new PropertyMetadata(Color.FromArgb(0, 0, 0, 0), OnBackgroundChanged));
        /// <summary>
        /// Identifies the LeftIndent dependency property.
        /// </summary>
        /// <returns>The identifier of the LeftIndent dependency property.</returns>
        public static readonly DependencyProperty LeftIndentProperty = DependencyProperty.Register("LeftIndent", typeof(double), typeof(TableFormat), new PropertyMetadata(0d, OnLeftIndentChanged));
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableFormat"/> class.
        /// </summary>
        public TableFormat()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TableFormat" /> class.
        /// </summary>
        /// <param name="baseNode">The base node.</param>
        public TableFormat(BaseNode baseNode)
            : base(baseNode)
        {
        }
        #endregion

        #region Static Events
        /// <summary>
        /// Called when left indent changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLeftIndentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        /// <summary>
        /// Called when background changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Copies the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CopyFormat(TableFormat format)
        {
            if (format.ReadLocalValue(LeftIndentProperty) is double)
                LeftIndent = format.LeftIndent;
            if (format.ReadLocalValue(BackgroundProperty) is Color)
                Background = format.Background;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            SetOwner(null);
            ClearValue(BackgroundProperty);
            ClearValue(LeftIndentProperty);
        }
        #endregion
    }
    internal sealed class Borders : BaseNode
    {
        Node node;
        public Border Left
        {
            get { return (Border)GetValue(LeftBorderProperty); }
            set { SetValue(LeftBorderProperty, value); }
        }

        public Border Right
        {
            get { return (Border)GetValue(RightBorderProperty); }
            set { SetValue(RightBorderProperty, value); }
        }

        public Border Top
        {
            get { return (Border)GetValue(TopBorderProperty); }
            set { SetValue(TopBorderProperty, value); }
        }

        public Border Bottom
        {
            get { return (Border)GetValue(BottomBorderProperty); }
            set { SetValue(BottomBorderProperty, value); }
        }

        public Borders()
            : this(null)
        {
        }
        public Borders(BaseNode baseNode)
            : base(baseNode)
        {
        }

        #region Static Dependency Properties
        internal static readonly DependencyProperty LeftBorderProperty = DependencyProperty.Register("Left", typeof(Border), typeof(Borders), new PropertyMetadata(new Border(), OnLeftBorderPropertyChanged));
        internal static readonly DependencyProperty RightBorderProperty = DependencyProperty.Register("Right", typeof(Border), typeof(Borders), new PropertyMetadata(new Border(), OnRightBorderPropertyChanged));
        internal static readonly DependencyProperty TopBorderProperty = DependencyProperty.Register("Top", typeof(Border), typeof(Borders), new PropertyMetadata(new Border(), OnTopBorderPropertyChanged));
        internal static readonly DependencyProperty BottomBorderProperty = DependencyProperty.Register("Bottom", typeof(Border), typeof(Borders), new PropertyMetadata(new Border(), OnBottomBorderPropertyChanged));
        #endregion

        private static void OnLeftBorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        private static void OnRightBorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        private static void OnTopBorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        private static void OnBottomBorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
    internal sealed class RowFormat : BaseNode
    {
        #region Properties
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        internal DocumentAdv Document
        {
            get
            {
                if (OwnerBase is TableRowAdv
                     && (OwnerBase as TableRowAdv).OwnerTable != null
                    && (OwnerBase as TableRowAdv).OwnerTable.Document != null)
                    return (OwnerBase as TableRowAdv).OwnerTable.Document;
                return null;
            }
        }
        #endregion

        #region Static Dependency Properties
        //public static readonly DependencyProperty PageSizeProperty = DependencyProperty.Register("PageSize", typeof(Size), typeof(SectionFormat), new PropertyMetadata(new Size(612, 792), OnPageSizeChanged));
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RowFormat"/> class.
        /// </summary>
        public RowFormat()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="RowFormat" /> class.
        /// </summary>
        /// <param name="baseNode">The base node.</param>
        public RowFormat(BaseNode baseNode)
            : base(baseNode)
        {
        }
        #endregion

        #region Static Events
      
        #endregion

        #region Implementations
        /// <summary>
        /// Copies the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CopyFormat(RowFormat format)
        {
            //if (format.ReadLocalValue(PageSizeProperty) is Size)
            //    PageSize = format.PageSize;
            //if (format.ReadLocalValue(PageMarginProperty) is Thickness)
            //    PageMargin = format.PageMargin;
            //if (format.ReadLocalValue(HeaderDistanceProperty) is double)
            //    HeaderDistance = format.HeaderDistance;
            //if (format.ReadLocalValue(FooterDistanceProperty) is double)
            //    FooterDistance = format.FooterDistance;
        }
        #endregion
    }
    public sealed class CellFormat : BaseNode
    {
        #region Properties
        /// <summary>
        /// Gets or Sets the cell width.
        /// </summary>
        /// <value>
        /// The width of the cell.
        /// </value>
        public double CellWidth
        {
            get
            {
                return (double)GetValue(CellWidthProperty);
            }
            set
            {
                SetValue(CellWidthProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the cell margin.
        /// </summary>
        /// <value>
        /// The cell margin.
        /// </value>
        public Thickness CellMargin
        {
            get
            {
                return (Thickness)GetValue(CellMarginProperty);
            }
            set
            {
                SetValue(CellMarginProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the column span.
        /// </summary>
        /// <value>
        /// The column span.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">ColumnSpan value must be greater than 1</exception>
        public int ColumnSpan
        {
            get
            {
                return (int)GetValue(ColumnSpanProperty);
            }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("ColumnSpan value must be greater than 1");
                SetValue(ColumnSpanProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the row span.
        /// </summary>
        /// <value>
        /// The row span.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">RowSpan value must be greater than 1</exception>
        public int RowSpan
        {
            get
            {
                return (int)GetValue(RowSpanProperty);
            }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("RowSpan value must be greater than 1");
                SetValue(RowSpanProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the cell background color.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public Color Background
        {
            get
            {
                return (Color)GetValue(BackgroundProperty);
            }
            set
            {
                SetValue(BackgroundProperty, value);
            }
        }
        #endregion

        #region Static Dependency Properties
        /// <summary>
        /// Identifies the CellWidth dependency property.
        /// </summary>
        /// <returns>The identifier of the CellWidth dependency property.</returns>
        public static readonly DependencyProperty CellWidthProperty = DependencyProperty.Register("CellWidth", typeof(double), typeof(CellFormat), new PropertyMetadata(0d, OnCellWidthChanged));
        /// <summary>
        /// Identifies the CellMargin dependency property.
        /// </summary>
        /// <returns>The identifier of the CellMargin dependency property.</returns>
        public static readonly DependencyProperty CellMarginProperty = DependencyProperty.Register("CellMargin", typeof(Thickness), typeof(CellFormat), new PropertyMetadata(new Thickness(0), OnCellMarginChanged));
        /// <summary>
        /// Identifies the ColumnSpan dependency property.
        /// </summary>
        /// <returns>The identifier of the ColumnSpan dependency property.</returns>
        public static readonly DependencyProperty ColumnSpanProperty = DependencyProperty.Register("ColumnSpan", typeof(int), typeof(CellFormat), new PropertyMetadata(1, OnColumnSpanChanged));
        /// <summary>
        /// Identifies the RowSpan dependency property.
        /// </summary>
        /// <returns>The identifier of the RowSpan dependency property.</returns>
        public static readonly DependencyProperty RowSpanProperty = DependencyProperty.Register("RowSpan", typeof(int), typeof(CellFormat), new PropertyMetadata(1, OnRowSpanChanged));
        /// <summary>
        /// Identifies the Background dependency property.
        /// </summary>
        /// <returns>The identifier of the Background dependency property.</returns>
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Color), typeof(CellFormat), new PropertyMetadata(Color.FromArgb(0, 0, 0, 0), OnBackgroundChanged));
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CellFormat"/> class.
        /// </summary>
        public CellFormat()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CellFormat" /> class.
        /// </summary>
        /// <param name="baseNode">The base node.</param>
        public CellFormat(BaseNode baseNode)
            : base(baseNode)
        {
        }
        #endregion

        #region Static Events
        /// <summary>
        /// Called when cell width changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCellWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        /// <summary>
        /// Called when cell margin changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCellMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        /// <summary>
        /// Called when column span changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnColumnSpanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        /// <summary>
        /// Called when row span changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRowSpanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        /// <summary>
        /// Called when background changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Copies the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CopyFormat(CellFormat format)
        {
            if (format.ReadLocalValue(CellWidthProperty) is double)
                CellWidth = format.CellWidth;
            if (format.ReadLocalValue(CellMarginProperty) is Thickness)
                CellMargin = format.CellMargin;
            if (format.ReadLocalValue(ColumnSpanProperty) is int)
                ColumnSpan = format.ColumnSpan;
            if (format.ReadLocalValue(RowSpanProperty) is int)
                RowSpan = format.RowSpan;
            if (format.ReadLocalValue(BackgroundProperty) is Color)
                Background = format.Background;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            SetOwner(null);
            ClearValue(BackgroundProperty);
            ClearValue(RowSpanProperty);
            ClearValue(ColumnSpanProperty);
            ClearValue(CellMarginProperty);
            ClearValue(CellWidthProperty);
        }
        #endregion
    }
}
