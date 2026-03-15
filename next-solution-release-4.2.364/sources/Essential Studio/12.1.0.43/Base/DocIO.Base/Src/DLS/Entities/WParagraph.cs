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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.OLEObject;
using Syncfusion.Layouting;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
#if !SILVERLIGHT && !WP
using Syncfusion.DocIO.Rendering;
using Syncfusion.DocIO.DLS.Rendering;
using Image = System.Drawing.Image;
using Syncfusion.CompoundFile.DocIO.Native;
#endif
#if WINRT
using Syncfusion.DocIO.Security.Cryptography;
#else
#if !WP
using System.Drawing;
#endif
using System.Security.Cryptography;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a paragraph of text.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class WParagraph
      : TextBodyItem
#if !SILVERLIGHT && !WP
, IWidgetContainer
      , IWidget
#endif
, IWParagraph
    {
        #region Constants
        /// <summary>
        /// 
        /// </summary>
        private const string DEF_NORMAL_STYLE = "Normal";
        private const int DEF_LIST_STYLE_ID = 179;
        private const int DEF_USER_STYLE_ID = 4094;
        #endregion

        #region Fields
        /// <summary>
        /// The paragraph style
        /// </summary>
        protected IWParagraphStyle m_style;
        /// <summary>
        /// The paragraph text
        /// </summary>
        //private string m_strText = "";
        private StringBuilder m_strTextBuilder = new StringBuilder(1);
        /// <summary>
        /// The paragraph format
        /// </summary>
        protected WParagraphFormat m_prFormat = null;
        /// <summary>
        /// The list format
        /// </summary>
        protected WListFormat m_listFormat = null;
        /// <summary>
        /// The paragraph items
        /// </summary>
        protected ParagraphItemCollection m_pItemColl = null;
        /// <summary>
        /// The paragra[h items with one empty item.
        /// </summary>
        private ParagraphItemCollection m_pEmptyItemColl = null;
        /// <summary>
        /// Defines whether to remove paragraph if empty 
        /// </summary>
        private bool m_bRemoveEmpty;
        /// <summary>
        /// 
        /// </summary>
        private WCharacterFormat m_charFormat;
        /// <summary>
        /// Specifies the owner textbody item.
        /// Denotes the owner table, if the current body item is in table cell.
        /// </summary>
        private TextBodyItem m_ownerTextBodyItem;
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Defines whether Split Widget Container is drawn or not.
        /// </summary>
        private bool m_bSplitWidgetContainerDrawn;
#endif
#if (!SILVERLIGHT && !WP) || WINRT
        private bool m_bIsStyleApplied;
#endif        /// <summary>
        /// Handled for preservation of inline SDT items in Doc to PDF conversion.
        /// Defines whether the paragraph has SDT inline items or not.
        /// </summary>
        internal bool m_bHasSDTInlineItem;
        #endregion

        #region Properties
#if (!SILVERLIGHT && !WP) || WINRT
        /// <summary>
        /// Gets/Sets the boolean value to indicate whetheer the style is applied
        /// </summary>
        internal bool IsStyleApplied
        {
            get
            {
                return m_bIsStyleApplied;
            }
            set
            {
                m_bIsStyleApplied = value;
            }
        }
#endif
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets the boolean value to indicate whether need to measure size of the BookMarks
        /// </summary>
        internal bool IsNeedToMeasureBookMarkSize
        {
            get
            {
                for (int i = 0; i < this.ChildEntities.Count; i++)
                {
                    if (!(this.ChildEntities[i] is BookmarkStart || this.ChildEntities[i] is BookmarkEnd))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
#endif
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.Paragraph;
            }
        }
        /// <summary>
        /// Gets the child entities.
        /// </summary>
        /// <value>The child entities.</value>
        public EntityCollection ChildEntities
        {
            get
            {
                return m_pItemColl as EntityCollection;
            }
        }
        /// <summary>
        /// Gets paragraph style name.
        /// </summary>
        /// <value></value>
        public string StyleName
        {
            get
            {
                if (m_style == null)
                {
                    return null;
                }
                return m_style.Name;
            }
        }
        /// <summary>
        /// Gets / sets paragraph text.
        /// </summary>
        /// <value></value>
        /// <remarks>All internal formatting will be cleared when new text is set.</remarks>
        public string Text
        {
            get
            {
                return m_strTextBuilder.ToString();
            }
            set
            {
                Items.Clear();
                IWTextRange text = AppendText(value);
                text.CharacterFormat.ImportContainer(this.BreakCharacterFormat);
            }
        }
        /// <summary>
        /// Gets paragraph item by index.
        /// </summary>
        /// <value></value>
        public ParagraphItem this[int index]
        {
            get
            {
                return m_pItemColl[index];
            }
        }
        /// <summary>
        /// Gets paragraph items.
        /// </summary>
        /// <value>The items.</value>
        public ParagraphItemCollection Items
        {
            get
            {
                return m_pItemColl;
            }
        }
        /// <summary>
        /// Gets paragraph format.
        /// </summary>
        /// <value></value>
        public WParagraphFormat ParagraphFormat
        {
            get
            {
                return m_prFormat;
            }
        }
        /// <summary>
        /// Gets character format for the break symbol.
        /// </summary>
        /// <value></value>
        public WCharacterFormat BreakCharacterFormat
        {
            get
            {
                return m_charFormat;
            }
        }
        /// <summary>
        /// Gets format of the list for the paragraph.
        /// </summary>
        public WListFormat ListFormat
        {
            get
            {
                if (m_listFormat == null)
                {
                    m_listFormat = new WListFormat(this);
                }
                return m_listFormat;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this paragraph is in cell.
        /// </summary>
        /// <value>
        /// 	if this paragraph is in cell, set to <c>true</c>.
        /// </value>
        public bool IsInCell
        {
            get
            {
                return Owner is WTableCell;
            }
        }
        //indicate whether floating element is layouted or not.
        internal bool IsFloatingItemsLayouted = false;

        //Indicate whether xposition is updated or not while layouting the wrapping element
        internal bool IsXpositionUpated = false;
        /// <summary>
        /// Gets a value indicating whether this paragraph is end of section.
        /// </summary>
        /// <value>
        ///    if this paragraph is end of section, set to <c>true</c>.
        /// </value>
        public bool IsEndOfSection
        {
            get
            {
                return (Owner.Owner is WSection && NextSibling == null);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this paragraph is end of document.
        /// </summary>
        /// <value>
        /// 	if this instance is end of document, set to <c>true</c>.
        /// </value>
        public bool IsEndOfDocument
        {
            get
            {
                if (IsEndOfSection)
                {
                    return ((Owner.Owner as WSection).NextSibling == null);
                }
                return false;
            }
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets count of child items.
        /// </summary>
        int IWidgetContainer.Count
        {
            get
            {
                return WidgetCollection.Count;
            }
        }
        /// <summary>
        /// Gets child widgets.
        /// </summary>
        EntityCollection IWidgetContainer.WidgetInnerCollection
        {
            get
            {
                return WidgetCollection as EntityCollection;
            }
        }
        /// <summary>
        /// Gets child item by index.
        /// </summary>
        IWidget IWidgetContainer.this[int index]
        {
            get
            {
                return (WidgetCollection as ParagraphItemCollection).GetCurrentWidget(index);
            }
        }
        /// <summary>
        /// Get Next widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        internal IWidget GetNextSibling(IWidget widget)
        {
            int index = (WidgetCollection as ParagraphItemCollection).InnerList.IndexOf(widget);

            if (index < 0 || index > WidgetCollection.Count - 2)
            {
                return null;
            }

            return (WidgetCollection as ParagraphItemCollection).InnerList[index + 1] as IWidget;
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        protected IEntityCollectionBase WidgetCollection
        {
            get
            {
                if (m_pItemColl.Count == 0)
                {
                    return m_pEmptyItemColl;
                }
                else if (m_bHasSDTInlineItem)
                {
                    return GetParagraphItems();
                }
                //To preserve the paragraph mark ,if the paragraph last item is break then add empty textrange to the widget collection
                else if ((m_pItemColl[m_pItemColl.Count - 1] is Break
                    && !(m_pItemColl[m_pItemColl.Count - 1] as Break).TextRange.CharacterFormat.Hidden//Skip this add empty textrange when break item contain hidden  
                    && ((m_pItemColl[m_pItemColl.Count - 1] as Break).BreakType == BreakType.LineBreak
                    || (((m_pItemColl[m_pItemColl.Count - 1] as Break).BreakType == BreakType.PageBreak
                    && Document.DOP.Dop2000.Copts.SplitPgBreakAndParaMark))
                    || (m_pItemColl[m_pItemColl.Count - 1] as Break).BreakType == BreakType.ColumnBreak))
                    || IsContainFloatingItems())
                {
                    ParagraphItemCollection paragraphItems = GetParagraphItems();
                    paragraphItems.InnerList.Add(m_pEmptyItemColl[0]);
                    return paragraphItems;
                }
                return m_pItemColl;
            }
        }
        /// <summary>
        /// Determine whether the paragraph contains floating items alone
        /// </summary>
        /// <returns></returns>
        private bool IsContainFloatingItems()
        {
            for (int i = 0; i < this.ChildEntities.Count; i++)
            {
                TextWrappingStyle wrapStyle = (this.ChildEntities[i] is WTextBox) ? (this.ChildEntities[i] as WTextBox).TextBoxFormat.TextWrappingStyle
                                              : (this.ChildEntities[i] is WPicture) ? (this.ChildEntities[i] as WPicture).TextWrappingStyle
                                              : (this.ChildEntities[i] is Shape) ? (this.ChildEntities[i] as Shape).WrapFormat.TextWrappingStyle : TextWrappingStyle.Inline;
                if (wrapStyle == TextWrappingStyle.Inline)
                    return false;
                else
                    continue;
            }
            return true;
        }
        /// <summary>
        /// Gets or sets a value indicating whether to remove empty paragraph.
        /// </summary>
        /// <value>if its specifies to remove empty paragraph, set to <c>true</c>.</value>
        internal bool RemoveEmpty
        {
            get
            {
                return m_bRemoveEmpty;
            }
            set
            {
                m_bRemoveEmpty = value;
            }
        }
        /// <summary>
        /// Gets the last item.
        /// </summary>
        /// <value>The last item.</value>
        internal ParagraphItem LastItem
        {
            get
            {
                return this[m_pItemColl.Count - 1];
            }
        }
        /// <summary>
        /// Gets the paragraph style.
        /// </summary>
        /// <value>The style.</value>
        internal IWParagraphStyle ParaStyle
        {
            get
            {
                return m_style;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the paragraph is section end mark.
        /// </summary>
        /// <value><c>true</c> if the paragraph is section end mark; otherwise, <c>false</c>.</value>
        internal bool SectionEndMark
        {
            get
            {
                return IsSectionEndMark();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WParagraph"/> class.
        /// </summary>
        /// <param name="doc"></param>
        public WParagraph(IWordDocument doc)
            : base((WordDocument)doc)
        {
            m_pItemColl = new ParagraphItemCollection(this);
            m_charFormat = new WCharacterFormat(Document);
            m_prFormat = new WParagraphFormat(Document);
            m_listFormat = new WListFormat(this);

            m_charFormat.SetOwner(this);
            m_prFormat.SetOwner(this);
            m_listFormat.SetOwner(this);
            //Applies normal paragraph style as default.
            ApplyStyle("Normal");
            CreateEmptyParagraph();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Move the paragraph items to another paragraph.
        /// </summary>
        /// <param name="targetParagraph"></param>
        /// <param name="sourceParagraph"></param>
        /// <param name="index"></param>
        private void MoveParagraphItems(WParagraph targetParagraph, WParagraph sourceParagraph, int startIndex)
        {
            int itemCount = sourceParagraph.Items.Count - startIndex;
            for (int i = 0; i < itemCount; i++)
            {
                Entity item = sourceParagraph.ChildEntities[startIndex];
                if ((item is WField && !(item is WMergeField)) || item is WFormField)
                {
                    targetParagraph.ChildEntities.InnerList.Add(item);
                    item.SetOwner(targetParagraph);
                    sourceParagraph.ChildEntities.InnerList.RemoveAt(startIndex);
                    (item as ParagraphItem).StartPos = (targetParagraph.ChildEntities.InnerList[targetParagraph.ChildEntities.InnerList.Count - 2] as ParagraphItem).EndPos;
                }
                else
                    targetParagraph.ChildEntities.Add(item);
            }
        }
        /// Split the textrange inser into new paragpahs if new line character exsist in the text 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>

        /// <summary>
        internal void SplitTextRange()
        {
            for (int j = 0; j < this.ChildEntities.Count; j++)
            {
                if (this.ChildEntities[j].EntityType == EntityType.TextRange
                    || (this.ChildEntities[j].EntityType == EntityType.MergeField && (this.ChildEntities[j] as WMergeField).ConvertedToText))
                {
                    WTextRange txtRange = this.ChildEntities[j] as WTextRange;
                    string textToDisplay = ModifyText(txtRange.Text);
                    int crIndex = textToDisplay.IndexOf(DocxSerializator.CarriageReturn);
                    if (crIndex != -1 && txtRange.OwnerParagraph != null)
                    {
                        WParagraph newPara = null;
                        WParagraph ownerPara = txtRange.GetOwnerParagraph();
                        string[] m_splittedTextNode = textToDisplay.Split('\r');
                        txtRange.Text = m_splittedTextNode[0];
                        int newParaIndex = ownerPara.GetIndexInOwnerCollection();
                        for (int i = 1; i < m_splittedTextNode.Length; i++)
                        {
                            newPara = new WParagraph(this.Document);
                            WTextRange newTextRange = new WTextRange(this.Document);
                            WTextBody ownerTextBody = txtRange.OwnerParagraph.Owner as WTextBody;
                            //apply the properties to the newly created paragraph
                            newPara.ParagraphFormat.ImportContainer(ownerPara.ParagraphFormat);
                            //Copies property hash
                            newPara.ParagraphFormat.CopyProperties(ownerPara.ParagraphFormat);
                            newPara.ListFormat.ImportContainer(ownerPara.ListFormat);

                            //insert the new paragraph in the onwer textbody
                            ownerTextBody.ChildEntities.Insert(newParaIndex + i, newPara);

                            //Add the new textRange to the new paragraph
                            newTextRange.Text = m_splittedTextNode[i];
                            newTextRange.CharacterFormat.ImportContainer(txtRange.CharacterFormat);
                            newTextRange.CharacterFormat.CopyProperties(txtRange.CharacterFormat);
                            newPara.Items.Insert(0, newTextRange);
                            if (ownerPara.StyleName != null)
                                newPara.ApplyStyle(ownerPara.StyleName);
                            if (i == m_splittedTextNode.Length - 1)
                            {
                                newPara.BreakCharacterFormat.ImportContainer(ownerPara.BreakCharacterFormat);
                                newPara.BreakCharacterFormat.CopyProperties(ownerPara.BreakCharacterFormat);
                            }
                            else
                            {
                                newPara.BreakCharacterFormat.ImportContainer(newTextRange.CharacterFormat);
                                newPara.BreakCharacterFormat.CopyProperties(newTextRange.CharacterFormat);
                            }
                        }
                        //Move the proceeding paragraph items to the new paragraph
                        MoveParagraphItems(newPara, ownerPara, txtRange.GetIndexInOwnerCollection() + 1);
                        ownerPara.BreakCharacterFormat.ClearFormatting();
                        ownerPara.BreakCharacterFormat.ImportContainer(txtRange.CharacterFormat);
                        ownerPara.BreakCharacterFormat.CopyProperties(txtRange.CharacterFormat);
                    }
                }
            }
        }
        /// <summary>
        /// Insert the page/column break after formattings.
        /// </summary>
        /// <param name="paragraph"></param>
        internal void InsertBreak(BreakType breakType)
        {
            if (this.Owner == null)
                return;
            if (breakType == BreakType.PageBreak)
                this.ParagraphFormat.PageBreakAfter = false;
            else if (breakType == BreakType.ColumnBreak)
                this.ParagraphFormat.ColumnBreakAfter = false;

            if (this.NextSibling is WParagraph)
            {
                WParagraph nextPara = this.NextSibling as WParagraph;
                nextPara.Items.Insert(0, new Break(this.Document, breakType));
            }
            else
            {
                int paraIndex = this.GetIndexInOwnerCollection();
                WParagraph breakPara = new WParagraph(this.Document);
                breakPara.AppendBreak(breakType);
                breakPara.AppendText(" ");

                ICompositeEntity composite = this.Owner as ICompositeEntity;
                if (composite.ChildEntities.Count == paraIndex + 1)
                {
                    composite.ChildEntities.Add(breakPara);
                }
                else
                {
                    composite.ChildEntities.Insert(paraIndex + 1, breakPara);
                }
            }
        }
        /// <summary>
        /// Applies the specified style.
        /// </summary>
        /// <param name="styleName">Style name</param>
        /// <remarks>Specified style must exist in Document.Styles collection</remarks>
        public void ApplyStyle(string styleName)
        {
#if (!SILVERLIGHT && !WP) || WINRT
            IsStyleApplied = true;
#endif
            IWParagraphStyle newStyle = Document.Styles.FindByName(styleName, StyleType.ParagraphStyle) as IWParagraphStyle;

            if (newStyle == null && styleName == "Normal")
                newStyle = (WParagraphStyle)Style.CreateBuiltinStyle(BuiltinStyle.Normal, Document);

            if (newStyle == null)
                throw new ArgumentException("specified partagraph style not found");

            ApplyStyle(newStyle);
            EnsureNextStyle(newStyle as Style);
        }
        /// <summary>
        /// Applies the built-in style.
        /// </summary>
        /// <param name="builtinStyle">The built-in style.</param>
        public void ApplyStyle(BuiltinStyle builtinStyle)
        {
#if (!SILVERLIGHT && !WP) || WINRT
            IsStyleApplied = true;
#endif
            bool isListStyle = Style.IsListStyle(builtinStyle);

            CheckNormalStyle();

            if (isListStyle)
            {
                ApplyListStyle(builtinStyle);
            }
            else
            {
                string builtinName = Style.BuiltInToName(builtinStyle);
                IStyle pStyle = Document.Styles.FindByName(builtinName, StyleType.ParagraphStyle) as IWParagraphStyle;
                if (pStyle == null)
                {
                    pStyle = (IWParagraphStyle)Style.CreateBuiltinStyle(builtinStyle, Document);
                    if ((pStyle as WParagraphStyle).StyleId > 10)
                        (pStyle as WParagraphStyle).StyleId = DEF_USER_STYLE_ID;
                    Document.Styles.Add(pStyle);
                    if (builtinStyle != BuiltinStyle.MacroText && builtinStyle != BuiltinStyle.CommentSubject)
                    {
                        (pStyle as WParagraphStyle).ApplyBaseStyle(DEF_NORMAL_STYLE);
                    }
                }
                ApplyStyle(pStyle as IWParagraphStyle);
                EnsureNextStyle(pStyle as Style);
            }
        }
        /// <summary>
        /// Ensure Next style
        /// </summary>
        /// <param name="style"></param>
        private void EnsureNextStyle(Style style)
        {
            //The following code Preserves the next style if it is not populated already
            if (style.NextStyle == null || style.NextStyle == string.Empty)
            {
                if (!style.Name.Contains("List") && style.Name != "No Spacing")
                    (style as Style).NextStyle = "Normal";
                else
                    (style as Style).NextStyle = style.Name;
            }
        }
        /// <summary>
        /// Gets related style.
        /// </summary>
        public IWParagraphStyle GetStyle()
        {
            return m_style;
        }
        /// <summary>
        /// Removes the absolute position data. If paragraph has absolute position in the document,
        /// all position data will be erased.
        /// </summary>
        public void RemoveAbsPosition()
        {
            if (m_prFormat != null)
                m_prFormat.RemovePositioning();
        }
        #endregion

        #region Public methods / append items
        /// <summary>
        /// Appends text to end of document.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public IWTextRange AppendText(string text)
        {
            IWTextRange textRange = AppendItem(ParagraphItemType.TextRange) as IWTextRange;
            textRange.Text = text;

            return textRange;
        }
        /// <summary>
        /// Appends image to end of paragraph.
        /// </summary>
        /// <returns></returns>
        public IWPicture AppendPicture(byte[] imageBytes)
        {
            IWPicture pic = (IWPicture)AppendItem(ParagraphItemType.Picture);
            pic.LoadImage(imageBytes);
            Document.HasPicture = true;
            return pic;
        }
        /// <summary>
        /// Appends field to end of paragraph
        /// </summary>
        /// <returns></returns>
        public IWField AppendField(string fieldName, FieldType fieldType)
        {
            if (fieldName == null)
                throw new ArgumentNullException("fieldName");

            // Process form fields
            if (fieldType == FieldType.FieldFormCheckBox)
            {
                return AppendCheckBox(fieldName, false);
            }
            else if (fieldType == FieldType.FieldFormDropDown)
            {
                return AppendDropDownFormField(fieldName);
            }
            else if (fieldType == FieldType.FieldFormTextInput)
            {
                return AppendTextFormField(fieldName, fieldName);
            }
            else if (fieldType == FieldType.FieldIndexEntry)
            {
                return AppendIndexEntry(fieldName);
            }

            WField field;
            if (fieldType == FieldType.FieldMergeField)
            {
                WMergeField mField = new WMergeField(Document);
                mField.FieldName = fieldName;
                field = mField;
            }
            else if (fieldType == FieldType.FieldSequence)
            {
                field = new WSeqField(Document);
            }
            else
            {
                field = new WField(Document);
            }

            field.FieldType = fieldType;

            if (field.FieldType == FieldType.FieldFormula)
            {
                fieldName = fieldName.Replace(" ", string.Empty);
                fieldName = fieldName.Replace("\"", string.Empty);
                fieldName = fieldName.Replace("=", string.Empty);
            }

            if (field.FieldType != FieldType.FieldMergeField)
            {
                if (fieldName.IndexOf(' ') != -1 && field.FieldType != FieldType.FieldIndex)
                    field.m_fieldValue = "\"" + fieldName + "\"";
                else
                    field.m_fieldValue = fieldName;
            }

            if (fieldType == FieldType.FieldDocVariable)
            {
                field.m_formattingString = "\\* MERGEFORMAT";
            }

            //AppendParagraphItem( mField );
            m_pItemColl.Add(field);
            if ((field.FieldType != FieldType.FieldMergeField && field.FieldType != FieldType.FieldNext))
            {
                field.FieldSeparator = AppendFieldMark(FieldMarkType.FieldSeparator);
            }
            if (fieldType != FieldType.FieldMergeField && fieldType != FieldType.FieldNext)
            {
                if (fieldType != FieldType.FieldSequence)
                {
                    WTextRange range = new WTextRange(Document);
                    range.Text = fieldName;
                    m_pItemColl.Add(range);

                    if (fieldType == FieldType.FieldHyperlink)
                    {
                        range.CharacterFormat.TextColor = Color.Blue;
                        range.CharacterFormat.UnderlineStyle = UnderlineStyle.Single;
                    }
                }

                WFieldMark end = new WFieldMark(Document, FieldMarkType.FieldEnd);
                m_pItemColl.Add(end);
                field.FieldEnd = end;
            }
            return field;
        }
        /// <summary>
        /// Appends the hyperlink.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="type">The hyperlink type.</param>
        /// <returns></returns>
        public IWField AppendHyperlink(string link, string text, HyperlinkType type)
        {
            return AppendHyperlink(link, text, null, type);
        }
        /// <summary>
        /// Appends the hyperlink.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="picture">The picture to display.</param>
        /// <param name="type">The type of hyperlink.</param>
        /// <returns></returns>
        public IWField AppendHyperlink(string link, WPicture picture, HyperlinkType type)
        {
            return AppendHyperlink(link, null, picture, type);
        }
        /// <summary>
        /// Appends start of the bookmark with specified name into paragraph.
        /// </summary>
        public BookmarkStart AppendBookmarkStart(string name)
        {
            BookmarkStart bkmk = new BookmarkStart(Document, name);
            Items.Add(bkmk);
            return bkmk;
        }
        /// <summary>
        /// Appends end of the bookmark with specified name into paragraph.
        /// </summary>
        public BookmarkEnd AppendBookmarkEnd(string name)
        {
            BookmarkEnd bkmk = new BookmarkEnd(Document, name);
            Items.Add(bkmk);
            return bkmk;
        }
        /// <summary>
        /// Appends the comment.
        /// </summary>
        /// <param name="text">The string.</param>
        /// <returns>Returns WComment.</returns>
        public WComment AppendComment(string text)
        {
            WComment comm = (WComment)AppendItem(ParagraphItemType.Comment);
            IWParagraph para = comm.TextBody.AddParagraph();
            para.AppendText(text);

            return comm;
        }
        /// <summary>
        /// Appends the footnote.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>returns the footnotes.</returns>
        public WFootnote AppendFootnote(FootnoteType type)
        {
            WFootnote foot = (WFootnote)AppendItem(ParagraphItemType.Footnote);
            foot.FootnoteType = type;
            foot.EnsureFtnStyle();
            return foot;
        }
        /// <summary>
        /// Append Textbox to the end of the paragraph
        /// </summary>
        /// <param name="width">Textbox width</param>
        /// <param name="height">Textbox height</param>
        /// <returns></returns>
        public IWTextBox AppendTextBox(float width, float height)
        {
            IWTextBox textbox = AppendItem(ParagraphItemType.TextBox) as IWTextBox;
            textbox.TextBoxFormat.Width = width;
            textbox.TextBoxFormat.Height = height;
            return textbox;
        }
        /// <summary>
        /// Appends the check box.
        /// </summary>
        /// <returns></returns>
        public WCheckBox AppendCheckBox()
        {
            string titleName = "Check_" + (Guid.NewGuid().ToString()).Replace("-", "_");
            titleName = titleName.Substring(0, 20);
            return AppendCheckBox(titleName, false);
        }
        /// <summary>
        /// Appends the check box.
        /// </summary>
        /// <param name="checkBoxName">Name of the check box.</param>
        /// <param name="defaultCheckBoxValue">Default checkbox value</param>
        /// <returns></returns>
        public WCheckBox AppendCheckBox(string checkBoxName, bool defaultCheckBoxValue)
        {
            WCheckBox checkBox = Document.CreateParagraphItem(ParagraphItemType.CheckBox) as WCheckBox;
            checkBox.Name = checkBoxName;
            checkBox.DefaultCheckBoxValue = defaultCheckBoxValue;
            this.Items.Add(checkBox);
            return checkBox;
        }
        /// <summary>
        /// Appends the text form field.
        /// </summary>
        /// <param name="defaultText">The default text. Pass "null" to insert default Word text</param>
        /// <returns></returns>
        public WTextFormField AppendTextFormField(string defaultText)
        {
            string titleName = "Text_" + (Guid.NewGuid().ToString()).Replace("-", "_");
            titleName = titleName.Substring(0, 20);
            return AppendTextFormField(titleName, defaultText);
        }
        /// <summary>
        /// Appends the text form field.
        /// </summary>
        /// <param name="formFieldName">Name of the form field.</param>
        /// <param name="defaultText">The default text. Pass "null" to insert default Word text</param>
        /// <returns></returns>
        public WTextFormField AppendTextFormField(string formFieldName, string defaultText)
        {
            WTextFormField textFormField = Document.CreateParagraphItem(ParagraphItemType.TextFormField) as WTextFormField;
            textFormField.Name = formFieldName;
            this.Items.Add(textFormField);
            if (defaultText == null)
            {
                textFormField.DefaultText = WTextFormField.DEF_TEXT;
                textFormField.Text = WTextFormField.DEF_TEXT;
            }
            else
            {
                textFormField.DefaultText = defaultText;
                textFormField.Text = defaultText;
            }
            return textFormField;
        }
        /// <summary>
        /// Appends the drop down form field.
        /// </summary>
        /// <returns></returns>
        public WDropDownFormField AppendDropDownFormField()
        {
            string titleName = "Drop_" + (Guid.NewGuid().ToString()).Replace("-", "_");
            titleName = titleName.Substring(0, 20);
            return AppendDropDownFormField(titleName);
        }
        /// <summary>
        /// Appends the drop down form field.
        /// </summary>
        /// <param name="dropDropDownName">Name of the drop drop down.</param>
        /// <returns></returns>
        public WDropDownFormField AppendDropDownFormField(string dropDropDownName)
        {
            WDropDownFormField dropDown = Document.CreateParagraphItem(ParagraphItemType.DropDownFormField) as WDropDownFormField;
            dropDown.Name = dropDropDownName;
            this.Items.Add(dropDown);
            return dropDown;
        }
        /// <summary>
        /// Appends special symbol to end of paragraph.
        /// </summary>
        /// <param name="characterCode">The character code.</param>
        /// <returns></returns>
        public WSymbol AppendSymbol(byte characterCode)
        {
            WSymbol symbol = (WSymbol)AppendItem(ParagraphItemType.Symbol);
            symbol.CharacterCode = characterCode;
            return symbol;
        }
        /// <summary>
        /// Appends break to end of paragraph.
        /// </summary>
        public Break AppendBreak(BreakType breakType)
        {
            Break docBreak = new Break(Document, breakType);
            Items.Add(docBreak);

            return docBreak;
        }
        /// <summary>
        /// Appends Shape to the end of paragraph
        /// </summary>
        /// <param name="autoShapeType"></param>
        /// <returns></returns>
        public Shape AppendShape(AutoShapeType autoShapeType, float width, float height)
        {
            Shape shape = new Shape(Document, autoShapeType);
            shape.Width = width;
            shape.Height = height;
            Items.Add(shape);
            return shape;
        }
        /// <summary>
        /// Appends the table of content to the end of the paragraph.
        /// </summary>
        /// <param name="lowerHeadingLevel">The starting heading level of the table of content.</param>
        /// <param name="upperHeadingLevel">The ending heading level of the table of content.</param>
        /// <returns></returns>
        public TableOfContent AppendTOC(int lowerHeadingLevel, int upperHeadingLevel)
        {
            TableOfContent toc = AppendItem(ParagraphItemType.TOC) as TableOfContent;
            toc.LowerHeadingLevel = lowerHeadingLevel;
            toc.UpperHeadingLevel = upperHeadingLevel;
            AppendFieldMark(FieldMarkType.FieldSeparator);
            AppendText("TOC");
            AppendFieldMark(FieldMarkType.FieldEnd);
            Document.TOC = toc;
            return toc;
        }
#if SILVERLIGHT || WP
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <returns></returns>
        public IWPicture AppendPicture(Stream imageStream)
        {
            WPicture picture = AppendItem(ParagraphItemType.Picture) as WPicture;
            picture.LoadImage(imageStream);
            return picture;
        }
#else
        /// <summary>
        /// Appends the picture
        /// </summary>
        /// <param name="image">The image</param>
        /// <returns></returns>
        public IWPicture AppendPicture(Image image)
        {
            WPicture picture = AppendItem(ParagraphItemType.Picture) as WPicture;
            picture.LoadImage(image);
            Document.HasPicture = true;
            return picture;
        }
#endif
#if (!SILVERLIGHT && !WP) || WINRT
        /// <summary>
        /// Appends the HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        public void AppendHTML(string html)
        {
            Document.IsOpening = true;
            string lowHtml = html.ToLower();
            if (!lowHtml.StartsWith("<html") && !lowHtml.StartsWith("<body") && !lowHtml.StartsWith("<!doctype") && !lowHtml.StartsWith("<?xml"))
            {
                html = "<html><head><title/></head><body>" + html + "</body></html>";
            }

            IHtmlConverter htmlConverter = HtmlConverterFactory.GetInstance();
            (htmlConverter as HTMLConverterImpl).HtmlImportSettings = this.Document.HTMLImportSettings;

            if (this.IsStyleApplied)
            {
                htmlConverter.AppendToTextBody(this.OwnerTextBody,
                 html, this.GetIndexInOwnerCollection(), Items.Count, this.ParaStyle, this.ListFormat.CurrentListStyle);
            }
            else
            {
                htmlConverter.AppendToTextBody(this.OwnerTextBody,
                  html, this.GetIndexInOwnerCollection(), Items.Count, null, this.ListFormat.CurrentListStyle);
            }
            Document.IsOpening = false;
        }
#endif
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Appends the OLE object into paragraph.
        /// </summary>
        /// <param name="pathToFile">The path to file.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <param name="type">The type of OLE object.</param>
        /// <returns></returns>
        public WOleObject AppendOleObject(string pathToFile, WPicture olePicture, OleObjectType type)
        {
            if (!File.Exists(pathToFile))
                throw new ArgumentException("File does not exists, please use valid file path.");
            byte[] buffer = File.ReadAllBytes(pathToFile);
            WOleObject oleObject = new WOleObject(m_doc);
            Items.Add(oleObject);
            oleObject.SetOlePicture(olePicture);
            oleObject.SetLinkType(OleLinkType.Embed);

            oleObject.ObjectType = OleTypeConvertor.ToString(type, false);
            oleObject.OleObjectType = type;
            oleObject.CreateOleObjContainer(buffer, Path.GetFileName(pathToFile));
            oleObject.Field.FieldType = FieldType.FieldEmbed;

            WFieldMark separator = new WFieldMark(m_doc);
            separator.Type = FieldMarkType.FieldSeparator;
            separator.CharacterFormat.CharacterProps.PicLocation = int.Parse(oleObject.OleStorageName);
            separator.CharacterFormat.CharacterProps.IsOle2 = true;
            Items.Add(separator);
            Items.Add(oleObject.OlePicture);
            AppendFieldMark(FieldMarkType.FieldEnd);

            return oleObject;
        }

        /// <summary>
        /// Appends the OLE object.
        /// </summary>
        /// <param name="pathToFile">The path to file.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <returns></returns>
        public WOleObject AppendOleObject(string pathToFile, WPicture olePicture)
        {
            return AppendOleObject(pathToFile, olePicture, OleObjectType.Package);
        }
#endif
        /// <summary>
        /// Appends the OLE object.
        /// </summary>
        /// <param name="oleStorage">The OLE object (file) stream.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <param name="type">The type of OLE object.</param>
        /// <returns></returns>
        public WOleObject AppendOleObject(Stream oleStream, WPicture olePicture, OleObjectType type)
        {
            if (oleStream == null || oleStream.Length == 0)
                return null;

            if (type == OleObjectType.Package)
                throw new ArgumentException("Please use AppendOleObject(Stream oleStream, WPicture olePicture, string fileExtension) method.  Package type is invalid in this context.");

            oleStream.Position = 0;
            WOleObject oleObject = AppendItem(ParagraphItemType.OleObject) as WOleObject;
            oleObject.SetOlePicture(olePicture);
            oleObject.SetLinkType(OleLinkType.Embed);
            oleObject.ObjectType = OleTypeConvertor.ToString(type, false);
            oleObject.OleObjectType = type;
            oleObject.ParseOleStream(oleStream);
            oleObject.Field.FieldType = FieldType.FieldEmbed;

            WFieldMark separator = new WFieldMark(m_doc);
            separator.Type = FieldMarkType.FieldSeparator;
            separator.CharacterFormat.CharacterProps.PicLocation = int.Parse(oleObject.OleStorageName);
            separator.CharacterFormat.CharacterProps.IsOle2 = true;
            Items.Add(separator);
            Items.Add(oleObject.OlePicture);
            AppendFieldMark(FieldMarkType.FieldEnd);

            return oleObject;
        }
        /// <summary>
        /// Appends the OLE object into paragraph.
        /// </summary>
        /// <param name="oleBytes">The OLE object (file) bytes.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <param name="type">The type of OLE object.</param>
        /// <returns></returns>
        public WOleObject AppendOleObject(byte[] oleBytes, WPicture olePicture, OleObjectType type)
        {
            if (oleBytes == null || oleBytes.Length == 0)
                return null;

            if (type == OleObjectType.Package)
                throw new ArgumentException("Please use AppendOleObject(byte[] oleBytes, WPicture olePicture, string fileExtension) method.  Package type is invalid in this context.");

            MemoryStream oleStream = new MemoryStream(oleBytes);
            return AppendOleObject(oleStream, olePicture, type);
        }
        /// <summary>
        /// Appends the OLE object into paragraph.
        /// </summary>
        /// <param name="oleStorage">The OLE storage.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <param name="oleLinkType">The type of OLE object link type.</param>
        /// <param name="oleLinkType"></param>
        /// <returns></returns>
        public WOleObject AppendOleObject(Stream oleStream, WPicture olePicture, OleLinkType oleLinkType)
        {
            WOleObject oleObject = AppendItem(ParagraphItemType.OleObject) as WOleObject;
            oleObject.SetOlePicture(olePicture);
            oleObject.SetLinkType(oleLinkType);
            oleStream.Position = 0;
            oleObject.ParseOleStream(oleStream);

            if (oleLinkType == OleLinkType.Embed)
                oleObject.Field.FieldType = FieldType.FieldEmbed;
            else
                oleObject.Field.FieldType = FieldType.FieldLink;

            WFieldMark separator = new WFieldMark(m_doc);
            separator.Type = FieldMarkType.FieldSeparator;
            separator.CharacterFormat.CharacterProps.PicLocation = int.Parse(oleObject.OleStorageName);
            separator.CharacterFormat.CharacterProps.IsOle2 = true;
            this.Items.Add(separator);
            this.Items.Add(oleObject.OlePicture);
            AppendFieldMark(FieldMarkType.FieldEnd);

            return oleObject;
        }
        /// <summary>
        /// Appends the OLE object.
        /// </summary>
        /// <param name="oleBytes">The OLE bytes.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <param name="oleLinkType">Type of the OLE link.</param>
        /// <returns></returns>
        public WOleObject AppendOleObject(byte[] oleBytes, WPicture olePicture, OleLinkType oleLinkType)
        {
            MemoryStream oleStream = new MemoryStream(oleBytes);
            return AppendOleObject(oleStream, olePicture, oleLinkType);
        }
        /// <summary>
        /// Appends the package OLE object (ole object without specified type).
        /// </summary>
        /// <param name="oleBytes">The OLE object bytes.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns></returns>
        public WOleObject AppendOleObject(byte[] oleBytes, WPicture olePicture, string fileExtension)
        {
            WOleObject oleObject = new WOleObject(m_doc);
            Items.Add(oleObject);
            oleObject.SetOlePicture(olePicture);
            oleObject.SetLinkType(OleLinkType.Embed);

            oleObject.ObjectType = OleTypeConvertor.ToString(OleObjectType.Package, false);
            oleObject.OleObjectType = OleObjectType.Package;
            string dataPath = "Package" + "." + fileExtension.Replace(".", string.Empty);
            oleObject.CreateOleObjContainer(oleBytes, dataPath);
            oleObject.Field.FieldType = FieldType.FieldEmbed;

            WFieldMark separator = new WFieldMark(m_doc);
            separator.Type = FieldMarkType.FieldSeparator;
            separator.CharacterFormat.CharacterProps.PicLocation = int.Parse(oleObject.OleStorageName);
            separator.CharacterFormat.CharacterProps.IsOle2 = true;
            Items.Add(separator);
            Items.Add(oleObject.OlePicture);
            AppendFieldMark(FieldMarkType.FieldEnd);

            return oleObject;
        }
        /// <summary>
        /// Appends the package OLE object (ole object without specified type).
        /// </summary>
        /// <param name="oleStream">The OLE file stream.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns></returns>
        public WOleObject AppendOleObject(Stream oleStream, WPicture olePicture, string fileExtension)
        {
            oleStream.Position = 0;
            byte[] oleBytes = new byte[oleStream.Length];
            oleStream.Read(oleBytes, 0, oleBytes.Length);
            return AppendOleObject(oleBytes, olePicture, fileExtension);
        }
        #endregion

        #region Internal methods / append items
        /// <summary>
        /// Appends the field mark.
        /// </summary>
        /// <param name="type">The type.</param>
        internal WFieldMark AppendFieldMark(FieldMarkType type)
        {
            WFieldMark fldMark = (WFieldMark)AppendItem(ParagraphItemType.FieldMark);
            fldMark.Type = type;
            return fldMark;
        }
        /// <summary>
        /// Appends lineBreak
        /// </summary>
        /// <param name="lineBreakText">The line break text.</param>
        /// <returns></returns>
        internal Break AppendLineBreak(string lineBreakText)
        {
            Break lineBreak = new Break(Document, BreakType.LineBreak);
            lineBreak.TextRange.Text = lineBreakText;
            Items.Add(lineBreak);

            return lineBreak;
        }
        /// <summary>
        /// Appends the hyperlink.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="text">The text.</param>
        /// <param name="pict">The picture.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        internal IWField AppendHyperlink(string link, string text, WPicture pict, HyperlinkType type)
        {
            WField fieldStart = new WField(Document);
            fieldStart.FieldType = FieldType.FieldHyperlink;
            Items.Add(fieldStart);
            AppendFieldMark(FieldMarkType.FieldSeparator);

            if (text != null)
            {
                IWTextRange textRange = AppendText(text);
                textRange.CharacterFormat.TextColor = Color.Blue;
                textRange.CharacterFormat.UnderlineStyle = UnderlineStyle.Single;
            }
            else if (pict != null)
            {
                Items.Add(pict);
            }
            else
            {
                AppendText("Hyperlink");
            }

            WFieldMark end = new WFieldMark(Document, FieldMarkType.FieldEnd);
            Items.Add(end);

            Hyperlink hl = new Hyperlink(fieldStart);
            hl.Type = type;
            if (type == HyperlinkType.WebLink || type == HyperlinkType.EMailLink)
                hl.Uri = link;
            else if (hl.Type == HyperlinkType.Bookmark)
                hl.BookmarkName = link;
            else if (hl.Type == HyperlinkType.FileLink)
                hl.FilePath = link;

            return fieldStart;
        }
        /// <summary>
        /// Loads the picture.
        /// </summary>
        /// <param name="picture">The picture.</param>
        /// <param name="imageRecord">The image record.</param>
        internal void LoadPicture(WPicture picture, ImageRecord imageRecord)
        {
            picture.LoadImage(imageRecord);
            Document.HasPicture = true;
        }
        /// <summary>
        /// Appends the index entry.
        /// </summary>
        /// <param name="entryToMark">The entry to mark.</param>
        /// <returns></returns>
        internal IWField AppendIndexEntry(string entryToMark)
        {
            WField field = new WField(Document);
            field.FieldType = FieldType.FieldIndexEntry;
            field.m_formattingString = "\"" + entryToMark + "\"";
            field.CharacterFormat.FieldVanishComplex = (byte)129;
            this.Items.Add(field);

            WFieldMark fldMark = AppendFieldMark(FieldMarkType.FieldEnd);
            fldMark.CharacterFormat.FieldVanishComplex = (byte)129;

            return field;
        }
        #endregion

        #region Public methods / find and replace
        /// <summary>
        /// Returns first entry of given regex.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public override TextSelection Find(Regex pattern)
        {
            if (FindUtils.IsPatternEmpty(pattern))
                throw new ArgumentException("Search string cannot be empty");

            List<TextSelection> selections = TextFinder.Instance.Find(this, pattern, true);
            return (selections.Count > 0) ? selections[0] : null;
        }
        /// <summary>
        /// Returns first entry of given string, taking into consideration caseSensitive
        /// and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="caseSensitive">if it is case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies to search a whole word, set to <c>true</c>.</param>
        /// <returns></returns>
        public TextSelection Find(string given, bool caseSensitive, bool wholeWord)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);

            return Find(pattern);
        }
        /// <summary>
        /// Replaces all entries of given regular expression with replace string.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replace">The replace.</param>
        /// <returns></returns>
        public override int Replace(Regex pattern, string replace)
        {
            if (FindUtils.IsPatternEmpty(pattern))
                throw new ArgumentException("Search string cannot be empty");

            TextReplacer textReplacer = TextReplacer.Instance;
            return textReplacer.Replace(this, pattern, replace);
        }
        /// <summary>
        /// Replaces all entries of given string with replace string, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given text to replace.</param>
        /// <param name="replace">The replace text .</param>
        /// <param name="caseSensitive">if specifies case sensitive, set to <c>true</c> .</param>
        /// <param name="wholeWord">if it specifies to search a whole word, set to <c>true</c>.</param>
        /// <returns></returns>
        public override int Replace(string given, string replace, bool caseSensitive, bool wholeWord)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);

            return Replace(pattern, replace);
        }
        /// <summary>
        /// Replaces all entries of given regular expression with TextRangesHolder.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <returns></returns>
        public override int Replace(Regex pattern, TextSelection textSelection)
        {
            return Replace(pattern, textSelection, false);
        }
        /// <summary>
        /// Replaces all entries of given regular expression with TextRangesHolder.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="saveFormatting">if it specifies save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public override int Replace(Regex pattern, TextSelection textSelection, bool saveFormatting)
        {
            if (FindUtils.IsPatternEmpty(pattern))
                throw new ArgumentException("Search string cannot be empty");

            textSelection.CacheRanges();
            TextSelectionList selections = FindAll(pattern);

            if (selections != null)
            {
                foreach (TextSelection sel in selections)
                {
                    WCharacterFormat srcFormat = null;
                    if (sel.StartTextRange is WTextRange && saveFormatting)
                        srcFormat = (sel.StartTextRange as WTextRange).CharacterFormat;

                    int selIndex = sel.SplitAndErase();
                    WParagraph para = sel.OwnerParagraph;

                    // Inserts textSelection
                    textSelection.CopyTo(para, selIndex, saveFormatting, srcFormat);
                }

                return selections.Count;
            }

            //TextSelectionReplacer textReplacer = TextSelectionReplacer.Instance;
            //return textReplacer.Replace( this, pattern, textSelection );      
            return 0;
        }
        /// <summary>
        /// Replaces all entries of given string with TextRangesHolder, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="caseSensitive">if it specifies case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies to check whole word, set to <c>true</c> .</param>
        public int Replace(string given, TextSelection textSelection, bool caseSensitive, bool wholeWord)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);
            return Replace(pattern, textSelection, false);
        }
        /// <summary>
        /// Replaces all entries of given string with TextRangesHolder, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="caseSensitive">if it specifies case sensitive, set to <c>true</c> .</param>
        /// <param name="wholeWord">if it specifies to search a whole word, set to <c>true</c> .</param>
        /// <param name="saveFormatting">if it specifies save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public int Replace(string given, TextSelection textSelection, bool caseSensitive, bool wholeWord, bool saveFormatting)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);
            return Replace(pattern, textSelection, saveFormatting);
        }
        /// <summary>
        /// Replaces first entry of given string with replace string, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The string to replace</param>
        /// <param name="replace">Replace string</param>
        /// <param name="caseSensitive">Is case sensitive replace?</param>
        /// <param name="wholeWord">Search for whole word?</param>
        /// <returns></returns>
        internal int ReplaceFirst(string given, string replace, bool caseSensitive, bool wholeWord)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);
            return ReplaceFirst(pattern, replace);
        }
        /// <summary>
        /// Replaces all entries of given regular expression with replace string.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="replace"></param>
        internal int ReplaceFirst(Regex pattern, string replace)
        {
            if (FindUtils.IsPatternEmpty(pattern))
                throw new ArgumentException("Search string cannot be empty");

            TextReplacer textReplacer = TextReplacer.Instance;
            bool prevState = Document.ReplaceFirst;
            Document.ReplaceFirst = true;
            int repItemsCnt = textReplacer.Replace(this, pattern, replace);
            Document.ReplaceFirst = prevState;

            return repItemsCnt;
        }
        #endregion

        #region Implementation / Insert section break
        /// <summary>
        /// Inserts the section break.
        /// Creates new section with the break type new page.
        /// </summary>
        /// <returns></returns>
        public WSection InsertSectionBreak()
        {
            return InsertSectionBreak(SectionBreakCode.NewPage);
        }
        /// <summary>
        /// Inserts the section break.
        /// Creates new section with the specified break type.
        /// </summary>
        /// <param name="breakType">Type of the break.</param>
        /// <returns></returns>
        public WSection InsertSectionBreak(SectionBreakCode breakType)
        {
            //Gets the current owner section.
            WSection section = GetOwnerSection();

            if (section == null)
                throw new Exception("Owner section cannot be null.");

            if (m_ownerTextBodyItem.Owner is HeaderFooter)
                throw new NotSupportedException("Cannot insert section break for header footer items.");

            //Gets the index of the current section.
            int sectionIndex = section.GetIndexInOwnerCollection();

            //Creates new section by cloning the page setup, column and header footers of the current section.
            WSection newSection = section.CloneWithoutBodyItems();
            Document.Sections.Insert(sectionIndex + 1, newSection);

            //Sets the specified break type.
            newSection.BreakCode = breakType;

            //Updates the text body items to the new section.
            int itemIndex = m_ownerTextBodyItem.GetIndexInOwnerCollection();
            int itemsCount = section.Body.Items.Count;
            for (int i = itemIndex + 1; i < itemsCount; i++)
            {
                newSection.Body.Items.Insert(newSection.Body.Items.Count, section.Body.Items[itemIndex + 1]);
            }
            return newSection;
        }
        /// <summary>
        /// Gets the owner section.
        /// </summary>
        /// <returns></returns>
        private WSection GetOwnerSection()
        {
            Entity entity = this as Entity;
            while (!(entity is WSection))
            {
                if (entity is TextBodyItem)
                    m_ownerTextBodyItem = entity as TextBodyItem;
                if (entity.Owner != null)
                    entity = entity.Owner;
                else
                    break;
            }
            return entity as WSection;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the paragraph items.
        /// </summary>
        /// <returns></returns>
        internal ParagraphItemCollection GetParagraphItems()
        {
            ParagraphItemCollection paragraphItems = new ParagraphItemCollection(this);
            for (int i = 0; i < m_pItemColl.Count; i++)
            {
                if (m_pItemColl[i] is StructureDocumentTagInline)
                    (m_pItemColl[i] as StructureDocumentTagInline).CopyItemsTo(paragraphItems);
                else
                    paragraphItems.InnerList.Add(m_pItemColl[i]);
            }
            return paragraphItems;
        }
        /// <summary>
        /// Clears the items.
        /// </summary>
        internal void ClearItems()
        {
            //Removes all the items of the paragraph.
            Items.InnerList.Clear();
            m_strTextBuilder = new StringBuilder(1);
        }
        /// <summary>
        /// Returns all entries of given regex.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        internal override TextSelectionList FindAll(Regex pattern)
        {
            return TextFinder.Instance.Find(this, pattern, false);
        }
        /// <summary>
        /// Returns first entry of given regex.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        internal TextSelectionList FindFirst(Regex pattern)
        {
            return TextFinder.Instance.Find(this, pattern, true);
        }
        /// <summary>
        /// Removes the items range.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="toEnd">if it speicifes the end, set to <c>true</c>.</param>
        internal void RemoveItems(int startIndex, bool toEnd)
        {
            if (toEnd)
            {
                while (startIndex < Items.Count)
                {
                    Items.RemoveAt(startIndex);
                }
            }
            else
            {
                while (startIndex > -1)
                {
                    Items.RemoveAt(startIndex);
                    startIndex--;
                }
            }
        }
        /// <summary>
        /// Clones without paragraph items.
        /// </summary>
        /// <returns></returns>
        internal WParagraph CloneWithoutItems()
        {
            return CloneParagraph(false);
        }
        /// <summary>
        /// Appends paragraph item to the end of paragraph.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        internal IParagraphItem AppendItem(ParagraphItemType itemType)
        {
            IParagraphItem item = Document.CreateParagraphItem(itemType);
            Items.Add(item);
            return item;
        }
        /// <summary>
        /// Updates the text.
        /// </summary>
        /// <param name="pItem">The p item.</param>
        /// <param name="newText">The new text.</param>
        internal void UpdateText(WTextRange pItem, string newText)
        {
            UpdateText(pItem, pItem.TextLength, newText);
        }
        /// <summary>
        /// Updates the text.
        /// </summary>
        /// <param name="pItem">The paragraph item.</param>
        /// <param name="removeTextLength">Length of the text to remove.</param>
        /// <param name="newText">The new text.</param>
        internal void UpdateText(ParagraphItem pItem, int removeTextLength, string newText)
        {
            m_strTextBuilder.Remove(pItem.StartPos, removeTextLength);
            m_strTextBuilder.Insert(pItem.StartPos, newText);

            // Corrects positions of next items.
            int offset = newText.Length - removeTextLength;
            int pItemIndex = m_pItemColl.IndexOf(pItem);

            if (pItemIndex < 0)
            {
                throw new InvalidOperationException("pItem haven't found in paragraph items");
            }

            for (int i = pItemIndex + 1, len = m_pItemColl.Count; i < len; i++)
            {
                ParagraphItem item = m_pItemColl[i];

                if (item != null)
                {
                    item.StartPos += offset;
                }
            }
        }
        /// <summary>
        /// Applies the specified style.
        /// </summary>
        /// <param name="style">Style name</param>
        /// <remarks>Specified style must exist in Document.Styles collection</remarks>
        internal void ApplyStyle(IWParagraphStyle style)
        {
            if (style == null)
                throw new ArgumentNullException("newStyle");

            m_style = style;
            ApplyBaseStyleFormats();
            EnsureNextStyle(style as Style);
            //To handle the case paragraph style id not present in the style sheet
            if (style.Name == DEF_NORMAL_STYLE && ParagraphFormat.Sprms != null
                && ParagraphFormat.Sprms[WordSprmOptions.sprmPIstd] != null
                && ParagraphFormat.Sprms[WordSprmOptions.sprmPIstd].ShortValue != 0)
                ParagraphFormat.Sprms[WordSprmOptions.sprmPIstd].ShortValue = 0;
        }
        /// <summary>
        /// Replaces the substring.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        /// <param name="replacement">The replacement.</param>
        internal void ReplaceWithoutCorrection(int start, int length, string replacement)
        {
            int offset = replacement.Length - length;
            m_strTextBuilder.Remove(start, length);
            m_strTextBuilder.Insert(start, replacement);
        }
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        {
            foreach (ParagraphItem item in Items)
            {
                item.AddSelf();
            }
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            if (doc.ImportOption == ImportOptions.UseDestinationStyles)
                CloneStyleRelations(doc);
            else if (doc.ImportOption == ImportOptions.MergeFormatting)
                UpdateMergeFormatting(doc);
            else
                UpdateSourceFormatting(doc);
            CloneListStyleTo(doc);
            if (doc != this.Document)
            {
                // Updates the document default formattings if the destination and source document is different.
                this.m_charFormat.UpdateDefaultFormats();
                this.m_prFormat.UpdateDefaultFormats();
            }
            ParagraphItem item = null;
            for (int i = 0, cnt = Items.Count; i < cnt; i++)
            {
                item = Items[i];
                item.CloneRelationsTo(doc, nextOwner);
            }
            if (doc.ImportOption != ImportOptions.UseDestinationStyles)
                ApplyStyle(m_style);
        }
        /// <summary>
        /// Updates the merge formatting.
        /// </summary>
        /// <param name="doc">The doc.</param>
        private void UpdateMergeFormatting(WordDocument doc)
        {
            //Merges the source and destination formatting.
            WParagraph lastParagraph = doc.LastParagraph;
            if (lastParagraph == null)
                lastParagraph = new WParagraph(doc);
            ParagraphFormat.ImportContainer(lastParagraph.ParagraphFormat);
            ParagraphFormat.CopyProperties(lastParagraph.ParagraphFormat);
            BreakCharacterFormat.MergeFormat(lastParagraph.BreakCharacterFormat);
            m_style = lastParagraph.m_style;
        }
        /// <summary>
        /// Updates the source formatting.
        /// </summary>
        /// <param name="doc">The doc.</param>
        private void UpdateSourceFormatting(WordDocument doc)
        {
            WParagraphStyle style = doc.Styles.FindByName(DEF_NORMAL_STYLE, StyleType.ParagraphStyle) as WParagraphStyle;
            if (style == null)
            {
                style = (WParagraphStyle)Style.CreateBuiltinStyle(BuiltinStyle.Normal, doc);
                if (doc.Styles.FindByName(DEF_NORMAL_STYLE, StyleType.ParagraphStyle) == null)
                    doc.Styles.Add(style);
            }
            //Updates the source formatting.
            if (doc.ImportOption == ImportOptions.KeepSourceFormatting)
            {
                WParagraphStyle paraStyle = style;
                if (m_style != null)
                    paraStyle = m_style as WParagraphStyle;
                ParagraphFormat.UpdateSourceFormat(paraStyle.ParagraphFormat);
                BreakCharacterFormat.UpdateSourceFormat(paraStyle.CharacterFormat);
            }
            m_style = style;
        }
        /// <summary>
        /// Clones the style relations.
        /// </summary>
        /// <param name="doc">The doc.</param>
        private void CloneStyleRelations(WordDocument doc)
        {
            if (doc.ImportStyles) CloneStyleTo(doc);
            else
            {
                IStyle foundStyle = doc.Styles.FindByName(m_style.Name, StyleType.ParagraphStyle);
                if (foundStyle != null)
                {
                    ApplyStyle(foundStyle as WParagraphStyle);
                }
                else
                {
                    // Clones the list style to destination document.
                    if (m_style is WParagraphStyle)
                        (m_style as WParagraphStyle).CloneListRelationsTo(doc);
                    // Clones the paragraph style to destination document.
                    WParagraphStyle pStyle = m_style.Clone() as WParagraphStyle;
                    doc.Styles.Add(pStyle);
                    ApplyStyle(pStyle);
                }
            }
        }
        /// <summary>
        /// Clones the list style.
        /// </summary>
        /// <param name="doc">The doc.</param>
        private void CloneListStyleTo(WordDocument doc)
        {
            if (ListFormat.ListType != ListType.NoList)
            {
                ListStyle lstStyle = ListFormat.CurrentListStyle;

                if (doc.ListStyles.FindByName(lstStyle.Name) == null)
                {
                    doc.ListStyles.Add((ListStyle)lstStyle.Clone());
                }
            }

            if (ListFormat.LFOStyleName == null)
                return;

            if (doc.ListOverrides.FindByName(ListFormat.LFOStyleName) == null)
            {
                ListOverrideStyle lfoStyle = this.Document.ListOverrides.FindByName(ListFormat.LFOStyleName);
                if (lfoStyle != null) doc.ListOverrides.Add((ListOverrideStyle)lfoStyle.Clone());
            }
        }
        /// <summary>
        /// Clones the style.
        /// </summary>
        /// <param name="doc">The doc.</param>
        private void CloneStyleTo(WordDocument doc)
        {
            if (m_style != null)
            {
                IStyle foundStyle = doc.Styles.FindByName(m_style.Name, StyleType.ParagraphStyle);

                // Export style object
                if (foundStyle == null)
                {
                    (m_style as Style).ImportStyleTo(doc);
                    if (m_style is WParagraphStyle)
                        (m_style as WParagraphStyle).CloneListRelationsTo(doc);
                    foundStyle = doc.Styles.FindByName(m_style.Name, StyleType.ParagraphStyle);
                    if (foundStyle is IWParagraphStyle)
                        ApplyStyle(foundStyle as IWParagraphStyle);
                }
                else
                {
                    if (doc.CurClonedSection != null)
                    {
                        m_style = (WParagraphStyle)(m_style as Style).ApplyOrImportStyleTo(doc, foundStyle);
                        (m_style as WParagraphStyle).CloneListRelationsTo(doc);
                        ApplyStyle(m_style);
                    }
                }
            }
        }
        /// <summary>
        /// Clone method implementation.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            return CloneParagraph(true);
        }
        /// <summary>
        /// Clones itself without paragraph items.
        /// </summary>
        /// <returns></returns>
        private WParagraph CloneParagraph(bool cloneItems)
        {
            WParagraph para = (WParagraph)base.CloneImpl();
            para.m_strTextBuilder = new StringBuilder(Text);

            para.m_pItemColl = new ParagraphItemCollection(para);

            if (cloneItems)
            {
                m_pItemColl.CloneItemsTo(para.m_pItemColl);
            }

            // Copies formats
            para.m_charFormat = new WCharacterFormat(Document);
            para.m_prFormat = new WParagraphFormat(Document);

            para.m_charFormat.ImportContainer(BreakCharacterFormat);
            para.m_charFormat.CopyProperties(BreakCharacterFormat);
            para.m_prFormat.ImportContainer(ParagraphFormat);
            para.m_prFormat.CopyProperties(ParagraphFormat);


            if (para.ListFormat.ListType != ListType.NoList)
            {
                para.m_listFormat = new WListFormat(this);
                para.m_listFormat.ImportContainer(ListFormat);
                para.m_listFormat.SetOwner(para);
            }

            IWParagraphStyle pStyle = GetStyle();

            if (pStyle != null)
            {
                WParagraphStyle clonedStyle = pStyle.Clone() as WParagraphStyle;
                para.ApplyStyle(clonedStyle);
            }

            para.CreateEmptyParagraph();
            para.ParagraphFormat.SetOwner(para);
            para.BreakCharacterFormat.SetOwner(para);
            return para;
        }
        /// <summary>
        /// Clones the paragraph as text only.
        /// </summary>
        /// <returns></returns>
        internal string GetParagraphText()
        {
            string text = GetText(0, m_pItemColl.Count - 1);
            return (text + ControlChar.ParagraphBreak);
        }
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns></returns>
        internal string GetText(int startIndex, int endIndex)
        {
            string text = string.Empty;
            for (int i = startIndex; i <= endIndex; i++)
            {
                ParagraphItem item = m_pItemColl[i];
                if (item is WMergeField)
                    text += (item as WTextRange).Text;
                else if (item is WField)
                {
                    text += (item as WField).GetFieldResult();
                    if (Document.m_prevClonedEntity != null)
                        break;
                    else if ((item as WField).FieldEnd.OwnerParagraph == this)
                        i = (item as WField).FieldEnd.GetIndexInOwnerCollection();
                }
                else if (item is WTextRange)
                    text += (item as WTextRange).Text;
                else if (item is Break)
                    text += ControlChar.ParagraphBreak;
            }
            return text;
        }
        /// <summary>
        /// 
        /// </summary>
        private void ApplyBaseStyleFormats()
        {
            if (m_style != null)
            {
                m_charFormat.ApplyBase(m_style.CharacterFormat);
                m_prFormat.ApplyBase(m_style.ParagraphFormat);

                ParagraphItem item = null;
                for (int i = 0, cnt = m_pItemColl.Count; i < cnt; i++)
                {
                    item = m_pItemColl[i];
                    item.ParaItemCharFormat.ApplyBase(m_style.CharacterFormat);
                    if (item is WMergeField)
                        (item as WMergeField).ApplyBaseFormat();
                    else if (item is Break)
                        (item as Break).TextRange.CharacterFormat.ApplyBase(m_style.CharacterFormat);
                    else if (item is StructureDocumentTagInline)
                        (item as StructureDocumentTagInline).ApplyBaseFormat();
                }
            }
        }
        /// <summary>
        /// Checks the name of the form field.
        /// </summary>
        /// <param name="formFieldName">Name of the form field.</param>
        internal void CheckFormFieldName(string formFieldName)
        {

            foreach (WSection section in this.Document.Sections)
            {
                if (section.Body.FormFields.ContainsName(formFieldName))
                {
                    throw new ArgumentException("Form field with name \"" + formFieldName + "\" already exist.");
                }
            }

            if (Document.Bookmarks[formFieldName] != null)
            {
                throw new ArgumentException("Can\'t create formfield with \"" + formFieldName
                  + "\" name: bookmark with such name already exists.");
            }
        }
        /// <summary>
        /// Applies the list style.
        /// </summary>
        /// <param name="builtinStyle">The built-in style.</param>
        private void ApplyListStyle(BuiltinStyle builtinStyle)
        {
            string listStyleName = Style.BuiltInToName(builtinStyle);
            ListStyle listStyle = Document.ListStyles.FindByName(listStyleName);
            if (listStyle == null)
            {
                listStyle = (ListStyle)Style.CreateBuiltinStyle(builtinStyle, StyleType.OtherStyle, Document);
                Document.ListStyles.Add(listStyle);
            }

            IWParagraphStyle pStyle = Document.Styles.FindByName(listStyle.Name) as IWParagraphStyle;
            if (pStyle == null)
            {
                pStyle = new WParagraphStyle(Document);
                pStyle.Name = listStyleName;
                (pStyle as WParagraphStyle).ApplyBaseStyle("Normal");
                Document.Styles.Add(pStyle);
            }

            ApplyStyle(pStyle);

            this.ListFormat.ApplyStyle(listStyle.Name);
            //listStyle.IsBuiltInStyle = true;
        }
        /// <summary>
        /// Checks the normal style.
        /// </summary>
        private void CheckNormalStyle()
        {
            WParagraphStyle pStyle = Document.Styles.FindByName("Normal", StyleType.ParagraphStyle) as WParagraphStyle;
            if (pStyle == null)
            {
                pStyle = (WParagraphStyle)Style.CreateBuiltinStyle(BuiltinStyle.Normal, Document);
                Document.Styles.Add(pStyle);
            }
        }
        /// <summary>
        /// Closes the item.
        /// </summary>
        internal override void Close()
        {
            if (m_pItemColl != null && m_pItemColl.Count > 0)
            {
                ParagraphItem item = null;
                for (int i = 0; i < m_pItemColl.Count; i++)
                {
                    item = m_pItemColl[i];
                    item.Close();
                    item = null;
                }

                m_pItemColl.Clear();
                m_pItemColl = null;
            }

            if (m_prFormat != null)
            {
                m_prFormat.Close();
                m_prFormat = null;
            }

            if (m_charFormat != null)
            {
                m_charFormat.Close();
                m_charFormat = null;
            }

            m_listFormat = null;
        }
        /// <summary>
        /// Applies the list paragraph style.
        /// </summary>
        internal void ApplyListParaStyle()
        {
            if (m_style != null && (m_style as WParagraphStyle).StyleId == DEF_LIST_STYLE_ID)
                return;

            WParagraphStyle style = (Document.Styles as StyleCollection).FindById(DEF_LIST_STYLE_ID) as WParagraphStyle;
            if (style == null)
            {
                style = new WParagraphStyle(Document);
                style.StyleId = DEF_LIST_STYLE_ID;
                style.Name = "List Paragraph";
                style.NextStyle = "List Paragraph";
                Document.Styles.Add(style);
            }

            m_style = style;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal FieldType GetLastFieldType()
        {
            IEntity item = Items.LastItem;
            while (item != null && !(item is WField))
            {
                item = item.PreviousSibling;
            }
            if (item != null && item is WField)
            {
                return (item as WField).FieldType;
            }
            return FieldType.FieldUnknown;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal WField GetLastField()
        {
            IEntity item = Items.LastItem;
            while (item != null && !(item is WField))
            {
                item = item.PreviousSibling;
            }
            if (item != null && item is WField)
            {
                return item as WField;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        private void CreateEmptyParagraph()
        {
            m_pEmptyItemColl = new ParagraphItemCollection(this);
            WTextRange textRange = (WTextRange)Document.CreateParagraphItem(
              ParagraphItemType.TextRange);
            textRange.Text = " ";
            textRange.CharacterFormat.ApplyBase(m_charFormat);
            (m_pEmptyItemColl as ParagraphItemCollection).UnsafeAdd(textRange);
        }
        /// <summary>
        /// Determines whether the paragraph is section end mark.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the paragraph is section end mark; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSectionEndMark()
        {
            if (this != null
                && this.NextSibling == null
                && this.OwnerTextBody != null
                && !(this.OwnerTextBody is HeaderFooter))
            {
                WSection sec = this.OwnerTextBody.Owner as WSection;
                if (sec != null && sec.NextSibling != null)
                {
                    string paraText = ModifyText(this.Text);
                    if (sec != null && !paraText.Contains('\r'.ToString()) && this.ChildEntities.Count == 0)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Modifies the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private string ModifyText(string text)
        {
            char carriageReturn = (char)13;
            char newLine = (char)10;
            text = text.Replace(Environment.NewLine, carriageReturn.ToString());
            text = text.Replace(newLine, carriageReturn);
            text = text.Replace('\a'.ToString(), string.Empty);
            text = text.Replace('\b'.ToString(), string.Empty);
            return text;
        }
        #endregion

        #region Implementation / track changes
        /// <summary>
        /// Accepts or rejects changes tracked from the moment of last change acceptance.
        /// </summary>
        /// <param name="acceptChanges">if it accepts changes, set to <c>true</c>.</param>
        internal override void MakeChanges(bool acceptChanges)
        {
            // Make changes in list format.
            if (acceptChanges && m_listFormat != null)
            {
                if (m_listFormat.NewStyleName != string.Empty)
                    m_listFormat.ApplyStyle(m_listFormat.NewStyleName);
                if (m_listFormat.NewListLevelNumber != -1)
                    m_listFormat.ListLevelNumber = m_listFormat.NewListLevelNumber;
                if (m_listFormat.NewLfoStyleName != null)
                    m_listFormat.LFOStyleName = m_listFormat.NewLfoStyleName;
                m_listFormat.OwnerParagraph.ParagraphFormat.ParaProps.Sprms.RemoveValue(WordSprmOptions.sprmPFNumRMIns);
                m_listFormat.OwnerParagraph.ParagraphFormat.ParaProps.Sprms.RemoveValue(WordSprmOptions.sprmPNumRM);
            }

            ParagraphItem item = null;
            BreakCharacterFormat.AcceptChanges();
            for (int i = 0; i < m_pItemColl.Count; i++)
            {
                item = m_pItemColl[i];

                if (item.IsDeleteRevision && acceptChanges || item.IsInsertRevision && !acceptChanges)
                {
                    m_pItemColl.RemoveAt(i);
                    i -= 1;
                }
                else
                {
                    if (item.IsChangedCFormat && !acceptChanges)
                    {
                        item.RemoveChanges();
                    }
                    if (item is Break)
                    {
                        (item as Break).TextRange.AcceptChanges();
                    }
                    item.AcceptChanges();

                    if (item is WTextBox)
                    {
                        (item as WTextBox).TextBoxBody.MakeChanges(acceptChanges);
                    }
                    else if (item is WFootnote)
                    {
                        (item as WFootnote).TextBody.MakeChanges(acceptChanges);
                    }
                }
            }
        }
        /// <summary>
        /// Removes the character format changes.
        /// </summary>
        internal override void RemoveCFormatChanges()
        {
            if (m_charFormat != null)
            {
                m_charFormat.RemoveChanges();
            }
        }
        /// <summary>
        /// Removes the paragraph/table format changes.
        /// </summary>
        internal override void RemovePFormatChanges()
        {
            if (m_prFormat != null)
            {
                m_prFormat.RemoveChanges();
            }
        }
        /// <summary>
        /// Accepts the changes for character format.
        /// </summary>
        internal override void AcceptCChanges()
        {
            if (m_charFormat != null)
            {
                m_charFormat.AcceptChanges();
            }
        }
        /// <summary>
        /// Accepts changes in paragraph/table format.
        /// </summary>
        internal override void AcceptPChanges()
        {
            if (m_prFormat != null)
            {
                m_prFormat.AcceptChanges();
                if ((m_prFormat.m_sprms == null || m_prFormat.m_sprms.Length == 0) || (!m_prFormat.m_sprms.Contain(WordSprmOptions.sprmPIlfo) && !m_prFormat.m_sprms.Contain(WordSprmOptions.sprmPIlvl)))
                    m_listFormat.IsEmptyList = false;
            }
        }
        /// <summary>
        /// Defines whether paragraph format is changed.
        /// </summary>
        /// <returns></returns>
        internal override bool CheckChangedPFormat()
        {
            if (m_prFormat != null)
            {
                return m_prFormat.IsChangedFormat;
            }
            return false;
        }

        /// <summary>
        /// Checks a value indicating whether this item was inserted to the document,
        /// when "Track Changes" is or was set to "true".
        /// </summary>
        /// <returns></returns>
        /// <value>
        /// 	if this instance was inserted, set to <c>true</c>.
        /// </value>
        internal override bool CheckInsertRev()
        {
            if (m_charFormat != null)
            {
                return m_charFormat.IsInsertRevision;
            }
            return false;
        }
        /// <summary>
        /// Checks a value indicating whether this item was deleted from the document,
        /// when "Track Changes" is or was set to "true".
        /// </summary>
        /// <returns></returns>
        /// <value>
        /// 	if this instance is delete revision, set to <c>true</c>.
        /// </value>
        internal override bool CheckDeleteRev()
        {
            if (m_charFormat != null)
            {
                return m_charFormat.IsDeleteRevision;
            }

            return false;
        }

        /// <summary>
        /// Defines whether format was changed.
        /// </summary>
        /// <returns></returns>
        internal override bool CheckChangedCFormat()
        {
            if (m_charFormat != null)
            {
                return m_charFormat.IsChangedFormat;
            }

            return false;
        }
        /// <summary>
        /// Checks whether full paragraph can be removed. 
        /// </summary>
        /// <returns></returns>
        internal bool CheckOnRemove()
        {
            foreach (ParagraphItem item in m_pItemColl)
            {
                if (!item.IsDeleteRevision)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Determines whether item has tracked changes.
        /// </summary>
        /// <returns>
        /// 	if has tracked changes, set to <c>true</c>.
        /// </returns>
        internal override bool HasTrackedChanges()
        {
            if (IsInsertRevision || IsDeleteRevision ||
              IsChangedCFormat || IsChangedPFormat)
            {
                return true;
            }

            foreach (ParagraphItem item in m_pItemColl)
            {
                if (item.HasTrackedChanges())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Sets the delete rev.
        /// </summary>
        /// <param name="check">if specifies delete revision, set to <c>true</c>.</param>
        internal override void SetDeleteRev(bool check)
        {
            if (m_charFormat != null)
            {
                m_charFormat.IsDeleteRevision = check;
            }
        }
        /// <summary>
        /// Sets the insert rev.
        /// </summary>
        /// <param name="check">if it specifies insert revision, set to <c>true</c>.</param>
        internal override void SetInsertRev(bool check)
        {
            if (m_charFormat != null)
            {
                m_charFormat.IsInsertRevision = check;
            }
        }
        /// <summary>
        /// Sets the changed C format.
        /// </summary>
        /// <param name="check">if it specifies formatting, set to <c>true</c>.</param>
        internal override void SetChangedCFormat(bool check)
        {
            if (m_charFormat != null)
            {
                m_charFormat.IsChangedFormat = check;
            }
        }
        /// <summary>
        /// Sets the changed P format.
        /// </summary>
        /// <param name="check">if it specifies changed format, set to <c>true</c>.</param>
        internal override void SetChangedPFormat(bool check)
        {
            if (m_prFormat != null)
            {
                m_prFormat.IsChangedFormat = check;
            }
        }
        #endregion

        #region Implementation /  Next textbody item
        /// <summary>
        /// Gets Next the text body item in the document.
        /// </summary>
        /// <returns></returns>
        internal override TextBodyItem GetNextTextBodyItem()
        {
            if (this.NextSibling != null)
                return this.NextSibling as TextBodyItem;

            if (this.Owner is WTableCell)
            {
                return (this.Owner as WTableCell).GetNextTextBodyItem();
            }
            else if (this.Owner is WTextBody)
            {
                if (this.OwnerTextBody.Owner is WTextBox)
                    return (this.OwnerTextBody.Owner as WTextBox).GetNextTextBodyItem();
                else if (this.OwnerTextBody.Owner is WSection)
                    return GetNextInSection(this.OwnerTextBody.Owner as WSection);
            }

            return null;
        }
        #endregion

        #region Implementation / xml
        //#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteValue(XDLSConstants.TypeTag, XDLSConstants.ItemTypeParagraphValue);
        }
        /// <summary>
        /// Registers paragraph elements for xml serialization
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddRefElement(XDLSConstants.StyleItemTag, GetStyle());
            XDLSHolder.AddElement(XDLSConstants.ParagraphFormatTag, m_prFormat);
            XDLSHolder.AddElement(XDLSConstants.CharacterFormatTag, m_charFormat);
            XDLSHolder.AddElement(XDLSConstants.ListFormatTag, ListFormat);
            XDLSHolder.AddElement(XDLSConstants.ItemsTag, m_pItemColl);
        }
        /// <summary>
        /// Restores object references after deserialization
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void RestoreReference(string name, int index)
        {
            if (name == XDLSConstants.StyleItemTag && index > -1)
            {
                m_style = Document.Styles[index] as IWParagraphStyle;
                ApplyBaseStyleFormats();
            }
        }
        //#endif
        #endregion

        #region Implementation / layout
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="ltWidget"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        void IWidget.Draw(DrawingContext dc, LayoutedWidget ltWidget)
        {
            base.DrawImpl(dc, ltWidget);

            bool isLineConatiner = ltWidget.ChildWidgets.Count > 0 && ltWidget.ChildWidgets[0].Widget == this;

            if (ltWidget.Widget is SplitWidgetContainer && (ltWidget.Widget as SplitWidgetContainer).RealWidgetContainer is WParagraph)
            {
                WParagraph para = (ltWidget.Widget as SplitWidgetContainer).RealWidgetContainer as WParagraph;
                int count = para.ChildEntities.Count;
                for (int i = 0; i < ltWidget.ChildWidgets.Count; i++)
                {
                    LayoutedWidget Widget = ltWidget.ChildWidgets[i];
                    dc.UpdateTabPosition(Widget);
                }
                int index = count - 1 - (ltWidget.Widget as SplitWidgetContainer).Count;
                if (count > (ltWidget.Widget as SplitWidgetContainer).Count && index >= 0 && index < count
                    && para.ChildEntities[index] is Break)
                {
                    Entity ent = (Entity)para.ChildEntities[index];
                    Break br = ent as Break;
                    if (br.BreakType == BreakType.PageBreak && !m_bSplitWidgetContainerDrawn)
                    {
                        isLineConatiner = true;
                        m_bSplitWidgetContainerDrawn = true;
                    }
                }
            }
            if (isLineConatiner)
            {
                dc.DrawParagraph(this, ltWidget);
            }
        }
        /// <summary>
        /// Initializing LayoutInfo to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutParagraphInfoImpl(this);
            if ((IsHiddenParagraph()
                && this.BreakCharacterFormat.HasValue(WCharacterFormat.HiddenKey)
                && this.BreakCharacterFormat.Hidden)
                || (this.Items.Count == 0 && IsSkipCellMark()))
                m_layoutInfo.IsSkip = true;
            if ((this.IsInCell && (this.OwnerTextBody as WTableCell).CellFormat.TextDirection != TextDirection.Horizontal)
                || (this.OwnerTextBody.Owner is Shape && (this.OwnerTextBody.Owner as Shape).TextFrame.TextDirection != TextDirection.Horizontal))
                m_layoutInfo.IsVerticalText = true;
            //Skip Empty Paragraph
            if (this.Text == string.Empty && this.RemoveEmpty)
                m_layoutInfo.IsSkip = true;
            //Splits the widget by line break
            SplitByLineBreak(this.Items);
            //if (this.ParagraphFormat.HasValue(WParagraphFormat.FramePosKey) ||
            //    ParagraphFormat.HasValue(WParagraphFormat.FrameXKey) ||
            //    ParagraphFormat.HasValue(WParagraphFormat.FrameYKey))
            //    m_layoutInfo.IsSkip = true;
        }
        /// <summary>
        /// Determine whether the current paragraph is need to be hidden
        /// </summary>
        /// <returns></returns>
        private bool IsHiddenParagraph()
        {
            bool isHidden = false;
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i].ParaItemCharFormat.Hidden)
                    isHidden = true;
                else
                {
                    isHidden = false;
                    break;
                }
            }
            if (this.Items.Count == 0)
                isHidden = true;
            return isHidden;
        }
        /// <summary>
        /// Splits the widget by line break.
        /// </summary>
        private void SplitByLineBreak(ParagraphItemCollection paraItems)
        {
            if (this.Text.Contains(ControlChar.LineBreak))
            {
                for (int index = 0; index < paraItems.Count; index++)
                {
                    if ((paraItems[index] is WTextRange) && (paraItems[index] as WTextRange).Text.Contains(ControlChar.LineBreak))
                    {
                        WTextRange txtRange = paraItems[index] as WTextRange;
                        string[] splittedString = txtRange.Text.Split(ControlChar.LineBreakChar);
                        txtRange.Text = splittedString[0];
                        int txtIndex = paraItems.IndexOf(txtRange);
                        for (int i = 1; i < splittedString.Length; i++)
                        {
                            txtIndex++;
                            Break lineBreak = new Break(Document, BreakType.LineBreak);
                            lineBreak.TextRange.Text = "\v";
                            lineBreak.TextRange.CharacterFormat.ImportContainer(txtRange.CharacterFormat);
                            lineBreak.TextRange.CharacterFormat.CopyProperties(txtRange.CharacterFormat);
                            WTextRange newTextRange = new WTextRange(Document);
                            newTextRange.Text = splittedString[1];
                            newTextRange.CharacterFormat.ImportContainer(txtRange.CharacterFormat);
                            newTextRange.CharacterFormat.CopyProperties(txtRange.CharacterFormat);
                            paraItems.Insert(txtIndex, lineBreak);
                            paraItems.Insert(txtIndex + 1, newTextRange);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Determine the whether the paragraph is the firstparagraph of the document
        /// </summary>
        internal bool IsFirstParagraphOfDocument()
        {
            if (!this.IsInCell && (this.Owner is WTextBody) && (this.Owner as WTextBody) != null)
            {
                bool isFirstParagraph = (this.Owner as WTextBody).Items[0] == this;
                int secIndex = (this.Owner.Owner is WSection) ? this.Document.Sections.IndexOf(this.Owner.Owner) : -1;
                return (isFirstParagraph && secIndex == 0);
            }
            else
                return false;
        }
        /// <summary>
        /// Determines whether to skip cell mark layouting.
        /// For the cell mark present after a table in nested tables.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if is skip cell mark; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSkipCellMark()
        {
            return (IsInCell && this.Items.Count == 0 &&
                (this.Owner as WTableCell).LastParagraph.Equals(this)
                && ((this as Entity).PreviousSibling != null)
                && ((this as Entity).PreviousSibling is WTable));
        }
        /// <summary>
        /// The class specifies the Layout paragraph information.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal class LayoutParagraphInfoImpl : ParagraphLayoutInfo
        {
            #region Fields
            /// <summary>
            /// 
            /// </summary>
            private WParagraph m_paragraph;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the document.
            /// </summary>
            /// <value>The document.</value>
            protected IWordDocument Document
            {
                get
                {
                    return m_paragraph.Document;
                }
            }
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="LayoutParagraphInfoImpl"/> class.
            /// </summary>
            /// <param name="paragraph">The paragraph.</param>
            public LayoutParagraphInfoImpl(WParagraph paragraph)
                :
                base(ChildrenLayoutDirection.Horizontal)
            {
                IsLineContainer = true;
                m_paragraph = paragraph;
                Size = GetEmptyTextRangeSize();
                UpdateLayoutInfoIsClipped();
                InitFormat();
                InitPageBreaks();
                InitListFormat();
                InitBorders();
            }
            #endregion

            #region Implementation
            /// <summary>
            /// Get the Empty size of the empty TextRange
            /// </summary>
            /// <returns></returns>
            private SizeF GetEmptyTextRangeSize()
            {
                DrawingContext dc = DocumentLayouter.DrawingContext;
                return dc.MeasureString(" ", m_paragraph.BreakCharacterFormat.Font, null, m_paragraph.BreakCharacterFormat,false);
            }
            /// <summary>
            /// Updates IsClipped Property for the Paragraph.
            /// </summary>
            private void UpdateLayoutInfoIsClipped()
            {
                if (m_paragraph.IsInCell && ((m_paragraph.Owner.Owner as WTableRow).HeightType == TableRowHeightType.Exactly))
                {
                    if ((m_paragraph.Owner.Owner as WTableRow).OwnerTable.m_isTextBox)
                        IsClipped = true;
                    else
                    {
                        float height = (m_paragraph.Owner.Owner as WTableRow).Height;
                        if (height < 0)
                        {
                            height = -(height);
                        }
                        if (height > 1)
                            IsClipped = true;
                    }
                }
                else if (m_paragraph != null && m_paragraph.ParagraphFormat.IsFrame)
                {
                    // Handled for the preserving the text within the frame with exact height.
                    ushort heightValue = (ushort)m_paragraph.ParagraphFormat.FrameHeight;
                    bool isAtleastHeight = (heightValue & (1 << 15)) != 0;
                    if (!isAtleastHeight)
                        IsClipped = true;
                }
                else if ((m_paragraph.IsInCell
                        && (m_paragraph.OwnerTextBody as WTableCell).CellFormat.TextDirection != TextDirection.Horizontal)
                        || (m_paragraph.OwnerTextBody.Owner is Shape))
                {
                    IsClipped = true;
                }
            }
            /// <summary>
            /// Determines the list format.
            /// </summary>
            private void InitListFormat()
            {
                if (m_paragraph.ListFormat.IsEmptyList
                    || m_paragraph.SectionEndMark)
                    return;

                WListFormat listFormat = null;
                WParagraphStyle pStyle = m_paragraph.ParaStyle as WParagraphStyle;
                if (m_paragraph.ListFormat.ListType != ListType.NoList)
                    listFormat = m_paragraph.ListFormat;
                // Get the list format from the paragraph style
                else
                {
                    while (pStyle != null)
                    {
                        if (pStyle.ListFormat.ListType != ListType.NoList || pStyle.ListFormat.IsEmptyList)
                        {
                            listFormat = pStyle.ListFormat;
                            break;
                        }
                        else
                            pStyle = pStyle.BaseStyle;
                    }
                }
                pStyle = m_paragraph.ParaStyle as WParagraphStyle;

                if (listFormat != null
                    && listFormat.CurrentListStyle != null)
                {
                    ListStyle listStyle = listFormat.CurrentListStyle;
                    LevelNumber = 0;
                    if (m_paragraph.ListFormat.HasKey(WListFormat.ListLevelNumberKey))
                        LevelNumber = m_paragraph.ListFormat.ListLevelNumber;
                    //Get the level number from the paragraph style list format
                    else
                    {
                        while (pStyle != null)
                        {
                            if (pStyle.ListFormat.HasKey(WListFormat.ListLevelNumberKey))
                            {
                                LevelNumber = pStyle.ListFormat.ListLevelNumber;
                                break;
                            }
                            else
                                pStyle = pStyle.BaseStyle;
                        }
                    }
                    pStyle = m_paragraph.ParaStyle as WParagraphStyle;

                    // Updates current list level.
                    WListLevel level = listStyle.GetNearLevel(LevelNumber);

                    ListOverrideStyle listOverrideStyle = null;
                    if (listFormat.LFOStyleName != null
                        && listFormat.LFOStyleName.Length > 0)
                        listOverrideStyle = (Document as WordDocument).ListOverrides.FindByName(listFormat.LFOStyleName);
                    if (listOverrideStyle != null
                        && listOverrideStyle.OverrideLevels.HasOverrideLevel(LevelNumber)
                        && listOverrideStyle.OverrideLevels[LevelNumber].OverrideFormatting)
                        level = listOverrideStyle.OverrideLevels[LevelNumber].OverrideListLevel;

                    if (listStyle.ListType == ListType.Numbered || listStyle.ListType == ListType.Bulleted)
                    {
                        // Updates Left indent
                        if (level.ParagraphFormat.HasValue(WParagraphFormat.LeftIndentKey))
                            Margins.Left = level.ParagraphFormat.LeftIndent;
                        if (m_paragraph.ListFormat.ListType == ListType.NoList
                            && pStyle.ParagraphFormat.HasValue(WParagraphFormat.LeftIndentKey))
                            Margins.Left = pStyle.ParagraphFormat.LeftIndent;
                        if (m_paragraph.ParagraphFormat.HasValue(WParagraphFormat.LeftIndentKey))
                            Margins.Left = m_paragraph.ParagraphFormat.LeftIndent;

                        // Updates FirstLine indent
                        if (level.ParagraphFormat.HasValue(WParagraphFormat.FirstLineIndentKey))
                            FirstLineIndent = level.ParagraphFormat.FirstLineIndent;
                        if (m_paragraph.ListFormat.ListType == ListType.NoList
                            && pStyle.ParagraphFormat.HasValue(WParagraphFormat.FirstLineIndentKey))
                            FirstLineIndent = pStyle.ParagraphFormat.FirstLineIndent;
                        if (m_paragraph.ParagraphFormat.HasValue(WParagraphFormat.FirstLineIndentKey))
                            FirstLineIndent = m_paragraph.ParagraphFormat.FirstLineIndent;

                        if (FirstLineIndent < 0
                            && Margins.Left == 0
                            && !m_paragraph.ParagraphFormat.HasValue(WParagraphFormat.LeftIndentKey))
                            Margins.Left = Math.Abs(FirstLineIndent);
                    }
                    // Updates list value.
                    ListValue = (Document as WordDocument).UpdateListValue(m_paragraph, listFormat, level);
                    //Update Current List Type
                    CurrentListType = listFormat.ListType;
                    if (level.PatternType == ListPatternType.Bullet)
                        CurrentListType = ListType.Bulleted;
                    else if (listFormat.ListType == ListType.Bulleted && level.PatternType != ListPatternType.Bullet)
                    {
                        CurrentListType = ListType.Numbered;
                    }
                    // Updates character format for the list.
                    CharacterFormat = new WCharacterFormat(Document);
                    CharacterFormat.ImportContainer(m_paragraph.BreakCharacterFormat);
                    CharacterFormat.CopyProperties(m_paragraph.BreakCharacterFormat);
                    CharacterFormat.ApplyBase(m_paragraph.BreakCharacterFormat.BaseFormat);
                    if (CharacterFormat.PropertiesHash.ContainsKey(WCharacterFormat.UnderlineKey))
                    {
                        CharacterFormat.UnderlineStyle = UnderlineStyle.None;
                        CharacterFormat.PropertiesHash.Remove(WCharacterFormat.UnderlineKey);
                    }

                    //Copy character fomatting from list format
                    CopyCharacterFormatting(level.CharacterFormat, CharacterFormat);
                    // Updates list tab. 
                    if (level.FollowCharacter == FollowCharacterType.Tab)
                        UpdateListTab(level);
                    else
                        UpdateListWidth(level);
                    // Updates list number alignment.
                    ListAlignment = level.NumberAlignment;
                    //Update List Paragraph After/Before spacing
                    if (!m_paragraph.Document.DOP.Dop2000.Copts.DontUseHTMLParagraphAutoSpacing)
                    {
                        UpdateListParagraphSpacing(listFormat);
                    }
                }
            }
            /// <summary>
            /// Update List Paragraph After/Before spacing
            /// </summary>
            /// <param name="currentListFormat"></param>
            private void UpdateListParagraphSpacing(WListFormat currentListFormat)
            {
                if (m_paragraph.NextSibling != null && (m_paragraph.NextSibling is WParagraph) && m_paragraph.ParagraphFormat.SpaceAfterAuto)
                {
                    WParagraph nextParagraph = (m_paragraph.NextSibling as WParagraph);
                    WParagraphStyle pStyle = nextParagraph.ParaStyle as WParagraphStyle;
                    WListFormat nextListFormat = nextParagraph.ListFormat.ListType != ListType.NoList ? nextParagraph.ListFormat
                                                 : pStyle.ListFormat.ListType != ListType.NoList ? pStyle.ListFormat : null;
                    if (!nextParagraph.ListFormat.IsEmptyList
                        && nextListFormat != null
                        && nextListFormat.CurrentListStyle != null
                        && nextListFormat.CurrentListStyle == currentListFormat.CurrentListStyle)
                        Margins.Bottom = 0;
                }
                if (m_paragraph.PreviousSibling != null && (m_paragraph.PreviousSibling is WParagraph) && m_paragraph.ParagraphFormat.SpaceBeforeAuto)
                {
                    WParagraph prevParagraph = (m_paragraph.PreviousSibling as WParagraph);
                    WParagraphStyle pStyle = prevParagraph.ParaStyle as WParagraphStyle;
                    WListFormat prevListFormat = prevParagraph.ListFormat.ListType != ListType.NoList ? prevParagraph.ListFormat
                                                : pStyle.ListFormat.ListType != ListType.NoList ? pStyle.ListFormat : null;
                    if (!prevParagraph.ListFormat.IsEmptyList
                        && prevListFormat != null
                        && prevListFormat.CurrentListStyle != null
                        && prevListFormat.CurrentListStyle == currentListFormat.CurrentListStyle)
                        Margins.Top = 0;
                }
            }
            /// <summary>
            /// Updates the width of the list value, if the follow character is space or nothing.
            /// </summary>
            /// <param name="level">The level.</param>
            private void UpdateListWidth(WListLevel level)
            {
                DrawingContext dc = DocumentLayouter.DrawingContext;
                SizeF size = dc.MeasureString(ListValue, CharacterFormat.Font, null, CharacterFormat,false);
                float width = 0f;
                if (level.NumberAlignment == ListNumberAlignment.Left)
                    width = size.Width;
                else if (level.NumberAlignment == ListNumberAlignment.Center)
                    width = size.Width / 2;
                if (level.FollowCharacter == FollowCharacterType.Space)
                    ListTab = width + dc.MeasureString(" ", CharacterFormat.Font, null).Width;
                else
                    ListTab = width;
            }
            /// <summary>
            /// Updates the list tab.
            /// </summary>
            /// <param name="level"></param>
            /// <remarks></remarks>
            private void UpdateListTab(WListLevel level)
            {
                ListTabs tabs = new ListTabs(m_paragraph);
                WParagraphFormat baseFormat = m_paragraph.ParagraphFormat;
                while (baseFormat != null)//add all tab stop collection until base format is null.
                {
                    tabs.UpdateTabs(baseFormat);
                    baseFormat = baseFormat.BaseFormat as WParagraphFormat;
                }
                tabs.UpdateTabs(level.ParagraphFormat);

                DrawingContext dc = DocumentLayouter.DrawingContext;
                SizeF size = dc.MeasureString(ListValue, CharacterFormat.Font, null, CharacterFormat,true);
                if (level.NumberAlignment == ListNumberAlignment.Left)
                    UpdateTabWidth(level, tabs, size.Width);
                else if (level.NumberAlignment == ListNumberAlignment.Center)
                    UpdateTabWidth(level, tabs, size.Width / 2);
                else
                    UpdateTabWidth(level, tabs, 0);
                ListTabStop = tabs.m_currTab;
            }
            /// <summary>
            /// Updates the width of the tab, based on the list text.
            /// </summary>
            /// <param name="tabs">The tabs.</param>
            /// <param name="width">The width.</param>
            private void UpdateTabWidth(WListLevel level, ListTabs tabs, float width)
            {
                if (ListTab <= width
                    || !(Document as WordDocument).UseHangingIndentAsListTab)
                {
                    float position = (float)(width + Margins.Left + FirstLineIndent);
                    float tabWidth = (float)tabs.GetNextTabPosition(position);
                    if (width <= Math.Abs(FirstLineIndent)
                        && (Document as WordDocument).UseHangingIndentAsListTab)
                    {
                        if (tabs.m_currTab.Position != 0)
                            ListTab = Math.Min(width + tabWidth, Math.Abs(FirstLineIndent));
                        else
                            ListTab = Math.Abs(FirstLineIndent);
                    }
                    else
                    {
                        if (tabs.m_currTab.Position == 0
                            && width <= Math.Abs(FirstLineIndent))
                            ListTab = Math.Min(width + tabWidth, Math.Abs(FirstLineIndent));
                        else if (level.Word6Legacy && tabs.m_list.Count == 0)
                            ListTab = width + (level.LegacySpace / DLSConstants.TwipsInOnePoint);
                        else
                            ListTab = width + tabWidth;
                    }
                    //Update list tab width
                    if (width == 0 && ListTab == 0 && FirstLineIndent == 0)
                    {
                        ListTab = tabWidth;
                    }
                }
            }
            /// <summary>
            /// Copys Character formatting
            /// </summary>
            /// <param name="destFormat"></param>
            /// <param name="sourceFormat"></param>
            private void CopyCharacterFormatting(WCharacterFormat sourceFormat, WCharacterFormat destFormat)
            {
                if (sourceFormat.HasValue(WCharacterFormat.FontSizeKey))
                    destFormat.FontSize = sourceFormat.FontSize;
                if (sourceFormat.HasValue(WCharacterFormat.TextColorKey))
                    destFormat.TextColor = sourceFormat.TextColor;
                if (sourceFormat.HasValue(WCharacterFormat.FontNameKey))
                    destFormat.FontName = sourceFormat.FontName;
                if (sourceFormat.HasValue(WCharacterFormat.BoldKey))
                    destFormat.Bold = sourceFormat.Bold;
                if (sourceFormat.HasValue(WCharacterFormat.ItalicKey))
                    destFormat.Italic = sourceFormat.Italic;
                if (sourceFormat.HasValue(WCharacterFormat.UnderlineKey))
                    destFormat.UnderlineStyle = sourceFormat.UnderlineStyle;
                if (sourceFormat.HasValue(WCharacterFormat.HighlightColorKey))
                    destFormat.HighlightColor = sourceFormat.HighlightColor;
                if (sourceFormat.HasValue(WCharacterFormat.ShadowKey))
                    destFormat.Shadow = sourceFormat.Shadow;
                if (sourceFormat.HasValue(WCharacterFormat.SpacingKey))
                    destFormat.CharacterSpacing = sourceFormat.CharacterSpacing;
                if (sourceFormat.HasValue(WCharacterFormat.DoubleStrikeKey))
                    destFormat.DoubleStrike = sourceFormat.DoubleStrike;
                if (sourceFormat.HasValue(WCharacterFormat.EmbossKey))
                    destFormat.Emboss = sourceFormat.Emboss;
                if (sourceFormat.HasValue(WCharacterFormat.EngraveKey))
                    destFormat.Engrave = sourceFormat.Engrave;
                if (sourceFormat.HasValue(WCharacterFormat.SubSuperScriptKey))
                    destFormat.SubSuperScript = sourceFormat.SubSuperScript;
                destFormat.TextBackgroundColor = sourceFormat.TextBackgroundColor;
                if (sourceFormat.HasValue(WCharacterFormat.AllCapsKey))
                    destFormat.AllCaps = sourceFormat.AllCaps;
                if (sourceFormat.Bidi)
                {
                    destFormat.Bidi = true;
                    destFormat.FontNameBidi = sourceFormat.FontNameBidi;
                    destFormat.FontSizeBidi = sourceFormat.FontSizeBidi;
                }
                if (sourceFormat.HasValue(WCharacterFormat.BoldBidiKey))
                    destFormat.BoldBidi = sourceFormat.BoldBidi;
                if (sourceFormat.HasValue(WCharacterFormat.FieldVanishKey))
                    destFormat.FieldVanish = sourceFormat.FieldVanish;
                if (sourceFormat.HasValue(WCharacterFormat.HiddenKey))
                    destFormat.Hidden = sourceFormat.Hidden;
                if (sourceFormat.HasValue(WCharacterFormat.SmallCapsKey))
                    destFormat.SmallCaps = sourceFormat.SmallCaps;
            }
            /// <summary>
            /// Determines the borders.
            /// </summary>
            /// <summary>
            /// Determines the borders.
            /// </summary>
            private void InitBorders()
            {
                Borders borders = m_paragraph.ParagraphFormat.Borders;
                if (!borders.NoBorder && !m_paragraph.SectionEndMark)
                {
                    if (borders.Left.BorderType != BorderStyle.None)
                        Paddings.Left -= (borders.Left.Space + borders.Left.LineWidth / 2);
                    if (borders.Right.BorderType != BorderStyle.None)
                        Paddings.Right -= (borders.Right.Space + borders.Right.LineWidth / 2);
                    if (borders.Top.BorderType != BorderStyle.None)
                        Paddings.Top += (borders.Top.Space + borders.Top.LineWidth / 2);
                    //Update Top Padding based on the previous paragraph border
                    else if (m_paragraph.PreviousSibling != null && (m_paragraph.PreviousSibling is WParagraph)
                             && !(m_paragraph.PreviousSibling as WParagraph).ParagraphFormat.Borders.NoBorder
                             && !(m_paragraph.PreviousSibling as WParagraph).SectionEndMark)
                    {
                        if ((m_paragraph.PreviousSibling as WParagraph).ParagraphFormat.Borders.Bottom.BorderType != BorderStyle.None)
                            Paddings.Top += (m_paragraph.PreviousSibling as WParagraph).ParagraphFormat.Borders.Bottom.LineWidth / 2;
                    }
                    if (borders.Bottom.BorderType != BorderStyle.None)
                        Paddings.Bottom += (borders.Bottom.Space + borders.Bottom.LineWidth / 2);
                    if (m_paragraph.NextSibling != null && m_paragraph.NextSibling is WParagraph
                        && !(m_paragraph.NextSibling as WParagraph).ParagraphFormat.Borders.NoBorder)
                    {
                        WParagraphFormat nextParaFormat = (m_paragraph.NextSibling as WParagraph).ParagraphFormat;
                        if (nextParaFormat.Borders.Top.BorderType != BorderStyle.None)
                            Paddings.Bottom = 0;
                        if (nextParaFormat.IsFrame && !m_paragraph.ParagraphFormat.IsNextParagraphInSameFrame())
                        {
                            Paddings.Bottom = borders.Bottom.Space + borders.Bottom.LineWidth / 2;
                        }
                    }

                    if (m_paragraph.PreviousSibling != null && m_paragraph.PreviousSibling is WParagraph
                        && !(m_paragraph.PreviousSibling as WParagraph).ParagraphFormat.Borders.NoBorder)
                    {
                        WParagraphFormat prevParaFormat = (m_paragraph.PreviousSibling as WParagraph).ParagraphFormat;
                        if (prevParaFormat.Borders.Bottom.BorderType != BorderStyle.None)
                            Paddings.Top = 0;
                        if (prevParaFormat.IsFrame && !m_paragraph.ParagraphFormat.IsPreviousParagraphInSameFrame())
                        {
                            Paddings.Top = borders.Top.Space + borders.Top.LineWidth / 2;
                        }
                    }
                }
                //Update Top Padding based on the previous paragraph border
                else if (m_paragraph.PreviousSibling != null && (m_paragraph.PreviousSibling is WParagraph)
                       && !(m_paragraph.PreviousSibling as WParagraph).ParagraphFormat.Borders.NoBorder
                       && !(m_paragraph.PreviousSibling as WParagraph).SectionEndMark)
                {
                    if ((m_paragraph.PreviousSibling as WParagraph).ParagraphFormat.Borders.Bottom.BorderType != BorderStyle.None)
                        Paddings.Top += (m_paragraph.PreviousSibling as WParagraph).ParagraphFormat.Borders.Bottom.LineWidth / 2;
                }
            }
            /// <summary>
            /// Determines the page breaks.
            /// </summary>
            private void InitPageBreaks()
            {
                IEntity parent = m_paragraph.Owner as Entity;
                while (parent != null && parent.EntityType != EntityType.TextBody)
                {
                    parent = parent.Owner;
                }
                WTextBody parOwner = parent as WTextBody;
                bool sectionPageBreak = false;
                bool pageBreakBefore = false;
                IWParagraph par = null;
                if (parOwner != null)
                {
                    bool bLastParagraph = (parOwner.Paragraphs.IndexOf(m_paragraph) == parOwner.Paragraphs.Count - 1);
                    IWSection nextSection = null;
                    if (parOwner.Owner is WSection)
                    {
                        int secIndex = Document.Sections.IndexOf(parOwner.Owner);
                        nextSection = (secIndex + 1 < m_paragraph.Document.Sections.Count)
                          ? Document.Sections[secIndex + 1]
                          : null;
                    }

                    if (nextSection != null)
                    {
                        sectionPageBreak =
                          bLastParagraph &&
                          (nextSection.BreakCode == SectionBreakCode.NewPage
                          || nextSection.BreakCode == SectionBreakCode.Oddpage
                          || nextSection.BreakCode == SectionBreakCode.EvenPage
                          || nextSection.BreakCode == SectionBreakCode.NoBreak);
                    }

                    if (!bLastParagraph && !(m_paragraph.IsInCell))
                    {
                        if (m_paragraph.NextSibling is WTable
                            && (m_paragraph.NextSibling as WTable).Rows[0].Cells[0].Paragraphs.Count > 0)
                        {
                            par = (m_paragraph.NextSibling as WTable).Rows[0].Cells[0].Paragraphs[0] as WParagraph;
                        }
                        else
                        {
                            int pIndex = parOwner.Paragraphs.IndexOf(m_paragraph);
                            par = parOwner.Paragraphs[pIndex + 1] as IWParagraph;
                        }
                        if (par != null)
                        {
                            pageBreakBefore = par.ParagraphFormat.PageBreakBefore;
                        }
                    }
                }

                WParagraphFormat pFormat = m_paragraph.ParagraphFormat;
                IsPageBreak = pFormat.PageBreakAfter || pageBreakBefore ||
                  pFormat.ColumnBreakAfter || sectionPageBreak;
            }
            /// <summary>
            /// Determines the format.
            /// </summary>
            private void InitFormat()
            {
                WParagraphFormat parFormat = m_paragraph.ParagraphFormat;
                WParagraphStyle pStyle = m_paragraph.GetStyle() as WParagraphStyle;
                if (pStyle == null)
                    pStyle = m_paragraph.Document.Styles.FindByName("Normal") as WParagraphStyle;
                Margins.Left = parFormat.LeftIndent;
                Margins.Right = parFormat.RightIndent;

                Margins.Top = parFormat.BeforeSpacing;
                Margins.Bottom = parFormat.AfterSpacing;
                //Update Paragraph After/Before Spacing
                if (!m_paragraph.Document.DOP.Dop2000.Copts.DontUseHTMLParagraphAutoSpacing)
                {
                    UpdateParagraphSpacing();
                }
                if (m_paragraph.IsInCell
                     && !m_paragraph.Document.DOP.Dop2000.Copts.DontUseHTMLParagraphAutoSpacing
                    && !(m_paragraph.OwnerTextBody as WTableCell).OwnerRow.OwnerTable.m_isTextBox)
                {
                    if (m_paragraph.NextSibling == null && m_paragraph.ParagraphFormat.SpaceAfterAuto)
                        Margins.Bottom = 0.0;
                    if (m_paragraph.PreviousSibling == null && m_paragraph.ParagraphFormat.SpaceBeforeAuto)
                        Margins.Top = 0.0;
                }
                if (parFormat.ContextualSpacing)
                {
                    if ((m_paragraph as Entity).PreviousSibling != null
                        && (m_paragraph as Entity).PreviousSibling is WParagraph
                        && ((m_paragraph as Entity).PreviousSibling as WParagraph).StyleName == m_paragraph.StyleName)
                        Margins.Top = 0;
                    if ((m_paragraph as Entity).NextSibling != null
                        && (m_paragraph as Entity).NextSibling is WParagraph
                        && ((m_paragraph as Entity).NextSibling as WParagraph).StyleName == m_paragraph.StyleName)
                        Margins.Bottom = 0;
                }
                IsKeepTogether = parFormat.Keep;
                IsKeepWithNext = parFormat.KeepFollow;
                if (parFormat.HasKey(WParagraphFormat.FirstLineIndentKey) && parFormat.HasValue(WParagraphFormat.FirstLineIndentKey))
                    FirstLineIndent = parFormat.FirstLineIndent;
                else if (pStyle != null && pStyle.ParagraphFormat != null)
                    FirstLineIndent = parFormat.FirstLineIndent;
                else
                    FirstLineIndent = parFormat.FirstLineIndent;

                Justification =
                  (Layouting.HorizontalAlignment)parFormat.HorizontalAlignment;

                if (parFormat.Bidi)
                {
                    if (Justification == Syncfusion.Layouting.HorizontalAlignment.Left)
                        Justification = Syncfusion.Layouting.HorizontalAlignment.Right;
                    else if (Justification == Syncfusion.Layouting.HorizontalAlignment.Right)
                        Justification = Syncfusion.Layouting.HorizontalAlignment.Left;
                }
                if (m_paragraph.SectionEndMark)
                {
                    Margins.Top = 0;
                    Margins.Bottom = 0;
                }
            }
            /// <summary>
            /// Update Paragraph After/Before Spacing
            /// </summary>
            private void UpdateParagraphSpacing()
            {
                if (m_paragraph.ParagraphFormat.SpaceBeforeAuto)
                    Margins.Top = 14; //MSWord render default auto spacing value as 14.0
                if (m_paragraph.ParagraphFormat.SpaceAfterAuto)
                    Margins.Bottom = 14;
                if (m_paragraph.NextSibling != null && (m_paragraph.NextSibling is WParagraph)
                    && (((m_paragraph.NextSibling as WParagraph).ParagraphFormat.BeforeSpacing > Margins.Bottom && !(m_paragraph.NextSibling as WParagraph).ParagraphFormat.SpaceBeforeAuto)
                    || ((m_paragraph.NextSibling as WParagraph).ParagraphFormat.SpaceBeforeAuto && Margins.Bottom < 14)))
                {

                    if ((m_paragraph.NextSibling as WParagraph).ParagraphFormat.SpaceBeforeAuto)
                        Margins.Bottom = 14;
                    else
                        Margins.Bottom = (m_paragraph.NextSibling as WParagraph).ParagraphFormat.BeforeSpacing;
                }
                if (m_paragraph.PreviousSibling != null && (m_paragraph.PreviousSibling is WParagraph) && (m_paragraph.PreviousSibling as WParagraph).m_layoutInfo != null
                    && ((m_paragraph.PreviousSibling as WParagraph).m_layoutInfo.Margins.Bottom == Margins.Top
                    || (m_paragraph.PreviousSibling as WParagraph).m_layoutInfo.Margins.Bottom > Margins.Top))
                {
                    //Updating Top margin when previous paragraph is in frame with current paragraph
                    if (m_paragraph.ParagraphFormat.IsPreviousParagraphInSameFrame()
                       || (!(m_paragraph.PreviousSibling as WParagraph).ParagraphFormat.IsInFrame()
                       && !m_paragraph.ParagraphFormat.IsInFrame()))
                        Margins.Top = 0;
                }
                //Update top margin as zero for the first paragraph of the document which have before auto spacing
                if ((m_paragraph.IsFirstParagraphOfDocument() && m_paragraph.ParagraphFormat.SpaceBeforeAuto)
                    || (m_paragraph.PreviousSibling == null && m_paragraph.OwnerTextBody.Owner is SDTBlockContent))
                {
                    Margins.Top = 0;
                }
            }
            #endregion
        }
#endif
        #endregion

        #region Internal declarations
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Get the default tab width
        /// </summary>
        /// <param name="paragraphFormat">Paragraph</param>
        /// <returns>Default tab width value</returns>
        internal double GetDefaultTabWidth()
        {
            Entity ent = this.Owner as Entity;
            while (!(ent is WSection))
            {
                if (ent is WTable && (ent as WTable).m_isTextBox)
                    ent = (ent as WTable).m_textBoxFormat.OwnerBase as WTextBox;
                if (ent.Owner == null)
                    break;
                else
                    ent = ent.Owner as Entity;
            }
            if (ent is WSection)
            {
                return (ent as WSection).PageSetup.DefaultTabWidth;
            }
            return WPageSetup.DEF_AUTO_TAB_LENGHT;
        }
        /// <summary>
        /// 
        /// </summary>
        internal class ListTabs
          : TabsLayoutInfo
        {
            internal List<float> m_deleteTabPositions = new List<float>();
            /// <summary>
            /// Initializes a new instance of the <see cref="ListTabs"/> class.
            /// </summary>
            /// <param name="paragrath">The paragrath.</param>
            public ListTabs(WParagraph paragrath)
                : base(ChildrenLayoutDirection.Horizontal)
            {
                m_defaultTabWidth = paragrath.GetDefaultTabWidth();
            }
            /// <summary>
            /// Updates the tabs.
            /// </summary>
            /// <param name="paragraphFormat">The paragraph format.</param>
            internal void UpdateTabs(WParagraphFormat paragraphFormat)
            {
                for (int i = 0, count = paragraphFormat.Tabs.Count; i < count; i++)
                {
                    Tab tab = paragraphFormat.Tabs[i];

                    if (m_deleteTabPositions.Contains(tab.Position))
                        continue;

                    if (tab.DeletePosition == 0)
                    {
                        AddTab(
                      tab.Position,
                      (Layouting.TabJustification)tab.Justification,
                      (Layouting.TabLeader)tab.TabLeader);
                    }
                    else if (tab.DeletePosition != 0)
                        m_deleteTabPositions.Add(tab.DeletePosition / DLSConstants.TwipsInOnePoint);
                }
            }
        }
#endif
        #endregion
    }
}