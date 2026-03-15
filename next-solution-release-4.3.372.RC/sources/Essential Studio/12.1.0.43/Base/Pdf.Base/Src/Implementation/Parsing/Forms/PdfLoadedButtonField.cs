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
    /// Represents a button field of an existing PDF document`s form.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Load the form
    /// PdfLoadedForm form = doc.Form;
    /// // Load an existing button field.
    /// PdfLoadedButtonField buttonField = form.Fields["Submit"] as PdfLoadedButtonField;
    /// buttonField.ToolTip = "SubmitButton";
    /// doc.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document.
    /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Load the form
    /// Dim form As PdfLoadedForm = doc.Form
    /// ' Load an existing button field.
    /// Dim buttonField As PdfLoadedButtonField = TryCast(form.Fields("Submit"), PdfLoadedButtonField)
    /// buttonField.ToolTip = "SubmitButton"
    /// doc.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedDocument"/> Class    
    /// <seealso cref="PdfLoadedForm"/> Class    
    public class PdfLoadedButtonField : PdfLoadedStyledField
    {
        #region Fields
        /// <summary>
        /// Collection of button items.
        /// </summary>
        private PdfLoadedButtonItemCollection m_items;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the caption text.
        /// </summary>
        /// <value>A string value specifying the caption of the button.</value>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load the form
        /// PdfLoadedForm form = doc.Form;
        /// // Load an existing button field.
        /// PdfLoadedButtonField buttonField = form.Fields["Submit"] as PdfLoadedButtonField;
        /// buttonField.ToolTip = "SubmitButton";
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load the form
        /// Dim form As PdfLoadedForm = doc.Form
        /// ' Load an existing button field.
        /// Dim buttonField As PdfLoadedButtonField = TryCast(form.Fields("Submit"), PdfLoadedButtonField)
        /// buttonField.ToolTip = "SubmitButton"
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class    
        /// <seealso cref="PdfLoadedForm"/> Class
        public string Text
        {
            get
            {
                string text = GetText();

                return text;
            }
            set
            {
                bool readOnly = ((FieldFlags.ReadOnly & Flags) != 0);
                if (!readOnly)
                {
                    (this as PdfField).Form.SetAppearanceDictionary = true;
                    SetText(value);
                }
            }
        }

        /// <summary>
        /// Gets the collection of button items.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument doc = new PdfLoadedDocument("SourceForm.pdf");
        /// // Load an existing button field
        /// PdfLoadedButtonField buttonField = doc.Form.Fields["Submit"] as PdfLoadedButtonField;
        /// // Reading button collection item
        /// PdfLoadedButtonItemCollection buttonCollection = buttonField.Items;
        /// // Load an existing button item
        /// PdfLoadedButtonItem buttonItem = buttonCollection[0];
        /// buttonItem.Bounds = new RectangleF(0, 0, 20, 30);
        /// doc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim doc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
        /// ' Load an existing button field
        /// Dim buttonField As PdfLoadedButtonField = TryCast(doc.Form.Fields("Submit"), PdfLoadedButtonField)
        /// ' Reading button collection item
        /// Dim buttonCollection As PdfLoadedButtonItemCollection = buttonField.Items
        /// ' Load an existing button item
        /// Dim buttonItem As PdfLoadedButtonItem = buttonCollection(0)
        /// buttonItem.Bounds = New RectangleF(0, 0, 20, 30)
        /// doc.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfLoadedDocument"/> Class    
        /// <seealso cref="PdfLoadedButtonField"/> Class 
        public PdfLoadedButtonItemCollection Items
        {
            get
            {
                return m_items;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedButtonField"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedButtonField(PdfDictionary dictionary, PdfCrossTable crossTable)
            : base(dictionary, crossTable)
        {
            PdfArray kids = Kids;
            m_items = new PdfLoadedButtonItemCollection();

            if (kids != null)
            {
                for (int i = 0; i < kids.Count; ++i)
                {
                    PdfDictionary itemDictionary = crossTable.GetObject(kids[i]) as PdfDictionary;

                    PdfLoadedButtonItem item = new PdfLoadedButtonItem(this, i, itemDictionary);
                    m_items.Add(item);
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <returns>The text of the field.</returns>
        private string GetText()
        {
            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            if (widget == null)
            {
                widget = Dictionary;
            }

            string str = null;

            if (widget.ContainsKey(DictionaryProperties.MK))
            {
                PdfDictionary appearance = CrossTable.GetObject(widget[DictionaryProperties.MK]) as PdfDictionary;

                if (appearance.ContainsKey(DictionaryProperties.CA))
                {
                    PdfString text = CrossTable.GetObject(appearance[DictionaryProperties.CA]) as PdfString;
                    str = text.Value;
                }
            }

            if (str == null)
            {
                PdfString val = CrossTable.GetObject(Dictionary[DictionaryProperties.V]) as PdfString;

                if (val == null)
                {
                    val = GetValue(Dictionary, CrossTable, DictionaryProperties.V, true) as PdfString;
                    //val = CrossTable.GetObject( Dictionary.GetValue( 
                    //    CrossTable, DictionaryProperties.V, DictionaryProperties.Parent ) ) as PdfString;
                }
                if (val != null)
                    str = val.Value;
                else
                    str = "";
            }

            return str;
        }

        /// <summary>
        /// Sets the text of the field.
        /// </summary>
        /// <param name="value">Text field.</param>
        private void SetText(string value)
        {
            string text = value;

            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            if (widget == null)
            {
                widget = Dictionary;
            }

            if (widget.ContainsKey(DictionaryProperties.MK))
            {
                PdfDictionary appearance = CrossTable.GetObject(widget[DictionaryProperties.MK]) as PdfDictionary;
                appearance.SetString(DictionaryProperties.CA, text);
                widget.SetProperty(DictionaryProperties.MK, new PdfReferenceHolder(appearance));
            }
            else
            {
                PdfDictionary appearance = new PdfDictionary();
                appearance.SetString(DictionaryProperties.CA, text);
                widget.SetProperty(DictionaryProperties.MK, new PdfReferenceHolder(appearance));
            }

            Changed = true;
        }

        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal override void Draw()
        {
            base.Draw();

            PdfArray kids = Kids;
            if ((kids != null) && (kids.Count > 1))
            {
                for (int i = 0; i < kids.Count; ++i)
                {
                    PdfLoadedFieldItem item = Items[i];

                    DrawButton(item.Page.Graphics, item);
                }
            }
            else
            {
                DrawButton(Page.Graphics, null);
            }
        }

        /// <summary>
        /// Begins the save.
        /// </summary>
        internal override void BeginSave()
        {
            base.BeginSave();
            PdfArray kids = Kids;

            if ((kids != null))
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
        /// Creates a copy of PdfLoadedButton field.
        /// </summary>
        internal PdfField Clone(PdfDictionary dictionary, PdfPage page)
        {
            PdfCrossTable newTable = page.Section.ParentDocument.CrossTable;
            PdfLoadedButtonField field = new PdfLoadedButtonField(dictionary, newTable);
            field.Page = page;
            field.SetName(GetFieldName());
            field.Widget.Dictionary = Widget.Dictionary.Clone(newTable) as PdfDictionary;

            return field;
        }

        /// <summary>
        /// Creates a copy of PdfLoadedButtonItem.
        /// </summary>
        internal override PdfLoadedFieldItem CreateLoadedItem(PdfDictionary dictionary)
        {
            base.CreateLoadedItem(dictionary);

            PdfLoadedButtonItem item = new PdfLoadedButtonItem(this, m_items.Count, dictionary);
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
                    PdfTemplate pressedTemplate = new PdfTemplate(bounds.Size);

                    DrawButton(template.Graphics, item);
                    DrawButton(pressedTemplate.Graphics, item);

                    appearance.SetProperty(DictionaryProperties.N, new PdfReferenceHolder(template));
                    appearance.SetProperty(DictionaryProperties.D, new PdfReferenceHolder(pressedTemplate));
                    widget.SetProperty(DictionaryProperties.AP, appearance);
                }
            }
            else if ((this as PdfField).Form.SetAppearanceDictionary)
                (this as PdfField).Form.NeedAppearances = true;
        }

        /// <summary>
        /// Draws the button.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="item">The item.</param>
        private void DrawButton(PdfGraphics graphics, PdfLoadedFieldItem item)
        {
            PdfLoadedStyledField.GraphicsProperties gp;
            GetGraphicsProperties(out gp, item);

            if (!Flatten)
            {
                gp.Rect.Location = new PointF(0, 0);
            }

            PaintParams prms = new PaintParams(gp.Rect, gp.BackBrush, gp.ForeBrush, gp.Pen,
                gp.Style, gp.BorderWidth, gp.ShadowBrush, gp.RotationAngle);

            if (Flatten)
                gp.StringFormat.Alignment = PdfTextAlignment.Center;

            FieldPainter.DrawButton(graphics, prms, Text, gp.Font, gp.StringFormat);
        }

        /// <summary>
        /// Gets the height of the font.
        /// </summary>
        /// <param name="family"></param>
        /// <returns>The calculated size of font.</returns>
        internal override float GetFontHeight(PdfFontFamily family)
        {
            PdfFont font = new PdfStandardFont(family, 12);
            float max = font.MeasureString(Text).Width;
            float s = ((12 * (Bounds.Size.Width - 4 * BorderWidth)) / max);
            s = (s > 12) ? 12 : s;

            return s;
        }

        /// <summary>
        /// Adds Print action to current button field.</summary>
        /// <remarks>Clicking on the specified button will trigger the Print Dialog Box.</remarks>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document
        /// PdfLoadedDocument ldoc = new PdfLoadedDocument("SourceDoc.pdf");
        /// // Load the existing form
        /// PdfLoadedForm form = ldoc.Form;
        /// // Load an existing button field.
        /// PdfLoadedButtonField buttonField = form.Fields["Submit"] as PdfLoadedButtonField;
        /// // Adding print action
        /// buttonField.AddPrintAction();
        /// // Save the document to a disk
        /// ldoc.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document
        /// Dim ldoc As PdfLoadedDocument = New PdfLoadedDocument("SourceDoc.pdf")
        /// ' Load the existing form
        /// Dim form As PdfLoadedForm = ldoc.Form
        /// ' Load an existing button field.
        /// Dim buttonField As PdfLoadedButtonField = TryCast(form.Fields("Submit"), PdfLoadedButtonField)
        /// ' Adding print action
        /// buttonField.AddPrintAction()
        /// ' Save the document to a disk
        /// ldoc.Save("Form.pdf")
        /// </code>
        /// </example>
        public void AddPrintAction()
        {
            PdfDictionary actionDictionary = new PdfDictionary();
            actionDictionary.SetProperty(DictionaryProperties.N, new PdfName(DictionaryProperties.Print));
            actionDictionary.SetProperty(DictionaryProperties.S, new PdfName(DictionaryProperties.Named));
            PdfArray kidsArray = Dictionary[DictionaryProperties.Kids] as PdfArray;
            if (kidsArray != null)
            {
                PdfReferenceHolder buttonObject = kidsArray[0] as PdfReferenceHolder;
                PdfDictionary buttonDictionary = buttonObject.Object as PdfDictionary;
                buttonDictionary.SetProperty(DictionaryProperties.A, actionDictionary);
            }
            else
            {
                Dictionary.SetProperty(DictionaryProperties.A, actionDictionary);
            }

        }
        #endregion
    }
}
