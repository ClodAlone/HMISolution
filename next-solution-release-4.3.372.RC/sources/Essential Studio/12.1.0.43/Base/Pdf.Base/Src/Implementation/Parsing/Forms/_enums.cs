#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Text;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    internal enum PdfLoadedFieldTypes
    {
        /// <summary>
        /// Identify push button field.
        /// </summary>
        PushButton = 0,
        /// <summary>
        /// Identify check box field.
        /// </summary>
        CheckBox = 1,
        /// <summary>
        /// Identify radio button field.
        /// </summary>
        RadioButton = 2,
        /// <summary>
        /// Identify text field.
        /// </summary>
        TextField = 3,
        /// <summary>
        /// Identify listbox field.
        /// </summary>
        ListBox = 4,
        /// <summary>
        /// Identify combobox field.
        /// </summary>
        ComboBox = 5,
        /// <summary>
        /// Identify signature field.
        /// </summary>
        SignatureField = 6,
        /// <summary>
        /// Identify that field has no type.
        /// </summary>
        Null = 7
    }

    /// <summary>
    /// Specifies the format of Export or Import data.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document
    /// PdfLoadedDocument ldoc = new PdfLoadedDocument("SourceForm.pdf");
    /// // Load the existing form
    /// PdfLoadedForm form = ldoc.Form;
    /// // Export the form data as XML file
    /// form.ExportData("Export.xml", DataFormat.Xml, "SourceForm.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document
    /// Dim ldoc As PdfLoadedDocument = New PdfLoadedDocument("SourceForm.pdf")
    /// ' Load the existing form
    /// Dim form As PdfLoadedForm = ldoc.Form
    /// ' Export the form data as XML file
    /// form.ExportData("Export.xml", DataFormat.Xml, "SourceForm.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedForm"/> Class  
    /// <seealso cref="PdfLoadedDocument"/> Class  
    public enum DataFormat
    {
        /// <summary>
        /// Specifies  XML file format
        /// </summary>
        Xml = 0,
        /// <summary>
        /// Specifies  Forms Data Format file format
        /// </summary>
        Fdf = 1,
        /// <summary>
        /// Specifies  XFDF file format.
        /// </summary>    
        XFdf = 2
    }

}
