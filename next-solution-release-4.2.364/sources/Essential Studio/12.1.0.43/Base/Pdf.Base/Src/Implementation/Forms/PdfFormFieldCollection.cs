#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.Parsing;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents a collection of form fields.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new PDf document
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);          
    /// //Create a combo box
    /// PdfComboBoxField positionComboBox = new PdfComboBoxField(page, "positionComboBox");
    /// positionComboBox.Editable = true;
    /// positionComboBox.Bounds = new RectangleF(100, 115, 200, 20);
    /// positionComboBox.Font = font;
    /// positionComboBox.Editable = true;
    /// //Add it to document
    /// document.Form.Fields.Add(positionComboBox);
    /// PdfFieldCollection fieldCollection = document.Form.Fields;
    /// // Flatten the form collection
    /// for (int i = 0; i < fieldCollection.Count; i++)
    /// {
    ///  fieldCollection[i].Flatten = true;
    /// }
    /// document.Save("Form.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = document.Pages.Add()
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Courier, 12f)
    /// 'Create a combo box
    /// Dim positionComboBox As PdfComboBoxField = New PdfComboBoxField(page, "positionComboBox")
    /// positionComboBox.Editable = True
    /// positionComboBox.Bounds = New RectangleF(100, 115, 200, 20)
    /// positionComboBox.Font = font
    /// positionComboBox.Editable = True
    /// 'Add it to document
    /// document.Form.Fields.Add(positionComboBox)
    /// Dim fieldCollection As PdfFieldCollection = document.Form.Fields
    /// ' Flatten the form collection
    /// For i As Integer = 0 To fieldCollection.Count - 1
    /// fieldCollection(i).Flatten = True
    /// Next i
    /// document.Save("Form.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfFieldCollection"/> Class 
    /// <seealso cref="PdfDocument"/> Class 
    /// <seealso cref="PdfComboBoxField"/> Class 
    public class PdfFormFieldCollection : PdfFieldCollection
    {
        #region Fields
        /// <summary>
        /// Internal variable to store form.
        /// </summary>
        private PdfForm m_form = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFormFieldCollection"/> class.
        /// </summary>
        public PdfFormFieldCollection()
            : base()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the form.
        /// </summary>
        internal PdfForm Form
        {
            get
            {
                return m_form;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("form");
                }

                m_form = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds a field to collection.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        protected override int DoAdd(PdfField field)
        {
            field.SetForm(Form);

            string name = string.Empty;
            if (field is PdfLoadedField)
                name = (field as PdfLoadedField).ActualFieldName;
            else
                name = field.Name;

            if (name == null)
                name = Guid.NewGuid().ToString();

            m_form.FieldNames.Add(name);

            if (m_form.FieldAutoNaming)
            {
                string newName = m_form.GetCorrectName(name);
                field.ApplyName(newName);
            }
            else
            {
                //TODO: Implement corect field adding to group.
                throw new PdfDocumentException(String.Format(c_exisingFieldException, name));
            }

            return base.DoAdd(field);
        }

        /// <summary>
        /// Inserts a filed into collection.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="field">The field.</param>
        protected override void DoInsert(int index, PdfField field)
        {
            if (!IsValidName(field.Name))
            {
                throw new PdfDocumentException(String.Format(c_exisingFieldException, field.Name));
            }

            field.SetForm(Form);
            base.DoInsert(index, field);
        }

        /// <summary>
        /// Removes the field from collection.
        /// </summary>
        /// <param name="field">The field.</param>
        protected override void DoRemove(PdfField field)
        {
            field.SetForm(null);
            base.DoRemove(field);
        }

        /// <summary>
        /// Removes the field at the specified position.
        /// </summary>
        /// <param name="index">The index.</param>
        protected override void DoRemoveAt(int index)
        {
            PdfField field = (PdfField)Items[index];
            field.SetForm(null);
            base.DoRemoveAt(index);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        protected override void DoClear()
        {
            foreach (PdfField field in this)
            {
                m_form.DeleteFromPages(field);
                m_form.DeleteAnnotation(field);
                field.Page = null;
                field.Dictionary.Clear();
                field.SetForm(null);
            }

            base.DoClear();
        }

        /// <summary>
        /// Check whether the field with the same name already exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// <c>true</c> if there are no fields with the same name within the collection; 
        /// otherwise <c>false</c>.
        /// </returns>
        private bool IsValidName(string name)
        {

            //foreach (PdfField field in List)
            //{
            //    if (field.Name == name)
            //    {
            //        return false;
            //    }
            //}

            //return true;

            return m_form.FieldNames.Contains(name);
        }
        #endregion
    }
}
