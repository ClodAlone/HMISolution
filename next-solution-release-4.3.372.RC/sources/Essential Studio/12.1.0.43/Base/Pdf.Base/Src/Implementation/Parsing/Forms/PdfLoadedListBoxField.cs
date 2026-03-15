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
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Security;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents loaded list box field.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    ///  //Load an existing document
    ///  PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    ///  // Load the list box field          
    ///  PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;
    ///  // Flatten the list field
    ///  listField.Flatten = true;
    ///  doc.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    ///  'Load an existing document
    ///  Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    ///  ' Load the list box field          
    ///  Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
    ///  ' Flatten the list field
    ///  listField.Flatten = True
    ///  doc.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedChoiceField"/> Class  
    public class PdfLoadedListBoxField : PdfLoadedChoiceField
    {
        #region Fields
        /// <summary>
        /// Represents collection of items.
        /// </summary>
        private PdfLoadedListFieldItemCollection m_items;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the field is multiselectable..
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Loads an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load the list box field 
        /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;
        /// // Enabling the multi selection option
        /// listField.MultiSelect = true;
        /// doc.Save("Sample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load the list box field 
        /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
        /// ' Enabling the multi selection option
        /// listField.MultiSelect = True
        /// doc.Save("Sample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class  
        /// <seealso cref="PdfLoadedListBoxField"/> Class  
        public bool MultiSelect
        {
            get
            {
                bool multiSelect = ((FieldFlags.MultiSelect & Flags) != 0);

                return multiSelect;
            }
            set
            {
                bool multiSelect = value;

                if (multiSelect)
                {
                    Flags |= FieldFlags.MultiSelect;
                }
                else
                {
                    Flags &= ~FieldFlags.MultiSelect;
                }
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The collection of list box items.</value>
        /// <example>
        /// <code lang="C#">
        /// // Loads an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load the list box field 
        /// PdfLoadedListBoxField listField = doc.Form.Fields["Course"] as PdfLoadedListBoxField;
        /// // List box items collection
        /// PdfLoadedListFieldItemCollection listcollection = listField.Items;
        /// listcollection[0].Location = new PointF(100, 200);
        /// doc.Save("Sample.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Loads an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load the list box field 
        /// Dim listField As PdfLoadedListBoxField = TryCast(doc.Form.Fields("Course"), PdfLoadedListBoxField)
        /// ' List box items collection
        /// Dim listcollection As PdfLoadedListFieldItemCollection = listField.Items
        /// listcollection(0).Location = New PointF(100, 200)
        /// doc.Save("Sample.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class  
        /// <seealso cref="PdfLoadedListBoxField"/> Class 
        public PdfLoadedListFieldItemCollection Items
        {
            get
            {
                return m_items;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedListBoxField"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedListBoxField(PdfDictionary dictionary, PdfCrossTable crossTable)
            : base(dictionary, crossTable)
        {
            PdfArray kids = Kids;
            m_items = new PdfLoadedListFieldItemCollection();

            if (kids != null)
            {
                for (int i = 0; i < kids.Count; ++i)
                {
                    PdfDictionary itemDictionary = crossTable.GetObject(kids[i]) as PdfDictionary;

                    PdfLoadedListFieldItem item = new PdfLoadedListFieldItem(this, i, itemDictionary);
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

            PdfTemplate template = new PdfTemplate(Bounds.Size);

            PdfArray kids = Kids;
            if ((kids != null) && (kids.Count > 1))
            {
                for (int i = 0; i < kids.Count; ++i)
                {
                    PdfLoadedFieldItem item = (Items[i] as PdfLoadedFieldItem);
                    template = new PdfTemplate(item.Size);

                    DrawListBox(template.Graphics, item);
                    item.Page.Graphics.DrawPdfTemplate(template, Bounds.Location);
                }
            }
            else
            {
                DrawListBox(template.Graphics, null);
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
        /// Creates a copy of PdfLoadedListBoxField.
        /// </summary>
        internal PdfField Clone(PdfDictionary dictionary, PdfPage page)
        {
            PdfCrossTable newTable = page.Section.ParentDocument.CrossTable;
            PdfLoadedListBoxField field = new PdfLoadedListBoxField(dictionary, newTable);
            field.Page = page;
            field.SetName(GetFieldName());
            field.Widget.Dictionary = Widget.Dictionary.Clone(newTable) as PdfDictionary;

            return field;
        }

        /// <summary>
        /// Creates a copy of PdfLoadedListField Item.
        /// </summary>
        /// <param name="dictionary"></param>
        internal override PdfLoadedFieldItem CreateLoadedItem(PdfDictionary dictionary)
        {
            base.CreateLoadedItem(dictionary);

            PdfLoadedListFieldItem item = new PdfLoadedListFieldItem(this, m_items.Count, dictionary);
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
                    PdfTemplate template = new PdfTemplate(bounds.Size);

                    DrawListBox(template.Graphics, item);

                    appearance.Remove(DictionaryProperties.N);
                    appearance.SetProperty(DictionaryProperties.N, new PdfReferenceHolder(template));
                    widget.SetProperty(DictionaryProperties.AP, appearance);
                }
            }
            else if ((this as PdfField).Form.SetAppearanceDictionary)
                (this as PdfField).Form.NeedAppearances = true;
        }

        /// <summary>
        /// Draws the list box.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="item">The item.</param>
        private void DrawListBox(PdfGraphics graphics, PdfLoadedFieldItem item)
        {
            PdfLoadedStyledField.GraphicsProperties gp;
            GetGraphicsProperties(out gp, item);

            gp.Rect.Location = PointF.Empty;

            PaintParams prms = new PaintParams(gp.Rect, gp.BackBrush, gp.ForeBrush, gp.Pen, gp.Style,
                gp.BorderWidth, gp.ShadowBrush, gp.RotationAngle);

            PdfListFieldItemCollection items = ConvertToListItems(Values);

            FieldPainter.DrawListBox(graphics, prms, items, SelectedIndex, gp.Font, gp.StringFormat);
        }

        /// <summary>
        /// Converts to list items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The PdfListItemCollection.</returns>
        private PdfListFieldItemCollection ConvertToListItems(PdfLoadedListItemCollection items)
        {
            PdfListFieldItemCollection itemCollection = new PdfListFieldItemCollection();

            foreach (PdfLoadedListItem loadItem in items)
            {
                itemCollection.Add(new PdfListFieldItem(loadItem.Text, loadItem.Value));
            }

            return itemCollection;
        }

        /// <summary>
        /// Gets the height of the font.
        /// </summary>
        /// <param name="family"></param>
        /// <returns>The calculated size of font.</returns>
        internal override float GetFontHeight(PdfFontFamily family)
        {
            PdfLoadedListItemCollection items = Values;

            float s = 0;

            if (items.Count > 0)
            {
                PdfFont font = new PdfStandardFont(family, 12);

                float max = font.MeasureString(items[0].Text).Width;

                for (int i = 1, size = items.Count; i < size; ++i)
                {
                    float temp = font.MeasureString(items[i].Text).Width;

                    max = (max > temp) ? max : temp;
                }

                s = ((12 * (Bounds.Size.Width - 4 * BorderWidth)) / max);

                s = (s > 12) ? 12 : s;
            }

            return s;
        }
        #endregion
    }
}
