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
using System.Collections;
using Syncfusion.DocIO.DLS;
using System.Windows;
using Syncfusion.DocIO.DLS.XML;
#if !SILVERLIGHT && !WP
using Syncfusion.DocIO.Rendering;
using Syncfusion.Layouting;
using System.Drawing;
using Syncfusion.DocIO.DLS.Rendering;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a document footnote or endnote.
    /// </summary>
    public class WFootnote
      : ParagraphItem
#if !SILVERLIGHT && !WP
      , ILeafWidget
#endif
    {
        #region Constants
        internal const int DEF_FTNSTYLE_REF_ID = 38;
        internal const int DEF_EDNSTYLE_REF_ID = 39;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private FootnoteType m_footnoteType = FootnoteType.Footnote;
        /// <summary>
        /// 
        /// </summary>
        private WTextBody m_textBody;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsAutoNumbered = true;
        /// <summary>
        /// 
        /// </summary>
        private byte m_symbolCode = 0;
        /// <summary>
        /// 
        /// </summary>
        private string m_strSymbolFontName = "Symbol";
        /// <summary>
        /// 
        /// </summary>
        private string m_strCustomMarker = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        private int m_changesCount;
        /// <summary>
        ///Indicate whether the endnote instance layouted or not. 
        /// </summary>
        private bool m_isLayouted=false;
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
                return EntityType.Footnote;
            }
        }
        /// <summary>
        /// Gets / sets footnote type: footnote or endnote
        /// </summary>
        public FootnoteType FootnoteType
        {
            get
            {
                return m_footnoteType;
            }
            set
            {
                m_footnoteType = value;
            }
        }
        /// <summary>
        /// Gets the value indicating if the footnote is auto numbered
        /// </summary>
        public bool IsAutoNumbered
        {
            get
            {
                return m_bIsAutoNumbered;
            }
            set
            {
                if (m_bIsAutoNumbered != value)
                {
                    if (!Document.IsOpening)
                    {
                        UpdateChangeFlag(true);
                        UpdateAutoMarker(value);
                    }
                    m_bIsAutoNumbered = value;
                }
            }
        }
        /// <summary>
        /// Gets the text body of the footnote.
        /// </summary>
        /// <value>The text body.</value>
        public WTextBody TextBody
        {
            get
            {
                return m_textBody;
            }
        }
        /// <summary>
        /// Gets the marker character format
        /// </summary>
        public WCharacterFormat MarkerCharacterFormat
        {
            get
            {
                return m_charFormat;
            }
        }
        /// <summary>
        /// Gets or sets the marker symbol code.
        /// </summary>
        /// <value>The symbol code.</value>
        public byte SymbolCode
        {
            get
            {
                return m_symbolCode;
            }
            set
            {
                if (value != m_symbolCode && !Document.IsOpening)
                {
                    UpdateChangeFlag(true);
                    UpdateSymbolMarker(value);
                }
                m_symbolCode = value;
            }
        }
        /// <summary>
        /// Gets or sets the name of the marker symbol font.
        /// </summary>
        /// <value>The name of the symbol font.</value>
        internal string SymbolFontName
        {
            get
            {
                return m_strSymbolFontName;
            }
            set
            {
                if (value != m_strSymbolFontName && !Document.IsOpening)
                {
                    UpdateChangeFlag(true);
                }
                m_strSymbolFontName = value;
            }
        }
        /// <summary>
        /// Gets or sets the custom footnote marker.
        /// </summary>
        /// <value>The custom marker.</value>
        public string CustomMarker
        {
            get
            {
                return m_strCustomMarker;
            }
            set
            {
                if (value != m_strCustomMarker && !m_bIsAutoNumbered && !Document.IsOpening)
                {
                    UpdateChangeFlag(true);
                    UpdateCustomMarker(value);
                }
                m_strCustomMarker = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool CustomMarkerIsSymbol
        {
            get
            {
                return m_symbolCode > 0;
            }
        }
        /// <summary>
        /// indicate whether the endnote instance layouted or not. 
        /// </summary>
        internal bool IsLayouted
        {
            get
            {
                return m_isLayouted;
            }
            set 
            {
                m_isLayouted = value;
            }
        }
        #endregion

        #region Class Initialise / Finalise methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WFootnote"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public WFootnote(IWordDocument doc)
            : base((WordDocument)doc)
        {
            m_textBody = new WTextBody(Document, this);
            m_charFormat = new WCharacterFormat(Document);
            m_charFormat.SetOwner(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="marker"></param>
        internal WFootnote(IWordDocument doc, string marker)
            : this(doc)
        {
            m_strCustomMarker = marker;
            m_bIsAutoNumbered = false;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        {
            if (m_textBody != null)
                m_textBody.AddSelf();
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Creates the layout info.
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutFootnoteInfoImpl(this);
        }
#endif
        /// <summary>
        /// Clones the specified paragraph.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WFootnote foot = (WFootnote)base.CloneImpl();
            foot.m_textBody = (WTextBody)m_textBody.Clone();
            foot.m_textBody.SetOwner(foot);
            return foot;
        }
        /// <summary>
        /// "Listener" for state change.
        /// </summary>
        /// <param name="sender"></param>
        internal override void OnStateChange(object sender)
        {
            if (sender is WCharacterFormat)
            {
                UpdateChangeFlag(true);
            }
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            base.Close();
            if (m_textBody != null)
            {
                m_textBody.Close();
                m_textBody = null;
            }
        }
        #endregion

        #region IXDLSSerializable implement
//#if !SILVERLIGHT
        /// <summary>
        /// Initialize the XDLS holder.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddElement(XDLSConstants.TextBodyTag, m_textBody);
            XDLSHolder.AddElement(XDLSConstants.FootnoteMarkerCPTag, m_charFormat);
        }
        /// <summary>
        /// Writes the XML attributes.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteValue(XDLSConstants.TypeTag, FootnoteType.Footnote);
            writer.WriteValue(XDLSConstants.FootnoteIsAutoNumberedAttr, m_bIsAutoNumbered);

            if (m_footnoteType == FootnoteType.Endnote)
            {
                writer.WriteValue(XDLSConstants.FootnoteTypeAttr, true);
            }

            if (m_strCustomMarker != string.Empty)
            {
                writer.WriteValue(XDLSConstants.FootnoteCustomMarkerAttr, m_strCustomMarker);
            }

            if (CustomMarkerIsSymbol)
            {
                writer.WriteValue(XDLSConstants.FootnoteSymbolCodeAttr, m_symbolCode);
                writer.WriteValue(XDLSConstants.FootnoteSymbolFontNameAttr, m_strSymbolFontName);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.FootnoteIsAutoNumberedAttr))
            {
                m_bIsAutoNumbered = reader.ReadBoolean(XDLSConstants.FootnoteIsAutoNumberedAttr);
            }
            if (reader.HasAttribute(XDLSConstants.FootnoteCustomMarkerAttr))
            {
                m_strCustomMarker = reader.ReadString(XDLSConstants.FootnoteCustomMarkerAttr);
            }
            if (reader.HasAttribute(XDLSConstants.FootnoteTypeAttr))
            {
                m_footnoteType = (reader.ReadBoolean(XDLSConstants.FootnoteTypeAttr)) ? FootnoteType.Endnote : FootnoteType.Footnote;
            }
            if (reader.HasAttribute(XDLSConstants.FootnoteSymbolCodeAttr))
            {
                m_symbolCode = reader.ReadByte(XDLSConstants.FootnoteSymbolCodeAttr);
            }
            if (reader.HasAttribute(XDLSConstants.FootnoteSymbolFontNameAttr))
            {
                m_strSymbolFontName = reader.ReadString(XDLSConstants.FootnoteSymbolFontNameAttr);
            }
        }
//#endif
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            base.CloneRelationsTo(doc, nextOwner);
            if (m_textBody != null)
                m_textBody.CloneRelationsTo(doc, nextOwner);
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isAutoNumbered"></param>
        private void UpdateAutoMarker(bool isAuto)
        {
            if (Document.IsOpening || (!m_bIsAutoNumbered && string.IsNullOrEmpty(m_strCustomMarker)))
                return;

            string destMarker = string.Empty;
            string curMarker = string.Empty;

            if (!isAuto)
            {
                curMarker = SpecialCharacters.FootnoteAscii.ToString();
                destMarker = m_strCustomMarker;
            }
            else
            {
                curMarker = m_strCustomMarker;
                destMarker = SpecialCharacters.FootnoteAscii.ToString();
            }
            UpdateFtnMarker(curMarker, destMarker);
        }
        /// <summary>
        /// Updates the custom marker.
        /// </summary>
        /// <param name="destMarker">The destination marker.</param>
        private void UpdateCustomMarker(string destMarker)
        {
            if (Document.IsOpening || string.IsNullOrEmpty(m_strCustomMarker))
                return;

            UpdateFtnMarker(m_strCustomMarker, destMarker);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        private void UpdateSymbolMarker(byte symbolCode)
        {
            if (Document.IsOpening || m_textBody.Items.Count == 0 || m_bIsAutoNumbered ||
              symbolCode <= 0 || string.IsNullOrEmpty(m_strCustomMarker))
                return;

            WParagraph para = m_textBody.Items[0] as WParagraph;
            TextSelection selection = para.Find(m_strCustomMarker, true, true);
            WTextRange symbol = GenerateSymbol(symbolCode);
            ReplaceSelection(symbol, para, selection);
            m_charFormat.FontName = m_strSymbolFontName;
            UpdateChangeFlag(false);
        }
        /// <summary>
        /// Updates the footnote marker.
        /// </summary>
        internal void UpdateFtnMarker(string curMarker, string destMarker)
        {
            if (m_textBody.Items.Count == 0)
                return;

            WParagraph para = m_textBody.Items[0] as WParagraph;
            if (!string.IsNullOrEmpty(curMarker))
            {
                TextSelection selection = para.Find(curMarker, true, true);
                ReplaceMarker(selection, destMarker);
            }
            else
            {
                AppendMarker(destMarker, para);
            }

            UpdateChangeFlag(false);
        }
        /// <summary>
        /// Ensures the footnote/endnote marker.
        /// </summary>
        internal void EnsureFtnMarker()
        {
            if (m_changesCount <= 0)
                return;

            if (!m_bIsAutoNumbered && m_symbolCode > 0)
            {
                AppendFtnSymbol();
                UpdateChangeFlag(false);
                return;
            }

            string marker = (m_bIsAutoNumbered) ? SpecialCharacters.FootnoteAscii.ToString() : m_strCustomMarker;
            if (marker.TrimStart(' ') != string.Empty)
                marker = marker.TrimStart(' ');
            if (m_textBody.Items.Count == 0)
            {
                WParagraph para = new WParagraph(m_doc);
                AppendMarker(marker, para);
                m_textBody.Items.Insert(0, para);
            }
            else
            {
                WParagraph para = m_textBody.Items[0] as WParagraph;
                TextSelection selection = para.Find(marker, true, true);
                if (selection == null)
                {
                    AppendMarker(marker, para);
                }
                else
                {
                    ReplaceMarker(selection, marker);
                }
            }
            UpdateChangeFlag(false);
        }
        /// <summary>
        /// Appends the footnote symbol.
        /// </summary>
        private void AppendFtnSymbol()
        {
            WTextRange symbol = GenerateSymbol(m_symbolCode);
            m_charFormat.FontName = m_strSymbolFontName;
            if (m_textBody.Items.Count == 0)
            {
                WParagraph para = new WParagraph(m_doc);
                para.Items.Add(symbol);
                para.AppendText(" ");
                m_textBody.Items.Insert(0, para);
            }
            else
            {
                string marker = (m_bIsAutoNumbered) ? SpecialCharacters.FootnoteAscii.ToString() : m_strCustomMarker;
                WParagraph para = m_textBody.Items[0] as WParagraph;
                TextSelection selection = null;
                if (marker != string.Empty)
                    selection = para.Find(marker, true, true);
                ReplaceSelection(symbol, para, selection);
            }
        }
        /// <summary>
        /// Generates the symbol.
        /// </summary>
        /// <param name="symbolCode">The symbol code.</param>
        /// <returns></returns>
        private WTextRange GenerateSymbol(byte symbolCode)
        {
            WTextRange symbol = new WTextRange(Document);
            symbol.Text = ((char)symbolCode).ToString();
            symbol.CharacterFormat.ImportContainer(m_charFormat);
            symbol.CharacterFormat.FontName = m_strSymbolFontName;

            return symbol;
        }
        /// <summary>
        /// Replaces the selection.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="para">The paragraph.</param>
        private void ReplaceSelection(WTextRange symbol, WParagraph para, TextSelection selection)
        {
            if (selection == null)
            {
                para.Items.Insert(0, symbol);
                WTextRange tr = new WTextRange(para.Document);
                tr.Text = " ";
                para.Items.Insert(1, tr);
            }
            else
            {
                WTextRange selRange = selection.GetAsOneRange();
                int index = selRange.GetIndexInOwnerCollection();
                para.Items.Remove(selRange);
                para.Items.Insert(index, symbol);
            }
        }
        /// <summary>
        /// Ensures the footnote/endnote style.
        /// </summary>
        internal void EnsureFtnStyle()
        {
            if (Document.IsOpening)
                return;

            string charStyleName = m_charFormat.CharStyleName;

            Style charStyle = null;
            if (!string.IsNullOrEmpty(charStyleName))
            {
                charStyle = Document.Styles.FindByName(charStyleName) as Style;
            }

            if (charStyle != null &&
              (charStyle.StyleId == DEF_EDNSTYLE_REF_ID || charStyle.StyleId == DEF_FTNSTYLE_REF_ID))
            {
                return;
            }

            CharacterStyle ftnStyle = null;
            if (m_footnoteType == FootnoteType.Footnote)
                ftnStyle = (CharacterStyle)Style.CreateBuiltinStyle(BuiltinStyle.FootnoteReference, StyleType.CharacterStyle, Document);
            else if (m_footnoteType == FootnoteType.Endnote)
                ftnStyle = (CharacterStyle)Style.CreateBuiltinStyle(BuiltinStyle.EndnoteReference, StyleType.CharacterStyle, Document);

            if (ftnStyle != null)
            {
                m_charFormat.CharStyleName = ftnStyle.Name;
                Document.Styles.Add(ftnStyle);
            }
        }
        /// <summary>
        /// Updates the change flag.
        /// </summary>
        private void UpdateChangeFlag(bool value)
        {
            if (!Document.IsOpening)
            {
                if (value)
                    m_changesCount += 1;
                else
                    m_changesCount -= 1;
            }
        }
        /// <summary>
        /// Replaces the marker.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="replaceText">The replace text.</param>
        private void ReplaceMarker(TextSelection selection, string replaceText)
        {
            if (selection != null)
            {
                WTextRange range = selection.GetAsOneRange();
                range.Text = replaceText;
            }
        }
        /// <summary>
        /// Appends the marker.
        /// </summary>
        /// <param name="marker">The marker.</param>
        /// <param name="para">The destination paragraph.</param>
        private void AppendMarker(string marker, WParagraph para)
        {
            WTextRange textRange = GenerateText(marker);
            para.Items.Insert(0, textRange);
            WTextRange spaceRange = new WTextRange(m_textBody.Document);
            spaceRange.Text = " ";
            para.Items.Insert(1, spaceRange);
        }
        /// <summary>
        /// Generates the text.
        /// </summary>
        /// <returns></returns>
        internal WTextRange GenerateText(string marker)
        {
            WTextRange textRange = new WTextRange(m_doc);
            textRange.Text = marker;
            textRange.CharacterFormat.ImportContainer(m_charFormat);

            return textRange;
        }
        #endregion
#if !SILVERLIGHT && !WP
        #region ILeafWidget Members
        internal override void DrawImpl(DrawingContext dc, LayoutedWidget ltWidget)
        {
            FootnoteLayoutInfo footnoteInfo = (this.m_layoutInfo as FootnoteLayoutInfo);
            dc.DrawTextRange(footnoteInfo.TextRange, ltWidget, footnoteInfo.FootnoteID);
        }
        /// <summary>
        /// Measures self size.
        /// </summary>
        /// <param name="graphics"></param>
        /// <returns></returns>
        SizeF ILeafWidget.Measure(DrawingContext dc)
        {
            return m_layoutInfo.Size;
        }
        void IWidget.InitLayoutInfo()
        {
            if (m_layoutInfo == null || this.FootnoteType == FootnoteType.Endnote)
                return;
            if (DocumentLayouter.DrawingContext.m_footnoteId > 0)
            {
                DrawingContext drawingContext = DocumentLayouter.DrawingContext;
                drawingContext.m_footnoteId--;
            }
            if (DocumentLayouter.m_footnoteIDRestartEachPage > 1)
            {
                DocumentLayouter.m_footnoteIDRestartEachPage--;
            }
            if (DocumentLayouter.m_footnoteIDRestartEachSection > 1)
            {
                DocumentLayouter.m_footnoteIDRestartEachSection--;
            }
            m_layoutInfo = null;
        }

        #endregion
        #endif
    }
#if !SILVERLIGHT && !WP
     /// <summary>
     /// The class specifies the Layout Footnote information.
     /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class LayoutFootnoteInfoImpl : FootnoteLayoutInfo
    {
        public LayoutFootnoteInfoImpl(WFootnote footnote) : base(ChildrenLayoutDirection.Horizontal)
        {
            WParagraph ownerParagraph = footnote.GetOwnerParagraph();
            DrawingContext drawingContext = DocumentLayouter.DrawingContext;
            int id = (footnote.FootnoteType == FootnoteType.Endnote) ? drawingContext.m_endnoteId++ : drawingContext.m_footnoteId++;
            WSection section = GetBaseEntity(footnote) as WSection;
            if (section != null)
            {
                if (footnote.FootnoteType == FootnoteType.Footnote)
                {
                    if (section.PageSetup.RestartIndexForFootnotes == FootnoteRestartIndex.DoNotRestart)
                    {
                        id += section.PageSetup.InitialFootnoteNumber - 1;
                    }
                    else if (section.PageSetup.RestartIndexForFootnotes == FootnoteRestartIndex.RestartForEachPage)
                    {
                        id = DocumentLayouter.m_footnoteIDRestartEachPage++;
                    }
                    else if (section.PageSetup.RestartIndexForFootnotes == FootnoteRestartIndex.RestartForEachSection)
                    {
                        id = DocumentLayouter.m_footnoteIDRestartEachSection++;
                    }
                }
                else
                {
                    if (section.PageSetup.RestartIndexForEndnote == EndnoteRestartIndex.DoNotRestart)
                    {
                        id += section.PageSetup.InitialEndnoteNumber - 1;
                    }
                    else if (section.PageSetup.RestartIndexForEndnote == EndnoteRestartIndex.RestartForEachSection)
                    {
                        id = DocumentLayouter.m_footnoteIDRestartEachSection++;
                    }
                }
            }
            if (footnote.CustomMarkerIsSymbol || (footnote.CustomMarker != string.Empty))
            {
                drawingContext.m_footnoteId--;
            }
            FootnoteID = GetFootnoteID(footnote, id);
            TextRange = footnote.GenerateText(FootnoteID);
            if (footnote.CustomMarkerIsSymbol && ((!footnote.MarkerCharacterFormat.HasValue(0) && (footnote.SymbolFontName != string.Empty)) && (footnote.SymbolFontName != footnote.MarkerCharacterFormat.FontName)))
            {
                TextRange.CharacterFormat.FontName = footnote.SymbolFontName;
            }
            TextRange.SetOwner(ownerParagraph);
            Size = drawingContext.MeasureTextRange(TextRange, FootnoteID);
        }
    }
#endif
}