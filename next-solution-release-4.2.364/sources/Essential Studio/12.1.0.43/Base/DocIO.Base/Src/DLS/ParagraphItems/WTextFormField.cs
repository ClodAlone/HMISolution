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

#region File using directives
using System;
using Syncfusion.DocIO.DLS.XML;
#if !SILVERLIGHT && !WP
using Syncfusion.Layouting;
#endif
using System.Windows;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
	/// <summary>
	/// Summary description for WTextFormField.
	/// </summary>
    public class WTextFormField : WFormField
#if !SILVERLIGHT && !WP
        ,ILeafWidget
#endif
    {
        #region Class constants
        /// <summary>
        /// Default text of form field.
        /// </summary>
        internal const string DEF_TEXT = "\u2002\u2002\u2002\u2002\u2002";
        #endregion

        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private TextFormFieldType m_formFieldType;
        private string m_defText;
        private int m_maxLength;
        private string m_strTextFormat;
        //    private NumberFormat m_numberFormat;
        private WTextRange m_text;
        //for storing the index of the field separator
        private int m_iFieldSeparator = 0;
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
                return EntityType.TextFormField;
            }
        }
        /// <summary>
        /// Get/sets text form field type.
        /// </summary>
        public TextFormFieldType Type
        {
            get
            {
                return m_formFieldType;
            }
            set
            {
                m_formFieldType = value;
            }
        }
        /// <summary>
        /// Gets/sets string text format (text, date/time, number) directly.
        /// </summary>
        public string StringFormat
        {
            get
            {
                return m_strTextFormat;
            }
            set
            {
                m_strTextFormat = value;
            }
        }
        /// <summary>
        /// Gets/sets default text for text form field.
        /// </summary>
        public string DefaultText
        {
            get
            {
                return m_defText;
            }
            set
            {
                m_defText = value;
            }
        }
        /// <summary>
        /// Gets/sets maximum text length.
        /// </summary>
        public int MaximumLength
        {
            get
            {
                return m_maxLength;
            }
            set
            {
                if ((value < m_defText.Length && value != 0) && (this.Type != TextFormFieldType.Calculation))
                {
                    throw new ArgumentOutOfRangeException("MaximumLength is lower than current text length");
                }
                m_maxLength = value;
            }
        }
        /// <summary>
        /// Gets/sets form field text range;
        /// </summary>
        public WTextRange TextRange
        {
            get
            {
                GetTextRange();
                WTextRange textRange = GetFirstTextRange();
                if (textRange != null)
                    return textRange;
                else
                    return m_text;
            }
            set
            {
                m_text = value;
                SetTextRange(m_text);
            }
        }
        /// <summary>
        /// Gets / sets the text of text form field.
        /// </summary>
        /// <value></value>
        public override string Text
        {
            get
            {
                GetTextRange();
                return m_text.Text;
            }
            set
            {
                WTextRange textRange = GetFirstTextRange();
                if (textRange != null)
                {
                    textRange.Text = value;
                    SetTextRange(textRange);
                }
                else
                {
                    m_text.Text = value;
                    SetTextRange(m_text);
                }
            }
        }

        //    /// <summary>
        //    /// Gets/sets number format for text form field.
        //    /// </summary>
        //    public NumberFormat NumberFormat
        //    {
        //      get
        //      {
        //        return m_numberFormat;
        //      }
        //      set
        //      {
        //        m_numberFormat = value;
        //      }
        //    }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WTextFormField"/> class.
        /// </summary>
        /// <param name="doc"></param>
        public WTextFormField(IWordDocument doc)
            : base(doc)
        {
            m_curFormFieldType = FormFieldType.TextInput;
            m_paraItemType = ParagraphItemType.TextFormField;
            FieldType = FieldType.FieldFormTextInput;
            Params = 128;
            m_defText = string.Empty;
            m_text = new WTextRange(doc);
            m_strTextFormat = string.Empty;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected override object CloneImpl()
        {
            WTextFormField textFormField = (WTextFormField)base.CloneImpl();

            return textFormField;
        }
        /// <summary>
        /// Gets the first text range.
        /// </summary>
        /// <returns></returns>
        private WTextRange GetFirstTextRange()
        {
            for (int i = 0; i < this.Range.Count; i++)
            {
                if (this.Range.Items[i] is WTextRange)
                    return (this.Range.Items[i] as WTextRange);
            }
            return null;
        }
        /// <summary>
        /// Gets the text range.
        /// </summary>
        /// <returns></returns>
        private void GetTextRange()
        {
            if (this.Range.Count < 1)
                return;
            string text = string.Empty;
            for (int i = 0; i < this.Range.Count; i++)
            {
                //Updates text retrieving from the paragraph items
                if (this.Range.Items[i] is ParagraphItem)
                {
                    text += UpdateTextForParagraphItem(this.Range.Items[i] as Entity);
                }
                else
                {
                    text += UpdateTextForTextBodyItem(this.Range.Items[i] as Entity);
                }
            }
            m_text.Text = text;
        }
        /// <summary>
        /// Updates the text for text body item.
        /// </summary>
       /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private string UpdateTextForTextBodyItem(Entity entity)
        {
            string text = string.Empty;
            if (entity is WParagraph)
            {
                for (int i = 0; i < (entity as WParagraph).Items.Count; i++)
                {
                    text += UpdateTextForParagraphItem((entity as WParagraph).Items[i]);
                    if (m_iFieldSeparator == 0)
                        return text;
                }
            }
            else if (entity is WTable)
            {
                //Updates text while the Formfield is in table
                text += UpdateTextForTable(entity);
            }
            return text;
       }
        /// <summary>
        /// Updates the text for table.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private string UpdateTextForTable(Entity entity)
        {
            string text = string.Empty;
            for (int i = 0; i < (entity as WTable).Rows.Count; i++)
            {
                WTableRow row = (entity as WTable).Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    WTableCell cell = row.Cells[j];
                    //Updates text for formfield inside table cell
                    for (int k = 0; k < cell.Items.Count; k++)
                    {
                        text += UpdateTextForTextBodyItem(cell.Items[k]);
                        if (m_iFieldSeparator == 0)
                            return text;
                    }
                }
            }
            return text;
        }
        /// <summary>
        /// Updates the text for paragraph item.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private string UpdateTextForParagraphItem(Entity entity)
        {
            string text = string.Empty;
            if (entity is WTextRange)
                text = (entity as WTextRange).Text;
            else if (entity is WFieldMark && (entity as WFieldMark).Type == FieldMarkType.FieldSeparator)
                m_iFieldSeparator++;
            else if (entity is WFieldMark && (entity as WFieldMark).Type == FieldMarkType.FieldEnd)
                m_iFieldSeparator--;
            return text;
        }
        /// <summary>
        /// Sets the text range.
        /// </summary>
        /// <param name="textRange">The text range.</param>
        private void SetTextRange(WTextRange textRange)
        {
            RemovePreviousText();
            // Updates the modified text.
            int index = this.GetIndexInOwnerCollection();
            for (int j = index; j < this.OwnerParagraph.Items.Count; j++)
            {
                ParagraphItem item = this.OwnerParagraph.Items[j];
                if (item is WFieldMark && (item as WFieldMark).Type == FieldMarkType.FieldSeparator)
                {
                    this.OwnerParagraph.Items.Insert(j + 1, textRange.Clone());
                    break;
                }
            }
            m_bIsFieldRangeUpdated = false;
        }
        /// <summary>
        /// Removes the previous text.
        /// </summary>
        private void RemovePreviousText()
        {
            m_iFieldSeparator = 0;
            // Remove the previous text values.
            for (int i = 0; i < this.Range.Count; i++)
            {
                int count = this.Range.Count;
                //Removes the paragraph items
                if (this.Range.Items[i] is ParagraphItem)
                    RemoveParagraphItem(this.Range.Items[i] as Entity);
                else
                    RemoveTextBodyItem(this.Range.Items[i] as Entity);
                if (this.Range.Count < count)
                    i -= count - this.Range.Count;
            }
        }
        /// <summary>
        /// Removes the text body item.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private void RemoveTextBodyItem(Entity entity)
        {
            string text = string.Empty;
            if (entity is WParagraph)
            {
                for (int i = 0; i < (entity as WParagraph).Items.Count; i++)
                {
                    int count = (entity as WParagraph).Items.Count;
                    RemoveParagraphItem((entity as WParagraph).Items[i]);
                    if (m_iFieldSeparator == 0)
                    {
                        (entity.Owner as WTextBody).Items.Remove(entity);
                        InsertParagraphItems(entity as WParagraph);
                        (entity.Owner as WTextBody).Items.Remove(entity);
                        break;
                    }
                    if ((entity as WParagraph).Items.Count < count)
                        i -= count - (entity as WParagraph).Items.Count;
                }
            }
            else if (entity is WTable)
            {
                (entity.Owner as WTextBody).Items.Remove(entity);
            }
        }
        /// <summary>
        /// Inserts the paragraph items.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        private void InsertParagraphItems(WParagraph paragraph)
        {
            int index = this.GetIndexInOwnerCollection();
            for (int i = index; i < this.OwnerParagraph.Items.Count; i++)
            {
                ParagraphItem item = this.OwnerParagraph.Items[i];
                if (item is WFieldMark && (item as WFieldMark).Type == FieldMarkType.FieldSeparator)
                {
                    for (int j = 0; j < paragraph.Items.Count; j++)
                    {
                        this.OwnerParagraph.Items.Insert(i + j, paragraph.Items[0]);
                    }
                    break;
                }
            }
        }
        /// <summary>
        /// Removes the paragraph item.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private void RemoveParagraphItem(Entity entity)
        {
            if (entity is WTextRange)
                (entity as ParagraphItem).OwnerParagraph.Items.Remove(entity);
            else if (entity is WFieldMark && (entity as WFieldMark).Type == FieldMarkType.FieldSeparator)
            {
                m_iFieldSeparator++;
                if (m_iFieldSeparator > 1)
                    (entity as ParagraphItem).OwnerParagraph.Items.Remove(entity);
            }
            else if (entity is WFieldMark && (entity as WFieldMark).Type == FieldMarkType.FieldEnd)
            {
                m_iFieldSeparator--;
                if (m_iFieldSeparator >= 1)
                    (entity as ParagraphItem).OwnerParagraph.Items.Remove(entity);
            }
        }
        #endregion

        #region Implementation / xml
//#if !SILVERLIGHT
        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.FormFieldMaxLengthAttr))
            {
                m_maxLength = reader.ReadInt(XDLSConstants.FormFieldMaxLengthAttr);
            }
            if (reader.HasAttribute(XDLSConstants.FormFieldDefTextAttr))
            {
                m_defText = reader.ReadString(XDLSConstants.FormFieldDefTextAttr);
            }
            if (reader.HasAttribute(XDLSConstants.FormFieldStrTextFormatAttr))
            {
                m_strTextFormat = reader.ReadString(XDLSConstants.FormFieldStrTextFormatAttr);
            }
            if (reader.HasAttribute(XDLSConstants.FormFieldTextTypeAttr))
            {
                m_formFieldType = (TextFormFieldType)reader.ReadEnum(XDLSConstants.FormFieldTextTypeAttr, typeof(TextFormFieldType));
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            writer.WriteValue(XDLSConstants.FormFieldMaxLengthAttr, m_maxLength);
            writer.WriteValue(XDLSConstants.FormFieldDefTextAttr, m_defText);
            writer.WriteValue(XDLSConstants.FormFieldStrTextFormatAttr, m_strTextFormat);
            writer.WriteValue(XDLSConstants.FormFieldTextTypeAttr, (int)m_formFieldType);
        }
        /// <summary>
        /// Registers child objects in XDSL holder.
        /// </summary>
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddElement(XDLSConstants.TextRangeTag, m_text);
        }
//#endif
        #endregion

#if !SILVERLIGHT && !WP

        #region ILeafWidget Members

        SizeF ILeafWidget.Measure(Syncfusion.DocIO.Rendering.DrawingContext dc)
        {
            return new Size();
        }

        #endregion

        #region Implementation / layout
        /// <summary>
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = (Text == "\t")
                           ? new LayoutTextRangeInfo(this)
                           : new LayoutInfo(ChildrenLayoutDirection.Horizontal);
            m_layoutInfo.Size = (this as WTextRange).GetTextRangeSize();
        }
        #endregion
        
        #region IWidget Members

        ILayoutInfo IWidget.LayoutInfo
        {
            get 
            {
                if (m_layoutInfo == null)
                    CreateLayoutInfo();

                return m_layoutInfo; 
            }
        }

        void IWidget.Draw(Syncfusion.DocIO.Rendering.DrawingContext dc, LayoutedWidget ltWidget)
        {
            //dc.DrawTextRange(this, ltWidget, Text);
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }
        #endregion
#endif
    }
}
