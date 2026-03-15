#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents an item of a radio button list.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();           
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
    /// PdfBrush brush = PdfBrushes.Black;
    /// PdfGraphics g = page.Graphics;
    /// //Create a Radiobutton
    /// PdfRadioButtonListField employeesRadioList = new PdfRadioButtonListField(page, "employeesRadioList");
    /// //Add to document
    /// document.Form.Fields.Add(employeesRadioList);
    /// //Create radiobutton items 
    /// PdfRadioButtonListItem radioItem1 = new PdfRadioButtonListItem("1-9");
    /// radioItem1.Bounds = new RectangleF(100, 140, 20, 20);
    /// g.DrawString("1-9", font, brush, new RectangleF(150, 145, 180, 20));
    /// PdfRadioButtonListItem radioItem2 = new PdfRadioButtonListItem("10-49");
    /// radioItem2.Bounds = new RectangleF(100, 170, 20, 20);
    /// g.DrawString("10-49", font, brush, new RectangleF(150, 175, 180, 20));
    /// PdfRadioButtonListItem radioItem3 = new PdfRadioButtonListItem("50-99");
    /// radioItem3.Bounds = new RectangleF(100, 200, 20, 20);
    /// g.DrawString("50-99", font, brush, new RectangleF(150, 205, 180, 20));
    /// PdfRadioButtonListItem radioItem4 = new PdfRadioButtonListItem("100-499");
    /// radioItem4.Bounds = new RectangleF(100, 230, 20, 20);
    /// g.DrawString("100-499", font, brush, new RectangleF(150, 235, 180, 20));
    /// PdfRadioButtonListItem radioItem5 = new PdfRadioButtonListItem("500-more");
    /// radioItem5.Bounds = new RectangleF(100, 260, 20, 20);
    /// g.DrawString("500-more", font, brush, new RectangleF(150, 265, 180, 20));
    /// //add the items to radio button group
    /// employeesRadioList.Items.Add(radioItem1);
    /// employeesRadioList.Items.Add(radioItem2);
    /// employeesRadioList.Items.Add(radioItem3);
    /// employeesRadioList.Items.Add(radioItem4);
    /// employeesRadioList.Items.Add(radioItem5);
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
    /// Dim brush As PdfBrush = PdfBrushes.Black
    /// Dim g As PdfGraphics = page.Graphics
    /// 'Create a Radiobutton
    /// Dim employeesRadioList As PdfRadioButtonListField = New PdfRadioButtonListField(page, "employeesRadioList")
    /// 'Add to document
    /// document.Form.Fields.Add(employeesRadioList)
    /// 'Create radiobutton items 
    /// Dim radioItem1 As PdfRadioButtonListItem = New PdfRadioButtonListItem("1-9")
    /// radioItem1.Bounds = New RectangleF(100, 140, 20, 20)
    /// g.DrawString("1-9", font, brush, New RectangleF(150, 145, 180, 20))
    /// Dim radioItem2 As PdfRadioButtonListItem = New PdfRadioButtonListItem("10-49")
    /// radioItem2.Bounds = New RectangleF(100, 170, 20, 20)
    /// g.DrawString("10-49", font, brush, New RectangleF(150, 175, 180, 20))
    /// Dim radioItem3 As PdfRadioButtonListItem = New PdfRadioButtonListItem("50-99")
    /// radioItem3.Bounds = New RectangleF(100, 200, 20, 20)
    /// g.DrawString("50-99", font, brush, New RectangleF(150, 205, 180, 20))
    /// Dim radioItem4 As PdfRadioButtonListItem = New PdfRadioButtonListItem("100-499")
    /// radioItem4.Bounds = New RectangleF(100, 230, 20, 20)
    /// g.DrawString("100-499", font, brush, New RectangleF(150, 235, 180, 20))
    /// Dim radioItem5 As PdfRadioButtonListItem = New PdfRadioButtonListItem("500-more")
    /// radioItem5.Bounds = New RectangleF(100, 260, 20, 20)
    /// g.DrawString("500-more", font, brush, New RectangleF(150, 265, 180, 20))
    /// 'add the items to radio button group
    /// employeesRadioList.Items.Add(radioItem1)
    /// employeesRadioList.Items.Add(radioItem2)
    /// employeesRadioList.Items.Add(radioItem3)
    /// employeesRadioList.Items.Add(radioItem4)
    /// employeesRadioList.Items.Add(radioItem5)
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfField"/> Class  
    /// <seealso cref="PdfDocument"/> Class   
    /// <seealso cref="PdfPage"/> Class 
    /// <seealso cref="PdfRadioButtonListField"/> Class 
    public class PdfRadioButtonListItem :
        PdfCheckFieldBase,
        IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store field which this item belongs to.
        /// </summary>
        private PdfRadioButtonListField m_field = null;

        /// <summary>
        /// Internal variable to store item's value.
        /// </summary>
        private string m_value = String.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRadioButtonListItem"/> class.
        /// </summary>
        public PdfRadioButtonListItem()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRadioButtonListItem"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PdfRadioButtonListItem(string value)
            : base()
        {
            Value = value;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the form of the field.
        /// </summary>
        /// <value>The <see cref="PdfForm"> object of the field.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);
        /// PdfBrush brush = PdfBrushes.Black;
        /// PdfGraphics g = page.Graphics;
        /// //Create a Radiobutton
        /// PdfRadioButtonListField employeesRadioList = new PdfRadioButtonListField(page, "employeesRadioList");
        /// //Add to document
        /// document.Form.Fields.Add(employeesRadioList);
        /// //Create radiobutton items 
        /// PdfRadioButtonListItem radioItem1 = new PdfRadioButtonListItem("1-9");
        /// radioItem1.Bounds = new RectangleF(100, 140, 20, 20);
        /// g.DrawString("1-9", font, brush, new RectangleF(150, 145, 180, 20));
        /// //add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1);
        /// // Gets the form
        /// PdfForm form = employeesRadioList.Form;
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
        /// Dim brush As PdfBrush = PdfBrushes.Black
        /// Dim g As PdfGraphics = page.Graphics
        /// 'Create a Radiobutton
        /// Dim employeesRadioList As PdfRadioButtonListField = New PdfRadioButtonListField(page, "employeesRadioList")
        /// 'Add to document
        /// document.Form.Fields.Add(employeesRadioList)
        /// 'Create radiobutton items 
        /// Dim radioItem1 As PdfRadioButtonListItem = New PdfRadioButtonListItem("1-9")
        /// radioItem1.Bounds = New RectangleF(100, 140, 20, 20)
        /// g.DrawString("1-9", font, brush, New RectangleF(150, 145, 180, 20))
        /// 'add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1)
        /// ' Gets the form
        /// Dim form As PdfForm = employeesRadioList.Form
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfField"/> Class  
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListField"/> Class 
        public override PdfForm Form
        {
            get
            {
                if (m_field != null)
                {
                    return m_field.Form;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a Radiobutton
        /// PdfRadioButtonListField employeesRadioList = new PdfRadioButtonListField(page, "employeesRadioList");
        /// //Add to document
        /// document.Form.Fields.Add(employeesRadioList);
        /// //Create radiobutton items 
        /// PdfRadioButtonListItem radioItem1 = new PdfRadioButtonListItem("1-9");
        /// radioItem1.Bounds = new RectangleF(100, 140, 20, 20);           
        /// //add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a Radiobutton
        /// Dim employeesRadioList As PdfRadioButtonListField = New PdfRadioButtonListField(page, "employeesRadioList")
        /// 'Add to document
        /// document.Form.Fields.Add(employeesRadioList)
        /// 'Create radiobutton items 
        /// Dim radioItem1 As PdfRadioButtonListItem = New PdfRadioButtonListItem("1-9")
        /// radioItem1.Bounds = New RectangleF(100, 140, 20, 20)
        /// 'add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfField"/> Class  
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListField"/> Class 
        public override RectangleF Bounds
        {
            get
            {
                RectangleF rect = base.Bounds;

                if (m_field != null)
                {
                    rect = GetBoundsAtLoadedPage(m_field.Page, rect);
                }

                return rect;
            }

            set
            {
                base.Bounds = value;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        /// <example>
        /// <code lang="C#">
        /// //Create a new PDf document
        /// PdfDocument document = new PdfDocument();           
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a Radiobutton
        /// PdfRadioButtonListField employeesRadioList = new PdfRadioButtonListField(page, "employeesRadioList");
        /// //Add to document
        /// document.Form.Fields.Add(employeesRadioList);
        /// //Create radiobutton items 
        /// PdfRadioButtonListItem radioItem1 = new PdfRadioButtonListItem();
        /// radioItem1.Value = "1-9";
        /// radioItem1.Bounds = new RectangleF(100, 140, 20, 20);           
        /// //add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1);
        /// document.Save("Form.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDf document
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a Radiobutton
        /// Dim employeesRadioList As PdfRadioButtonListField = New PdfRadioButtonListField(page, "employeesRadioList")
        /// 'Add to document
        /// document.Form.Fields.Add(employeesRadioList)
        /// 'Create radiobutton items 
        /// Dim radioItem1 As PdfRadioButtonListItem = New PdfRadioButtonListItem()
        /// radioItem1.Value = "1-9"
        /// radioItem1.Bounds = New RectangleF(100, 140, 20, 20)
        /// 'add the items to radio button group
        /// employeesRadioList.Items.Add(radioItem1)
        /// document.Save("Form.pdf")
        /// </code>
        /// </example>      
        /// <seealso cref="PdfDocument"/> Class   
        /// <seealso cref="PdfPage"/> Class 
        /// <seealso cref="PdfRadioButtonListField"/> Class 
        public string Value
        {
            get
            {
                return m_value;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Value");
                }

                if (value.Length == 0)
                {
                    throw new ArgumentException("Value can't be an empty string.");
                }

                m_value = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            Widget.BeginSave += new EventHandler(Widget_Save);
            Style = PdfCheckBoxStyle.Circle;
        }

        /// <summary>
        /// Sets the field.
        /// </summary>
        /// <param name="field">The field.</param>
        internal void SetField(PdfRadioButtonListField field)
        {
            Widget.Parent = field;

            PdfPage simplePage = (field != null) ? (field.Page as PdfPage) : (m_field.Page as PdfPage);

            if (simplePage != null)
            {
                if (field == null)
                {
                    int index = simplePage.Annotations.IndexOf(Widget);
                    simplePage.Annotations.RemoveAt(index);
                }
                else
                {
                    simplePage.Annotations.Add(Widget);
                }
            }
            else
            {
                PdfLoadedPage loadedPage = field.Page as PdfLoadedPage;
                PdfDictionary pageDic = loadedPage.Dictionary;
                PdfArray annots = null;

                if (pageDic.ContainsKey(DictionaryProperties.Annots))
                {
                    annots = loadedPage.CrossTable.GetObject(pageDic[DictionaryProperties.Annots]) as PdfArray;
                }
                else
                {
                    annots = new PdfArray();
                }

                PdfReferenceHolder reference = new PdfReferenceHolder(Widget);

                if (field == null)
                {
                    int index = annots.IndexOf(reference);

                    if (index >= 0)
                    {
                        annots.RemoveAt(index);
                    }
                }
                else
                {
                    BoundsAtLoadedPage(field.Page, Widget.Bounds);
                    annots.Add(reference);
                }

                field.Page.Dictionary.SetProperty(DictionaryProperties.Annots, annots);
            }

            m_field = field;
        }

        /// <summary>
        /// Handles the Save event of the Widget control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Widget_Save(object sender, EventArgs e)
        {
            Save();
        }

        /// <summary>
        /// Saves an instance.
        /// </summary>
        internal override void Save()
        {
            base.Save();

            if (Form != null)
            {
                string value = GetValue();

                Widget.ExtendedAppearance.Normal.OnMappingName = value;
                Widget.ExtendedAppearance.Pressed.OnMappingName = value;

                if (m_field.SelectedItem == this)
                {
                    Widget.AppearanceState = GetValue();
                }
                else
                {
                    Widget.AppearanceState = DictionaryProperties.Off;
                }
            }
        }

        /// <summary>
        /// Draws the appearance.
        /// </summary>
        protected override void DrawAppearance()
        {
            base.DrawAppearance();

            PaintParams paintParams = new PaintParams(
                new RectangleF(PointF.Empty, Size), BackBrush, ForeBrush,
                BorderPen, BorderStyle, BorderWidth, ShadowBrush, RotationAngle);

            PdfTemplate template = Widget.ExtendedAppearance.Normal.On;
            FieldPainter.DrawRadioButton(template.Graphics, paintParams, StyleToString(Style),
                PdfCheckFieldState.Checked);

            template = Widget.ExtendedAppearance.Normal.Off;
            FieldPainter.DrawRadioButton(template.Graphics, paintParams, StyleToString(Style),
                PdfCheckFieldState.Unchecked);

            template = Widget.ExtendedAppearance.Pressed.On;
            FieldPainter.DrawRadioButton(template.Graphics, paintParams, StyleToString(Style),
                PdfCheckFieldState.PressedChecked);

            template = Widget.ExtendedAppearance.Pressed.Off;
            FieldPainter.DrawRadioButton(template.Graphics, paintParams, StyleToString(Style),
                PdfCheckFieldState.PressedUnchecked);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>m_value</returns>
        private string GetValue()
        {
            if (m_value == String.Empty)
            {
                int index = m_field.Items.IndexOf(this);
                return index.ToString();
            }
            else
            {
                return m_value;
            }
        }

        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal override void Draw()
        {
            RemoveAnnoationFromPage(m_field.Page, Widget);

            PaintParams parameters
                = new PaintParams(Bounds, BackBrush, ForeBrush, BorderPen, BorderStyle, BorderWidth, ShadowBrush, RotationAngle);

            PdfCheckFieldState state = PdfCheckFieldState.Unchecked;
            if ((m_field.SelectedIndex >= 0) && (m_field.SelectedValue == Value))
            {
                state = PdfCheckFieldState.Checked;
            }

            FieldPainter.DrawRadioButton(m_field.Page.Graphics, parameters, StyleToString(Style), state);
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return (Widget as IPdfWrapper).Element;
            }
        }
        #endregion
    }
}
