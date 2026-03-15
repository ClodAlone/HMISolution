#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
#if !WINRT && !WP
using System.Drawing;
#endif
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.Layouting;
using IXDLSAttributeReader = Syncfusion.DocIO.DLS.XML.IXDLSAttributeReader;
using IXDLSAttributeWriter = Syncfusion.DocIO.DLS.XML.IXDLSAttributeWriter;
#if (SILVERLIGHT || WP)&& !SkipSilverlightNamespaces
#if WINRT
using Color = Syncfusion.DocIO.DLS.Color;
using Syncfusion.DocIO.WinrtHelper;
using System.Windows;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Media;
#if WP
using Color = Syncfusion.DocIO.DLS.Color;
#else
using Color = System.Drawing.Color;
#endif
#endif
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for Watermark.
    /// </summary>
    public class TextWatermark : Watermark
    {
        #region Fields
        /// <summary>
        /// Text watermark members.
        /// </summary>
        private string m_text = "Urgent";
        private string m_fontName = "Times New Roman";
        private float m_fontSize = 36;
        private Color m_fontColor = Color.Gray;
        private bool m_semitransparent = true;
        private WatermarkLayout m_layout;

        private int m_shapeHeigh = -1;
        private int m_shapeWidth = -1;
        #endregion

        #region Properties
        /// <summary>
        /// Get/set watermark text
        /// </summary>
        public string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
            }
        }
        /// <summary>
        /// Get/set watermark text's font name. 
        /// </summary>
        public string FontName
        {
            get
            {
                return m_fontName;
            }
            set
            {
                m_fontName = value;
                m_shapeHeigh = -1;
                m_shapeWidth = -1;
            }
        }
        /// <summary>
        /// Get/set text watermark size.
        /// </summary>
        public float Size
        {
            get
            {
                return m_fontSize;
            }
            set
            {
                m_fontSize = value;
                m_shapeHeigh = -1;
                m_shapeWidth = -1;
            }
        }
        /// <summary>
        /// Get/set text watermark color.
        /// </summary>
        public Color Color
        {
            get
            {
                return m_fontColor;
            }
            set
            {
                m_fontColor = value;
            }
        }
        /// <summary>
        /// Get/set semitransparent property for Text watermark.
        /// </summary>
        public bool Semitransparent
        {
            get
            {
                return m_semitransparent;
            }
            set
            {
                m_semitransparent = value;
            }
        }
        /// <summary>
        /// Get/set layout for Text watermark.
        /// </summary>
        public WatermarkLayout Layout
        {
            get
            {
                return m_layout;
            }
            set
            {
                m_layout = value;
            }
        }
        /// <summary>
        /// Get/set shape height in pixels.
        /// </summary>
        internal int ShapeHeightInPixels
        {
            get
            {
                return m_shapeHeigh;
            }
            set
            {
                m_shapeHeigh = value;
            }
        }
        /// <summary>
        /// Get/set shape width in pixels.
        /// </summary>
        internal int ShapeWidthInPixels
        {
            get
            {
                return m_shapeWidth;
            }
            set
            {
                m_shapeWidth = value;
            }
        }

        /// <summary>
        /// Gets shape size.
        /// </summary>
        /// <value>The size of the shape.</value>
        internal SizeF ShapeSize
        {
            get
            {
                return GetShapeSize();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TextWatermark"/> class.
        /// </summary>
        public TextWatermark()
            : base(WatermarkType.TextWatermark)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextWatermark"/> class.
        /// </summary>
        /// <param name="text">Watermark text</param>
        public TextWatermark(string text) :
            base(WatermarkType.TextWatermark)
        {
            m_text = text;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextWatermark"/> class.
        /// </summary>
        /// <param name="text">Watermark text</param>
        /// <param name="fontName">Text font name</param>
        /// <param name="fontSize">Font size</param>
        /// <param name="layout">Watermark layout</param>
        public TextWatermark(string text, string fontName, int fontSize, WatermarkLayout layout) :
            base(WatermarkType.TextWatermark)
        {
            m_text = text;
            m_fontName = fontName;
            m_fontSize = fontSize;
            m_layout = layout;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextWatermark"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal TextWatermark(WordDocument doc)
            : base(doc, WatermarkType.TextWatermark)
        { }

        #endregion

        #region Class overrides
//#if !SILVERLIGHT
        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);
            if (reader.HasAttribute(XDLSConstants.WatermarkTextAttr))
            {
                m_text = reader.ReadString(XDLSConstants.WatermarkTextAttr);
            }
            if (reader.HasAttribute(XDLSConstants.WatermarkTextFontNameAttr))
            {
                m_fontName = reader.ReadString(XDLSConstants.WatermarkTextFontNameAttr);
            }
            if (reader.HasAttribute(XDLSConstants.WatermarkTextFontSizeAttr))
            {
                m_fontSize = reader.ReadFloat(XDLSConstants.WatermarkTextFontSizeAttr);
            }
            if (reader.HasAttribute(XDLSConstants.WatermarkTextLayoutAttr))
            {
                m_layout = (WatermarkLayout)reader.ReadEnum(XDLSConstants.WatermarkTextLayoutAttr, typeof(WatermarkLayout));
            }
            if (reader.HasAttribute(XDLSConstants.WatermarkTextSemitransAttr))
            {
                m_semitransparent = reader.ReadBoolean(XDLSConstants.WatermarkTextSemitransAttr);
            }
            if (reader.HasAttribute(XDLSConstants.WatermarkTextFontColorAttr))
            {
                m_fontColor = reader.ReadColor(XDLSConstants.WatermarkTextFontColorAttr);
            }
            if (reader.HasAttribute(XDLSConstants.WatermarkShapeHeightAttr))
            {
                m_shapeHeigh = reader.ReadInt(XDLSConstants.WatermarkShapeHeightAttr);
            }
            if (reader.HasAttribute(XDLSConstants.WatermarkShapeWidthAttr))
            {
                m_shapeWidth = reader.ReadInt(XDLSConstants.WatermarkShapeWidthAttr);
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            writer.WriteValue(XDLSConstants.WatermarkTextAttr, m_text);
            writer.WriteValue(XDLSConstants.WatermarkTextFontNameAttr, m_fontName);
            writer.WriteValue(XDLSConstants.WatermarkTextFontSizeAttr, m_fontSize);
            writer.WriteValue(XDLSConstants.WatermarkTextLayoutAttr, m_layout);
            if (m_fontColor != Color.Gray)
            {
                writer.WriteValue(XDLSConstants.WatermarkTextFontColorAttr, m_fontColor);
            }
            if (!m_semitransparent)
            {
                writer.WriteValue(XDLSConstants.WatermarkTextSemitransAttr, m_semitransparent);
            }
            if (m_shapeHeigh != 0)
            {
                writer.WriteValue(XDLSConstants.WatermarkShapeHeightAttr, m_shapeHeigh);
            }
            if (m_shapeWidth != 0)
            {
                writer.WriteValue(XDLSConstants.WatermarkShapeWidthAttr, m_shapeWidth);
            }
        }
//#endif
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the size of the shape
        /// </summary>
        /// <returns></returns>
        private SizeF GetShapeSize()
        {
#if (SILVERLIGHT || WP) && !SkipSilverlightNamespaces
            return UnitsConvertor.MeasureString(m_text, m_fontSize, m_fontName);
#else
#if SkipSilverlightNamespaces
            return new SizeF( 0, 0 );
#else
            FontStyle style = FontStyle.Regular;
            Font textWatermarkFont = new Font(m_fontName, m_fontSize, style);
            return UnitsConvertor.Instance.EmptyGraphics.MeasureString(m_text, textWatermarkFont);
#endif
#endif
        }
        #endregion
    }
}

