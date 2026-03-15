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
using System.Text.RegularExpressions;

using Syncfusion.DocIO.DLS;
#if !SILVERLIGHT && !WP
using Syncfusion.Layouting;
using Syncfusion.DocIO.Rendering;
using Syncfusion.DocIO.DLS.Rendering;
#endif
using Syncfusion.DocIO.DLS.XML;
using System.Windows;
using System;
#if !WINRT && !WP
using System.Drawing;
using System.Collections.Generic;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// WTextRange is range of text of the paragraph. 
    /// </summary>
    [Documentation.DocumentationExclude()]
    public class WTextRange
      : ParagraphItem
      , IWTextRange
#if !SILVERLIGHT && !WP
, IStringWidget
#endif
    {
        #region Members
        /// <summary>
        /// 
        /// </summary>
        private int m_txtLength = 0;
        /// <summary>
        /// 
        /// </summary>
        private string m_detachedText = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        private bool m_safeText;
#if !SILVERLIGHT && !WP
        /// <summary>
        /// For Doc To PDF - duplicate copy of text used in StringSplitWidget
        /// </summary>
        internal string m_textToSplit = string.Empty;
        /// <summary>
        /// For Doc To PDF - boolean variable used to verify whether text is assiged
        /// during Second Layouting of Doc To PDF.
        /// </summary>
        internal bool m_isTextToSplitAssignedInSecondLayouting = false;
#endif
        #endregion

        #region Properties
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.TextRange;
            }
        }
        /// <summary>
        /// Gets / sets text.
        /// </summary>
        public virtual string Text
        {
            get
            {
                if (!ItemDetached && OwnerParagraph != null)
                {
                    string paraText = OwnerParagraph.Text;
                    m_detachedText = paraText.Substring(StartPos, m_txtLength);
                }
#if !SILVERLIGHT && !WP
                if (TextToSplit == string.Empty || (!DocumentLayouter.IsFirstLayouting && !IsTextToSplitAssignedInSecondLayouting))
                {
                    TextToSplit = m_detachedText;
                    IsTextToSplitAssignedInSecondLayouting = true;
                }
#endif
                return m_detachedText;
            }
            set
            {
                if (ItemDetached || OwnerParagraph == null)
                {
                    m_detachedText = value;
                }
                else if (value != Text)
                {
                    OwnerParagraph.UpdateText(this, value);
                    m_txtLength = value.Length;
                }
#if !SILVERLIGHT && !WP
                TextToSplit = m_detachedText;
#endif
                m_safeText = (Document.IsOpening) ? true : false;
            }
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// For Doc To PDF - used to check whether duplicate copy of text is assigned to TextToSplit
        /// </summary>
        internal bool IsTextToSplitAssignedInSecondLayouting
        {
            get
            {
                return m_isTextToSplitAssignedInSecondLayouting;
            }
            set
            {
                if (!DocumentLayouter.IsFirstLayouting)
                    m_isTextToSplitAssignedInSecondLayouting = value;
            }
        }
        /// <summary>
        /// Gets/Sets the Text to split during Doc to PDF conversions
        /// Internal variable - Stores duplicate copy of Text
        /// </summary>
        internal string TextToSplit
        {
            get { return m_textToSplit; }
            set { m_textToSplit = value; }
        }
#endif
        /// <summary>
        /// Gets  character format( font properties ).
        /// </summary>
        public WCharacterFormat CharacterFormat
        {
            get
            {
                return m_charFormat;
            }
            internal set
            {
                m_charFormat = value;
            }
        }
        /// <summary>
        /// Gets or Sets the length of the text.
        /// </summary>
        internal int TextLength
        {
            get
            {
                return ItemDetached ? m_detachedText.Length : m_txtLength;
            }
            set
            {
                m_txtLength = value;
            }
        }
        /// <summary>
        /// Gets the end pos.
        /// </summary>
        /// <value>The end pos.</value>
        internal override int EndPos
        {
            get
            {
                return base.EndPos + m_txtLength;
            }
        }
        /// <summary>
        /// Defines if text of current text range is safe.
        /// </summary>
        internal bool SafeText
        {
            get
            {
                return m_safeText;
            }
            set
            {
                m_safeText = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WTextRange"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public WTextRange(IWordDocument doc)
            : base((WordDocument)doc)
        {
            m_charFormat = new WCharacterFormat(Document);
            m_charFormat.SetOwner(this);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Attaches to paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="itemPos">The item pos.</param>
        internal override void Attach(WParagraph paragraph, int itemPos)
        {
            m_txtLength = 0;
            base.Attach(paragraph, itemPos);
            Text = m_detachedText;
        }
        /// <summary>
        /// Detaches from owner.
        /// </summary>
        internal override void Detach()
        {
            base.Detach();

            if (OwnerParagraph != null)
            {
                m_detachedText = Text;
                OwnerParagraph.UpdateText(this, string.Empty);
            }
        }
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WTextRange tr = (WTextRange)base.CloneImpl();
            tr.m_detachedText = Text;
            return tr;
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            base.CloneRelationsTo(doc, nextOwner);

            //if( CharacterFormat.CharStyleName != null && doc.ImportStyles )
            //{
            //  CharacterStyle cs = Document.Styles.FindByName( CharacterFormat.CharStyleName, StyleType.CharacterStyle ) as CharacterStyle;

            //  if( cs != null )
            //  {
            //    IStyle foundStyle = doc.Styles.FindByName( cs.Name, StyleType.CharacterStyle );

            //    if( foundStyle == null )
            //    {
            //      cs.ImportStyleTo( doc );
            //    }
            //    else
            //    {
            //      if( doc.CurClonedSection != null )
            //      {
            //        cs = ( CharacterStyle )( cs as Style ).ApplyOrImportStyleTo( doc, foundStyle );
            //        CharacterFormat.CharStyleName = cs.Name;
            //      }
            //    }         
            //  }
            //}
        }
        /// <summary>
        /// Sets the character format.
        /// </summary>
        /// <param name="charFormat">The character format.</param>
        public void ApplyCharacterFormat(WCharacterFormat charFormat)
        {
            if (charFormat != null)
            {
                m_charFormat = charFormat.CloneInt() as WCharacterFormat;
            }
        }
        /// <summary>
        /// Split the widgets based on the control characters tab ("\t") and carriage return ("\r")
        /// </summary>
        internal void SplitWidgets()
        {
            SplitByTab();
            WParagraph paragraph = GetOwnerParagraph();
            if (paragraph != null && !(paragraph.IsInCell && (paragraph.OwnerTextBody as WTableCell).LastParagraph == paragraph
                && ((paragraph.OwnerTextBody as WTableCell).OwnerRow.OwnerTable.OwnerTextBody is WTableCell)))
                SplitByParagraphBreak();
        }
        /// <summary>
        /// Splits the widget by tab.
        /// </summary>
        private void SplitByTab()
        {
            string text = string.Empty;
            if (Text != ControlChar.Tab && Text.Contains(ControlChar.Tab))
            {
                int index = Text.IndexOf(ControlChar.Tab);
                text = Text;
                int txtIndex = OwnerParagraph.Items.IndexOf(this);
                string remainder = text.Substring(index + 1);
                //Split text range.
                WTextRange txtRange = Clone() as WTextRange;
                if (index > 0)
                {
                    txtRange.Text = text.Substring(index);
                    Text = text.Substring(0, index);
                }
                else if (remainder != string.Empty)
                {
                    txtRange.Text = remainder;
                    Text = ControlChar.Tab;
                }
                //Splits the tab as new text range.
                OwnerParagraph.Items.Insert(txtIndex + 1, txtRange);
            }
        }
        /// <summary>
        /// Splits the widget by paragraph break.
        /// </summary>
        private void SplitByParagraphBreak()
        {
            string text = string.Empty;
            text = Text.Replace(ControlChar.CrLf, ControlChar.ParagraphBreak);
            text = text.Replace(ControlChar.LineFeedChar, ControlChar.ParagraphBreakChar);
            if (text.Contains(ControlChar.ParagraphBreak))
            {
                int index = text.IndexOf(ControlChar.ParagraphBreak);
                string remainder = text.Substring(index + 1);
                WTextRange txtRange = Clone() as WTextRange;
                if (index > 0)
                {
                    txtRange.Text = text.Substring(index + 1);
                    Text = text.Substring(0, index);
                }
                else if (remainder != string.Empty)
                {
                    txtRange.Text = remainder;
                    Text = string.Empty;
                }
                //Splits as new paragraph.
                WParagraph para = OwnerParagraph.Clone() as WParagraph;
                para.ClearItems();
                int paraIndex = OwnerParagraph.GetIndexInOwnerCollection();
                OwnerParagraph.OwnerTextBody.Items.Insert(paraIndex + 1, para);
                para.Items.Add(txtRange);
                if (text == ControlChar.ParagraphBreak)
                {
                    Text = string.Empty;
                    txtRange.Text = string.Empty;
                }
                int txtIndex = OwnerParagraph.Items.IndexOf(this);
                //Updates the remaining items to next paragraph.
                while (txtIndex + 1 < OwnerParagraph.Items.Count)
                {
                    para.Items.Add(OwnerParagraph.Items[txtIndex + 1]);
                }
            }
        }
        #endregion
        //#if !SILVERLIGHT
        #region Implementation / xml
        /// <summary>
        /// Registers child objects in XDSL holder.
        /// </summary>
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddElement(XDLSConstants.CharacterFormatTag, m_charFormat);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(IXDLSContentWriter writer)
        {
            base.WriteXmlContent(writer);
            writer.WriteChildStringElement(XDLSConstants.TextTag, Text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override bool ReadXmlContent(IXDLSContentReader reader)
        {
            if (reader.TagName == XDLSConstants.TextTag)
            {
                if (OwnerParagraph != null) StartPos = OwnerParagraph.Text.Length;
                Text = reader.ReadChildStringContent();

                if (Text == "")
                    reader.InnerReader.Read();
                m_safeText = true;

                return true;
            }

            return false;
        }
        #endregion
#if !SILVERLIGHT && !WP
        #region Implementation / layout
        /// <summary>
        /// 
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            SplitWidgets();
            m_layoutInfo = (Text == "\t")
              ? new LayoutTextRangeInfo(this)
              : new LayoutInfo(ChildrenLayoutDirection.Horizontal);
            if (CharacterFormat.Position != 0)
            {
                m_layoutInfo.Margins.Top = -CharacterFormat.Position;
            }

            if (PreviousSibling != null && PreviousSibling.EntityType == EntityType.FieldMark)
            {
                WField field = PreviousSibling.PreviousSibling as WField;
                if ((field != null) && ((field.FieldType == FieldType.FieldPage) || (field.FieldType == FieldType.FieldSymbol)))
                {
                    m_layoutInfo.IsSkip = true;
                }
            }
            if (OwnerParagraph == null)
            {
                WParagraph para = null;
                if (Owner is SDTInlineContent)
                    para = GetOwnerParagraph();
                else
                    para = CharacterFormat.BaseFormat.OwnerBase as WParagraph;
                if (para != null)
                    m_layoutInfo.IsVerticalText = (para as IWidget).LayoutInfo.IsVerticalText;
                //Check the whether the paragraph is first and last pargaraph of the section 
                if (para != null && para.SectionEndMark && para.PreviousSibling != null)
                    m_layoutInfo.IsSkip = true;
            }
            else
                m_layoutInfo.IsVerticalText = (OwnerParagraph as IWidget).LayoutInfo.IsVerticalText;
            if (this.CharacterFormat.Hidden)
            {
                m_layoutInfo.IsSkip = true;
            }
            //Update TextRange Size
            m_layoutInfo.Size = GetTextRangeSize();
        }
        /// <summary>
        /// Measure Size of the TextRange
        /// </summary>
        /// <returns></returns>
        internal SizeF GetTextRangeSize()
        {
            DrawingContext dc = DocumentLayouter.DrawingContext;
            string text = Text;
            //Check whether the text denotes foonote reference
            if (text == ((char)2).ToString() && this.GetOwnerParagraph().OwnerTextBody.Owner is WFootnote)
                text = ((this.GetOwnerParagraph().OwnerTextBody.Owner as WFootnote).m_layoutInfo as FootnoteLayoutInfo).FootnoteID;
            SizeF size = new SizeF();
            if (Text.Equals(string.Empty) || IsLastTextRangeWithSpace(text))
            {
                size = dc.MeasureTextRange(this, " ");
                size.Width = 0.0f;
                return size;
            }
            //Condition handled to layout Horizontal Alignment other than left
            if (this.OwnerParagraph != null
                && this.GetIndexInOwnerCollection() == this.OwnerParagraph.Items.Count - 1 //Checks for last item
                && Text.Trim() != string.Empty) //Text should not be equal to empty when trimmed.
                text = Text.TrimEnd();
            return dc.MeasureTextRange(this, text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="ltWidget"></param>
        void IWidget.Draw(DrawingContext dc, LayoutedWidget ltWidget)
        {
            string text = ltWidget.TextTag != null ? ltWidget.TextTag : Text;
            (this as IStringWidget).Draw(dc, ltWidget, text);
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="ltWidget"></param>
        /// <param name="text"></param>
        void IStringWidget.Draw(DrawingContext dc, LayoutedWidget ltWidget, string text)
        {
            dc.DrawTextRange(this, ltWidget, text);
            DrawImpl(dc, ltWidget);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="offset"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        SizeF ITextMeasurable.Measure(string text)
        {
            DrawingContext dc = new DrawingContext();
            if (text != Text)
            {
                if (text != null && (text == string.Empty || IsLastTextRangeWithSpace(text)))
                {
                    SizeF size = dc.MeasureTextRange(this, " ");
                    size.Width = 0.0f;
                    return size;
                }
                return dc.MeasureTextRange(this, text);
            }
            else
                return (this as IWidget).LayoutInfo.Size;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="offset"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        SizeF ITextMeasurable.Measure(DrawingContext dc, string text)
        {
            if (text != Text)
            {
                if (text != null && (text == string.Empty || IsLastTextRangeWithSpace(text)))
                {
                    SizeF size = dc.MeasureTextRange(this, " ");
                    size.Width = 0.0f;
                    return size;
                }
                return dc.MeasureTextRange(this, text);
            }
            else
                return (this as IWidget).LayoutInfo.Size;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <returns></returns>
        double IStringWidget.GetTextAscent(DrawingContext dc, ref float exceededLineAscent)
        {
            Font font = GetFont();
            double ascent = dc.GetAscent(font);
            //Get the exceeded line Ascent to align line y position 
            if (this.CharacterFormat.FontName == ("Arial Unicode MS") && Text.Trim(' ') != "" && !this.CharacterFormat.ComplexScript)
                exceededLineAscent = (float)(dc.GetExceededLineHeightForArialUnicodeMSFont(font) - ascent) / 2;
            return ascent;
        }

        /// <summary>
        /// Get Font of the TextRange or Field
        /// </summary>
        /// <returns></returns>
        Font GetFont()
        {
            WCharacterFormat charFormat = this.CharacterFormat;
            if ((this is WField) && ((this as WField).FieldType == FieldType.FieldPage || (this as WField).FieldType == FieldType.FieldNumPages))
            {
                charFormat = (this as WField).GetCharacterFormat();
            }
            return new Font(charFormat.GetFontNameFromHint(), charFormat.Font.Size, charFormat.Font.Style);
        }
        /// <summary>
        /// Offsets to index.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="offset">The offset.</param>
        /// <param name="text">The text.</param>
        /// <param name="clientWidth">The clientWidth.</param>
        /// <returns></returns>
        int IStringWidget.OffsetToIndex(DrawingContext dc, double offset, string text, float clientWidth, float clientActiveAreaWidth)
        {
            float width = GetClientWidth(dc, clientWidth);
            ParagraphLayoutInfo paragraphInfo = m_layoutInfo as ParagraphLayoutInfo;
            bool textWrap = (paragraphInfo != null) ? paragraphInfo.TextWrap : true;
            return dc.GetSplitIndexByOffset(text, this, offset, !textWrap, this.GetOwnerParagraph().IsInCell, width, clientActiveAreaWidth);
        }
        /// <summary>
        /// Determines current section client width.
        /// </summary>
        /// <param name="clientWidth">The clientWidth.</param>
        /// <returns>Client Width</returns>
        internal float GetClientWidth(DrawingContext dc, float clientWidth)
        {
            WSection sec;
            float width = 0.0f;
            if (this.Owner != null)
            {
                Entity ent = this.Owner as Entity;
                bool isFootNote = false;
                while (!(ent is WSection))
                {
                    if (((ent is WTable) || (ent is Shape)) && !isFootNote)
                    {
                        break;
                    }
                    if (ent.Owner == null)
                        break;
                    else
                        ent = ent.Owner as Entity;
                    if (ent is WFootnote)
                        isFootNote = true;
                }
                if (ent is WSection)
                {
                    ParagraphLayoutInfo paragraphInfo = ((this.GetOwnerParagraph()) as IWidget).LayoutInfo as ParagraphLayoutInfo;
                    if (paragraphInfo.IsFirstLine)
                        width = clientWidth - (float)(paragraphInfo.Margins.Left + paragraphInfo.Margins.Right + paragraphInfo.FirstLineIndent + paragraphInfo.ListTab);
                    else
                        width = clientWidth - (float)(paragraphInfo.Margins.Left + paragraphInfo.Margins.Right + paragraphInfo.ListTab);
                }
                else if (ent is WTable)
                {
                    width = dc.GetCellWidth(this);
                }
                else if (ent is Shape)
                {
                    width = (((ent as Shape) as IWidget).LayoutInfo as ShapeLayoutInfo).TextLayoutingBounds.Width;
                }
            }
            return width;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="offset"></param>
        /// <param name="clientWidth">The clientWidth.</param>
        /// <returns></returns>
        ISplitLeafWidget[] ISplitLeafWidget.SplitBySize(DrawingContext dc, SizeF offset, float clientWidth, float clientActiveAreaWidth, ref bool isLastWordFit)
        {
            return SplitStringWidget.SplitBySize(dc, offset.Width, this, null, clientWidth, clientActiveAreaWidth, ref isLastWordFit);
        }

        SizeF ILeafWidget.Measure(DrawingContext dc)
        {
            return (this as IWidget).LayoutInfo.Size;
        }
        /// <summary>
        /// Determine whether the text range is last item of the paragraph and which have text with empty space
        /// </summary>
        /// <returns></returns>
        private bool IsLastTextRangeWithSpace(string text)
        {
            text = text.Trim(ControlChar.SpaceChar);
            if (text == string.Empty && (this.m_layoutInfo as TabsLayoutInfo) == null)
            {
                Entity ent = this.NextSibling as Entity;
                while ((ent is WTextRange && (ent as WTextRange).Text.Trim(ControlChar.SpaceChar) == string.Empty
                        && ((ent as WTextRange).m_layoutInfo as TabsLayoutInfo) == null)
                        || (ent is BookmarkStart) || (ent is BookmarkEnd) || (ent is WFieldMark))
                {
                    ent = ent.NextSibling as Entity;
                    if (ent == null)
                        break;
                }
                if (ent == null)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        #endregion

        #region Internal declarations
        /// <summary>
        /// /
        /// </summary>
        internal class LayoutTextRangeInfo
          : TabsLayoutInfo
        {
            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="LayoutTextRangeInfo"/> class.
            /// </summary>
            /// <param name="textRange">The text range.</param>
            public LayoutTextRangeInfo(WTextRange textRange)
                : base(Layouting.ChildrenLayoutDirection.Horizontal)
            {
                textRange.Text = string.Empty;
                WParagraph paragraph = textRange.OwnerParagraph as WParagraph;
                if (textRange.Owner is SDTInlineContent)
                    paragraph = textRange.GetOwnerParagraph();
                m_defaultTabWidth = paragraph.GetDefaultTabWidth();
                WParagraphFormat pFormat = paragraph.ParagraphFormat;

                IWParagraphStyle pStyle = paragraph.GetStyle();
                if (pStyle == null)
                {
                    pStyle = textRange.Document.Styles.FindByName("Normal", StyleType.ParagraphStyle) as IWParagraphStyle;
                    if (pStyle == null)
                        pStyle = (WParagraphStyle)Style.CreateBuiltinStyle(BuiltinStyle.Normal, textRange.Document);
                }

                Tab tab;
                List<float> delPosition = new List<float>();//Collection for delete position.  

                pFormat = paragraph.ParagraphFormat;
                //add the tab stop collection until base format is null.
                while (pFormat != null)
                {
                    UpdateTabCollection(pFormat, ref delPosition);
                    pFormat = pFormat.BaseFormat as WParagraphFormat;
                }
            }
            /// <summary>
            /// add the tab stop collection to list until base format is null.
            /// </summary>
            /// <param name="pFormat">paragraph format</param>
            /// <param name="delPosition">collection for delete position</param>
            /// <returns></returns>
            private void UpdateTabCollection(WParagraphFormat pFormat, ref List<float> delPosition)
            {
                Tab tab;
                // fill tabs
                for (int i = 0, size = pFormat.Tabs.Count; i < size; i++)
                {
                    tab = pFormat.Tabs[i];
                    bool isTabExist = false;
                    bool isTabClear = false;
                    bool isTabDelPosition = false;
                    int index = 0;
                    if (m_list.Count != 0)
                    {
                        for (int k = 0; k < m_list.Count; k++)
                        {
                            if (Math.Truncate((m_list[k] as LayoutTab).Position) == Math.Truncate(tab.Position))
                            {
                                isTabExist = true;
                                index = k;
                                break;
                            }
                            else if (tab.Position == 0 && tab.DeletePosition != 0 && Math.Truncate((m_list[k] as LayoutTab).Position) == Math.Truncate((tab.DeletePosition / DLSConstants.TwipsInOnePoint)))
                            {
                                if (!delPosition.Contains(tab.DeletePosition / DLSConstants.TwipsInOnePoint))
                                    delPosition.Add(tab.DeletePosition / DLSConstants.TwipsInOnePoint);
                                isTabClear = true;
                                index = k;
                                break;
                            }
                        }
                    }
                    if (delPosition.Contains(tab.Position))
                    {
                        isTabDelPosition = true;
                    }
                    if (!isTabDelPosition && !(tab.Position == 0 && tab.DeletePosition != 0) && !isTabClear && !isTabExist)
                        AddTab(
                          (tab.Position != 0 ? tab.Position : tab.DeletePosition / DLSConstants.TwipsInOnePoint),
                          (Layouting.TabJustification)tab.Justification,
                          (Layouting.TabLeader)tab.TabLeader);
                    else if (!delPosition.Contains(tab.DeletePosition / DLSConstants.TwipsInOnePoint))
                        delPosition.Add(tab.DeletePosition / DLSConstants.TwipsInOnePoint);
                    if (!isTabDelPosition && isTabClear)
                    {
                        m_list.RemoveAt(index);
                    }
                    else if (!isTabDelPosition && isTabExist)
                    {
                        m_list[index].Justification = (Layouting.TabJustification)tab.Justification;
                        m_list[index].Position = (tab.Position != 0 ? tab.Position : tab.DeletePosition / DLSConstants.TwipsInOnePoint);
                        m_list[index].TabLeader = (Layouting.TabLeader)tab.TabLeader;
                    }
                }
            }
            #endregion
        }
        #endregion
#endif
        //#endif
    }
}
