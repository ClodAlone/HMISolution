#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Text;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Security;
using System.Collections.Generic;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents the combo box field of an existing item.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Read a combo box field
    /// PdfLoadedComboBoxField comboField = doc.Form.Fields["EmployeeCombo"] as PdfLoadedComboBoxField;
    /// comboField.SelectedIndex = 0;
    /// doc.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Read a combo box field
    /// Dim comboField As PdfLoadedComboBoxField = TryCast(doc.Form.Fields("EmployeeCombo"), PdfLoadedComboBoxField)
    /// comboField.SelectedIndex = 0
    /// doc.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedChoiceField"/> Class   
    public class PdfLoadedComboBoxField : PdfLoadedChoiceField
    {
        #region Fields
        /// <summary>
        /// Stores the collection of items.
        /// </summary>
        private PdfLoadedComboBoxItemCollection m_items;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfComboBoxField"/> is editable.
        /// </summary>
        /// <value>True if the drop down list is editable, false otherwise. Default is false.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load an existing combo field
        /// PdfLoadedComboBoxField comboField = doc.Form.Fields["EmployeeCombo"] as PdfLoadedComboBoxField;
        /// comboField.Editable = false;
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load an existing Check field
        /// Dim choiceField As PdfLoadedChoiceField = TryCast(doc.Form.Fields("Java"), PdfLoadedChoiceField)
        /// ' Change the selected item
        /// Dim item As PdfLoadedListItem = choiceField.SelectedItem
        /// item.Text = "New Text"
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedListItem"/> Class
        /// <seealso cref="PdfLoadedDocument"/> Class
        public bool Editable
        {
            get
            {
                bool eidtable = ((FieldFlags.Edit & Flags) != 0);

                return eidtable;
            }
            set
            {
                bool editable = value;

                if (editable)
                {
                    Flags |= FieldFlags.Edit;
                }
                else
                {
                    Flags -= FieldFlags.Edit;
                }
            }
        }

        /// <summary>
        /// Gets the collection of combo box items.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");           
        /// // Load an existing combo field
        /// PdfLoadedComboBoxField comboField = doc.Form.Fields["EmployeeCombo"] as PdfLoadedComboBoxField;
        /// // Load combo field collection
        /// PdfLoadedComboBoxItemCollection comboCollection = comboField.Items;
        /// // Reading first item of the collection.
        /// PdfLoadedComboBoxItem item = comboCollection[0];
        /// item.Location = new PointF(200, 200);
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load an existing combo field
        /// Dim comboField As PdfLoadedComboBoxField = TryCast(doc.Form.Fields("EmployeeCombo"), PdfLoadedComboBoxField)
        /// ' Load combo field collection
        /// Dim comboCollection As PdfLoadedComboBoxItemCollection = comboField.Items
        /// ' Reading first item of the collection.
        /// Dim item As PdfLoadedComboBoxItem = comboCollection(0)
        /// item.Location = New PointF(200, 200)
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class
        /// <seealso cref="PdfLoadedComboBoxItem"/> Class
        public PdfLoadedComboBoxItemCollection Items
        {
            get
            {
                return m_items;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return GetSelectedIndex()[0];
            }
            set
            {
                 SetSelectedIndex(new int[] { value });
            }
        }
        public string SelectedValue
        {
            get
            {
                return GetSelectedValue()[0];
            }
            set
            {
                SetSelectedValue(new string[] { value }); 
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedComboBoxField"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedComboBoxField(PdfDictionary dictionary, PdfCrossTable crossTable)
            : base(dictionary, crossTable)
        {
            PdfArray kids = Kids;
            m_items = new PdfLoadedComboBoxItemCollection();

            if (kids != null)
            {
                for (int i = 0; i < kids.Count; ++i)
                {
                    PdfDictionary itemDictionary = crossTable.GetObject(kids[i]) as PdfDictionary;

                    PdfLoadedComboBoxItem item = new PdfLoadedComboBoxItem(this, i, itemDictionary);
                    m_items.Add(item);
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal override void Draw()
        {
            base.Draw();

            RectangleF rect = Bounds;
            rect.Location = PointF.Empty;

            string text = String.Empty;

            if (SelectedIndex != -1)
            {
                text = SelectedItem[0].Text;
            }
			else if(Dictionary.ContainsKey(DictionaryProperties.DV))
            {
                text = (Dictionary[DictionaryProperties.DV] as PdfString).Value;
            }
            //else if (Values.Count > 0)
            //{
            //    text = Values[0].Text;
            //}

            PdfTemplate template = new PdfTemplate(rect.Size);

            PdfArray kids = Kids;
            if ((kids != null) && (kids.Count > 1))
            {
                for (int i = 0; i < kids.Count; ++i)
                {
                    PdfLoadedFieldItem item = (Items[i] as PdfLoadedFieldItem);
                    template = new PdfTemplate(item.Size);
                    rect = item.Bounds;
                    rect.Location = PointF.Empty;

                    DrawComboBox(template.Graphics, item);
                    template.Graphics.DrawString(text, item.Font, item.ForeBrush, rect, item.StringFormat);
                    item.Page.Graphics.DrawPdfTemplate(template, item.Bounds.Location);
                }
            }
            else
            {
                DrawComboBox(template.Graphics, null);
                template.Graphics.DrawString(text, Font, ForeBrush, rect, StringFormat);
                Page.Graphics.DrawPdfTemplate(template, Bounds.Location);
            }
        }

        /// <summary>
        /// Begins the save.
        /// </summary>
        internal override void BeginSave()
        {
            base.BeginSave();
            PdfArray kids = Kids;
            if (kids != null)
            {
                for (int i = 0; i < kids.Count; ++i)
                {
                    PdfDictionary widget = CrossTable.GetObject(kids[i]) as PdfDictionary;
                    ApplyAppearance(widget, Items[i]);
                }
            }
            else
            {
                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                ApplyAppearance(widget, null);
            }
        }

        /// <summary>
        /// Creates a copy of PdfLoadedComboBoxField.
        /// </summary>
        internal PdfField Clone(PdfDictionary dictionary, PdfPage page)
        {
            PdfCrossTable newTable = page.Section.ParentDocument.CrossTable;
            PdfLoadedComboBoxField field = new PdfLoadedComboBoxField(dictionary, newTable);
            field.Page = page;
            field.SetName(GetFieldName());
            field.Widget.Dictionary = Widget.Dictionary.Clone(newTable) as PdfDictionary;

            return field;
        }

        /// <summary>
        /// Creates a copy of PdfLoadedComboBoxItem.
        /// </summary>
        internal override PdfLoadedFieldItem CreateLoadedItem(PdfDictionary dictionary)
        {
            base.CreateLoadedItem(dictionary);

            PdfLoadedComboBoxItem item = new PdfLoadedComboBoxItem(this, m_items.Count, dictionary);
            m_items.Add(item);

            if (Kids == null)
                Dictionary[DictionaryProperties.Kids] = new PdfArray();

            Kids.Add(new PdfReferenceHolder(dictionary));

            return item;
        }

        /// <summary>
        /// Applies the appearance.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <param name="item">The item.</param>
        private void ApplyAppearance(PdfDictionary widget, PdfLoadedFieldItem item)
        {
            if ((widget != null) && (widget.ContainsKey(DictionaryProperties.AP)))
            {
                PdfDictionary appearance = CrossTable.GetObject(widget[DictionaryProperties.AP]) as PdfDictionary;

                if ((appearance != null) && (appearance.ContainsKey(DictionaryProperties.N)))
                {
                    RectangleF bounds = (item == null) ? Bounds : item.Bounds;
                    PdfTemplate template = new PdfTemplate(Bounds.Size);

                    DrawComboBox(template.Graphics, item);

                    appearance.Remove(DictionaryProperties.N);
                    appearance.SetProperty(DictionaryProperties.N, new PdfReferenceHolder(template));
                    widget.SetProperty(DictionaryProperties.AP, appearance);
                }
            }
            else if((this as PdfField).Form.SetAppearanceDictionary)
                (this as PdfField).Form.NeedAppearances = true;
        }

        /// <summary>
        /// Draws the combo box.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="item">The item.</param>
        private void DrawComboBox(PdfGraphics graphics, PdfLoadedFieldItem item)
        {
            PdfLoadedStyledField.GraphicsProperties gp;
            GetGraphicsProperties(out gp, item);

            gp.Rect.Location = PointF.Empty;

            PaintParams prms = new PaintParams(gp.Rect, gp.BackBrush, gp.ForeBrush, gp.Pen, gp.Style,
                gp.BorderWidth, gp.ShadowBrush, gp.RotationAngle);

            FieldPainter.DrawComboBox(graphics, prms);
        }

        /// <summary>
        /// Gets the height of the font.
        /// </summary>
        /// <param name="family"></param>
        /// <returns>The calculated size of font.</returns>
        internal override float GetFontHeight(PdfFontFamily family)
        {
            List<float> widths = new List<float>();
            foreach (PdfLoadedListItem item in SelectedItem)
            {
                //PdfLoadedListItem item = SelectedItem;
                PdfFont font = new PdfStandardFont(family, 12);
               widths.Add(font.MeasureString(item.Text).Width);
            }
            widths.Sort();
            float s = 0;
            s = ((12 * (Bounds.Size.Width - 4 * BorderWidth)) /widths[widths.Count-1]);

            return s;
        }
        #endregion
    }
}
