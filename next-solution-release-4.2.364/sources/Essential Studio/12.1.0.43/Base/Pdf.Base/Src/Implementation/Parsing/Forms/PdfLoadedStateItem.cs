#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents the loaded state item.
    /// </summary>
    /// <seealso cref="PdfLoadedFieldItem"/> Class
    public class PdfLoadedStateItem : PdfLoadedFieldItem
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfLoadedStateItem"/> is checked.
        /// </summary>           
        public bool Checked
        {
            get
            {
                bool check = false;

                PdfName state = PdfCrossTable.Dereference(Dictionary[DictionaryProperties.AS]) as PdfName;

                if (state == null)
                {
                    PdfName name = PdfLoadedField.GetValue(Parent.Dictionary, Parent.CrossTable,
                        DictionaryProperties.V, false) as PdfName;

                    if (name != null)
                    {
                        check = (name.Value == PdfLoadedStateField.GetItemValue(Dictionary, CrossTable));
                    }
                }
                else
                {
                    check = (state.Value != DictionaryProperties.Off);
                }

                return check;
            }
            set
            {
                bool readOnly = ((FieldFlags.ReadOnly & this.Field.Flags) != 0);
                if (!readOnly)
                {
                    if (value != Checked)
                    {
                        SetCheckedStatus(value);
                        (this.Field as PdfField).Form.SetAppearanceDictionary = true;
                    }
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedStateItem"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="index">The index.</param>
        /// <param name="dictionary">The dictionary.</param>
        internal PdfLoadedStateItem(PdfLoadedStyledField field, int index, PdfDictionary dictionary)
            : base(field, index, dictionary)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets checked status of the field.
        /// </summary>
        /// <param name="value">Checked status.</param>
        private void SetCheckedStatus(bool value)
        {
            bool check = value;
            string val = PdfLoadedCheckBoxField.GetItemValue(Dictionary, CrossTable);

            (Parent as PdfLoadedStateField).UncheckOthers(this, val, value);

            if (check)
            {
                if (val == null || val == string.Empty)
                    val = DictionaryProperties.Yes;

                Parent.Dictionary.SetName(DictionaryProperties.V, val);
                Dictionary.SetProperty(DictionaryProperties.AS, new PdfName(val));
                Dictionary.SetProperty(DictionaryProperties.V, new PdfName(val));
            }
            else
            {
                PdfName v = PdfCrossTable.Dereference(Parent.Dictionary[DictionaryProperties.V]) as PdfName;

                if (v != null && val == v.Value)
                {
                    Parent.Dictionary.Remove(DictionaryProperties.V);
                }

                Dictionary.SetProperty(DictionaryProperties.AS, new PdfName(DictionaryProperties.Off));
            }

            Parent.Changed = true;
        }
        #endregion
    }
}
