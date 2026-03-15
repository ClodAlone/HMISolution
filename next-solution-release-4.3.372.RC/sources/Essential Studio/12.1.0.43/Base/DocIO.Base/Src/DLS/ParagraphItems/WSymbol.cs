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
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
#if !SILVERLIGHT && !WP
using Syncfusion.DocIO.Rendering;
using Syncfusion.Layouting;
using System.Windows;
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WSymbol.
    /// </summary>
    public class WSymbol : ParagraphItem
#if !SILVERLIGHT && !WP
    	,ILeafWidget
#endif
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private string m_fontName = "Symbol";
        private byte m_charCode = 0;
        private byte m_charCodeExt = 0;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.Symbol;
            }
        }
        /// <summary>
        /// Gets character format for the symbol.
        /// </summary>
        public WCharacterFormat CharacterFormat
        {
            get
            {
                return m_charFormat;
            }
        }
        /// <summary>
        /// Gets / sets symbol font name.
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
            }
        }
        /// <summary>
        /// Gets / sets symbol's character code.
        /// </summary>
        public byte CharacterCode
        {
            get
            {
                return m_charCode;
            }
            set
            {
                m_charCode = value;
            }
        }
        /// <summary>
        /// Get/set character code extension.
        /// </summary>
        internal byte CharCodeExt
        {
            get
            {
                return m_charCodeExt;
            }
            set
            {
                m_charCodeExt = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializing constructor.
        /// </summary>
        public WSymbol(IWordDocument doc)
            : base((WordDocument)doc)
        {
            m_charFormat = new WCharacterFormat(doc);
            m_charFormat.SetOwner(this);
        }
        #endregion

        #region Class overrides
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo();
            m_layoutInfo.IsSkip = false;
        }
#endif
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WSymbol sm = (WSymbol)base.CloneImpl();
            return sm;
        }
        #endregion

        #region Class XDLSSerializable implementation
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        protected override void InitXDLSHolder()
        {
            base.InitXDLSHolder();

            XDLSHolder.AddElement(XDLSConstants.CharacterFormatTag, m_charFormat);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            writer.WriteValue(XDLSConstants.TypeTag, ParagraphItemType.Symbol);
            writer.WriteValue(XDLSConstants.TextFontNameAttr, FontName);
            writer.WriteValue(XDLSConstants.SymbolCharCodeAttr, CharacterCode);
            writer.WriteValue(XDLSConstants.SymbolCharCodeExtAttr, CharCodeExt);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.TextFontNameAttr))
            {
                FontName = reader.ReadString(XDLSConstants.TextFontNameAttr);
            }
            if (reader.HasAttribute(XDLSConstants.SymbolCharCodeAttr))
            {
                CharacterCode = reader.ReadByte(XDLSConstants.SymbolCharCodeAttr);
            }
            if (reader.HasAttribute(XDLSConstants.SymbolCharCodeExtAttr))
            {
                CharCodeExt = reader.ReadByte(XDLSConstants.SymbolCharCodeExtAttr);
            }
        }
//#endif
        #endregion

        #region ILeafWidget Members
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets the size of the symbol for lay outing.
        /// </summary>
        /// <param name="dc"></param>
        /// <returns></returns>
        SizeF ILeafWidget.Measure(DrawingContext dc)
        {
            string text = Char.ConvertFromUtf32(this.CharacterCode);
            WCharacterFormat charFormat = new WCharacterFormat(this.Document);
            if (!this.CharacterFormat.HasValue(WCharacterFormat.FontKey) && this.FontName != string.Empty && this.FontName != this.CharacterFormat.FontName)
            {
                charFormat.ImportContainer(this.CharacterFormat);
                charFormat.CopyProperties(this.CharacterFormat);
                charFormat.ApplyBase(this.OwnerParagraph.BreakCharacterFormat.BaseFormat);
                charFormat.FontName = this.FontName;
                return dc.MeasureString(text, dc.GetFont(charFormat, text), null, charFormat,false);
            }
            return dc.MeasureString(text, dc.GetFont(this.CharacterFormat, text), null, this.CharacterFormat,false);
        }
        /// <summary>
        /// Get Symbol font
        /// </summary>
        /// <returns></returns>
        internal Font GetFont(DrawingContext dc)
        {
            if (!this.CharacterFormat.HasValue(WCharacterFormat.FontKey) && this.FontName != string.Empty && this.FontName != this.CharacterFormat.FontName)
            {
                return new Font(this.FontName, this.CharacterFormat.FontSize, this.CharacterFormat.Font.Style);
            }
            return this.CharacterFormat.Font;
        }
        /// <summary>
        /// Draws the symbol
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="ltWidget"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        void IWidget.Draw(Syncfusion.DocIO.Rendering.DrawingContext dc, LayoutedWidget ltWidget)
        {
            dc.DrawSymbol(this, ltWidget);
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }

#endif

        #endregion
    }
}
