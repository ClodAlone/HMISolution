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
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Syncfusion.RDL.DOM
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class Style
    {
        #region DefaultDomValues

        private Size default_paddingLeft = "0pt"; // Default: 0pt. Max 1000pt
        private Size default_paddingRight = "0pt"; // Default: 0pt. Max 1000pt
        private Size default_paddingTop = "0pt"; // Default: 0pt. Max 1000pt
        private Size default_paddingBottom = "0pt"; // Default: 0pt. Max 1000pt
        private Size default_lineHeight = "10pt"; // Min 1pt, Max 1000pt. Default: Renderer determines the height of the text line based on the font size.

        private Size default_fontSize = "10pt";        

        private string m_backgroundColor = "Transparent"; // Expression, Color
        private BackgroundGradientTypes m_backgroundGradientType = BackgroundGradientTypes.Default;
        private string m_backgroundGradientEndColor = null; // Expression, Color
        private string m_fontStyle = RDL.DOM.FontStyle.Default.ToString();
        private string m_fontFamily = "Arial"; // Expression. Name of the Font
        private Size m_fontSize = "10pt"; // Default 10pt. Min 1pt, Max 200pt;
        private string m_fontWeight = RDL.DOM.FontWeight.Default.ToString();
        private string m_format = null; // Expression. Default: No Formatting 
        private string m_textDecoration = RDL.DOM.TextDecoration.Default.ToString();
        private string m_textAlign = "Default";
        private string m_verticalAlign = "Default";
        private string m_color = "Black"; // Default: Black (except within ChartDataPoint and ChartSeries, where the default is to use the palette colors).
        private Size m_paddingLeft = "0pt"; // Default: 0pt. Max 1000pt
        private Size m_paddingRight = "0pt"; // Default: 0pt. Max 1000pt
        private Size m_paddingTop = "0pt"; // Default: 0pt. Max 1000pt
        private Size m_paddingBottom = "0pt"; // Default: 0pt. Max 1000pt
        private Size m_lineHeight = "10pt"; // Min 1pt, Max 1000pt. Default: Renderer determines the height of the text line based on the font size.
        private Direction m_direction = Direction.Default;
        private string m_writingMode = "Default";
        private string m_language = null; // Expression. Language Type. The primary Language of the text. Default is ReportDefinition Language. Used for text formatting operations for: Textbox.Value, DataLabel.Value, ChartMember.Label and DataValue.Value
        private Calendar m_calendar = Calendar.Default;
        private string m_numeralLanguage = null; // Expression. Language Type. The digit format to use as described by its primaryLanguage. Any Language is valid. Default is the Language property.
        private TextEffects m_textEffect = TextEffects.Default;
        private BackgroundHatchTypes m_backgroundHatchType = BackgroundHatchTypes.Default;
        private string m_shadowColor = null; // Expression. Shadow Color
        //private Size m_shadowOffset = null; // Expression. Default 0. Considered as 0pt. Metric not specified in document implicitly.   eg
        #endregion

        [DefaultValue("Transparent")]
        public string BackgroundColor
        {
            get { return m_backgroundColor; }
            set { m_backgroundColor = value; }
        }

        [DefaultValue("Arial")]
        public string FontFamily
        {
            get { return m_fontFamily; }
            set { m_fontFamily = value; }
        }

        public Size FontSize
        {
            get { return m_fontSize; }
            set { m_fontSize = value; }
        }

        [DefaultValue("Default")]
        public string FontWeight
        {
            get { return m_fontWeight; }
            set { m_fontWeight = value; }
        }

        [DefaultValue(BackgroundGradientTypes.Default)]
        public BackgroundGradientTypes BackgroundGradientType
        {
            get { return m_backgroundGradientType; }
            set { m_backgroundGradientType = value; }
        }
 
        public string BackgroundGradientEndColor
        {
            get { return m_backgroundGradientEndColor; }
            set { m_backgroundGradientEndColor = value; }
        }

        [DefaultValue("Default")]
        public string FontStyle
        {
            get { return m_fontStyle; }
            set { m_fontStyle = value; }
        }

        public string Format
        {
            get { return m_format; }
            set { m_format = value; }
        }

        [DefaultValue("Default")]
        public string TextDecoration
        {
            get { return m_textDecoration; }
            set { m_textDecoration = value; }
        }

        [DefaultValue("Default")]
        public string TextAlign
        {
            get { return m_textAlign; }
            set { m_textAlign = value; }
        }

        [DefaultValue("Default")]
        public string VerticalAlign
        {
            get { return m_verticalAlign; }
            set { m_verticalAlign = value; }
        }

        [DefaultValue("Black")]
        public string Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        public Size PaddingLeft
        {
            get { return m_paddingLeft; }
            set { m_paddingLeft = value; }
        }

        public Size PaddingRight
        {
            get { return m_paddingRight; }
            set { m_paddingRight = value; }
        }

        public Size PaddingTop
        {
            get { return m_paddingTop; }
            set { m_paddingTop = value; }
        }

        public Size PaddingBottom
        {
            get { return m_paddingBottom; }
            set { m_paddingBottom = value; }
        }

        public Size LineHeight
        {
            get { return m_lineHeight; }
            set { m_lineHeight = value; }
        }

        [DefaultValue(Direction.Default)]
        public Direction Direction
        {
            get { return m_direction; }
            set { m_direction = value; }
        }

        [DefaultValue("Default")]
        public string WritingMode
        {
            get { return m_writingMode; }
            set { m_writingMode = value; }
        }
        
        public string Language
        {
            get { return m_language; }
            set { m_language = value; }
        }

        [DefaultValue(Calendar.Default)]
        public Calendar Calendar
        {
            get { return m_calendar; }
            set { m_calendar = value; }
        }

        public string NumeralLanguage
        {
            get { return m_numeralLanguage; }
            set { m_numeralLanguage = value; }
        }

        [DefaultValue(TextEffects.Default)]
        public TextEffects TextEffect
        {
            get { return m_textEffect; }
            set { m_textEffect = value; }
        }

        [DefaultValue(BackgroundHatchTypes.Default)]
        public BackgroundHatchTypes BackgroundHatchType
        {
            get { return m_backgroundHatchType; }
            set { m_backgroundHatchType = value; }
        }

        public string ShadowColor
        {
            get { return m_shadowColor; }
            set { m_shadowColor = value; }
        }

        public Border Border { get; set; }
        public TopBorder TopBorder { get; set; }
        public BottomBorder BottomBorder { get; set; }
        public LeftBorder LeftBorder { get; set; }
        public RightBorder RightBorder { get; set; }
        public BackgroundImage BackgroundImage { get; set; }
        public string NumeralVariant { get; set; }
        public string ShadowOffset { get; set; }

        public bool ShouldSerializeFontWeight()
        {
            return this.FontWeight != null && (!this.FontWeight.Equals(m_fontWeight) || !this.FontWeight.Equals(RDL.DOM.FontWeight.Normal.ToString()));
        }

        public void ResetFontWeight()
        {
            this.FontWeight = m_fontWeight;
        }

        public bool ShouldSerializeFontSize()
        {
            return this.FontSize != null && !this.FontSize.Equals(default_fontSize);
        }

        public void ResetFontSize()
        {
            this.FontSize = default_fontSize;
        }

        public bool ShouldSerializePaddingBottom()
        {
            return this.PaddingBottom != null && !this.PaddingBottom.Equals(default_paddingBottom);
        }

        public void ResetPaddingBottom()
        {
            this.PaddingBottom = default_paddingBottom;
        }

        public bool ShouldSerializePaddingTop()
        {
            return this.PaddingTop != null && !this.PaddingTop.Equals(default_paddingTop);
        }

        public void ResetPaddingTop()
        {
            this.PaddingTop = default_paddingTop;
        }

        public bool ShouldSerializePaddingRight()
        {
            return this.PaddingRight != null && !this.PaddingRight.Equals(default_paddingRight);
        }

        public void ResetPaddingRight()
        {
            this.PaddingRight = default_paddingRight;
        }

        public bool ShouldSerializePaddingLeft()
        {
            return this.PaddingLeft !=null && !this.PaddingLeft.Equals(default_paddingLeft);
        }

        public void ResetPaddingLeft()
        {
            this.PaddingLeft = default_paddingLeft;
        }

        public bool ShouldSerializeLineHeight()
        {
            return this.LineHeight != null && (!this.LineHeight.Equals(m_lineHeight));
        }

        public void ResetLineHeight()
        {
            this.LineHeight = this.m_lineHeight;
        }
    }
}
